using System;
using System.Collections.Generic;
using System.Windows.Forms;
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
            if (HtmlUtil.CheckUrl(txtUrl.Text) == DownloadType.YouTube)
            {
                currentDownload = DownloadType.YouTube;
                RunResult<string> result = null;
                if(AppSettings.Default.General.UsePreferredFormat)
                {
                    result = await VideoUtil.DownloadPreferred(VideoUtil.YouTubeString(txtUrl.Text), Classes.StreamType.AudioAndVideo, ytProgress);
                }
                else
                {
                    result = await VideoUtil.DownloadBestYT(VideoUtil.YouTubeString(txtUrl.Text), Classes.StreamType.AudioAndVideo, ytProgress);
                }
                if (!result.Success)
                {
                    MessageBox.Show("Download Failed\n" + String.Join("\n", result.ErrorOutput));
                }

                progressMarquee.Hide();
                progressMarquee.Text = "";
                await Historian.RecordDownload(new DownloadLog(
                    DownloadType.YouTube,
                    VideoUtil.YouTubeString(txtUrl.Text),
                    Classes.StreamType.AudioAndVideo,
                    DateTime.Now,
                    result.Data));

                btnOpenDL.Text = result.Data;
                btnOpenDL.Visible = true;
                this.txtUrl.Text = "";
                this.txtUrl.Enabled = true;
                this.Locked = false;

                if(AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                {
                    openDLLocation(btnOpenDL.Text);
                    this.Close();
                }
            }
            else if (HtmlUtil.CheckUrl(txtUrl.Text) == DownloadType.Reddit)
            {
                currentDownload = DownloadType.Reddit;
                redditDL(txtUrl.Text);
            }
        }

        private async void redditDL(string playlistUrl)
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
                            try
                            {
                                await conversion.Start();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            bool saved = await Historian.RecordDownload(new DownloadLog(
                                DownloadType.Reddit, bestDash.Item2, Classes.StreamType.AudioAndVideo, DateTime.Now, destination
                                ));
                            pgTrayProgress.Visible = false;
                            progressMarquee.Hide();
                            progressMarquee.Text = "";
                            btnOpenDL.Text = destination;
                            btnOpenDL.Visible = true;

                            this.txtUrl.Text = "";
                            this.txtUrl.Enabled = true;
                            this.Locked = false;

                            if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                            {
                                openDLLocation(btnOpenDL.Text);
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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