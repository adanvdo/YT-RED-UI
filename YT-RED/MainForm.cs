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
        private YoutubeDLSharp.Metadata.FormatData selectedFormat = null;
        public YoutubeDLSharp.Metadata.FormatData SelectedFormat { get { return selectedFormat; } }
        public int FormatCount { get { return gvFormats.RowCount; } }
        private DownloadType currentDownload = DownloadType.Unknown;
        private bool downloadingSegment = false;
        private bool downloadingCropped = false;
        private bool quickDownloadInProgress = false;
        private InitialFunction initialFunction = InitialFunction.None;
        private string initialLink = string.Empty;
        private DownloadType initialDownloadType = DownloadType.Unknown;
        private MediaSource initialSource = MediaSource.None;
        private URISchemeService uriService = null;
        private bool enableQuickDownload = false;
        private bool trayBalloonShown = false;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repHistoryCheckEdit;

        static KeyboardHook hook = new KeyboardHook();

        public MainForm()
        {
            InitializeComponent();
            this.initialFunction = InitialFunction.None;
            this.initialLink = "";
            this.initialSource = MediaSource.None;
            cpMainControlPanel.ParentMainForm = this;
            repHistoryCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repHistoryCheckEdit.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Custom;
            repHistoryCheckEdit.ImageOptions.SvgImageChecked = Properties.Resources.actions_checkcircled;
            repHistoryCheckEdit.ImageOptions.SvgImageUnchecked = Properties.Resources.security_warningcircled2;
            repHistoryCheckEdit.ImageOptions.SvgImageSize = new Size(20, 20);
        }

        public MainForm(InitialFunction initialFunction, string initialLink, Classes.MediaSource mediaSource)
        {
            InitializeComponent();
            this.initialFunction = initialFunction;
            this.initialLink = initialLink;
            this.initialSource = mediaSource;
            cpMainControlPanel.ParentMainForm = this;
            repHistoryCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repHistoryCheckEdit.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Custom;
            repHistoryCheckEdit.ImageOptions.SvgImageChecked = Properties.Resources.actions_checkcircled;
            repHistoryCheckEdit.ImageOptions.SvgImageUnchecked = Properties.Resources.security_warningcircled2;
            repHistoryCheckEdit.ImageOptions.SvgImageSize = new Size(20, 20);
        }

        private void initializeProgram()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            AppSettings.Default.About.Version = assembly.GetName().Version.ToString();
            AppSettings.Default.About.Build = assembly.GetCustomAttributes(typeof(AssemblyBuildAttribute), false).Cast<AssemblyBuildAttribute>().FirstOrDefault().Value;
            MainForm.hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(Hook_KeyPressed);
            MainForm.UpdateDownloadHotkey();
            this.historyTimer = new Timer();
            this.historyTimer.Interval = 10000;
            this.historyTimer.Tick += HistoryTimer_Tick;
            if (AppSettings.Default.General.EnableDownloadHistory)
                this.historyTimer.Start();
            Historian.Init();
            Init();
            _blockDetector = new UIBlockDetector();
        }

        public static void UpdateDownloadHotkey()
        {
            MainForm.hook.UnregisterHotKey();
            if (AppSettings.Default.Advanced.EnableHotKeys && AppSettings.Default.Advanced.DownloadHotKey != Shortcut.None)
            {
                KeyShortcut shortcut = new KeyShortcut(AppSettings.Default.Advanced.DownloadHotKey);
                string[] listKeys = shortcut.Key.ToString().Replace(" ", "").Split(',');

                string modifierKeys = "";
                string keys = "";

                ModifierKeysConverter modifierKeysConverter = new ModifierKeysConverter();
                KeysConverter keysConverter = new KeysConverter();


                for (int i = listKeys.Length - 1; i >= 0; i--)
                {
                    if (modifierKeysConverter.IsValid(listKeys[i]))
                        modifierKeys += $"{listKeys[i]}+";
                    else if (keysConverter.IsValid(listKeys[i]))
                        keys += $"{listKeys[i]}+";
                }
                modifierKeys = modifierKeys.Remove(modifierKeys.Length - 1, 1);
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

                if (HtmlUtil.CheckUrl(copiedText) == DownloadType.Unknown && AppSettings.Default.General.ShowHostWarning)
                {
                    DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
                    if (res == DialogResult.No)
                        return;
                }

                if (activeTrayForm == null)
                {
                    try
                    {
                        TrayForm trayForm = new TrayForm();
                        activeTrayForm = trayForm;
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
                else if(!activeTrayForm.Locked)
                {
                    activeTrayForm.Url = copiedText;
                    activeTrayForm.BringToFront();
                    activeTrayForm.TopMost = true;
                    activeTrayForm.TriggerDownload();
                }
            }
        }

        private async void HistoryTimer_Tick(object sender, EventArgs e)
        {
            if (AppSettings.Default.General.EnableDownloadHistory)
            {
                await Historian.CleanHistory();
                refreshHistory();
            }
        }

        protected override async void OnLoad(EventArgs e)
        {
            initializeProgram();
            if (Program.DevRun)
            {
                ipMainInput.URL = AppSettings.Default.General.YouTubeSampleUrl;
            }
            if(Program.initialFunction == InitialFunction.UploadTest)
            {
                bool uploaded = await HttpUtil.UploadErrorLogs(1);
                if (!uploaded)
                    MsgBox.Show("Log Upload Failed", FormStartPosition.CenterParent);
            }
            base.OnLoad(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            this.notifyIcon.Visible = !this.Visible;
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                enableQuickDownload = true;
                notifyIcon.Visible = true;
                if (!trayBalloonShown)
                {
                    notifyIcon.ShowBalloonTip(3000);
                    trayBalloonShown = true;
                }
                this.ShowInTaskbar = false;
            }
            else
            {
                enableQuickDownload = false;
            }
            base.OnResize(e);
        }

        private async void Init()
        {
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(AppSettings.Default.General.ActiveSkin);
            if (AppSettings.Default.General.EnableDownloadHistory)
            {
                bool loadDownloadHistory = await Historian.LoadDownloadHistory();
                if (loadDownloadHistory)
                {
                    refreshHistory();
                }                
            }
            gcFormats.DataSource = new List<YTDLFormatData>();
            gvFormats.PopulateColumns();
            refreshFormatGrid(DownloadType.YouTube);
            VideoUtil.Init();
            VideoUtil.ytProgress = new Progress<DownloadProgress>(updateProgress);
            VideoUtil.ytOutput = new Progress<string>(processOutput);
            if(!Program.DevRun)
                ipMainInput.URL = initialLink;

            if(initialDownloadType == DownloadType.Unknown)
            {
                return;
            }
            else if(initialDownloadType == DownloadType.Reddit)
            {
                if (initialFunction == InitialFunction.ListFormats)
                    return;
                else if (initialFunction == InitialFunction.DownloadBest)
                    return;
            }
            else
            {
                if (initialFunction == InitialFunction.ListFormats)
                    getYtdlFormatList(ipMainInput.URL);
                else
                    return;
            }
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
            cpMainControlPanel.gcHistory.DataSource = Historian.DownloadHistory;
            refreshHistory();
            cpMainControlPanel.UpdatePanelStates();
            if (res == DialogResult.OK)
            {
                bsiMessage.Caption = "Settings Saved";
                await Task.Delay(3000);
                bsiMessage.Caption = String.Empty;
            }
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

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
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
                        trayForm.HideProgressPanel();
                        var loc = new Point(workingArea.Right - trayForm.Size.Width, workingArea.Bottom - (trayForm.Size.Height - 81));
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
                DialogResult res = MsgBox.Show("A download is in progress.\nAre you sure you want to exit?", "Download In Progress", YT_RED.Controls.Buttons.OKCancel, YT_RED.Controls.Icon.Warning);
                if (res != DialogResult.OK)
                    return;
            }

            Application.Exit();
        }

        private void tsiSettings_Click(object sender, EventArgs e)
        {
            openSettings(OpenPosition.BottomRight);
        }

        private async void ipMainInput_Url_Changed(object sender, EventArgs e)
        {
            gcFormats.DataSource = null;
            gvFormats.RefreshData();
            cpMainControlPanel.ResetControls();
            if(ipMainInput.URL == "crab")
            {
                ipMainInput.ShowCrab = true;
                await Task.Delay(3000);
                ipMainInput.ShowCrab = false;
            }
        }

        private void ipMainInput_ResetList_Click(object sender, EventArgs e)
        {
            gcFormats.DataSource = null;
            gvFormats.RefreshData();
            cpMainControlPanel.ResetControls();
        }

        private void ipMainInput_ListFormats_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ipMainInput.URL))
            {
                getYtdlFormatList(ipMainInput.URL);                
            }
        }

        private void cpMainControlPanel_DownloadBest_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ipMainInput.URL))
            {
                cpMainControlPanel.CurrentFormat = null;
                ytdlDownloadBest(ipMainInput.URL);
            }
        }

        private void cpMainControlPanel_DownloadAudio_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ipMainInput.URL))
            {
                cpMainControlPanel.CurrentFormat = new YTDLFormatData() { AudioCodec = "best" };
                ytdlDownloadBest(ipMainInput.URL, Classes.StreamType.Audio);
            }
        }

        private void cpMainControlPanel_DownloadSelection_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ipMainInput.URL))
            {
                ytdlDownloadSelection();
            }
        }

        private void refreshFormatGrid(DownloadType downloadType)
        {
            if (downloadType == DownloadType.Unknown)
                return;

            gvFormats.Columns.Clear();
            gvFormats.PopulateColumns();
            gvFormats.Columns["Format"].BestFit();
            gvFormats.Columns["RedditAudioFormat"].Visible = false;
            gvFormats.Columns["Url"].Visible = false;
            gvFormats.Columns["ManifestUrl"].Visible = false;
            gvFormats.Columns["FormatId"].Visible = false;
            gvFormats.Columns["FormatNote"].Visible = false;
            gvFormats.Columns["Resolution"].Visible = false;
            gvFormats.Columns["ContainerFormat"].Visible = false;
            gvFormats.Columns["AudioBitrate"].Visible = false;
            gvFormats.Columns["Extension"].Visible = false;
            gvFormats.Columns["FragmentBaseUrl"].Visible = false;
            gvFormats.Columns["Language"].Visible = false;
            gvFormats.Columns["LanguagePreference"].Visible = false;
            gvFormats.Columns["NoResume"].Visible = false;
            gvFormats.Columns["PlayerUrl"].Visible = false;
            gvFormats.Columns["Preference"].Visible = false;
            gvFormats.Columns["Protocol"].Visible = false;
            gvFormats.Columns["Quality"].Visible = false;
            gvFormats.Columns["SourcePreference"].Visible = false;
            gvFormats.Columns["Width"].Visible = false;
            gvFormats.Columns["Height"].Visible = false;
            gvFormats.Columns["StretchedRatio"].Visible = false;
            gvFormats.Columns["ApproximateFileSize"].Visible = false;
            gvFormats.Columns["Duration"].VisibleIndex = 3;
            gvFormats.RefreshData();
        }

        private void refreshHistory()
        {
            cpMainControlPanel.gcHistory.DataSource = null;
            cpMainControlPanel.gcHistory.DataSource = Historian.DownloadHistory;
            cpMainControlPanel.gvHistory.Columns.Clear();
            cpMainControlPanel.gvHistory.PopulateColumns();
            cpMainControlPanel.gvHistory.Columns["FileExists"].VisibleIndex = 0;
            cpMainControlPanel.gvHistory.Columns["FileExists"].ColumnEdit = repHistoryCheckEdit;
            cpMainControlPanel.gvHistory.Columns["FileExists"].FieldName = "FileExists";
            cpMainControlPanel.gvHistory.Columns["FileExists"].OptionsColumn.ShowCaption = false;
            cpMainControlPanel.gvHistory.Columns["FileExists"].MinWidth = 5;
            cpMainControlPanel.gvHistory.Columns["FileExists"].Width = 10;
            cpMainControlPanel.gvHistory.Columns["FileExists"].ToolTip = "File Exists?";
            cpMainControlPanel.gvHistory.Columns["DownloadType"].Width = 10;
            cpMainControlPanel.gvHistory.Columns["DownloadType"].Caption = "Type";
            cpMainControlPanel.gvHistory.Columns["FileName"].Visible = false;
            cpMainControlPanel.gvHistory.Columns["TimeLogged"].Visible = false;
            cpMainControlPanel.gvHistory.Columns["Type"].Visible = false;
            cpMainControlPanel.gvHistory.Columns["Downloaded"].Visible = false;
            cpMainControlPanel.gvHistory.RefreshData();
            selectedYTLog = cpMainControlPanel.gvHistory.GetRow(0) as DownloadLog;
        }

        private void updateProgress(DownloadProgress progress)
        {
            if (cpMainControlPanel.SegmentEnabled || cpMainControlPanel.CropEnabled)
                return;

            var percent = (int)(Math.Round(progress.Progress * 100));
            cpMainControlPanel.UpdateProgress(percent, true);
        }        

        public void processOutput(string output)
        {
            ipMainInput.marqeeMain.Text = output;
        }

        private void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
        {
            var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
            cpMainControlPanel.UpdateProgress(percent, true);
        }

        private async void getYtdlFormatList(string url)
        {
            try
            {
                this.currentDownload = HtmlUtil.CheckUrl(ipMainInput.URL);
                if (this.currentDownload == DownloadType.Unknown && AppSettings.Default.General.ShowHostWarning)
                {
                    DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
                    if (res == DialogResult.No)
                        return;
                }
                this.UseWaitCursor = true;
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
                ipMainInput.marqeeMain.Text = "Fetching Available Formats";
                ipMainInput.marqeeMain.Show();
                var data = await VideoUtil.GetVideoData(url);                
                var formatList = data.Formats.Where(f => !YTDLFormatData.ExcludeFormatIDs.Contains(f.FormatId))
                    .OrderBy(f => f.VideoCodec == "none" || f.VideoCodec == "" || f.VideoCodec == null ? 0 : 1)
                    .ThenBy(f => f.Height).ToList();
                List<YTDLFormatData> converted = new List<YTDLFormatData>();
                foreach (YoutubeDLSharp.Metadata.FormatData format in formatList)
                {
                    converted.Add(new YTDLFormatData(format, data.Duration));
                }
                if (this.currentDownload == DownloadType.Reddit)
                {
                    var audio = data.Formats.Where(f => f.AudioCodec != null && f.AudioCodec != "none").First();
                    List<YoutubeDLSharp.Metadata.FormatData> supplemented = new List<YoutubeDLSharp.Metadata.FormatData>();
                    foreach (var formatItem in data.Formats.Where(f => f.VideoCodec != null && f.VideoCodec != "none"))
                    {
                        YoutubeDLSharp.Metadata.FormatData item = formatItem;
                        converted.Add(new YTDLFormatData(item, data.Duration, audio));
                    }                    
                }
                gcFormats.DataSource = converted;
                refreshFormatGrid(this.currentDownload);
                this.UseWaitCursor = false;
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
                ipMainInput.marqeeMain.Hide();
                ipMainInput.marqeeMain.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
        }

        private async void ytdlDownloadBest(string url, Classes.StreamType streamType = Classes.StreamType.AudioAndVideo)
        {
            this.currentDownload = HtmlUtil.CheckUrl(ipMainInput.URL);
            if (this.currentDownload == DownloadType.Unknown && AppSettings.Default.General.ShowHostWarning)
            {
                DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
                if (res == DialogResult.No)
                    return;
            }

            this.UseWaitCursor = true;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
            ipMainInput.marqeeMain.Text = "Sending Download Request..";
            ipMainInput.marqeeMain.Show();
            int[] crops = null;
            TimeSpan? start = null;
            TimeSpan? duration = null;
            VideoFormat? videoFormat = null;
            AudioFormat? audioFormat = null;

            if (cpMainControlPanel.SegmentEnabled)
            {
                start = cpMainControlPanel.SegmentStart;
                duration = cpMainControlPanel.SegmentDuration;
            }

            if (streamType == Classes.StreamType.AudioAndVideo && cpMainControlPanel.CropEnabled)
            {
                crops = new int[] { Convert.ToInt32(cpMainControlPanel.CropTop), Convert.ToInt32(cpMainControlPanel.CropBottom), Convert.ToInt32(cpMainControlPanel.CropLeft), 
                    Convert.ToInt32(cpMainControlPanel.CropRight) };
            }

            if (cpMainControlPanel.ConversionEnabled)
            {
                videoFormat = cpMainControlPanel.ConvertVideoFormat == null ? VideoFormat.UNSPECIFIED : cpMainControlPanel.ConvertVideoFormat;
                audioFormat = cpMainControlPanel.ConvertAudioFormat == null ? AudioFormat.UNSPECIFIED : cpMainControlPanel.ConvertAudioFormat;
            }

            RunResult<string> result = null;
            if (cpMainControlPanel.PostProcessingEnabled)
            {
                if (streamType != Classes.StreamType.Audio)
                {
                    this.downloadingCropped = true;
                    IConversion conversion = await VideoUtil.PrepareBestYtdlConversion(url, "bestvideo+bestaudio", start, duration, 
                        AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, crops, videoFormat == null ? VideoFormat.UNSPECIFIED : (VideoFormat)videoFormat, 
                        audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat);
                    string destination = conversion.OutputFilePath;
                    conversion.OnProgress += Conversion_OnProgress;
                    try
                    {
                        cpMainControlPanel.ShowProgress();
                        await conversion.Start();
                        result = new RunResult<string>(true, new string[] { }, destination);
                        cpMainControlPanel.HideProgress();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogFFmpegException(ex);
                        result = new RunResult<string>(false, new string[] { ex.Message }, null);
                    }
                    this.downloadingCropped = false;
                }
                else
                {
                    IConversion conversion = await VideoUtil.PrepareBestYtdlConversion(url, "bestaudio", start, duration, 
                        AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, null, VideoFormat.UNSPECIFIED, 
                        audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat, cpMainControlPanel.EmbedThumbnail);
                    string destination = conversion.OutputFilePath;
                    conversion.OnProgress += Conversion_OnProgress;
                    try
                    {
                        await conversion.Start();
                        result = new RunResult<string>(true, new string[] { }, destination);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogFFmpegException(ex);
                        result = new RunResult<string>(false, new string[] { ex.Message }, null);
                    }
                }
            }
            else
            {
                cpMainControlPanel.ShowProgress();
                if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
                {
                    result = await Utils.VideoUtil.DownloadPreferredYtdl(VideoUtil.CorrectYouTubeString(url), streamType);
                }
                else
                {
                    result = await Utils.VideoUtil.DownloadBestYtdl(VideoUtil.CorrectYouTubeString(url), streamType, null, null, null, cpMainControlPanel.EmbedThumbnail);
                }
                cpMainControlPanel.HideProgress();
            }
            if (!result.Success)
            {
                MsgBox.Show("Download Failed\n" + String.Join("\n", result.ErrorOutput));
            }
            ipMainInput.marqeeMain.Hide();
            ipMainInput.marqeeMain.Text = "";
            await Historian.RecordDownload(new DownloadLog(
                DownloadType.YouTube,
                VideoUtil.CorrectYouTubeString(url),
                streamType,
                DateTime.Now,
                result.Data));
            cpMainControlPanel.gcHistory.DataSource = Historian.DownloadHistory;
            refreshHistory();
            cpMainControlPanel.ShowDownloadLocation(result.Data);
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

        private async void ytdlDownloadSelection()
        {
            this.currentDownload = HtmlUtil.CheckUrl(ipMainInput.URL);
            if(this.currentDownload == DownloadType.Unknown && AppSettings.Default.General.ShowHostWarning)
            {
                DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
                if (res == DialogResult.No)
                    return;
            }

            this.UseWaitCursor = true;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
            cpMainControlPanel.HideDownloadLocation();
            ipMainInput.marqeeMain.Text = "Preparing Download..";
            ipMainInput.marqeeMain.Show();
            VideoFormat? videoFormat = null;
            AudioFormat? audioFormat = null;

            RunResult<string> result = null;

            if (cpMainControlPanel.PostProcessingEnabled || cpMainControlPanel.CurrentFormat.RedditAudioFormat != null)
            {
                if (cpMainControlPanel.SegmentEnabled && cpMainControlPanel.SegmentDuration == TimeSpan.Zero)
                {
                    MsgBox.Show("Please specify a valid duration for the segment", "Invalid Duration");
                    return;
                }

                downloadingSegment = true;
                int[] crops = null;

                if (cpMainControlPanel.CropEnabled)
                {
                    crops = new int[] { Convert.ToInt32(cpMainControlPanel.CropTop), Convert.ToInt32(cpMainControlPanel.CropBottom), Convert.ToInt32(cpMainControlPanel.CropLeft), 
                        Convert.ToInt32(cpMainControlPanel.CropRight) };
                }

                TimeSpan? start = null;
                TimeSpan? duration = null;
                if (cpMainControlPanel.SegmentEnabled)
                {
                    start = cpMainControlPanel.SegmentStart;
                    duration = cpMainControlPanel.SegmentDuration;
                }

                if (cpMainControlPanel.ConversionEnabled)
                {
                    videoFormat = cpMainControlPanel.ConvertVideoFormat == null ? VideoFormat.UNSPECIFIED : cpMainControlPanel.ConvertVideoFormat;
                    audioFormat = cpMainControlPanel.ConvertAudioFormat == null ? AudioFormat.UNSPECIFIED : cpMainControlPanel.ConvertAudioFormat;
                }

                IConversion conversion = await Utils.VideoUtil.PrepareYoutubeConversion(VideoUtil.CorrectYouTubeString(ipMainInput.URL), cpMainControlPanel.CurrentFormat, 
                    start, duration, AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, crops, videoFormat == null ? VideoFormat.UNSPECIFIED : (VideoFormat)videoFormat, 
                    audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat);
                string destination = conversion.OutputFilePath;
                conversion.OnProgress += Conversion_OnProgress;
                cpMainControlPanel.ShowProgress();
                try
                {
                    await conversion.Start();

                    if (cpMainControlPanel.EmbedThumbnail)
                    {
                        var data = await VideoUtil.GetVideoData(VideoUtil.CorrectYouTubeString(ipMainInput.URL));
                        if (data != null)
                        {
                            var thumb = data.Thumbnails.Where(t => !t.Url.EndsWith("webp")).OrderByDescending(t => t.Height).ToArray()[0];
                            bool addArt = await TagUtil.AddAlbumCover(destination, thumb.Url);
                        }
                    }
                    
                    result = new RunResult<string>(true, new string[] { }, destination);
                }
                catch (Exception ex)
                {
                    result = new RunResult<string>(false, new string[] { ex.Message }, null);
                    ExceptionHandler.LogFFmpegException(ex);
                }
                cpMainControlPanel.HideProgress();
                downloadingSegment = false;
            }
            else
            {
                cpMainControlPanel.ShowProgress();
                result = await Utils.VideoUtil.DownloadYTDLFormat(VideoUtil.CorrectYouTubeString(ipMainInput.URL), cpMainControlPanel.CurrentFormat, cpMainControlPanel.EmbedThumbnail);
                cpMainControlPanel.HideProgress();
                if (!result.Success)
                {
                    YTRErrorMessageBox eb = new YTRErrorMessageBox(String.Join("\n", result.ErrorOutput), "Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                    eb.ShowDialog();
                }
            }

            YT_RED.Classes.StreamType t = Classes.StreamType.Audio;
            if (cpMainControlPanel.CurrentFormat.AudioCodec == "none")
                t = Classes.StreamType.Video;
            else if (cpMainControlPanel.CurrentFormat.Resolution == "audio only")
                t = Classes.StreamType.Audio;
            else
                t = Classes.StreamType.AudioAndVideo;
            ipMainInput.marqeeMain.Hide();
            ipMainInput.marqeeMain.Text = "";
            if (result.Success)
            {
                await Historian.RecordDownload(new DownloadLog(
                    DownloadType.YouTube,
                    VideoUtil.CorrectYouTubeString(ipMainInput.URL),
                    t, DateTime.Now,
                    result.Data
                    )); ;
                cpMainControlPanel.ShowDownloadLocation(result.Data);
                if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                {
                    string argument = "/select, \"" + result.Data + "\"";

                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
            }
            cpMainControlPanel.gcHistory.DataSource = Historian.DownloadHistory;
            refreshHistory();
            gvFormats.FocusedRowHandle = -1;
            this.UseWaitCursor = false;
            this.currentDownload = DownloadType.Unknown;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
        }

        private void gvFormats_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
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
                    size = ((Convert.ToDecimal(gvFormats.GetRowCellValue(e.ListSourceRowIndex, "ApproximateFileSize")) / 1024) / 1024).ToString();
                }
                e.DisplayText = size.Substring(0, size.IndexOf('.') + 2) + "MB";
            }
        }
        private void gvFormats_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
            {
                cpMainControlPanel.CurrentFormat = null;
                cpMainControlPanel.DownloadSelectionVisible = false;
                cpMainControlPanel.DownloadBestVisible = true;
                cpMainControlPanel.DownloadAudioVisible = true;
                return;
            }

            cpMainControlPanel.DownloadSelectionVisible = true;
            cpMainControlPanel.DownloadBestVisible = false;
            cpMainControlPanel.DownloadAudioVisible = false;
            Classes.YTDLFormatData fd = gvFormats.GetFocusedRow() as Classes.YTDLFormatData;
            cpMainControlPanel.CurrentFormat = fd;
            if(fd.VideoCodec == "none")
            {
                cpMainControlPanel.DisableToggle(false, true, false);
            }
            else
            {
                cpMainControlPanel.EnableToggle(false, true, false);
            }

        }

        private void ipMainInput_Crab_Click(object sender, EventArgs e)
        {
            using (DevExpress.XtraEditors.XtraForm dlg = new DevExpress.XtraEditors.XtraForm())
            {
                using (ImageControl ctrl = new ImageControl(this, Properties.Resources.CDM))
                {
                    dlg.AutoSize = true;
                    dlg.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    dlg.ControlBox = false;
                    dlg.SizeGripStyle = SizeGripStyle.Hide;
                    dlg.FormBorderStyle = FormBorderStyle.None;
                    dlg.MaximizeBox = false;
                    dlg.MinimizeBox = false;
                    dlg.StartPosition = FormStartPosition.CenterScreen;
                    dlg.Controls.Add(ctrl);
                    dlg.Show();
                    dlg.BringToFront();
                    dlg.TopMost = true;
                    System.Threading.Thread.Sleep(3000);
                    dlg.Close();
                }
            }
        }
    }
}