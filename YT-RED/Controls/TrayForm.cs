using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using Xabe.FFmpeg;
using YoutubeDLSharp;
using YT_RED.Logging;
using YT_RED.Settings;
using YT_RED.Utils;

namespace YT_RED.Controls
{
    public partial class TrayForm : DevExpress.XtraEditors.XtraForm
    {
        public bool Locked { get; set; } = false;

        public string Url
        {
            get { return txtUrl.Text; }
            set { txtUrl.Text = value; }
        }

        protected IProgress<DownloadProgress> ytProgress;
        protected IProgress<string> ytOutput;
        protected CancellationTokenSource cancellationTokenSource;
        private DownloadType currentDownload;

        public TrayForm()
        {
            InitializeComponent();
            ytProgress = new Progress<DownloadProgress>(showProgress);
            ytOutput = null;
            currentDownload = DownloadType.Unknown;
        }

        public void TriggerDownload()
        {
            if(txtUrl.Text.Length > 3 && !Locked)
            {
                startDownload();
            }
        }

        private void txtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter && txtUrl.Text.Length > 3 && !Locked)
            {
                startDownload();
            }
        }

        private void showProgress(DownloadProgress progress)
        {
            if (!pgTrayProgress.Visible)
                pgTrayProgress.Show();
            pgTrayProgress.Position = Convert.ToInt32(progress.Progress * 100);
            if(progress.State == DownloadState.Success)
            {
                pgTrayProgress.Position = 0;
                pgTrayProgress.Hide();
            }
        }

        private async void startDownload()
        {
            this.Locked = true;
            this.txtUrl.Enabled = false;
            this.progressPanel.Visible = true;
            progressMarquee.Text = "Starting Download Process..";
            progressMarquee.Show();

            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = cancellationTokenSource.Token;

            if (HtmlUtil.CheckUrl(txtUrl.Text) == DownloadType.YouTube)
            {
                currentDownload = DownloadType.YouTube;
                RunResult<string> result = null;
                if(AppSettings.Default.General.UsePreferredFormat)
                {
                    result = await VideoUtil.DownloadPreferred(VideoUtil.YouTubeString(txtUrl.Text), Classes.StreamType.AudioAndVideo, ytProgress, null, cancelToken);
                }
                else
                {
                    result = await VideoUtil.DownloadBestYT(VideoUtil.YouTubeString(txtUrl.Text), Classes.StreamType.AudioAndVideo, ytProgress, null, cancelToken);
                }
                if (!result.Success && result.Data != "canceled")
                {
                    MessageBox.Show("Download Failed\n" + String.Join("\n", result.ErrorOutput));
                }

                progressMarquee.Hide();
                progressMarquee.Text = "";
                if (result.Data != "canceled")
                {
                    await Historian.RecordDownload(new DownloadLog(
                        DownloadType.YouTube,
                        VideoUtil.YouTubeString(txtUrl.Text),
                        Classes.StreamType.AudioAndVideo,
                        DateTime.Now,
                        result.Data));
                }
                btnOpenDL.Text = result.Data;
                btnOpenDL.Visible = true;
                this.txtUrl.Text = "";
                this.txtUrl.Enabled = true;
                this.Locked = false;

                cancellationTokenSource.Dispose();
                if(result.Data != "canceled" && AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                {
                    openDLLocation(btnOpenDL.Text);
                    this.Close();
                }
            }
            else if (HtmlUtil.CheckUrl(txtUrl.Text) == DownloadType.Reddit)
            {
                currentDownload = DownloadType.Reddit;
                redditDL(txtUrl.Text, cancelToken);
            }
        }

        private async void redditDL(string playlistUrl, CancellationToken? cancellationToken = null)
        {
            try
            {
                List<Classes.StreamLink> streamLinks = await Utils.HtmlUtil.GetVideoFromRedditPage(playlistUrl);
                if (streamLinks != null)
                {

                    string id = Utils.VideoUtil.GetRedditVideoID(streamLinks[0].StreamUrl);
                    if (id != null)
                    {
                        this.progressPanel.Visible = true;
                        progressMarquee.Text = "Evaluating available formats..";
                        progressMarquee.Show();
                        Tuple<int, string> bestDash = await Utils.HtmlUtil.GetBestRedditDashVideo(id);
                        bool audioExists = await Utils.HtmlUtil.MediaExists(Utils.VideoUtil.RedditAudioUrl(id));
                        if (bestDash != null)
                        {
                            progressMarquee.Text = "Preparing download..";
                            IConversion conversion = await Utils.VideoUtil.PrepareStreamConversion(bestDash.Item2, audioExists ? Utils.VideoUtil.RedditAudioUrl(id) : String.Empty);
                            string destination = conversion.OutputFilePath;
                            conversion.OnProgress += Conversion_OnProgress;
                            progressMarquee.Text = string.Empty;
                            pgTrayProgress.Visible = true;
                            bool cancelled = false;
                            try
                            {
                                if (cancellationToken != null)
                                    await conversion.Start((CancellationToken)cancellationToken);
                                else
                                    await conversion.Start();
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.ToLower() != "a task was canceled.")
                                    ExceptionHandler.LogException(ex);
                                else
                                    cancelled = true;
                            }
                            if (!cancelled)
                            {
                                bool saved = await Historian.RecordDownload(new DownloadLog(
                                    DownloadType.Reddit, bestDash.Item2, Classes.StreamType.AudioAndVideo, DateTime.Now, destination
                                    ));
                            }
                            pgTrayProgress.Visible = false;
                            progressMarquee.Hide();
                            progressMarquee.Text = "";
                            btnOpenDL.Text = destination;
                            btnOpenDL.Visible = true;

                            this.txtUrl.Text = "";
                            this.txtUrl.Enabled = true;
                            this.Locked = false;

                            cancellationTokenSource.Dispose();
                            if (!cancelled && AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                            {
                                openDLLocation(btnOpenDL.Text);
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        cancellationTokenSource.Dispose();
                        MessageBox.Show("Failed to acquire Media ID", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
        }

        public void HideProgressPanel()
        {
            this.progressPanel.Visible = false;
        }

        private void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
        {
            var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
            safeUpdateDownloadProgress(percent);
        }

        private void safeUpdateDownloadProgress(int percent)
        {
                if (pgTrayProgress.InvokeRequired)
                {
                    Action safeUpdate = delegate { pgTrayProgress.Position = percent; };
                    pgTrayProgress.Invoke(safeUpdate);
                }
                else
                    pgTrayProgress.Position = percent;
        }

        private void btnOpenDL_Click(object sender, EventArgs e)
        {
            if (btnOpenDL.Text.Length > 0)
            {
                openDLLocation(btnOpenDL.Text);
                this.Close();
            }
        }

        private void openDLLocation(string path)
        {
            string argument = "/select, \"" + path + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(this.Locked)
            {
                this.StartPosition = FormStartPosition.Manual;
                System.Drawing.Rectangle workingArea = Screen.GetWorkingArea(this);
                var loc = new System.Drawing.Point(workingArea.Right - 400, workingArea.Bottom - 200);
                DialogResult res = MsgBox.Show("A download is in progress. Cancel the current download?", "Download In-Progress", YT_RED.Controls.Buttons.YesNo, YT_RED.Controls.Icon.Warning, loc);
                if(res == DialogResult.Yes)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                    this.Close();
                }
            }
            this.Close();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if(txtUrl.Text.Length > 3 && !Locked)
            {
                startDownload();
            }
        }
    }
}