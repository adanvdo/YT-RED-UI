using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Svg;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private YoutubeDLSharp.Metadata.FormatData selectedFormat = null;
        public YoutubeDLSharp.Metadata.FormatData SelectedFormat { get { return selectedFormat; } }
        public int FormatCount { get { return gvFormats.RowCount; } }
        private DownloadType currentDownload = DownloadType.Unknown;
        private InitialFunction initialFunction = InitialFunction.None;
        private string initialLink = string.Empty;
        private DownloadType initialDownloadType = DownloadType.Unknown;
        private bool newUpdater = false;
        private bool updated = false;
        private bool enableQuickDownload = false;
        private bool trayBalloonShown = false;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repHistoryCheckEdit;
        private int splitterNegativePosition = 0;

        static KeyboardHook hook = new KeyboardHook();

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(bool newUpdater = false, bool updated = false)
        {
            InitializeComponent();
            this.updated = updated;
            this.initialFunction = InitialFunction.None;
            this.initialLink = "";
            this.newUpdater = newUpdater;
            cpMainControlPanel.ParentMainForm = this;
            repHistoryCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            repHistoryCheckEdit.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Custom;
            repHistoryCheckEdit.ImageOptions.SvgImageChecked = Properties.Resources.actions_checkcircled;
            repHistoryCheckEdit.ImageOptions.SvgImageUnchecked = Properties.Resources.security_warningcircled2;
            repHistoryCheckEdit.ImageOptions.SvgImageSize = new Size(20, 20);
        }

        public MainForm(InitialFunction initialFunction, string initialLink, bool newUpdater = false, bool updated = false)
        {
            InitializeComponent();
            this.updated = updated;
            this.initialFunction = initialFunction;
            this.initialLink = initialLink;
            this.newUpdater = newUpdater;
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
            Init();
            if (AppSettings.Default.General.EnableDownloadHistory)
                this.historyTimer.Start();
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

                System.Windows.Input.ModifierKeysConverter modifierKeysConverter = new System.Windows.Input.ModifierKeysConverter();
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

                System.Windows.Input.ModifierKeys dlModifier = (System.Windows.Input.ModifierKeys)modifierKeysConverter.ConvertFrom(modifierKeys);
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
            if (AppSettings.Default.General.EnableDownloadHistory && Historian.Loaded)
            {
                await Historian.CleanHistory();
                refreshHistory();
            }
        }

        protected override async void OnLoad(EventArgs e)
        {
            initializeProgram();
            cpMainControlPanel.UpdateConvertState();
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
            splitterNegativePosition = sccMainSplitter.Size.Width - sccMainSplitter.SplitterPosition;
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
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(AppSettings.Default.General.ActiveSkin, AppSettings.Default.General.SkinPalette); 
            if (AppSettings.Default.General.EnableDownloadHistory)
            {
                bool loadDownloadHistory = await Historian.LoadDownloadHistory();
                if (loadDownloadHistory)
                {
                    refreshHistory();
                }                
            }
            gcFormats.DataSource = new List<YTDLFormatData>();
            refreshFormatGrid(DownloadType.Unknown);
            VideoUtil.Init();
            VideoUtil.ytProgress = new Progress<DownloadProgress>(updateProgress);
            VideoUtil.ytOutput = new Progress<string>(processOutput);
            if(!Program.DevRun)
                ipMainInput.URL = initialLink;

            if (initialDownloadType == DownloadType.Unknown)
            {
                
            }
            else if (initialDownloadType == DownloadType.Reddit)
            {

            }
            else if (initialFunction == InitialFunction.ListFormats)
            {
                getYtdlFormatList(ipMainInput.URL);
            }
            
            if (newUpdater)
            {
                bool updaterReplaced = await UpdateHelper.ReplaceUpdater();
                if (!updaterReplaced)
                {
                    YT_RED.Controls.MsgBox.Show($"Failed to Replace Updater", "Something Went Wrong", FormStartPosition.CenterParent);
                }
            }

            if (this.updated || Program.updated)
            {
                bool replaceDependency = await UpdateHelper.ReplaceZipDependency();
                bool deleteBackup = await UpdateHelper.DeleteRemnants();
            }
        }

        private void applyLayout(Settings.LayoutArea layoutArea)
        {
            if (layoutArea == LayoutArea.All || layoutArea == LayoutArea.FormatList)
            {
                if (AppSettings.Default.Layout.FormatMode == FormatMode.Preset)
                {
                    this.gvFormats.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
                    this.gvFormats.GridControl = this.gcFormats;
                    this.gvFormats.Name = "gvFormats";
                    this.gvFormats.OptionsBehavior.AlignGroupSummaryInGroupRow = DevExpress.Utils.DefaultBoolean.False;
                    this.gvFormats.OptionsBehavior.Editable = false;
                    this.gvFormats.OptionsCustomization.AllowGroup = false;
                    this.gvFormats.OptionsDetail.ShowDetailTabs = false;
                    this.gvFormats.OptionsSelection.MultiSelect = false;
                    this.gvFormats.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
                    this.gvFormats.OptionsSelection.UseIndicatorForSelection = true;
                    this.gvFormats.OptionsSelection.EnableAppearanceFocusedCell = false;
                    this.gvFormats.OptionsSelection.EnableAppearanceHideSelection = false;
                    this.gvFormats.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Fast;
                    this.gvFormats.OptionsView.ColumnAutoWidth = false;
                    this.gvFormats.OptionsView.ShowDetailButtons = false;
                    this.gvFormats.OptionsView.ShowGroupExpandCollapseButtons = false;
                    this.gvFormats.OptionsView.ShowGroupPanel = false;
                    this.gvFormats.OptionsView.ShowIndicator = true;
                    this.gvFormats.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gvFormats_CustomDrawCell);
                    this.gvFormats.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvFormats_FocusedRowChanged);
                    this.gvFormats.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvFormats_CustomColumnDisplayText);
                }
                else if (AppSettings.Default.Layout.FormatMode == FormatMode.Custom)
                {
                    this.gvFormats.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
                    this.gvFormats.GridControl = this.gcFormats;
                    this.gvFormats.Name = "gvFormats";
                    this.gvFormats.OptionsBehavior.AlignGroupSummaryInGroupRow = DevExpress.Utils.DefaultBoolean.False;
                    this.gvFormats.OptionsBehavior.Editable = false;
                    this.gvFormats.OptionsCustomization.AllowGroup = false;
                    this.gvFormats.OptionsDetail.ShowDetailTabs = false;
                    this.gvFormats.OptionsSelection.MultiSelect = true;
                    this.gvFormats.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
                    this.gvFormats.OptionsSelection.UseIndicatorForSelection = false;
                    this.gvFormats.OptionsSelection.EnableAppearanceFocusedCell = false;
                    this.gvFormats.OptionsSelection.EnableAppearanceHideSelection = false;
                    this.gvFormats.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Fast;
                    this.gvFormats.OptionsView.ColumnAutoWidth = false;
                    this.gvFormats.OptionsView.ShowDetailButtons = false;
                    this.gvFormats.OptionsView.ShowGroupExpandCollapseButtons = false;
                    this.gvFormats.OptionsView.ShowGroupPanel = false;
                    this.gvFormats.OptionsView.ShowIndicator = false;
                    this.gvFormats.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gvFormats_CustomDrawCell);
                    this.gvFormats.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvFormats_FocusedRowChanged);
                    this.gvFormats.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvFormats_CustomColumnDisplayText);
                }
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
            AppSettings.Default.General.SkinPalette = this.LookAndFeel.ActiveSvgPaletteName;
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
                cpMainControlPanel.CurrentFormatPair.Clear();
                ytdlDownloadBest(ipMainInput.URL);
            }
        }

        private void cpMainControlPanel_DownloadAudio_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ipMainInput.URL))
            {
                cpMainControlPanel.SetCurrentFormats(null, new YTDLFormatData() { AudioCodec = "best" });
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
            this.applyLayout(LayoutArea.FormatList);
            gvFormats.Columns.Clear();
            gvFormats.PopulateColumns();
            gvFormats.RefreshData();
            int start = 0;
            gvFormats.Columns["Selected"].Visible = gvFormats.OptionsSelection.MultiSelect;
            if (gvFormats.OptionsSelection.MultiSelect) 
            {
                gvFormats.Columns["Selected"].VisibleIndex = start;
                start++;
            }
            gvFormats.Columns["Type"].VisibleIndex = start+0;
            gvFormats.Columns["Format"].VisibleIndex = start + 1;
            gvFormats.Columns["Duration"].VisibleIndex = start + 2;
            gvFormats.Columns["Bitrate"].VisibleIndex = start + 3;
            gvFormats.Columns["ContainerFormat"].VisibleIndex = start + 4;
            gvFormats.Columns["VideoCodec"].VisibleIndex = start + 5;
            gvFormats.Columns["ContainerFormat"].Caption = "Container";
            gvFormats.Columns["FrameRate"].VisibleIndex = start + 6;
            gvFormats.Columns["FrameRate"].Caption = "FPS";
            gvFormats.Columns["AudioCodec"].VisibleIndex = start + 7;
            gvFormats.Columns["AudioSamplingRate"].VisibleIndex = start + 8;
            gvFormats.Columns["FileSize"].VisibleIndex = start + 9;
            gvFormats.Columns["Type"].Width = 25;
            gvFormats.Columns["Type"].Caption = "";
            gvFormats.Columns["Format"].BestFit();
            gvFormats.Columns["VideoCodec"].BestFit();
            gvFormats.Columns["FrameRate"].Width = 50;
            gvFormats.Columns["RedditAudioFormat"].Visible = false;
            gvFormats.Columns["Url"].Visible = false;
            gvFormats.Columns["ManifestUrl"].Visible = false;
            gvFormats.Columns["FormatId"].Visible = false;
            gvFormats.Columns["FormatNote"].Visible = false;
            gvFormats.Columns["Resolution"].Visible = false;
            gvFormats.Columns["AudioBitrate"].Visible = false;
            gvFormats.Columns["VideoBitrate"].Visible = false;
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
                var data = await VideoUtil.GetVideoData(url, true);
                List<YoutubeDLSharp.Metadata.FormatData> formatList = new List<YoutubeDLSharp.Metadata.FormatData>();
                List<YTDLFormatData> converted = new List<YTDLFormatData>();                
                if(data.Formats != null)
                {
                    YoutubeDLSharp.Metadata.FormatData supplementAudio = null;
                    if(AppSettings.Default.Layout.FormatMode == FormatMode.Preset && this.currentDownload == DownloadType.Reddit && data.Formats.Where(f => f.VideoCodec != null && f.VideoCodec != "none" && f.AudioCodec != null && f.AudioCodec != "none").Count() < 1)
                    {
                        var checkAudio = data.Formats.Where(af => af.Format.ToLower().Contains("audio only") || (af.AudioCodec != null && af.AudioCodec != "none" && (af.VideoCodec == null || af.VideoCodec == "none")));
                        if (checkAudio != null && checkAudio.Count() > 0)
                            supplementAudio = checkAudio.LastOrDefault();
                    }
                    formatList = data.Formats.Where(f => !YTDLFormatData.ExcludeFormatIDs.Contains(f.FormatId))
                        .OrderBy(f => f.VideoCodec == "none" || f.VideoCodec == "" || f.VideoCodec == null ? 0 : 1)
                        .ThenBy(f => f.Height).ToList(); 
                    foreach (YoutubeDLSharp.Metadata.FormatData format in formatList)
                    {
                        var convert = new YTDLFormatData(format, data.Duration);
                        converted.Add(convert);
                        if(convert.Type == Classes.StreamType.Video && supplementAudio != null)
                        {
                            converted.Add(new YTDLFormatData(format, data.Duration, supplementAudio));
                        }
                    }
                }
                else if(data.Extension.ToLower() == "gif") 
                {
                    IMediaInfo gifInfo = await FFmpeg.GetMediaInfo(data.Url);
                    converted.Add(new YTDLFormatData(data, gifInfo));
                }
                
                gcFormats.DataSource = converted;
                refreshFormatGrid(this.currentDownload);
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
                ipMainInput.marqeeMain.Hide();
                ipMainInput.marqeeMain.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            this.UseWaitCursor = false;
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
                    IConversion conversion = await VideoUtil.PrepareBestYtdlConversion(url, "bestvideo+bestaudio/best", start, duration, 
                        AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, crops, videoFormat == null ? VideoFormat.UNSPECIFIED : (VideoFormat)videoFormat, 
                        audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat, false, processOutput);
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
                }
                else
                {
                    IConversion conversion = await VideoUtil.PrepareBestYtdlConversion(url, "bestaudio", start, duration, 
                        AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, null, VideoFormat.UNSPECIFIED, 
                        audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat, cpMainControlPanel.EmbedThumbnail, processOutput);
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
                RunResult<string> test = null;
                if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)                
                    test = await Utils.VideoUtil.DownloadPreferredYtdl(VideoUtil.CorrectYouTubeString(url), streamType);
                else 
                    test = await Utils.VideoUtil.DownloadBestYtdl(VideoUtil.CorrectYouTubeString(url), streamType, null, null, null, cpMainControlPanel.EmbedThumbnail);

                if (!test.Success)
                {                  
                    // FALLBACK IS CURRENTLY LIMITED TO REDDIT
                    // THIS SECTION MAY NEED TO BE EXPANDED FOR OTHER SUPPORTED HOSTS

                    if (this.currentDownload == DownloadType.Reddit)
                    {
                        var data = await Utils.VideoUtil.GetVideoData(VideoUtil.CorrectYouTubeString(url));
                        List<YTDLFormatData> converted = new List<YTDLFormatData>();
                        if (data.Formats != null && data.Formats.Count() > 0)
                        {
                            YT_RED.Classes.StreamType detect = AppSettings.DetectStreamTypeFromExtension(data.Extension);
                            if (detect != Classes.StreamType.Unknown)
                            {                                
                                if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
                                    test = await Utils.VideoUtil.DownloadPreferredYtdl(VideoUtil.CorrectYouTubeString(url), detect);
                                else 
                                    test = await Utils.VideoUtil.DownloadBestYtdl(VideoUtil.CorrectYouTubeString(url), detect, null, null, null, cpMainControlPanel.EmbedThumbnail);
                            }
                        }
                        else if (data.Formats == null && data.Extension.ToLower() == "gif")
                        {
                            cpMainControlPanel.DisableToggle(true, true, true);
                            IMediaInfo gifMeta = await FFmpeg.GetMediaInfo(data.Url);
                            YTDLFormatData dlFormatData;
                            if (gifMeta != null)
                            {
                                dlFormatData = new YTDLFormatData(data, gifMeta);
                            }
                            else dlFormatData = new YTDLFormatData(data);
                            var options = YoutubeDLSharp.Options.OptionSet.Default;
                            if (options.CustomOptions.Where(o => o.OptionStrings.Contains("-o")).Count() > 0)
                            {
                                options.DeleteCustomOption("-o");
                            }
                            string outputFile = VideoUtil.GenerateUniqueYtdlFileName(Classes.StreamType.Video);
                            options.AddCustomOption<string>("-o", outputFile);
                            test = await Utils.VideoUtil.DownloadYTDLGif(dlFormatData.Url, options);
                        }
                    }
                }
                result = test;
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

            if (cpMainControlPanel.PostProcessingEnabled || (cpMainControlPanel.CurrentFormatPair.Type != Classes.StreamType.Video
                && (cpMainControlPanel.CurrentFormatPair.VideoFormat.RedditAudioFormat != null || cpMainControlPanel.CurrentFormatPair.AudioFormat.RedditAudioFormat != null)) 
                || (cpMainControlPanel.CurrentFormatPair.VideoFormat.VideoCodec != "gif" && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat))
            {
                if (cpMainControlPanel.SegmentEnabled && cpMainControlPanel.SegmentDuration == TimeSpan.Zero)
                {
                    this.UseWaitCursor = false;
                    ipMainInput.marqeeMain.Hide();
                    ipMainInput.marqeeMain.Text = "";
                    MsgBox.Show("Please specify a valid duration for the segment", "Invalid Duration");
                    return;
                }

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
                    videoFormat = cpMainControlPanel.ConvertVideoFormat == null ? AppSettings.Default.Advanced.PreferredVideoFormat : cpMainControlPanel.ConvertVideoFormat;
                    audioFormat = cpMainControlPanel.ConvertAudioFormat == null ? AppSettings.Default.Advanced.PreferredAudioFormat : cpMainControlPanel.ConvertAudioFormat;
                }

                IConversion conversion = await Utils.VideoUtil.PrepareYoutubeConversion(VideoUtil.CorrectYouTubeString(ipMainInput.URL), cpMainControlPanel.CurrentFormatPair, 
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
                        if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
                            audioFormat = AppSettings.Default.Advanced.PreferredAudioFormat;

                        var data = await VideoUtil.GetVideoData(VideoUtil.CorrectYouTubeString(ipMainInput.URL));
                        if (data != null)
                        {
                            var thumb = data.Thumbnails.Where(t => !t.Url.EndsWith("webp")).OrderByDescending(t => t.Height).ToArray()[0];
                            if (audioFormat == AudioFormat.MP3)
                            {
                                await TagUtil.AddMp3Tags(destination, thumb.Url, data.Title, "", -1, data.UploadDate != null ? ((DateTime)data.UploadDate).Year : -1);
                            }
                            else
                            {
                                await TagUtil.AddAlbumCover(destination, thumb.Url);
                            }
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
            }
            else
            {
                cpMainControlPanel.ShowProgress();
                result = await Utils.VideoUtil.DownloadYTDLFormat(VideoUtil.CorrectYouTubeString(cpMainControlPanel.CurrentFormatPair.VideoCodec == "gif" ? cpMainControlPanel.CurrentFormatPair.VideoFormat.Url : ipMainInput.URL), cpMainControlPanel.CurrentFormatPair, cpMainControlPanel.EmbedThumbnail);
                cpMainControlPanel.HideProgress();
                if (!result.Success)
                {
                    YTRErrorMessageBox eb = new YTRErrorMessageBox(String.Join("\n", result.ErrorOutput), "Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                    eb.ShowDialog();
                }
            }

            YT_RED.Classes.StreamType t = Classes.StreamType.Audio;
            if (cpMainControlPanel.CurrentFormatPair.AudioCodec == "none")
                t = Classes.StreamType.Video;
            else if (cpMainControlPanel.CurrentFormatPair.AudioFormat != null || (cpMainControlPanel.CurrentFormatPair.VideoFormat != null && cpMainControlPanel.CurrentFormatPair.VideoFormat.Resolution == "audio only"))
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
            try
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
                    if (size.IndexOf('.') < 0)
                        e.DisplayText = size + "MB";
                    else
                        e.DisplayText = size.Substring(0, size.IndexOf('.') + 2) + "MB";
                }
                if (e.Column.FieldName == "Bitrate" || e.Column.FieldName == "VideoBitrate")
                {
                    if (e.Value != null)
                    {
                        if (e.Value.ToString().IndexOf(".") < 0)
                            e.DisplayText = e.Value.ToString() + "k";
                        else
                        {
                            string br = e.Value.ToString().Substring(0, e.Value.ToString().IndexOf("."));
                            e.DisplayText = $"{br}k";
                        }
                    }
                }
                if(e.Column.FieldName == "Duration")
                {
                    if(e.Value != null)
                    {
                        e.DisplayText = ((TimeSpan)e.Value).ToString(@"hh\:mm\:ss");
                    }
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
        }
        private void gvFormats_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (AppSettings.Default.Layout.FormatMode == FormatMode.Custom)
                return;

            if (e.FocusedRowHandle < 0)
            {
                cpMainControlPanel.CurrentFormatPair.Clear();
                cpMainControlPanel.DownloadSelectionVisible = false;
                cpMainControlPanel.DownloadBestVisible = true;
                cpMainControlPanel.DownloadAudioVisible = true;
                return;
            }

            cpMainControlPanel.DownloadSelectionVisible = true;
            cpMainControlPanel.DownloadBestVisible = false;
            cpMainControlPanel.DownloadAudioVisible = false;
            Classes.YTDLFormatData fd = gvFormats.GetFocusedRow() as Classes.YTDLFormatData;
            cpMainControlPanel.SetCurrentFormats(fd);
            if(fd.VideoCodec == "none")
            {
                cpMainControlPanel.DisableToggle(false, true, false);
            }
            else if(fd.VideoCodec == "gif")
            {
                cpMainControlPanel.DisableToggle(true, true, true);
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

        private void ipMainInput_Url_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(ipMainInput.URL))
            {
                cpMainControlPanel.CurrentFormatPair.Clear();
                ytdlDownloadBest(ipMainInput.URL);
            }            
        }

        private void gvFormats_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if(e.Column.FieldName == "Type")
            {
                var palette = SvgPaletteHelper.GetSvgPalette(this.LookAndFeel, ObjectState.Normal);
                YT_RED.Classes.StreamType? type = e.CellValue as Classes.StreamType?;
                switch (type)
                {
                    case Classes.StreamType.AudioAndVideo:
                        e.Cache.DrawSvgImage(Properties.Resources.VideoSound, e.Bounds, palette);
                        break;
                    case Classes.StreamType.Video:
                        e.Cache.DrawSvgImage(Properties.Resources.glyph_video, e.Bounds, palette);
                        break;
                    case Classes.StreamType.Audio:
                        e.Cache.DrawSvgImage(Properties.Resources.sound, e.Bounds, palette);
                        break;
                }
                e.Handled = true;
            }
        }

        private void sccMainSplitter_Resize(object sender, EventArgs e)
        {
            sccMainSplitter.SplitterPosition = sccMainSplitter.Size.Width - splitterNegativePosition;
        }

        private void sccMainSplitter_SplitterMoved(object sender, EventArgs e)
        {
            splitterNegativePosition = sccMainSplitter.Size.Width - sccMainSplitter.SplitterPosition;
        }


        private int selectedVideoIndex = -1;
        private int selectedAudioIndex = -1;

        bool deselecting = false;
        private void gvFormats_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            
            if (deselecting || !gvFormats.OptionsSelection.MultiSelect) return;
            int handle = e.ControllerRow;
            YTDLFormatData selection = (YTDLFormatData)gvFormats.GetRow(handle);

            if (selection == null) return;

            if (e.Action == System.ComponentModel.CollectionChangeAction.Add)
            {
                deselecting = true;
                if (selection.Type == Classes.StreamType.Audio)
                {
                    if(cpMainControlPanel.CurrentFormatPair.Type == Classes.StreamType.AudioAndVideo)
                    {
                        gvFormats.UnselectRow(handle);
                        deselecting = false;
                        return;
                    }

                    if(selectedAudioIndex >= 0)
                        gvFormats.UnselectRow(selectedAudioIndex);
                    selectedAudioIndex = handle;
                } else
                {
                    if(selectedVideoIndex >= 0)
                        gvFormats.UnselectRow(selectedVideoIndex);
                    if (selection.Type == Classes.StreamType.AudioAndVideo && selectedAudioIndex >= 0)
                    {
                        gvFormats.UnselectRow(selectedAudioIndex);
                        cpMainControlPanel.RemoveCurrentFormat(Classes.StreamType.Audio);
                        selectedAudioIndex = -1;
                    }
                    selectedVideoIndex = handle;
                }
                deselecting = false;
                cpMainControlPanel.SetCurrentFormat(selection);
            }
            else if(e.Action == System.ComponentModel.CollectionChangeAction.Remove)
            {
                if(selection.Type == Classes.StreamType.Audio)
                    selectedAudioIndex = -1;
                else
                    selectedVideoIndex = -1;
                cpMainControlPanel.RemoveCurrentFormat(selection.Type);
            }

            if (gvFormats.GetSelectedRows().Length < 1)
            {
                cpMainControlPanel.CurrentFormatPair.Clear();
                cpMainControlPanel.DownloadSelectionVisible = false;
                cpMainControlPanel.DownloadBestVisible = true;
                cpMainControlPanel.DownloadAudioVisible = true;
                return;
            }

            cpMainControlPanel.DownloadSelectionVisible = true;
            cpMainControlPanel.DownloadBestVisible = false;
            cpMainControlPanel.DownloadAudioVisible = false;
            if (cpMainControlPanel.CurrentFormatPair.VideoCodec == "none")
            {
                cpMainControlPanel.DisableToggle(false, true, false);
            }
            else if (cpMainControlPanel.CurrentFormatPair.VideoCodec == "gif")
            {
                cpMainControlPanel.DisableToggle(true, true, true);
            }
            else
            {
                cpMainControlPanel.EnableToggle(false, true, false);
            }
        }

        private void gvFormats_RowClick(object sender, RowClickEventArgs e)
        {
            if (!gvFormats.IsValidRowHandle(e.RowHandle) || gvFormats.IsGroupRow(e.RowHandle)) return;
            GridHitInfo hit = gvFormats.CalcHitInfo(e.Location);
            if (hit.Column.FieldName == "Selected") return;

            e.Handled = true;
            if (selectedAudioIndex == e.RowHandle || selectedVideoIndex == e.RowHandle)
                gvFormats.UnselectRow(e.RowHandle);
            else
                gvFormats.SelectRow(e.RowHandle);
        }
    }
}