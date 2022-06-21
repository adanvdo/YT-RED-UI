using System;
using System.Collections.Generic;
using System.Drawing;
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
        private System.Windows.Forms.Timer activeTimer;
        private bool inactive = false;
        private bool hotkeyTriggered = false;

        public TrayForm()
        {
            InitializeComponent();
            ytProgress = new Progress<DownloadProgress>(showProgress);
            ytOutput = null;
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
                this.Close();
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
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = cancellationTokenSource.Token;
            currentDownload = HtmlUtil.CheckUrl(txtUrl.Text);
            if(currentDownload == DownloadType.Unknown && AppSettings.Default.General.ShowHostWarning)
            {
                DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
                if (res == DialogResult.No)
                    return;
            }

            RunResult<string> result = null;
            if (AppSettings.Default.General.UsePreferredFormat)
            {
                result = await VideoUtil.DownloadPreferredYtdl(VideoUtil.CorrectYouTubeString(txtUrl.Text), Classes.StreamType.AudioAndVideo, ytProgress, null, cancelToken);
            }
            else
            {
                result = await VideoUtil.DownloadBestYtdl(VideoUtil.CorrectYouTubeString(txtUrl.Text), Classes.StreamType.AudioAndVideo, ytProgress, null, cancelToken);
            }
            if (!result.Success && result.Data != "canceled")
            {
                MsgBox.Show("Download Failed\n" + String.Join("\n", result.ErrorOutput));
            }

            progressMarquee.Hide();
            progressMarquee.Text = "";
            if (result.Data != "canceled")
            {
                await Historian.RecordDownload(new DownloadLog(
                    currentDownload,
                    VideoUtil.CorrectYouTubeString(txtUrl.Text),
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
            if (result.Data != "canceled" && AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
            {
                openDLLocation(btnOpenDL.Text);
                this.Close();
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

        private void TrayForm_MouseMove(object sender, MouseEventArgs e)
        {
            this.inactive = false;
        }
    }
}