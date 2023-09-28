using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using YoutubeDLSharp;
using YTR.Classes;
using YTR.Logging;
using YTR.Settings;
using YTR.Utils;
using Xabe.FFmpeg;
using System.Diagnostics;

namespace YTR.Controls
{
    public partial class TrayForm : DevExpress.XtraEditors.XtraForm
    {
        public bool Locked { get; set; } = false;

        public string Url
        {
            get { return txtUrl.Text; }
            set { txtUrl.Text = value; }
        }
        
        private DownloadType currentDownload;
        private System.Windows.Forms.Timer activeTimer;
        private bool inactive = false;
        private bool hotkeyTriggered = false;
        public bool closeOnCompletedProgress = false;

        public TrayForm()
        {
            InitializeComponent();            
            currentDownload = DownloadType.Unknown;
            this.activeTimer = new System.Windows.Forms.Timer();
            this.activeTimer.Interval = 10000;
            this.activeTimer.Tick += ActiveTimer_Tick;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.activeTimer.Start();
            base.OnLoad(e);
        }

        private void ActiveTimer_Tick(object sender, EventArgs e)
        {
            if (!Locked && inactive)
                this.Hide();
            else if (!Locked)
                inactive = true;
        }

        public void TriggerDownload()
        {
            if(txtUrl.Text.Length > 3 && !Locked)
            {
                hotkeyTriggered = true;
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

        public void ShowProgress(int percent)
        {
            if (pgTrayProgress.InvokeRequired)
            {
                Action safeUpdate = delegate
                {
                    if (!pgTrayProgress.Visible)
                        pgTrayProgress.Show();
                    pgTrayProgress.Position = percent;
                    if (percent == 100)
                    {
                        pgTrayProgress.Position = 0;
                        pgTrayProgress.Hide();
                        progressMarquee.Hide();
                        if (closeOnCompletedProgress)
                        {
                            closeOnCompletedProgress = false;
                            this.Hide();
                        }
                    }
                };
                pgTrayProgress.Invoke(safeUpdate);
            }
            else
            {
                if (!pgTrayProgress.Visible)
                    pgTrayProgress.Show();
                pgTrayProgress.Position = percent;
                if (percent == 100)
                {
                    pgTrayProgress.Position = 0;
                    pgTrayProgress.Hide();
                    progressMarquee.Hide();
                    if (closeOnCompletedProgress)
                    {
                        closeOnCompletedProgress = false;
                        this.Hide();
                    }
                }
            }
        }

        public void ShowProgress(DownloadProgress progress)
        {
            if (pgTrayProgress.InvokeRequired)
            {
                Action safeUpdate = delegate
                {
                    if (!pgTrayProgress.Visible)
                        pgTrayProgress.Show();
                    pgTrayProgress.Position = Convert.ToInt32(progress.Progress * 100);
                    if (progress.State == DownloadState.Success)
                    {
                        pgTrayProgress.Position = 0;
                        pgTrayProgress.Hide();
                        progressMarquee.Hide(); 
                        if (closeOnCompletedProgress)
                        {
                            closeOnCompletedProgress = false;
                            this.Hide();
                        }
                    }
                };
                pgTrayProgress.Invoke(safeUpdate);
            }
            else
            {
                if (!pgTrayProgress.Visible)
                    pgTrayProgress.Show();
                pgTrayProgress.Position = Convert.ToInt32(progress.Progress * 100);
                if (progress.State == DownloadState.Success)
                {
                    pgTrayProgress.Position = 0;
                    pgTrayProgress.Hide();
                    progressMarquee.Hide();
                    if (closeOnCompletedProgress)
                    {
                        closeOnCompletedProgress = false;
                        this.Hide();
                    }
                }
            }
        }

        public void ShowProgressOutput(string output)
        {
            if (progressMarquee.InvokeRequired)
            {
                Action safeUpdate = delegate
                {
                    if (!progressMarquee.Visible)
                        progressMarquee.Show();
                    progressMarquee.Text = output;
                };
                progressMarquee.Invoke(safeUpdate);
            }
            else 
            {
                if (!progressMarquee.Visible)
                    progressMarquee.Show();
                progressMarquee.Text = output;
            }
        }

        private async void startDownload()
        {
            this.Locked = true;
            this.txtUrl.Enabled = false;
            this.progressPanel.Visible = true;
            progressMarquee.Text = "Starting Download Process..";
            progressMarquee.Show();
            if (hotkeyTriggered)
            {
                hotkeyTriggered = false;
            }
            else
            {
                Rectangle workingArea = Screen.GetWorkingArea(this);
                var loc = new Point(workingArea.Right - this.Size.Width, workingArea.Bottom - this.Size.Height);
                this.Location = loc;
            }
            currentDownload = HtmlUtil.CheckUrl(txtUrl.Text);
            if(currentDownload == DownloadType.Unknown && AppSettings.Default.General.ShowHostWarning)
            {
                DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YTR.Controls.Icon.Warning, FormStartPosition.CenterParent);
                if (res == DialogResult.No)
                    return;
            }
            if(currentDownload == DownloadType.YouTube)
            {
                YoutubeLink link = VideoUtil.ConvertToYouTubeLink(txtUrl.Text);
                if (link.Type == YoutubeLinkType.Playlist)
                {
                    DialogResult res = MsgBox.Show("Quick Download does not support Youtube Playlists", "Unsupported", Buttons.OK,YTR.Controls.Icon.Exclamation, FormStartPosition.CenterScreen, true);
                    if(res != DialogResult.None)
                    {
                        this.txtUrl.Text = "";
                        this.txtUrl.Enabled = true;
                        this.Locked = false;
                        this.Hide();
                    }
                    return;
                }
            }

            string useFormatString = "bestvideo{0}{1}+bestaudio/best{0}{1}";
            string finalFormatString = String.Format(useFormatString,
                AppSettings.Default.General.MaxResolutionValue > 0 ? $"[height<={AppSettings.Default.General.MaxResolutionValue}]" : "",
                AppSettings.Default.General.MaxFilesizeBest > 0 ? $"[filesize<={AppSettings.Default.General.MaxFilesizeBest}M]" : "");       

            RunResult<string> result = null;
            var convertedLink = VideoUtil.ConvertToYouTubeLink(txtUrl.Text);
            string url = convertedLink != null ? convertedLink.Url : txtUrl.Text;

            var pendingDL = new PendingDownload()
            {
                Url = url,
                Format = finalFormatString
            };

            if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
            {
                result = await Utils.VideoUtil.DownloadPreferredYtdl(url, Classes.StreamType.AudioAndVideo);
            }
            else
                result = await VideoUtil.DownloadBestYtdl(url, Classes.StreamType.AudioAndVideo);

            if (!result.Success && result.Data != "canceled")
            {
                MsgBox.Show("Download Failed\n" + String.Join("\n", result.ErrorOutput));
            }

            progressMarquee.Hide();
            progressMarquee.Text = "";
            if (result.Data != "canceled")
            {
                await Historian.RecordDownload(new DownloadLog(
                    url,
                    currentDownload,
                    Classes.StreamType.AudioAndVideo,
                    DateTime.Now,
                    result.Data,
                    pendingDL)
                {
                    Format = pendingDL.Format
                });
            }
            btnOpenDL.Text = result.Data;
            btnOpenDL.Visible = true;
            this.txtUrl.Text = "";
            this.txtUrl.Enabled = true;
            this.Locked = false;

            if (result.Data != "canceled" && AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
            {
                openDLLocation(btnOpenDL.Text);
                this.Hide();
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
                this.Hide();
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
                DialogResult res = MsgBox.Show("A download is in progress. Cancel the current download?", "Download In-Progress", YTR.Controls.Buttons.YesNo, YTR.Controls.Icon.Warning, loc);
                if(res == DialogResult.Yes)
                {
                    VideoUtil.CancellationTokenSource.Cancel();
                    VideoUtil.CancellationTokenSource.Dispose();
                    this.Hide();
                }
            }
            this.Hide();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if(txtUrl.Text.Length > 3 && !Locked)
            {
                startDownload();
            }
        }

        private void TrayForm_MouseMove(object sender, MouseEventArgs e)
        {
            this.inactive = false;
        }
    }
}