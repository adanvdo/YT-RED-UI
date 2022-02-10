using System;
using System.Collections.Generic;
using System.Windows.Forms;
using YT_RED.Controls;
using YT_RED.Classes;
using YT_RED.Logging;
using YT_RED.Settings;
using YT_RED.Utils;
using Xabe.FFmpeg;
using YoutubeDLSharp;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace YT_RED
{
    public partial class MainForm : DevExpress.XtraBars.TabForm
    {
        private UIBlockDetector _blockDetector;
        public bool IsLocked { 
            get 
            { 
                foreach(CustomTabFormPage pg in this.tcMainTabControl.Pages)
                {
                    if (pg.IsLocked)
                        return true;
                }
                return false;
            } 
        }

        private DownloadLog selectedYTLog = null;
        private Classes.ResultStream selectedStream = null;
        private DownloadLog selectedRedditLog = null;
        private YoutubeDLSharp.Metadata.FormatData selectedFormat = null;

        public MainForm()
        {
            InitializeComponent();
            Historian.Init();
            Init();
            _blockDetector = new UIBlockDetector(); 
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Program.DevRun)
            {
                txtYTUrl.Text = AppSettings.Default.General.YouTubeSampleUrl;
                txtRedditPost.Text = AppSettings.Default.General.RedditSampleUrl;
            }
            base.OnLoad(e);
        }

        private async void Init()
        {
            if (AppSettings.Default.General.EnableDownloadHistory)
            {
                bool loadDownloadHistory = await Historian.LoadDownloadHistory();
                if (loadDownloadHistory)
                {
                    gcHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.Reddit).ToList();
                    gvHistory.PopulateColumns();
                    refreshRedditHistory();
                    gcYTHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.YouTube).ToList();
                    gvYTHistory.PopulateColumns();
                    refreshYoutubeHistory();
                }
                gcReddit.DataSource = new List<ResultStream>();
                gvReddit.PopulateColumns();
                refreshRedditList();
                gcYoutube.DataSource = new List<YoutubeDLSharp.Metadata.FormatData>();
                gvYouTube.PopulateColumns();
                refreshYTGrid();
            }
            VideoUtil.Init();
            VideoUtil.ytProgress = new Progress<DownloadProgress>(showYTProgress);
            tcMainTabControl.SelectedPage = tfpYouTube;
        }

        #region Validation

        private DownloadType checkUrl(string url)
        {
            if (url.StartsWith(@"https://www.youtube.com") || url.StartsWith("https://youtu.be") || url.StartsWith(@"https://youtube.com"))
                return DownloadType.YouTube;
            if (url.StartsWith(@"https://reddit.com") || url.StartsWith(@"https://www.reddit.com"))
                return DownloadType.Reddit;
            return DownloadType.Unknown;
        }

        #endregion

        #region Reddit
        private void btnRedditDefault_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRedditPost.Text))
            {
                if (checkUrl(txtRedditPost.Text) == DownloadType.Reddit)
                {
                    redditScrape(this.txtRedditPost.Text);
                    return;
                }
                else if (checkUrl(txtRedditPost.Text) == DownloadType.YouTube)
                {
                    txtYTUrl.Text = txtRedditPost.Text;
                    tcMainTabControl.SelectedPage = tfpYouTube;

                    return;
                }
            }
            MessageBox.Show("The url provided is not a valid Youtube or Reddit url", "Unsupported URL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        private void btnRedditList_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRedditPost.Text))
            {
                if (checkUrl(txtRedditPost.Text) == DownloadType.Reddit)
                {
                    listRedditFormats(txtRedditPost.Text);
                    return;
                }
                else if(checkUrl(txtRedditPost.Text) == DownloadType.YouTube)
                {
                    txtYTUrl.Text = txtRedditPost.Text;
                    tcMainTabControl.SelectedPage = tfpYouTube;
                    getYTFormats(txtYTUrl.Text);
                    return;
                }
            }
            MessageBox.Show("The url provided is not a valid Youtube or Reddit url", "Unsupported URL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        private void listRedditFormats(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                redditScrape(url, true);
            }
            else
            {
                throw new ArgumentNullException("url is null");
            }
        }

        private async void btnDownloadReddit_Click(object sender, EventArgs e)
        {
            if (selectedStream != null)
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
                catch (Exception ex)
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
                this.pbDownloadProgress.Position = 0;
                this.btnRedDL.Text = destination;
                this.btnRedDL.Visible = true;
            }
        }

        private async void redditScrape(string playlistUrl, bool listFormats = false)
        {
            this.gcReddit.DataSource = null;
            this.gvReddit.RefreshData();

            this.UseWaitCursor = true;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
            this.redditListMarquee.Show();
            try
            {
                List<Classes.StreamLink> streamLinks = await Utils.HtmlUtil.GetVideoFromRedditPage(playlistUrl);
                if (streamLinks != null)
                {
                    if (listFormats)
                    {
                        this.redditListMarquee.Text = "Fetching available formats..";
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
                        this.redditListMarquee.Text = string.Empty;
                    }
                    else
                    {
                        string id = Utils.VideoUtil.GetRedditVideoID(streamLinks[0].StreamUrl);
                        if (id != null)
                        {
                            this.redditListMarquee.Text = "Evaluating available formats..";
                            Tuple<int,string> bestDash = await Utils.HtmlUtil.GetBestRedditDashVideo(id);
                            bool audioExists = await Utils.HtmlUtil.MediaExists(Utils.VideoUtil.RedditAudioUrl(id));
                            if(bestDash != null)
                            {
                                this.redditListMarquee.Text = "Preparing download..";
                                IConversion conversion = await Utils.VideoUtil.PrepareDashConversion(bestDash.Item2, audioExists ? Utils.VideoUtil.RedditAudioUrl(id) : String.Empty);
                                string destination = conversion.OutputFilePath;
                                conversion.OnProgress += Conversion_OnProgress;
                                this.redditListMarquee.Text = string.Empty;
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
                                this.pbDownloadProgress.Position = 0;
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
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
            this.redditListMarquee.Hide();
        }

        private void refreshRedditList()
        {
            gvReddit.Columns["StreamType"].Visible = false;
            gvReddit.Columns["VideoPath"].Visible = false;
            gvReddit.Columns["AudioPath"].Visible = false;
            gvReddit.Columns["VideoStream"].Visible = false;
            gvReddit.Columns["AudioStream"].Visible = false;
        }

        private void refreshRedditHistory()
        {
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
            if(tcMainTabControl.SelectedPage != null && tcMainTabControl.SelectedPage.Text == "Reddit")
            {
                gcHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.Reddit).ToList();
                refreshRedditHistory();
                if(gvHistory.RowCount > 0)
                    gvHistory.FocusedRowHandle = 0;
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

        #endregion

        #region YOUTUBE
        private void toggleYTSegment_Toggled(object sender, EventArgs e)
        {
            tsYTStart.Enabled = toggleYTSegment.IsOn;
            tsYTEnd.Enabled = toggleYTSegment.IsOn;
        }

        private void btnYTListFormats_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtYTUrl.Text))
            {
                if(checkUrl(txtYTUrl.Text) == DownloadType.YouTube)
                    getYTFormats(VideoUtil.YouTubeString(txtYTUrl.Text));
                else if(checkUrl(txtYTUrl.Text) == DownloadType.Reddit)
                {
                    txtRedditPost.Text = txtYTUrl.Text;
                    tcMainTabControl.SelectedPage = tfpReddit;
                    listRedditFormats(txtRedditPost.Text);
                }
                else
                {
                    MessageBox.Show("The url provided is not a valid Youtube or Reddit url", "Unsupported URL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }            
        }

        private async void getYTFormats(string url)
        {
            try
            {
                this.UseWaitCursor = true;
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
                this.ytMarquee.Text = "Fetching Available Formats";                
                this.ytMarquee.Show();
                var data = await VideoUtil.GetVideoData(url);
                var formatList = data.Formats.Where(f => f.FormatId != "sb0").OrderBy(f => f.AudioCodec != null ? 0 : 1).ThenBy(f => f.Height).ToList();
                gcYoutube.DataSource = formatList;
                refreshYTGrid();        
                this.UseWaitCursor=false;
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
                this.ytMarquee.Hide();
                this.ytMarquee.Text = string.Empty;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void refreshYTGrid()
        {
            gvYouTube.Columns["Format"].Width = gvYouTube.Columns["Format"].GetBestWidth();
            gvYouTube.Columns["Url"].Visible = false;
            gvYouTube.Columns["ManifestUrl"].Visible = false;
            gvYouTube.Columns["FormatId"].Visible = false;
            gvYouTube.Columns["FormatNote"].Visible = false;
            gvYouTube.Columns["Resolution"].Visible = false;
            gvYouTube.Columns["ContainerFormat"].Visible = false;
            gvYouTube.Columns["AudioBitrate"].Visible = false;
            gvYouTube.Columns["Extension"].Visible = false;
            gvYouTube.Columns["FragmentBaseUrl"].Visible = false;
            gvYouTube.Columns["Language"].Visible = false;
            gvYouTube.Columns["LanguagePreference"].Visible = false;
            gvYouTube.Columns["NoResume"].Visible = false;
            gvYouTube.Columns["PlayerUrl"].Visible = false;
            gvYouTube.Columns["Preference"].Visible = false;
            gvYouTube.Columns["Protocol"].Visible = false;
            gvYouTube.Columns["Quality"].Visible = false;
            gvYouTube.Columns["SourcePreference"].Visible = false;
            gvYouTube.Columns["Width"].Visible = false;
            gvYouTube.Columns["Height"].Visible = false;
            gvYouTube.Columns["StretchedRatio"].Visible = false;
            gvYouTube.Columns["ApproximateFileSize"].Visible = false;
            gvYouTube.RefreshData(); 
        }

        private void refreshYoutubeHistory()
        {
            gvYTHistory.Columns["DownloadType"].Width = 10;
            gvYTHistory.Columns["DownloadType"].Caption = "Type";
            gvYTHistory.Columns["FileName"].Visible = false;
            gvYTHistory.Columns["TimeLogged"].Visible = false;
            gvYTHistory.Columns["Type"].Visible = false;
            gvYTHistory.Columns["Downloaded"].Visible = false;
            gvYTHistory.RefreshData();
        }


        private void gvYouTube_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            btnYTSelectionDL.Visible = e.FocusedRowHandle >= 0;
            btnYTSelectionDL.Enabled = e.FocusedRowHandle >= 0; 
            if (e.FocusedRowHandle < 0)
            {
                lblYTSelectionText.Text = string.Empty;
            } else
            {
                YoutubeDLSharp.Metadata.FormatData fd = gvYouTube.GetFocusedRow() as YoutubeDLSharp.Metadata.FormatData;
                lblYTSelectionText.Text = fd.Format;
                selectedFormat = fd;
            }
        }

        private void showYTProgress(DownloadProgress progress)
        {
            if(!pbYTProgress.Visible)
            {
                pbYTProgress.Show();
            }
            pbYTProgress.Position = Convert.ToInt32(progress.Progress * 100);
            if(progress.State == DownloadState.Success)
            {
                pbYTProgress.Position = 0;
                pbYTProgress.Hide();
            }
        }

        private async void btnYTSelectionDL_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
            btnYTOpenDL.Text = String.Empty;
            btnYTOpenDL.Visible = false;
            btnDownloadAudio.Enabled = false;
            btnYTDownloadBest.Enabled = false;
            var result = await Utils.VideoUtil.DownloadYTFormat(VideoUtil.YouTubeString(txtYTUrl.Text), selectedFormat);
            if(!result.Success)
            {
                MessageBox.Show("Download Failed");
            }
            YT_RED.Classes.StreamType t = Classes.StreamType.Audio;
            if (selectedFormat.AudioCodec == "none")
                t = Classes.StreamType.Video;
            else if (selectedFormat.Resolution == "audio only")
                t = Classes.StreamType.Audio;
            else
                t = Classes.StreamType.AudioAndVideo;
            await Historian.RecordDownload(new DownloadLog(
                DownloadType.YouTube,
                VideoUtil.YouTubeString(txtYTUrl.Text),
                t, DateTime.Now,
                result.Data
                ));
            gcYTHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.YouTube).ToList();
            refreshYoutubeHistory();
            lblYTSelectionText.Text = String.Empty;
            btnYTOpenDL.Text = result.Data;
            btnYTOpenDL.Visible = true;
            btnDownloadAudio.Enabled = true;
            btnYTDownloadBest.Enabled = true;
            this.UseWaitCursor = false;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
        }

        private async void btnDownloadAudio_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtYTUrl.Text))
            {
                if (checkUrl(txtYTUrl.Text) == DownloadType.YouTube)
                {
                    this.UseWaitCursor = true;
                    (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
                    btnYTOpenDL.Text = String.Empty;
                    btnYTOpenDL.Visible = false;
                    RunResult<string> result = null;
                    if (chkUsePrefs.Checked)
                        result = await Utils.VideoUtil.DownloadPreferred(VideoUtil.YouTubeString(txtYTUrl.Text), Classes.StreamType.Audio);
                    else
                        result = await Utils.VideoUtil.DownloadBestYT(VideoUtil.YouTubeString(txtYTUrl.Text), Classes.StreamType.Audio);
                    if (!result.Success)
                    {
                        MessageBox.Show("Download Failed");
                    }
                    await Historian.RecordDownload(new DownloadLog(
                        DownloadType.YouTube,
                        VideoUtil.YouTubeString(txtYTUrl.Text),
                        YT_RED.Classes.StreamType.Audio, DateTime.Now,
                        result.Data
                        ));
                    gcYTHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.YouTube).ToList();
                    refreshYoutubeHistory();
                    btnYTOpenDL.Text = result.Data;
                    btnYTOpenDL.Visible = true;
                    this.UseWaitCursor = false;
                    (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
                    return;
                }                
            }
            MessageBox.Show("The url provided is not a valid Youtube url", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        private void btnYTDownloadBest_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtYTUrl.Text))
            {
                if (checkUrl(txtYTUrl.Text) == DownloadType.YouTube)
                {
                    ytDownloadBest(txtYTUrl.Text);
                    return;
                }
                else if(checkUrl(txtYTUrl.Text) == DownloadType.Reddit)
                {
                    txtRedditPost.Text = txtYTUrl.Text;
                    tcMainTabControl.SelectedPage = tfpReddit;
                    redditScrape(txtRedditPost.Text);
                    return;
                }
            }
            MessageBox.Show("The url provided is not a valid Youtube or Reddit url", "Unsupported URL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        private async void ytDownloadBest(string url)
        {
            this.UseWaitCursor = true;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
            btnYTOpenDL.Text = String.Empty;
            btnYTOpenDL.Visible = false;
            RunResult<string> result = null;
            if (chkUsePrefs.Checked)
                result = await Utils.VideoUtil.DownloadPreferred(VideoUtil.YouTubeString(url), Classes.StreamType.AudioAndVideo);
            else
                result = await Utils.VideoUtil.DownloadBestYT(VideoUtil.YouTubeString(url), Classes.StreamType.AudioAndVideo);
            if (!result.Success)
            {
                MessageBox.Show("Download Failed\n" + String.Join("\n", result.ErrorOutput));
            }
            await Historian.RecordDownload(new DownloadLog(
                DownloadType.YouTube,
                VideoUtil.YouTubeString(url),
                Classes.StreamType.AudioAndVideo,
                DateTime.Now,
                result.Data));
            gcYTHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.YouTube).ToList();
            refreshYoutubeHistory();
            btnYTOpenDL.Text = result.Data;
            btnYTOpenDL.Visible = true;
            this.UseWaitCursor = false;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
            return;
        }

        private void lblYTSelectionText_SizeChanged(object sender, EventArgs e)
        {
            pnlYTOptionPanel.Height = groupControl1.MinimumSize.Height + lblYTSelectionText.Height;
        }

        private async void bbiSettings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SettingsDialog dlg = new SettingsDialog();
            DialogResult res = dlg.ShowDialog(); 
            gcYTHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.YouTube).ToList();
            refreshRedditHistory();
            gcHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.Reddit).ToList();
            refreshYoutubeHistory();
            if (res == DialogResult.OK)
            {
                bsiMessage.Caption = "Settings Saved";
                await Task.Delay(3000);
                bsiMessage.Caption = String.Empty;
            }            
        }

        private void btnYTOpenDL_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(btnYTOpenDL.Text))
            {
                string argument = "/select, \"" + btnYTOpenDL.Text + "\"";

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
        }

        private void gvYTHistory_DoubleClick(object sender, EventArgs e)
        {
            if (selectedYTLog != null)
            {
                string argument = "/select, \"" + selectedYTLog.DownloadLocation + "\"";

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
        }

        

        private void gvYTHistory_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
                return;

            selectedYTLog = gvYTHistory.GetFocusedRow() as DownloadLog;
        }

        private void txtYTUrl_Click(object sender, EventArgs e)
        {
            txtYTUrl.SelectAll();
        }
        #endregion

        private void gvYouTube_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if(e.Column.FieldName == "FileSize")
            {
                string size;
                if (e.Value != null)
                {
                    size = ((Convert.ToDecimal(e.Value) / 1024) / 1024).ToString();
                    
                }
                else
                {
                    size = ((Convert.ToDecimal(gvYouTube.GetRowCellValue(e.ListSourceRowIndex, "ApproximateFileSize")) / 1024) / 1024).ToString();
                }
                e.DisplayText = size.Substring(0, size.IndexOf('.') + 2) + "MB";
            }
        }

        private void txtRedditPost_Click(object sender, EventArgs e)
        {
            txtRedditPost.SelectAll();
        }
    }
}
