using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using URIScheme;
using Xabe.FFmpeg;
using YoutubeDLSharp;
using YT_RED.Classes;
using YT_RED.Controls;
using YT_RED.Logging;
using YT_RED.Settings;
using YT_RED.Utils;

namespace YT_RED
{
    public partial class MainForm : DevExpress.XtraBars.TabForm
    {
        private UIBlockDetector _blockDetector;
        private Timer historyTimer;
        public bool IsLocked
        {
            get
            {
                foreach (CustomTabFormPage pg in this.tcMainTabControl.Pages)
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
        private CustomTabFormPage youTubePage = null;
        private CustomTabFormPage redditPage = null;
        private DownloadType currentDownload = DownloadType.Unknown;
        private bool downloadingSegment = false;
        private bool downloadingCropped = false;
        private InitialFunction initialFunction = InitialFunction.None;
        private string initialLink = string.Empty;
        private MediaSource initialSource = MediaSource.None;
        private URISchemeService uriService = null;
        private bool enableQuickDownload = false;
        static KeyboardHook hook = new KeyboardHook();

        public MainForm()
        {
            InitializeComponent();
            initializeProgram();
        }

        public MainForm(InitialFunction initialFunction, string initialLink, Classes.MediaSource mediaSource)
        {
            InitializeComponent();
            initializeProgram(initialFunction, initialLink, mediaSource);
        }

        private void initializeProgram(InitialFunction initialFunction = InitialFunction.None, string initialLink = "", MediaSource mediaSource = MediaSource.None)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            AppSettings.Default.About.Version = assembly.GetName().Version.ToString();
            AppSettings.Default.About.Build = assembly.GetCustomAttributes(typeof(AssemblyBuildAttribute), false).Cast<AssemblyBuildAttribute>().FirstOrDefault().Value;
            MainForm.hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(Hook_KeyPressed);
            MainForm.UpdateDownloadHotkey();
            registerURIScheme();
            this.historyTimer = new Timer();
            this.historyTimer.Interval = 10000;
            this.historyTimer.Tick += HistoryTimer_Tick;
            if (AppSettings.Default.General.EnableDownloadHistory)
                this.historyTimer.Start();
            Historian.Init();
            Init();
            _blockDetector = new UIBlockDetector();
            this.initialFunction = initialFunction;
            this.initialLink = initialLink;
            initialSource = mediaSource;
        }

        public static void UpdateDownloadHotkey()
        {
            MainForm.hook.UnregisterHotKey();
            if (AppSettings.Default.Advanced.EnableHotKeys && AppSettings.Default.Advanced.DownloadHotKey != KeyShortcut.Empty && !string.IsNullOrEmpty(AppSettings.Default.Advanced.DownloadHotKey.ToString()))
            {
                ModifierKeysConverter modifierKeysConverter = new ModifierKeysConverter();
                KeysConverter keysConverter = new KeysConverter();
                string modifierKeys = "";
                foreach (string s in AppSettings.Default.Advanced.DownloadHotKey.ToString().Replace(" ", "").Split('+'))
                {
                    if (modifierKeysConverter.IsValid(s))
                        modifierKeys += $"{s}+";
                }
                modifierKeys = modifierKeys.Remove(modifierKeys.Length - 1, 1);
                string keys = "";
                foreach (string s in AppSettings.Default.Advanced.DownloadHotKey.ToString().Replace(" ", "").Split('+'))
                {
                    if (keysConverter.IsValid(s) && !modifierKeysConverter.IsValid(s))
                        keys += $"{s}+";
                }
                keys = keys.Remove(keys.Length - 1, 1);
                ModifierKeys dlModifier = (ModifierKeys)modifierKeysConverter.ConvertFrom(modifierKeys);
                Keys dlKey = (Keys)keysConverter.ConvertFrom(keys);

                MainForm.hook.RegisterHotKey(dlModifier, dlKey);
            }
        }

        private async void Hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (enableQuickDownload)
            {
                string tempText = string.Empty;
                string copiedText = string.Empty;
                if (Clipboard.ContainsText()) { tempText = Clipboard.GetText(); }
                SendKeys.SendWait("^c");
                await Task.Delay(500);
                if (Clipboard.ContainsText())
                    copiedText = Clipboard.GetText();
                if(!string.IsNullOrEmpty(tempText))
                    Clipboard.SetText(tempText);

                if (HtmlUtil.CheckUrl(copiedText) == DownloadType.Unknown)
                {
                    MessageBox.Show("The selected text is not a valid Youtube or Reddit media url", "No link detected");
                    return;
                }

                if (activeTrayForm == null)
                {
                    try
                    {
                        TrayForm trayForm = new TrayForm();

                        trayForm.FormClosed += TrayForm_FormClosed;
                        trayForm.StartPosition = FormStartPosition.Manual;
                        Rectangle workingArea = Screen.GetWorkingArea(this);
                        var loc = new Point(workingArea.Right - trayForm.Size.Width, workingArea.Bottom - trayForm.Size.Height);
                        trayForm.HideProgressPanel();
                        trayForm.Location = loc;
                        trayForm.Url = copiedText;
                        trayForm.Show();
                        trayForm.BringToFront();
                        trayForm.TopMost = true;
                        trayForm.TriggerDownload();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogException(ex);
                    }
                }
            }
        }

        private async void HistoryTimer_Tick(object sender, EventArgs e)
        {
            if (AppSettings.Default.General.EnableDownloadHistory)
            {
                await Historian.CleanHistory();
                refreshRedditHistory();
                refreshYoutubeHistory();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Program.DevRun)
            {
                txtYTUrl.Text = AppSettings.Default.General.YouTubeSampleUrl;
                txtRedditPost.Text = AppSettings.Default.General.RedditSampleUrl;
            }
            chkUsePrefs.Checked = AppSettings.Default.General.UsePreferredFormat;
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //uriService.Delete();
            base.OnClosing(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                enableQuickDownload = true;
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(3000);
                this.ShowInTaskbar = false;
            }
            else
            {
                enableQuickDownload = false;
            }
            base.OnResize(e);
        }

        private void registerURIScheme()
        {
            const string key = @"ytred";
            string exeLocation = Assembly.GetEntryAssembly().Location;
            uriService = new URISchemeService(key, @"URL:ytred", $"{exeLocation}");
            var isSet = uriService.Check();
            if (isSet)
                uriService.Delete();


            uriService.Set();

        }

        private async void Init()
        {
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(AppSettings.Default.General.ActiveSkin);
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
            if (this.initialSource == MediaSource.YouTube)
            {
                tcMainTabControl.SelectedPage = tfpYouTube;
                txtYTUrl.Text = this.initialLink;
                if (initialFunction == InitialFunction.ListFormats)
                    doYTFormatList();
                else if (initialFunction == InitialFunction.DownloadBest)
                    doYTBest();
            }
            else if (this.initialSource == MediaSource.Reddit)
            {
                tcMainTabControl.SelectedPage = tfpReddit;
                txtRedditPost.Text = this.initialLink;
                if (initialFunction == InitialFunction.ListFormats)
                    doRedditList();
                else if (initialFunction == InitialFunction.DownloadBest)
                    doRedBest();
            }
            else
                tcMainTabControl.SelectedPage = tfpYouTube;
        }

        #region Reddit
        private void btnRedditDefault_Click(object sender, EventArgs e)
        {
            doRedBest();
        }

        private void doRedBest()
        {
            if (!string.IsNullOrEmpty(txtRedditPost.Text))
            {
                if (HtmlUtil.CheckUrl(txtRedditPost.Text) == DownloadType.Reddit)
                {
                    redditScrape(this.txtRedditPost.Text);
                    return;
                }
                else if (HtmlUtil.CheckUrl(txtRedditPost.Text) == DownloadType.YouTube)
                {
                    txtYTUrl.Text = txtRedditPost.Text;
                    tcMainTabControl.SelectedPage = tfpYouTube;
                    btnYTDownloadBest.PerformClick();
                    return;
                }
            }
            MessageBox.Show("The url provided is not a valid Youtube or Reddit url", "Unsupported URL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        private void btnRedditList_Click(object sender, EventArgs e)
        {
            doRedditList();
        }

        private void doRedditList()
        {
            if (!string.IsNullOrEmpty(txtRedditPost.Text))
            {
                if (HtmlUtil.CheckUrl(txtRedditPost.Text) == DownloadType.Reddit)
                {
                    listRedditFormats(txtRedditPost.Text);
                    return;
                }
                else if (HtmlUtil.CheckUrl(txtRedditPost.Text) == DownloadType.YouTube)
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
                this.UseWaitCursor = true;
                this.currentDownload = DownloadType.Reddit;
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
                List<Classes.FFmpegParam> parameters = new List<Classes.FFmpegParam>();
                if (toggleRedSegment.IsOn)
                {                    
                    TimeSpan start = tsRedStart.TimeSpan;
                    TimeSpan duration = tsRedDuration.TimeSpan;
                    if (start != null)
                    {
                        parameters.Add(new Classes.FFmpegParam(Classes.ParamType.StartTime, $"-ss {((TimeSpan)start)}"));
                    }
                    if (duration != null)
                    {
                        parameters.Add(new Classes.FFmpegParam(Classes.ParamType.Duration, $"-t {((TimeSpan)duration)}"));
                    }
                }

                int[] crops = null;
                if (toggleRedditCrop.IsOn)
                {
                    crops = new int[] { Convert.ToInt32(txtRedCropTop.Text), Convert.ToInt32(txtRedCropBottom.Text), Convert.ToInt32(txtRedCropLeft.Text), Convert.ToInt32(txtRedCropRight.Text) };
                }                

                int outWidth = -1;
                int outHeight = -1;
                int x = -1;
                int y = -1;

                if (crops != null && crops.Length == 4 && selectedStream != null)
                {
                    int[] ffmpegCrop = Utils.VideoUtil.ConvertCrop(crops, Convert.ToInt32(selectedStream.Width), Convert.ToInt32(selectedStream.Height));
                    x = ffmpegCrop[0];
                    y = ffmpegCrop[1];
                    outWidth = ffmpegCrop[2];
                    outHeight = ffmpegCrop[3];
                }

                if (outWidth >= 0 && outHeight >= 0 && x >= 0 && y >= 0)
                {
                    parameters.Add(new Classes.FFmpegParam(Classes.ParamType.Crop, $"-filter:v \"crop={outWidth}:{outHeight}:{x}:{y}\""));
                }

                IConversion conversion = Utils.VideoUtil.PrepareConversion(selectedStream, parameters.Count > 0 ? parameters.ToArray() : null);
                string destination = conversion.OutputFilePath;
                conversion.OnProgress += Conversion_OnProgress;
                this.pbDownloadProgress.Visible = true;
                try
                {
                    await conversion.Start();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex);
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
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
                this.UseWaitCursor = false;
                this.currentDownload = DownloadType.Unknown;
                this.btnRedDL.Text = destination;
                this.btnRedDL.Visible = true;
                if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                {
                    string argument = "/select, \"" + btnRedDL.Text + "\"";

                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
            }
        }

        private async void redditScrape(string playlistUrl, bool listFormats = false)
        {
            this.gcReddit.DataSource = null;
            this.gvReddit.RefreshData();
            this.currentDownload = DownloadType.Reddit;
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
                        List<Classes.MediaStream> dashStreamList = new List<Classes.MediaStream>();
                        List<Classes.MediaStream> hlsStreamList = new List<Classes.MediaStream>();

                        foreach (Classes.StreamLink link in streamLinks)
                        {
                            IMediaInfo m3u8contents = await Utils.VideoUtil.ParseM3U8(link.StreamUrl);
                            List<Classes.MediaStream> l = new List<Classes.MediaStream>();
                            foreach (IStream ist in m3u8contents.Streams)
                            {
                                string transform = JsonConvert.SerializeObject(ist);
                                Classes.MediaStream rs = JsonConvert.DeserializeObject<Classes.MediaStream>(transform);
                                rs.DeliverStream = ist;
                                l.Add(rs);
                            }
                            foreach (Classes.MediaStream stream in l)
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
                            Tuple<int, string> bestDash = await Utils.HtmlUtil.GetBestRedditDashVideo(id);
                            bool audioExists = await Utils.HtmlUtil.MediaExists(Utils.VideoUtil.RedditAudioUrl(id));
                            if (bestDash != null)
                            {
                                this.redditListMarquee.Text = "Preparing download..";

                                int[] crops = null;
                                if (toggleRedditCrop.IsOn)
                                {
                                    crops = new int[] { Convert.ToInt32(txtRedCropTop.Text), Convert.ToInt32(txtRedCropBottom.Text), Convert.ToInt32(txtRedCropLeft.Text), Convert.ToInt32(txtRedCropRight.Text) };
                                }

                                IMediaInfo mediaInfo = null;
                                IVideoStream videoStream = null;
                                if (!string.IsNullOrEmpty(bestDash.Item2))
                                {
                                    mediaInfo = await FFmpeg.GetMediaInfo(bestDash.Item2);
                                    videoStream = mediaInfo.VideoStreams.FirstOrDefault();
                                }

                                int outWidth = -1;
                                int outHeight = -1;
                                int x = -1;
                                int y = -1;

                                if (crops != null && crops.Length == 4 && videoStream != null)
                                {
                                    int[] ffmpegCrop = Utils.VideoUtil.ConvertCrop(crops, videoStream.Width, videoStream.Height);
                                    x = ffmpegCrop[0];
                                    y = ffmpegCrop[1];
                                    outWidth = ffmpegCrop[2];
                                    outHeight = ffmpegCrop[3];
                                }

                                List<Classes.FFmpegParam> parameters = new List<Classes.FFmpegParam>();

                                if (outWidth >= 0 && outHeight >= 0 && x >= 0 && y >= 0)
                                {
                                    parameters.Add(new Classes.FFmpegParam(Classes.ParamType.Crop, $"-filter:v \"crop={outWidth}:{outHeight}:{x}:{y}\""));
                                }

                                IConversion conversion = await Utils.VideoUtil.PrepareStreamConversion(bestDash.Item2, audioExists ? Utils.VideoUtil.RedditAudioUrl(id) : String.Empty, parameters.ToArray());
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
                                    ExceptionHandler.LogException(ex);
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
                                if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                                {
                                    string argument = "/select, \"" + btnRedDL.Text + "\"";

                                    System.Diagnostics.Process.Start("explorer.exe", argument);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Unable to determine best download", "Oops");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            lblSelect.Visible = true;
            this.UseWaitCursor = false;
            this.currentDownload = DownloadType.Unknown;
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
            gvReddit.RefreshData();
        }

        private void refreshRedditHistory()
        {
            gcHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.Reddit).ToList();
            gvHistory.Columns["FileExists"].VisibleIndex = 0;
            gvHistory.Columns["FileExists"].ColumnEdit = repCheckEdit;
            gvHistory.Columns["FileExists"].ToolTip = "File Exists";
            gvHistory.Columns["FileExists"].OptionsColumn.ShowCaption = false;
            gvHistory.Columns["FileExists"].MinWidth = 5;
            gvHistory.Columns["FileExists"].Width = 10;
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
            CustomTabFormPage page = e.Page as CustomTabFormPage;
            if (page.Name == "tfpYouTube")
            {
                this.youTubePage = page;
            }
            else if (page.Name == "tfpReddit")
            {
                this.redditPage = page;
            }

            if (page.IsLocked)
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
                btnRedditDefault.Visible = true;
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
                btnRedditDefault.Visible = false;
                btnDownloadReddit.Visible = true;
                btnDownloadReddit.Enabled = true;
            }
            else
            {
                btnDownloadReddit.Visible = false;
                btnRedditDefault.Visible = true;
            }
            updateRedditSegmentState();
        }


        private void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
        {
            var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
            safeUpdateDownloadProgress(percent);
        }

        private void safeUpdateDownloadProgress(int percent)
        {
            if (this.currentDownload == DownloadType.Reddit)
            {
                if (pbDownloadProgress.InvokeRequired)
                {
                    Action safeUpdate = delegate { pbDownloadProgress.Position = percent; };
                    pbDownloadProgress.Invoke(safeUpdate);
                }
                else
                    pbDownloadProgress.Position = percent;
            }
            else if (currentDownload == DownloadType.YouTube)
            {
                if (pbYTProgress.InvokeRequired)
                {
                    Action safeUpdate = delegate { pbYTProgress.Position = percent; };
                    pbYTProgress.Invoke(safeUpdate);
                }
                else
                    pbYTProgress.Position = percent;
            }
        }

        private void lblDLLocation_Click(object sender, EventArgs e)
        {
            string argument = "/select, \"" + btnRedDL.Text + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void tabFormControl1_SelectedPageChanged(object sender, DevExpress.XtraBars.TabFormSelectedPageChangedEventArgs e)
        {
            if (tcMainTabControl.SelectedPage != null && tcMainTabControl.SelectedPage.Text == "Reddit")
            {
                gcHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.Reddit).ToList();
                refreshRedditHistory();
                if (gvHistory.RowCount > 0)
                    gvHistory.FocusedRowHandle = 0;
            }
        }

        private void gvHistory_DoubleClick(object sender, EventArgs e)
        {
            if (selectedRedditLog != null)
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

        private void toggleRedSegment_Toggled(object sender, EventArgs e)
        {
            updateRedditSegmentState();
        }

        private void updateRedditSegmentState()
        {
            tsRedStart.Enabled = toggleRedSegment.IsOn;
            tsRedDuration.Enabled = toggleRedSegment.IsOn;
            lblRedSegDisclaimer.Visible = toggleRedSegment.IsOn && (gvReddit.RowCount < 1 || selectedStream == null);
            btnRedditList.Invalidate();
            btnRedditDefault.Enabled = !toggleRedSegment.IsOn;
            gcRedSegment.CustomHeaderButtons[0].Properties.Enabled = !toggleRedSegment.IsOn && (gvReddit.RowCount < 1 || selectedStream == null);
        }

        #endregion

        #region YOUTUBE
        private void toggleYTSegment_Toggled(object sender, EventArgs e)
        {
            updateYTSegmentState();
        }

        private void updateYTSegmentState()
        {
            tsYTStart.Enabled = toggleYTSegment.IsOn;
            tsYTEnd.Enabled = toggleYTSegment.IsOn;
            lblSegmentDisclaimer.Visible = toggleYTSegment.IsOn && (gvYouTube.RowCount < 1 || selectedFormat == null);
            btnYTListFormats.Invalidate();
            btnYTDownloadBest.Enabled = !toggleYTSegment.IsOn && (gvYouTube.RowCount < 1 || selectedFormat == null);
            gcYTSegments.CustomHeaderButtons[0].Properties.Enabled = !toggleYTSegment.IsOn;
        }

        private void toggleYTCrop_Toggled(object sender, EventArgs e)
        {
            updateYTCropState();
        }

        private void updateYTCropState()
        {
            teYTCropTop.Enabled = toggleYTCrop.IsOn;
            teYTCropBottom.Enabled = toggleYTCrop.IsOn;
            teYTCropLeft.Enabled = toggleYTCrop.IsOn;
            teYTCropRight.Enabled = toggleYTCrop.IsOn;
            btnYTListFormats.Invalidate();
            btnYTDownloadAudio.Enabled = !toggleYTCrop.IsOn && gvYouTube.RowCount < 1;
            gcYTCrop.CustomHeaderButtons[0].Properties.Enabled = !toggleYTCrop.IsOn;
        }

        private void btnYTListFormats_Click(object sender, EventArgs e)
        {
            doYTFormatList();
        }

        private void doYTFormatList()
        {
            if (!string.IsNullOrEmpty(txtYTUrl.Text))
            {
                if (HtmlUtil.CheckUrl(txtYTUrl.Text) == DownloadType.YouTube)
                    getYTFormats(VideoUtil.YouTubeString(txtYTUrl.Text));
                else if (HtmlUtil.CheckUrl(txtYTUrl.Text) == DownloadType.Reddit)
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
                this.UseWaitCursor = false;
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
                this.ytMarquee.Hide();
                this.ytMarquee.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
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
            gcYTHistory.DataSource = null;
            gcYTHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.YouTube).ToList();
            gvYTHistory.Columns["FileExists"].VisibleIndex = 0;
            gvYTHistory.Columns["FileExists"].ColumnEdit = repCheckEdit;
            gvYTHistory.Columns["FileExists"].OptionsColumn.ShowCaption = false;
            gvYTHistory.Columns["FileExists"].MinWidth = 5;
            gvYTHistory.Columns["FileExists"].Width = 10;
            gvYTHistory.Columns["FileExists"].ToolTip = "File Exists?";
            gvYTHistory.Columns["DownloadType"].Width = 10;
            gvYTHistory.Columns["DownloadType"].Caption = "Type";
            gvYTHistory.Columns["FileName"].Visible = false;
            gvYTHistory.Columns["TimeLogged"].Visible = false;
            gvYTHistory.Columns["Type"].Visible = false;
            gvYTHistory.Columns["Downloaded"].Visible = false;
            gvYTHistory.RefreshData();
            selectedYTLog = gvYTHistory.GetRow(0) as DownloadLog;
        }

        private void gvYouTube_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            btnYTSelectionDL.Visible = e.FocusedRowHandle >= 0;
            btnYTSelectionDL.Enabled = e.FocusedRowHandle >= 0;
            btnYTDownloadAudio.Enabled = e.FocusedRowHandle < 0;
            btnYTDownloadBest.Enabled = e.FocusedRowHandle < 0;
            if (e.FocusedRowHandle < 0)
            {
                lblYTSelectionText.Text = string.Empty;
            }
            else
            {
                YoutubeDLSharp.Metadata.FormatData fd = gvYouTube.GetFocusedRow() as YoutubeDLSharp.Metadata.FormatData;
                lblYTSelectionText.Text = fd.Format;
                selectedFormat = fd;
            }
            updateYTSegmentState();
        }

        private void showYTProgress(DownloadProgress progress)
        {
            if (downloadingSegment || downloadingCropped)
                return;
            if (!pbYTProgress.Visible)
            {
                pbYTProgress.Show();
            }
            pbYTProgress.Position = Convert.ToInt32(progress.Progress * 100);
            if (progress.State == DownloadState.Success)
            {
                pbYTProgress.Position = 0;
                pbYTProgress.Hide();
            }
        }

        private async void btnYTSelectionDL_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            this.currentDownload = DownloadType.YouTube;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
            btnYTOpenDL.Text = String.Empty;
            btnYTOpenDL.Visible = false;
            ytMarquee.Text = "Preparing Download..";
            ytMarquee.Show();
            btnYTDownloadAudio.Enabled = false;
            btnYTDownloadBest.Enabled = false;
            RunResult<string> result = null;

            if (toggleYTSegment.IsOn || toggleYTCrop.IsOn)
            {
                if (toggleYTSegment.IsOn && this.tsYTEnd.TimeSpan == TimeSpan.Zero)
                {
                    MessageBox.Show("Please specify a valid duration for the segment", "Invalid Duration");
                    return;
                }
                downloadingSegment = true;
                int[] crops = null;
                if (toggleYTCrop.IsOn)
                {
                    crops = new int[] { Convert.ToInt32(teYTCropTop.Text), Convert.ToInt32(teYTCropBottom.Text), Convert.ToInt32(teYTCropLeft.Text), Convert.ToInt32(teYTCropRight.Text) };
                }
                TimeSpan? start = null;
                TimeSpan? duration = null;
                if (toggleYTSegment.IsOn)
                {
                    start = tsYTStart.TimeSpan;
                    duration = tsYTEnd.TimeSpan;
                }

                IConversion conversion = await Utils.VideoUtil.PrepareYoutubeConversion(VideoUtil.YouTubeString(txtYTUrl.Text), selectedFormat, start, duration, chkUsePrefs.Checked, crops);
                string destination = conversion.OutputFilePath;
                conversion.OnProgress += Conversion_OnProgress;
                this.pbYTProgress.Visible = true;
                try
                {
                    await conversion.Start();
                    result = new RunResult<string>(true, new string[] { }, destination);
                }
                catch (Exception ex)
                {
                    result = new RunResult<string>(false, new string[] { ex.Message }, null);
                }
                this.pbYTProgress.Hide();
                downloadingSegment = false;
            }
            else
            {
                result = await Utils.VideoUtil.DownloadYTFormat(VideoUtil.YouTubeString(txtYTUrl.Text), selectedFormat);

                if (!result.Success)
                {
                    MessageBox.Show("Download Failed");
                }
            }

            YT_RED.Classes.StreamType t = Classes.StreamType.Audio;
            if (selectedFormat.AudioCodec == "none")
                t = Classes.StreamType.Video;
            else if (selectedFormat.Resolution == "audio only")
                t = Classes.StreamType.Audio;
            else
                t = Classes.StreamType.AudioAndVideo;
            ytMarquee.Hide();
            ytMarquee.Text = "";
            if (result.Success)
            {
                await Historian.RecordDownload(new DownloadLog(
                    DownloadType.YouTube,
                    VideoUtil.YouTubeString(txtYTUrl.Text),
                    t, DateTime.Now,
                    result.Data
                    ));
                btnYTOpenDL.Text = result.Data;
                btnYTOpenDL.Visible = true;
                if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                {
                    string argument = "/select, \"" + result.Data + "\"";

                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
            }
            gcYTHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.YouTube).ToList();
            refreshYoutubeHistory();
            lblYTSelectionText.Text = String.Empty;
            this.UseWaitCursor = false;
            this.currentDownload = DownloadType.Unknown;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
        }

        private async void btnDownloadAudio_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtYTUrl.Text))
            {
                if (HtmlUtil.CheckUrl(txtYTUrl.Text) == DownloadType.YouTube)
                {
                    this.UseWaitCursor = true;
                    this.currentDownload = DownloadType.YouTube;
                    (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
                    btnYTOpenDL.Text = String.Empty;
                    btnYTOpenDL.Visible = false;
                    ytMarquee.Text = "Sending Download Request..";
                    ytMarquee.Show();
                    RunResult<string> result = null;
                    if (toggleYTSegment.IsOn)
                    {
                        if (this.tsYTEnd.TimeSpan == TimeSpan.Zero)
                        {
                            MessageBox.Show("Please specify a valid duration for the segment", "Invalid Duration");
                            return;
                        }
                        downloadingSegment = true;
                        IConversion conversion = await Utils.VideoUtil.PrepareYoutubeAudioConversion(VideoUtil.YouTubeString(txtYTUrl.Text), this.tsYTStart.TimeSpan, this.tsYTEnd.TimeSpan, chkUsePrefs.Checked);
                        string destination = conversion.OutputFilePath;
                        conversion.OnProgress += Conversion_OnProgress;
                        this.pbYTProgress.Visible = true;
                        try
                        {
                            await conversion.Start();
                            result = new RunResult<string>(true, new string[] { }, destination);
                        }
                        catch (Exception ex)
                        {
                            result = new RunResult<string>(false, new string[] { ex.Message }, null);
                        }
                        downloadingSegment = false;
                        this.pbYTProgress.Hide();
                    }
                    else
                    {

                        if (chkUsePrefs.Checked)
                            result = await Utils.VideoUtil.DownloadPreferred(VideoUtil.YouTubeString(txtYTUrl.Text), Classes.StreamType.Audio);
                        else
                            result = await Utils.VideoUtil.DownloadBestYT(VideoUtil.YouTubeString(txtYTUrl.Text), Classes.StreamType.Audio);
                        if (!result.Success)
                        {
                            MessageBox.Show("Download Failed");
                        }
                    }

                    if (result.Success)
                    {
                        await Historian.RecordDownload(new DownloadLog(
                            DownloadType.YouTube,
                            VideoUtil.YouTubeString(txtYTUrl.Text),
                            YT_RED.Classes.StreamType.Audio, DateTime.Now,
                            result.Data
                            ));
                        btnYTOpenDL.Text = result.Data;
                        btnYTOpenDL.Visible = true;
                        if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                        {
                            string argument = "/select, \"" + result.Data + "\"";

                            System.Diagnostics.Process.Start("explorer.exe", argument);
                        }
                    }
                    ytMarquee.Hide();
                    ytMarquee.Text = "";
                    gcYTHistory.DataSource = Historian.DownloadHistory.Where(h => h.DownloadType == DownloadType.YouTube).ToList();
                    refreshYoutubeHistory();
                    this.UseWaitCursor = false;
                    this.currentDownload = DownloadType.Unknown;
                    (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
                    return;
                }
            }
            MessageBox.Show("The url provided is not a valid Youtube url", "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        private void btnYTDownloadBest_Click(object sender, EventArgs e)
        {
            doYTBest();
        }

        private void doYTBest()
        {
            if (!string.IsNullOrEmpty(txtYTUrl.Text))
            {
                if (HtmlUtil.CheckUrl(txtYTUrl.Text) == DownloadType.YouTube)
                {
                    ytDownloadBest(txtYTUrl.Text);
                    return;
                }
                else if (HtmlUtil.CheckUrl(txtYTUrl.Text) == DownloadType.Reddit)
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
            this.currentDownload = DownloadType.YouTube;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
            btnYTOpenDL.Text = String.Empty;
            btnYTOpenDL.Visible = false;
            ytMarquee.Text = "Sending Download Request..";
            ytMarquee.Show();
            int[] crops = null;
            if (toggleYTCrop.IsOn)
            {
                crops = new int[] { Convert.ToInt32(teYTCropTop.Text), Convert.ToInt32(teYTCropBottom.Text), Convert.ToInt32(teYTCropLeft.Text), Convert.ToInt32(teYTCropRight.Text) };
            }
            RunResult<string> result = null;
            if (crops != null)
            {
                this.downloadingCropped = true;
                IConversion conversion = await VideoUtil.PrepareYoutubeConversion(url, "bestvideo+bestaudio", null, null, true, crops);
                string destination = conversion.OutputFilePath;
                conversion.OnProgress += Conversion_OnProgress;
                this.pbYTProgress.Visible = true;
                try
                {
                    await conversion.Start();
                    result = new RunResult<string>(true, new string[] { }, destination);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex);
                    result = new RunResult<string>(false, new string[] { ex.Message }, null);
                }
                this.pbYTProgress.Hide();
                this.downloadingCropped = false;
            }
            else
            {
                if (chkUsePrefs.Checked)
                {
                    result = await Utils.VideoUtil.DownloadPreferred(VideoUtil.YouTubeString(url), Classes.StreamType.AudioAndVideo);
                }
                else
                {                    
                    result = await Utils.VideoUtil.DownloadBestYT(VideoUtil.YouTubeString(url), Classes.StreamType.AudioAndVideo);                    
                }
            }
            if (!result.Success)
            {
                MessageBox.Show("Download Failed\n" + String.Join("\n", result.ErrorOutput));
            }
            ytMarquee.Hide();
            ytMarquee.Text = "";
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
            if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
            {
                string argument = "/select, \"" + result.Data + "\"";

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            this.UseWaitCursor = false;
            this.currentDownload = DownloadType.Unknown;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
            return;
        }

        private void lblYTSelectionText_SizeChanged(object sender, EventArgs e)
        {
            pnlYTOptionPanel.Height = gcYTSegments.MinimumSize.Height + lblYTSelectionText.Height;
        }

        private void bbiSettings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            openSettings();
        }

        private async void openSettings(OpenPosition openPosition = OpenPosition.Unspecified)
        {
            SettingsDialog dlg = new SettingsDialog();
            if(openPosition != OpenPosition.Unspecified)
            {
                if (openPosition == OpenPosition.BottomRight)
                {
                    Rectangle workingArea = Screen.GetWorkingArea(this);
                    var loc = new Point(workingArea.Right - (dlg.Size.Width + 100), workingArea.Bottom - (dlg.Size.Height + 100));
                    dlg.StartPosition = FormStartPosition.Manual;
                    dlg.Location = loc;
                }
                else
                {
                    // not supported yet
                }
            }
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
            if (e.Column.FieldName == "FileSize")
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

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            if (e.Info == null && e.SelectedControl is GridControl)
            {
                GridView view = (e.SelectedControl as GridControl).FocusedView as GridView;
                GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                if (info.InRowCell && info.Column.FieldName == "FileExists")
                {
                    bool exists = Convert.ToBoolean(view.GetRowCellValue(info.RowHandle, "FileExists"));
                    string text = exists ? "File Exists" : "File Not Found";
                    string cellKey = info.RowHandle.ToString() + " - " + info.Column.ToString();
                    e.Info = new ToolTipControlInfo(cellKey, text);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppSettings.Default.General.ActiveSkin = this.LookAndFeel.ActiveSkinName;
            AppSettings.Default.Save();
        }

        private void btnListReset_Click(object sender, EventArgs e)
        {
            this.gcYoutube.DataSource = null;
            this.gvYouTube.RefreshData();
            updateYTSegmentState();
            updateYTCropState();
        }

        private void btnRedditReset_Click(object sender, EventArgs e)
        {
            this.gcReddit.DataSource = null;
            this.gvReddit.RefreshData();
            updateRedditSegmentState();
        }

        private void chkUsePrefs_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.Default.General.UsePreferredFormat = chkUsePrefs.Checked;
            AppSettings.Default.Save();
        }

        private void btnRedditList_Paint(object sender, PaintEventArgs e)
        {
            if (toggleRedSegment.IsOn && (gvReddit.RowCount < 1 || selectedStream == null))
            {
                e.Graphics.DrawRectangle(new Pen(Color.Red), new Rectangle(0, 0, btnRedditList.Width - 1, btnRedditList.Height - 1));
            }
            else
            {
                base.OnPaint(e);
            }
        }

        private void btnYTListFormats_Paint(object sender, PaintEventArgs e)
        {
            if (toggleYTSegment.IsOn && (gvYouTube.RowCount < 1 || selectedFormat == null))
            {
                e.Graphics.DrawRectangle(new Pen(Color.Red), new Rectangle(0, 0, btnYTListFormats.Width - 1, btnYTListFormats.Height - 1));
            }
            else
            {
                base.OnPaint(e);
            }
        }

        private void gcYTSegments_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (pnlYTSegPanel.Visible)
            {
                pnlYTSegPanel.Visible = false;
                e.Button.Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
            }
            else
            {
                pnlYTSegPanel.Visible = true;
                e.Button.Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
            }
        }

        private void gcYTCrop_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (pnlYTCropPanel.Visible)
            {
                pnlYTCropPanel.Visible = false;
                e.Button.Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
            }
            else
            {
                pnlYTCropPanel.Visible = true;
                e.Button.Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
            }
        }

        private void gcRedCrop_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (pnlRedCropPanel.Visible)
            {
                pnlRedCropPanel.Visible = false;
                e.Button.Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
            }
            else
            {
                pnlRedCropPanel.Visible = true;
                e.Button.Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
            }
        }

        private void gcRedSegment_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (pnlRedSegPanel.Visible)
            {
                pnlRedSegPanel.Visible = false;
                e.Button.Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
            }
            else
            {
                pnlRedSegPanel.Visible = true;
                e.Button.Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
            }
        }

        private void txtYTUrl_EditValueChanged(object sender, EventArgs e)
        {
            if (gvYouTube.RowCount > 0)
            {
                this.gcYoutube.DataSource = null;
                this.gvYouTube.RefreshData();
                updateYTSegmentState();
                updateYTCropState();
            }
        }

        private void txtRedditPost_EditValueChanged(object sender, EventArgs e)
        {
            if (gvReddit.RowCount > 0)
            {
                this.gcReddit.DataSource = null;
                this.gvReddit.RefreshData();
                updateRedditSegmentState();
                updateRedditCropState();
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }

        private TrayForm activeTrayForm = null;
        private void tsiDownload_Click(object sender, EventArgs e)
        {
            if (activeTrayForm == null)
            {
                try
                {
                    using (TrayForm trayForm = new TrayForm())
                    {
                        activeTrayForm = trayForm;
                        trayForm.FormClosed += TrayForm_FormClosed;
                        trayForm.StartPosition = FormStartPosition.Manual;
                        Rectangle workingArea = Screen.GetWorkingArea(this);                        
                        var loc = new Point(workingArea.Right - trayForm.Size.Width, workingArea.Bottom - trayForm.Size.Height);
                        trayForm.HideProgressPanel();
                        trayForm.Location = loc;
                        trayForm.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex);
                }
            }
        }

        private void TrayForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            activeTrayForm = null;
        }

        private void tsiExit_Click(object sender, EventArgs e)
        {
            if (IsLocked || (activeTrayForm != null && activeTrayForm.Locked))
            {
                DialogResult res = MessageBox.Show("A download is in progress.\nAre you sure you want to exit?", "Download In Progress", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res != DialogResult.OK)
                    return;
            }

            Application.Exit();
        }

        private void toggleRedditCrop_Toggled(object sender, EventArgs e)
        {
            updateRedditCropState();
        }

        private void updateRedditCropState()
        {
            txtRedCropBottom.Enabled = toggleRedditCrop.IsOn;
            txtRedCropLeft.Enabled = toggleRedditCrop.IsOn;
            txtRedCropRight.Enabled = toggleRedditCrop.IsOn;
            txtRedCropTop.Enabled = toggleRedditCrop.IsOn;
            btnRedditList.Invalidate();
            gcRedCrop.CustomHeaderButtons[0].Properties.Enabled = !toggleRedditCrop.IsOn;
        }

        private void tsiSettings_Click(object sender, EventArgs e)
        {
            openSettings(OpenPosition.BottomRight);
        }

        private async void txtRedCropBottom_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.P && e.Modifiers == (Keys.Control | Keys.Shift))
            {
                using (DevExpress.XtraEditors.XtraForm tempForm = new DevExpress.XtraEditors.XtraForm())
                {
                    tempForm.Parent = this;
                    tempForm.Size = new System.Drawing.Size(302, 145);
                    tempForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    tempForm.ControlBox = false;
                    tempForm.StartPosition = FormStartPosition.CenterParent;
                    DevExpress.XtraEditors.PictureEdit pictureEdit = new DevExpress.XtraEditors.PictureEdit();
                    pictureEdit.Image = Properties.Resources.CDM;
                    pictureEdit.Properties.PictureAlignment = ContentAlignment.MiddleCenter;
                    pictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    pictureEdit.Size = new Size(302, 125);
                    pictureEdit.MinimumSize = new Size(302, 125);
                    pictureEdit.Dock = DockStyle.Fill;
                    pictureEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                    pictureEdit.Margin = new System.Windows.Forms.Padding(0);
                    tempForm.Controls.Add(pictureEdit);
                    tempForm.Show();
                    await Task.Delay(2000);
                    tempForm.Close();
                }
            }
        }
    }
}
