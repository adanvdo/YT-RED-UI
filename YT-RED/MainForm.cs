using System;
using System.Collections.Generic;
using System.Windows.Forms;
using YT_RED.Controls;
using YT_RED.Logging;
using YT_RED.Settings;
using Xabe.FFmpeg;
using Newtonsoft.Json;
using System.Linq;

namespace YT_RED
{
    public partial class MainForm : DevExpress.XtraBars.TabForm
    {
        public bool IsLocked { 
            get 
            { 
                foreach(CustomTabFormPage pg in this.tabFormControl1.Pages)
                {
                    if (pg.IsLocked)
                        return true;
                }
                return false;
            } 
        }
        public MainForm()
        {
            InitializeComponent();
            Init();
            Historian.Init();
        }

        protected override void OnLoad(EventArgs e)
        {
            txtRedditPost.Text = AppSettings.Default.General.RedditSampleUrl;
            base.OnLoad(e);
        }

        private async void Init()
        {
            this.settingsGrid.SelectedObject = AppSettings.Default.General;
            if (AppSettings.Default.General.EnableDownloadHistory)
            {
                bool loadDownloadHistory = await Historian.LoadDownloadHistory();
                if (loadDownloadHistory)
                {
                    gcHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.Reddit).ToList();
                    refreshRedditHistory();
                }
            }
            tabFormControl1.SelectedPage = tfpYouTube;
        }

        private void btnRedditDefault_Click(object sender, EventArgs e)
        {
            this.gcReddit.DataSource = null;
            this.gvReddit.RefreshData();
            if (!string.IsNullOrEmpty(this.txtRedditPost.Text))
            {
                redditScrape(this.txtRedditPost.Text);
            }
        }

        private void btnRedditList_Click(object sender, EventArgs e)
        {
            this.gcReddit.DataSource = null;
            this.gvReddit.RefreshData();
            if(!string.IsNullOrEmpty(this.txtRedditPost.Text))
            {
                redditScrape(this.txtRedditPost.Text, true);
            }
        }

        private async void redditScrape(string playlistUrl, bool listFormats = false)
        {
            this.UseWaitCursor = true;
            (this.tabFormControl1.SelectedPage as CustomTabFormPage).IsLocked = true;
            this.redditListMarquee.Show();
            try
            {
                List<Classes.StreamLink> streamLinks = await Utils.HtmlUtil.GetVideoFromRedditPage(playlistUrl);
                if (streamLinks != null)
                {
                    if (listFormats)
                    {
                        List<Classes.RedditStream> dashStreamList = new List<Classes.RedditStream>();
                        List<Classes.RedditStream> hlsStreamList = new List<Classes.RedditStream>();

                        foreach (Classes.StreamLink link in streamLinks)
                        {
                            IMediaInfo m3u8contents = await Utils.VideoUtil.ParseM3U8(link.StreamUrl);
                            List<Classes.RedditStream> l = new List<Classes.RedditStream>();
                            foreach (IStream ist in m3u8contents.Streams)
                            {
                                string transform = JsonConvert.SerializeObject(ist);
                                Classes.RedditStream rs = JsonConvert.DeserializeObject<Classes.RedditStream>(transform);
                                rs.DeliverStream = ist;
                                l.Add(rs);
                            }
                            foreach (Classes.RedditStream stream in l)
                            {
                                stream.PlaylistType = link.PlaylistType;
                            }
                            if (link.PlaylistType == Classes.StreamPlaylistType.DASH)
                                dashStreamList.AddRange(l);
                            else if (link.PlaylistType == Classes.StreamPlaylistType.HLS)
                                hlsStreamList.AddRange(l);
                        }

                        List<Classes.ResultStream> buildList = new List<Classes.ResultStream>();
                        buildList.AddRange(await Utils.VideoUtil.ConsolidateStreams(dashStreamList));
                        buildList.AddRange(await Utils.VideoUtil.ConsolidateStreams(hlsStreamList));
                        gcReddit.DataSource = buildList;
                        refreshRedditList();
                    }
                    else
                    {
                        string id = Utils.VideoUtil.GetRedditVideoID(streamLinks[0].StreamUrl);
                        if (id != null)
                        {
                            Tuple<int,string> bestDash = await Utils.HtmlUtil.GetBestRedditDashVideo(id);
                            if(bestDash != null)
                            {
                                IConversion conversion = await Utils.VideoUtil.PrepareDashConversion(bestDash.Item2, Utils.VideoUtil.RedditAudioUrl(id));
                                string destination = conversion.OutputFilePath;
                                conversion.OnProgress += Conversion_OnProgress;
                                this.pbDownloadProgress.Visible = true;
                                lblSelectionText.Text = $"Downloading DASH {bestDash.Item1}";
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
                                gcHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.Reddit).ToList();
                                refreshRedditHistory();
                                lblSelectionText.Text = string.Empty;
                                this.pbDownloadProgress.Visible = false;
                                this.btnRedDL.Text = destination;
                                this.btnRedDL.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            lblSelect.Visible = true;
            this.UseWaitCursor = false;
            (this.tabFormControl1.SelectedPage as CustomTabFormPage).IsLocked = false;
            this.redditListMarquee.Hide();
        }

        private void refreshRedditList()
        {
            gvReddit.PopulateColumns();
            gvReddit.Columns["StreamType"].Visible = false;
            gvReddit.Columns["VideoPath"].Visible = false;
            gvReddit.Columns["AudioPath"].Visible = false;
            gvReddit.Columns["VideoStream"].Visible = false;
            gvReddit.Columns["AudioStream"].Visible = false;
        }

        private void refreshRedditHistory()
        {
            gvHistory.PopulateColumns();
            gvHistory.Columns["DownloadType"].Width = 10;
            gvHistory.Columns["DownloadType"].Caption = "Type";
            gvHistory.Columns["FileName"].Visible = false;
            gvHistory.Columns["TimeLogged"].Visible = false;
            gvHistory.Columns["Type"].Visible = false;
            gvHistory.Columns["Downloaded"].Visible = false;
            gvHistory.RefreshData();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.IsLocked)
            {
                DialogResult res = MessageBox.Show("A Task is currently in progress. Would you like to cancel the task and exit?", "A Task is Busy", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }

        private void tabFormControl1_PageClosing(object sender, DevExpress.XtraBars.PageClosingEventArgs e)
        {
            CustomTabFormPage page = sender as CustomTabFormPage;
            if(page.IsLocked)
            {
                MessageBox.Show("A Task is currently in progress and cannot be cancelled.\nPlease wait for the operation to complete.", "A Task is Busy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }
        }

        private void gvReddit_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
            {
                lblSelectionText.Text = String.Empty;
                btnDownloadReddit.Enabled = false;
                btnDownloadReddit.Visible = false;
                btnRedDL.Text = String.Empty;
                btnRedDL.Visible = false;
                return;
            }
            selectedStream = gvReddit.GetFocusedRow() as Classes.ResultStream;
            if (selectedStream != null)
            {
                string displayText = $"Row {selectedStream.Row} - {selectedStream.StreamType}\n\n".Replace("AudioAndVideo", "Video + Audio");
                if (selectedStream.StreamType == Classes.StreamType.Video) displayText += $"Video: {selectedStream.PlaylistType} {selectedStream.VideoCodec} {selectedStream.Width}x{selectedStream.Height}";
                else if (selectedStream.StreamType.ToString() == "Audio") displayText += $"Audio: {selectedStream.AudioCodec} {selectedStream.AudioChannels}ch {selectedStream.AudioSampleRate}kHz";
                else displayText += $"Video: {selectedStream.PlaylistType} {selectedStream.VideoCodec} {selectedStream.Width}x{selectedStream.Height}\nAudio: {selectedStream.AudioCodec} {selectedStream.AudioChannels}ch {selectedStream.AudioSampleRate}kHz";
                lblSelectionText.Text = displayText;
                lblSelectionText.Refresh();
                btnDownloadReddit.Visible = true;
                btnDownloadReddit.Enabled = true;
            }
            else btnDownloadReddit.Visible = false;
        }

        private Classes.ResultStream selectedStream = null;

        private async void btnDownloadReddit_Click(object sender, EventArgs e)
        {
            if(selectedStream != null)
            {
                btnDownloadReddit.Enabled = false;
                IConversion conversion = Utils.VideoUtil.PrepareConversion(selectedStream);
                string destination = conversion.OutputFilePath;
                conversion.OnProgress += Conversion_OnProgress;
                this.pbDownloadProgress.Visible = true;
                try
                {
                    await conversion.Start();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                bool saved = await Historian.RecordDownload(new DownloadLog(
                    DownloadType.Reddit, 
                    string.IsNullOrEmpty(selectedStream.VideoPath) ? selectedStream.AudioPath : selectedStream.VideoPath,
                    selectedStream.StreamType, DateTime.Now, destination
                    ));
                gcHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.Reddit).ToList();
                refreshRedditHistory();
                this.pbDownloadProgress.Visible = false;
                this.btnRedDL.Text = destination;
                this.btnRedDL.Visible = true;
            }
        }

        private void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
        {
            var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
            safeUpdateDownloadProgress(percent);
        }

        private void safeUpdateDownloadProgress(int percent)
        {
            if (pbDownloadProgress.InvokeRequired)
            {
                Action safeUpdate = delegate { pbDownloadProgress.Position = percent; };
                pbDownloadProgress.Invoke(safeUpdate);
            }
            else
                pbDownloadProgress.Position = percent;
        }

        private void lblDLLocation_Click(object sender, EventArgs e)
        {
            string argument = "/select, \"" + btnRedDL.Text + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void tabFormControl1_SelectedPageChanged(object sender, DevExpress.XtraBars.TabFormSelectedPageChangedEventArgs e)
        {
            if(tabFormControl1.SelectedPage != null && tabFormControl1.SelectedPage.Text == "Reddit")
            {
                gcHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.Reddit).ToList();
                refreshRedditHistory();
            }
        }

        private void gvHistory_DoubleClick(object sender, EventArgs e)
        {
            if(selectedRedditLog != null)
            {
                string argument = "/select, \"" + selectedRedditLog.DownloadLocation + "\"";

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
        }

        private DownloadLog selectedRedditLog = null;
        private void gvHistory_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            selectedRedditLog = gvHistory.GetFocusedRow() as DownloadLog;
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            AppSettings.Default.Save();
        }

        private void btnRedDL_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(btnRedDL.Text))
            {
                string argument = "/select, \"" + btnRedDL.Text + "\"";

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
        }
    }
}
