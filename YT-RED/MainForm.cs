using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Svg;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xabe.FFmpeg;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YTR.Classes;
using YTR.Controls;
using YTR.Logging;
using YTR.Settings;
using YTR.Utils;

namespace YTR
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
        private TrayForm activeTrayForm = null;
        private PlaylistItemCollection playlistItemCollection = null;

        private DownloadLog selectedYTLog;
        [Browsable(false)]
        public DownloadLog TargetLog
        {
            get
            {
                if (selectedHistoryIndex >= 0)
                {
                    var log = gvHistory.GetRow(selectedHistoryIndex) as DownloadLog;
                    return log;
                }
                return null;
            }
        }

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
            this.activeTrayForm = new TrayForm();
            activeTrayForm.FormClosed += TrayForm_FormClosed;
            activeTrayForm.StartPosition = FormStartPosition.Manual;
            Rectangle workingArea = Screen.GetWorkingArea(this);
            var loc = new Point(workingArea.Right - activeTrayForm.Size.Width, workingArea.Bottom - activeTrayForm.Size.Height);
            activeTrayForm.Location = loc;
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
            this.activeTrayForm = new TrayForm();
            activeTrayForm.FormClosed += TrayForm_FormClosed;
            activeTrayForm.StartPosition = FormStartPosition.Manual;
            Rectangle workingArea = Screen.GetWorkingArea(this);
            var loc = new Point(workingArea.Right - activeTrayForm.Size.Width, workingArea.Bottom - activeTrayForm.Size.Height);
            activeTrayForm.Location = loc;
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
            AppSettings.Default.About.ReleaseChannel = EnumExtensions.ToEnum<ReleaseChannel>(assembly.GetCustomAttributes(typeof(AssemblyBuildAttribute), false).Cast<AssemblyBuildAttribute>().FirstOrDefault().Value);
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

                var check = HtmlUtil.CheckUrl(copiedText);
                if (check == DownloadType.Unknown && AppSettings.Default.General.ShowHostWarning)
                {
                    DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YTR.Controls.Icon.Warning, FormStartPosition.CenterParent);
                    if (res == DialogResult.No)
                        return;
                }
                else if(check == DownloadType.YouTube)
                {
                    YoutubeLink link = VideoUtil.ConvertToYouTubeLink(copiedText);
                    if (link.Type == YoutubeLinkType.Playlist)
                    {
                        MsgBox.Show("Quick Download does not support Youtube Playlists", "Unsupported", Buttons.OK, YTR.Controls.Icon.Exclamation, FormStartPosition.CenterScreen, true);
                        return;
                    }
                }

                if (activeTrayForm == null)
                {
                    try
                    {
                        activeTrayForm = new TrayForm();
                        activeTrayForm.FormClosed += TrayForm_FormClosed;
                        activeTrayForm.StartPosition = FormStartPosition.Manual;
                        Rectangle workingArea = Screen.GetWorkingArea(this);
                        var loc = new Point(workingArea.Right - activeTrayForm.Size.Width, workingArea.Bottom - activeTrayForm.Size.Height);
                        activeTrayForm.HideProgressPanel();
                        activeTrayForm.Location = loc;
                        activeTrayForm.Url = copiedText;
                        activeTrayForm.Show();
                        activeTrayForm.BringToFront();
                        activeTrayForm.TopMost = true;
                        activeTrayForm.TriggerDownload();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToLower() != "a task was canceled.")
                            ExceptionHandler.LogException(ex);
                    }
                }
                else if(!activeTrayForm.Locked)
                {
                    Rectangle workingArea = Screen.GetWorkingArea(this);
                    var loc = new Point(workingArea.Right - activeTrayForm.Size.Width, workingArea.Bottom - activeTrayForm.Size.Height);
                    activeTrayForm.HideProgressPanel();
                    activeTrayForm.Location = loc;
                    activeTrayForm.Url = copiedText;
                    activeTrayForm.Show();
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
            if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
            {
                cpMainControlPanel.EnableToggle(false, false, true, true, false);
                cpMainControlPanel.ConvertIntended = true;
                cpMainControlPanel.ConvertVideoFormat = AppSettings.Default.Advanced.PreferredVideoFormat;
                cpMainControlPanel.ConvertAudioFormat = AppSettings.Default.Advanced.PreferredAudioFormat;
            }
            if (AppSettings.Default.General.EnforceRestrictions)
            {
                cpMainControlPanel.EnableToggle(false, false, false, true, true);
            }
            cpMainControlPanel.MaxResolution = AppSettings.Default.General.MaxResolutionBest;
            cpMainControlPanel.MaxFilesize = AppSettings.Default.General.MaxFilesizeBest;
            cpMainControlPanel.RestoreControlGroupCollapseStates();
            updateControlPanelDisplay();
            
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
                this.ShowInTaskbar = false;
                if (VideoUtil.Running)
                {
                    activeTrayForm.closeOnCompletedProgress = true;
                    activeTrayForm.Url = this.ipMainInput.URL;
                    activeTrayForm.Show();
                }
                else if (!trayBalloonShown)
                {
                    notifyIcon.ShowBalloonTip(3000);
                    trayBalloonShown = true;
                }
            }
            else
            {
                notifyIcon.Visible = false;
                this.ShowInTaskbar = true;
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
            applyLayout();
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
                    YTR.Controls.MsgBox.Show($"Failed to Replace Updater", "Something Went Wrong", FormStartPosition.CenterParent);
                }
            }

            if (this.updated || Program.updated)
            {
                bool replaceDependency = await UpdateHelper.ReplaceZipDependency();
                bool deleteBackup = await UpdateHelper.DeleteRemnants();
            }
        }

        private int prevHistoryWidth = 300;

        private void updateHistoryPanel()
        {
            if (AppSettings.Default.General.EnableDownloadHistory)
            {
                bool expanding = !AppSettings.Default.General.CollapseHistoryPanel;
                prevHistoryWidth = expanding ? prevHistoryWidth : pnlHistoryPanel.Width;
                scHistorySplitter.Visible = !AppSettings.Default.General.CollapseHistoryPanel;
                sccMainSplitter.Padding = new Padding(5, 0, AppSettings.Default.General.CollapseHistoryPanel ? 5 : 0, 0);
                lblHeaderLabel.Visible = !AppSettings.Default.General.CollapseHistoryPanel;
                lblHistoryVert.Visible = AppSettings.Default.General.CollapseHistoryPanel;
                gcHistory.Visible = !AppSettings.Default.General.CollapseHistoryPanel;
                pnlHistoryPanel.MinimumSize = new Size(AppSettings.Default.General.CollapseHistoryPanel ? 32 : 300, 0);
                pnlHistoryPanel.Width = AppSettings.Default.General.CollapseHistoryPanel ? 32 : prevHistoryWidth;
                btnShowHideHistory.ImageOptions.SvgImage = AppSettings.Default.General.CollapseHistoryPanel ? Properties.Resources.doubleprev : Properties.Resources.doublenext;
                pnlHistoryControls.Visible = !AppSettings.Default.General.CollapseHistoryPanel;
                if (expanding && !AppSettings.Default.General.CollapseHistoryPanel)
                {
                    sccMainSplitter.SplitterPosition = sccMainSplitter.SplitterPosition - (pnlHistoryPanel.Width - 300);
                    scHistorySplitter.Location = new Point(tabFormContentContainer1.Width - pnlHistoryPanel.Width, 0);
                }
                this.pnlHistoryPanel.Visible = true;
            }
            else
            {
                this.pnlHistoryPanel.Visible = false;
            }
        }

        private void applyLayout(Settings.LayoutArea layoutArea = LayoutArea.All)
        {
            this.UseWaitCursor = true;

            updateHistoryPanel();

            if(layoutArea == LayoutArea.All || layoutArea == LayoutArea.Panels)
            {
                if(AppSettings.Default.Layout.InputPanelPosition == VerticalPanelPosition.Top)
                {
                    marqueeProgressBarControl1.Dock = DockStyle.Top;
                    ipMainInput.Dock = DockStyle.Top;
                }
                else
                {
                    ipMainInput.Dock = DockStyle.Bottom;
                    marqueeProgressBarControl1.Dock = DockStyle.Bottom;
                }

                if (AppSettings.Default.Layout.ControlPanelPosition == HorizontalPanelPosition.Right)
                    sccMainSplitter.RightToLeft = RightToLeft.No;
                else
                    sccMainSplitter.RightToLeft = RightToLeft.Yes;
                this.Refresh();
            }

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
            this.UseWaitCursor = false;
        }
        
        private void bbiSettings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            openSettings();
        }

        public void OpenSettingsDialog(string page = "")
        {
            openSettings(OpenPosition.Unspecified, page);
        }

        private async void openSettings(OpenPosition openPosition = OpenPosition.Unspecified, string page = "")
        {
            SettingsDialog dlg = new SettingsDialog(page);
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
            Cursor.Current = Cursors.Default;
            gcHistory.DataSource = Historian.DownloadHistory;
            refreshHistory();
            applyLayout(LayoutArea.Panels);
            cpMainControlPanel.UpdatePanelStates();
            if (res == DialogResult.OK)
            {
                bsiMessage.Caption = "Settings Saved";
                string currentUrl = ipMainInput.URL;
                cpMainControlPanel.ResetControls(gvFormats.RowCount > 0);
                if(ipMainInput.URL != currentUrl) { ipMainInput.URL = currentUrl; }
                pnlHistoryPanel.Visible = AppSettings.Default.General.EnableDownloadHistory;
                await Task.Delay(3000);
                bsiMessage.Caption = String.Empty;
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
        
        private void tsiDownload_Click(object sender, EventArgs e)
        {
            if (activeTrayForm == null)
            {
                try
                {
                    if (activeTrayForm == null) activeTrayForm = new TrayForm();
                    activeTrayForm.FormClosed += TrayForm_FormClosed;
                    activeTrayForm.StartPosition = FormStartPosition.Manual;
                    Rectangle workingArea = Screen.GetWorkingArea(this);
                    activeTrayForm.HideProgressPanel();
                    var loc = new Point(workingArea.Right - activeTrayForm.Size.Width, workingArea.Bottom - (activeTrayForm.Size.Height - 81));
                    activeTrayForm.Location = loc;
                    activeTrayForm.ShowDialog();
                    
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower() != "a task was canceled.")
                        ExceptionHandler.LogException(ex);
                }
            }
        }

        private void TrayForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //activeTrayForm = null;
        }

        private void tsiExit_Click(object sender, EventArgs e)
        {
            if (IsLocked || (activeTrayForm != null && activeTrayForm.Locked))
            {
                DialogResult res = MsgBox.Show("A download is in progress.\nAre you sure you want to exit?", "Download In Progress", YTR.Controls.Buttons.OKCancel, YTR.Controls.Icon.Warning);
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
            if(playlistItemCollection != null)
            {
                playlistItemCollection.Dispose();
                playlistItemCollection = null;
            }

            gcFormats.DataSource = null;
            gvFormats.RefreshData();
            cpMainControlPanel.CurrentFormatPair.Clear();
            cpMainControlPanel.ResetControls(false);
            selectedAudioIndex = -1;
            selectedVideoIndex = -1; 
            videoInfoPanel.Clear();
            videoInfoPanel.Visible = false;
            btnPLSelectAll.Visible = false;
            btnPLSelectAll.Text = "Select All";
            allSelected = false;
            
            var checkUrl = HtmlUtil.CheckUrl(ipMainInput.URL);
            if (gvFormats.GetSelectedRows().Length < 1 && checkUrl != DownloadType.Unknown && checkUrl == DownloadType.YouTube)
            {
                YoutubeLink link = VideoUtil.ConvertToYouTubeLink(ipMainInput.URL);
                if(ipMainInput.URL != link.Url)
                {
                    ipMainInput.URL = link.Url;
                    ipMainInput.btnListFormats.Focus();
                }
                if(link.Type == YoutubeLinkType.Playlist)
                {
                    cpMainControlPanel.ShowHideControlGroup(ControlGroups.Segment, false);
                    cpMainControlPanel.ShowHideControlGroup(ControlGroups.Crop, false);
                    cpMainControlPanel.DisableToggle(true, true, true);
                    cpMainControlPanel.SetSelectionText("Playlist Download");
                    cpMainControlPanel.btnDownloadBest.Text = "DOWNLOAD ALL [audio+video]       ";
                    cpMainControlPanel.btnDownloadAudio.Text = "DOWNLOAD ALL AUDIO     ";
                    ipMainInput.ListMode = ListMode.List;
                }
                else
                {
                    cpMainControlPanel.ShowHideControlGroup(ControlGroups.Segment, true);
                    cpMainControlPanel.ShowHideControlGroup(ControlGroups.Crop, true);
                    ipMainInput.ListMode = ListMode.Format;
                    if (gvFormats.GetSelectedRows().Length < 1)
                    {
                        cpMainControlPanel.DownloadSelectionVisible = false;
                        cpMainControlPanel.DownloadBestVisible = true;
                        cpMainControlPanel.DownloadAudioVisible = true;
                        return;
                    }
                    ipMainInput.btnListFormats.Enabled = true;
                    cpMainControlPanel.SetSelectionText(cpMainControlPanel.CurrentFormatPair != null && !string.IsNullOrEmpty(cpMainControlPanel.CurrentFormatPair.FormatDisplayText) ?
                        cpMainControlPanel.CurrentFormatPair.FormatDisplayText : "");
                }
            }
            else if (ipMainInput.URL == "crab")
            {
                ipMainInput.ShowCrab = true;
                await Task.Delay(3000);
                ipMainInput.ShowCrab = false;
            }
        }

        private void ipMainInput_ResetList_Click(object sender, EventArgs e)
        {
            if (playlistItemCollection != null)
            {
                playlistItemCollection.Dispose();
                playlistItemCollection = null;
            }

            gcFormats.DataSource = null;
            gvFormats.RefreshData();
            cpMainControlPanel.CurrentFormatPair.Clear();
            cpMainControlPanel.ResetControls(false);
            selectedAudioIndex = -1;
            selectedVideoIndex = -1;
            ipMainInput.URL = string.Empty; 
            btnPLSelectAll.Visible = false;
            btnPLSelectAll.Text = "Select All";
            allSelected = false;
            if (gvFormats.GetSelectedRows().Length < 1)
            {
                cpMainControlPanel.DownloadSelectionVisible = false;
                cpMainControlPanel.DownloadBestVisible = true;
                cpMainControlPanel.DownloadAudioVisible = true;
                return;
            }
        }

        private void ipMainInput_ListFormats_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ipMainInput.URL))
            {
                if (ipMainInput.btnListFormats.Text == "Cancel      ")
                {
                    VideoUtil.CancellationTokenSource.Cancel();
                    ipMainInput.UpdateListButtonText();
                }
                else 
                {
                    gcFormats.DataSource = null;
                    gvFormats.RefreshData();
                    if (ipMainInput.ListMode == ListMode.Format)
                    {
                        cpMainControlPanel.CurrentFormatPair.Clear();
                        cpMainControlPanel.ResetControls(true);
                        selectedAudioIndex = -1;
                        selectedVideoIndex = -1;
                        getYtdlFormatList(ipMainInput.URL);
                    }                    
                    else if (ipMainInput.ListMode == ListMode.List)
                    {
                        YoutubeLink link = VideoUtil.ConvertToYouTubeLink(ipMainInput.URL);
                        if (link != null)
                        {
                            getYtdlPlaylistItems(link.Url);
                        }
                    }
                }
            }
        }

        private void cpMainControlPanel_DownloadBest_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ipMainInput.URL))
            {
                if (ipMainInput.ListMode == ListMode.List)
                {
                    if(cpMainControlPanel.CurrentPlaylistItems != null && cpMainControlPanel.CurrentPlaylistItems.Count > 0)
                    {
                        ytdlDownloadPlaylistItems();
                    }
                    else
                    {
                        ytdlDownloadPlaylistItems(Classes.StreamType.AudioAndVideo, true);
                    }
                }
                else
                {
                    cpMainControlPanel.CurrentFormatPair.Clear();
                    ytdlDownloadBest(ipMainInput.URL);
                }
            }
        }

        private void cpMainControlPanel_DownloadAudio_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ipMainInput.URL))
            {
                if (ipMainInput.ListMode == ListMode.List)
                {
                    if (cpMainControlPanel.CurrentPlaylistItems != null && cpMainControlPanel.CurrentPlaylistItems.Count > 0)
                    {
                        ytdlDownloadPlaylistItems(Classes.StreamType.Audio);
                    }
                    else
                    {
                        ytdlDownloadPlaylistItems(Classes.StreamType.Audio, true);
                    }
                }
                else
                {
                    cpMainControlPanel.SetCurrentFormats(null, new YTDLFormatData() { AudioCodec = "best" });
                    ytdlDownloadBest(ipMainInput.URL, Classes.StreamType.Audio);
                }
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
            int start = 0;
            //gvFormats.Columns["Selected"].Visible = gvFormats.OptionsSelection.MultiSelect;
            if (gvFormats.OptionsSelection.MultiSelect)
            {
                gvFormats.VisibleColumns[0].OptionsColumn.ShowCaption = false;
                gvFormats.VisibleColumns[0].Width = 25;
                gvFormats.VisibleColumns[0].MaxWidth = 25;
                gvFormats.VisibleColumns[0].MinWidth = 25;
                start++;
            }
            
            if (downloadType == DownloadType.Playlist)
            {
                gvFormats.Columns.AddVisible("ThumbnailImage", "");
                gvFormats.Columns.AddVisible("Title", "Title");
                gvFormats.Columns.AddVisible("Duration", "Duration");
                gvFormats.Columns.AddField("Url");
                gvFormats.Columns.AddField("ID");
                gvFormats.Columns["ThumbnailImage"].OptionsColumn.ShowCaption = false;
                gvFormats.Columns["ThumbnailImage"].MinWidth = 100;
                gvFormats.RowHeight = 75;
                gvFormats.Columns["ThumbnailImage"].ColumnEdit = repPictureEdit;
                gvFormats.Columns["Title"].BestFit();
            }
            else
            {
                gvFormats.PopulateColumns();
                gvFormats.RefreshData();
                gvFormats.RowHeight = 0;
                gvFormats.Columns["Type"].VisibleIndex = start + 0;
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
                gvFormats.Columns["Type"].MaxWidth = 25;
                gvFormats.Columns["Type"].MinWidth = 25;
                gvFormats.Columns["Type"].OptionsColumn.ShowCaption = false;
                gvFormats.Columns["Format"].BestFit();
                gvFormats.Columns["VideoCodec"].BestFit();
                gvFormats.Columns["FrameRate"].Width = 50;
                gvFormats.Columns["Selected"].Visible = false;
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
        }

        private void refreshHistory()
        {
            int originalIndex = gvHistory.FocusedRowHandle;
            gcHistory.DataSource = null;
            gcHistory.DataSource = Historian.DownloadHistory;
            PopulateHistoryColumns();
            gvHistory.RefreshData();
            gvHistory.FocusedRowHandle = originalIndex;
            selectedYTLog = gvHistory.GetRow(originalIndex) as DownloadLog;
        }

        private void updateProgress(DownloadProgress progress)
        {
            if (cpMainControlPanel.SegmentEnabled || cpMainControlPanel.CropEnabled)
                return;

            var percent = (int)(Math.Round(progress.Progress * 100));
            if (this.activeTrayForm != null)
                this.activeTrayForm.ShowProgress(progress);
            cpMainControlPanel.UpdateProgress(percent, true);
        }        

        public void processOutput(string output)
        {
            ipMainInput.marqeeMain.Text = output;
            if (this.activeTrayForm != null)
                this.activeTrayForm.ShowProgressOutput(output);
        }

        private void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
        {
            var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
            if (activeTrayForm != null)
                activeTrayForm.ShowProgress(percent);
            cpMainControlPanel.UpdateProgress(percent, true);
        }

        private async void getYtdlPlaylistItems(string url)
        {
            try
            {
                VideoUtil.Running = true;
                this.ipMainInput.btnListFormats.Text = "Cancel      ";
                this.ipMainInput.btnListFormats.ImageOptions.SvgImage = Properties.Resources.close;
                this.UseWaitCursor = true;
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
                ipMainInput.marqeeMain.Text = "Fetching Playlist Details";
                ipMainInput.marqeeMain.Show();
                var data = await VideoUtil.GetPlaylistData(url);
                if(data != null)
                {
                    this.playlistItemCollection = new PlaylistItemCollection(data);
                    if(this.playlistItemCollection != null)
                    {
                        gcFormats.DataSource = this.playlistItemCollection.Items;
                        refreshFormatGrid(DownloadType.Playlist);
                        btnPLSelectAll.Visible = this.playlistItemCollection.Count > 0;
                        loadThumbnailsAsync();
                    }
                }
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex);
            }

            this.ipMainInput.UpdateListButtonText();
            this.ipMainInput.btnListFormats.ImageOptions.SvgImage = Properties.Resources.listnumbers;
            VideoUtil.Running = false;
            this.UseWaitCursor = false;
            this.ipMainInput.marqeeMain.Hide();
            ipMainInput.marqeeMain.Text = string.Empty;
        }

        private async void getYtdlFormatList(string url)
        {
            try
            {
                this.currentDownload = HtmlUtil.CheckUrl(ipMainInput.URL);
                if (this.currentDownload == DownloadType.Unknown && AppSettings.Default.General.ShowHostWarning)
                {
                    DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YTR.Controls.Icon.Warning, FormStartPosition.CenterParent);
                    if (res == DialogResult.No)
                        return;
                }
                VideoUtil.Running = true;
                this.ipMainInput.btnListFormats.Text = "Cancel      ";
                this.ipMainInput.btnListFormats.ImageOptions.SvgImage = Properties.Resources.close;
                this.UseWaitCursor = true;
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
                ipMainInput.marqeeMain.Text = "Fetching Available Formats";
                ipMainInput.marqeeMain.Show();
                var data = await VideoUtil.GetVideoData(url, AppSettings.Default.Advanced.GetMissingMetadata);
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
                    VideoUtil.CancellationTokenSource = new System.Threading.CancellationTokenSource();
                    IMediaInfo gifInfo = await FFmpeg.GetMediaInfo(data.Url, VideoUtil.CancellationTokenSource.Token);
                    converted.Add(new YTDLFormatData(data, gifInfo));
                }

                if(converted.Count > 0)
                {                   
                    await videoInfoPanel.Populate(data); 
                    videoInfoPanel.Visible = true;
                    ipMainInput.SendToBack();
                }
                else
                {
                    videoInfoPanel.Visible = false;
                    videoInfoPanel.Clear();
                }
                
                gcFormats.DataSource = converted;
                refreshFormatGrid(this.currentDownload);
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;                
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex);
            }
            this.ipMainInput.btnListFormats.Text = "List Available Formats";
            this.ipMainInput.btnListFormats.ImageOptions.SvgImage = Properties.Resources.listnumbers;
            VideoUtil.Running = false;
            this.UseWaitCursor = false;
            this.ipMainInput.marqeeMain.Hide();
            ipMainInput.marqeeMain.Text = string.Empty;
        }

        private async void ytdlDownloadPlaylistItems(Classes.StreamType streamType = Classes.StreamType.AudioAndVideo, bool downloadAll = false)
        {
            if (downloadAll)
            {
                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;

                ipMainInput.marqeeMain.Text = "Fetching Playlist Information..";
                ipMainInput.marqeeMain.Show();
                YoutubeLink link = VideoUtil.ConvertToYouTubeLink(ipMainInput.URL);
                var data = await VideoUtil.GetPlaylistData(link.Url);
                ipMainInput.marqeeMain.Hide();
                ipMainInput.marqeeMain.Text = "";

                (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;

                if (data != null)
                {
                    this.playlistItemCollection = new PlaylistItemCollection(data);
                    DialogResult res = MsgBox.Show($"There are {this.playlistItemCollection.Count} videos in this playlist.\nContinue download?", 
                        "Confirm Download", 
                        Buttons.YesNo, 
                        YTR.Controls.Icon.Warning, 
                        FormStartPosition.CenterParent);
                    if(res == DialogResult.No) { return; }
                    cpMainControlPanel.SetCurrentPlaylistItems(this.playlistItemCollection);
                }
                else
                {
                    MsgBox.Show("Empty or Invalid Playlist", "Error", Buttons.OK, YTR.Controls.Icon.Error, FormStartPosition.CenterParent);
                    return;
                }
            }
            else if (cpMainControlPanel.CurrentPlaylistItems.Count < 1) return;

            VideoUtil.Running = true;
            this.UseWaitCursor = true;
            cpMainControlPanel.DownloadSelectionVisible = false;
            cpMainControlPanel.DownloadBestVisible = false;
            cpMainControlPanel.DownloadAudioVisible = false;            
            this.cpMainControlPanel.btnCancelProcess.Visible = true;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
            ipMainInput.marqeeMain.Text = "Sending Download Request..";
            ipMainInput.marqeeMain.Show();

            int maxRes = cpMainControlPanel.LimitsEnabled ? cpMainControlPanel.MaxResolutionValue : AppSettings.Default.General.MaxResolutionValue;
            int maxSize = cpMainControlPanel.LimitsEnabled ? cpMainControlPanel.MaxFilesize : AppSettings.Default.General.MaxFilesizeBest;

            string mainFormatString = "bestvideo{0}{1}+bestaudio/best{0}{1}";
            string audioFormatString = "bestaudio{0}";
            string finalFormatString = String.Format(mainFormatString,
                maxRes > 0 ? $"[height<={maxRes}]" : "",
                maxSize > 0 ? $"[filesize<={maxSize}M]" : "");

            string finalAudioFormatString = String.Format(audioFormatString,
                maxSize > 0 ? $"[filesize<={maxSize}M]" : "");

            int downloaded = 0;
            string initialDLLocation = string.Empty;
            PlaylistItemCollection targetPlaylist = downloadAll ? this.playlistItemCollection : cpMainControlPanel.CurrentPlaylistItems;
            cpMainControlPanel.ShowListProgress();

            foreach(var playlistItem in targetPlaylist.Items)
            {
                PendingDownload pendingDL = new PendingDownload()
                {
                    Url = playlistItem.Url,
                    Format = streamType == Classes.StreamType.AudioAndVideo ? finalFormatString : finalAudioFormatString
                };

                RunResult<string> result = null;
                cpMainControlPanel.ShowProgress();
                if (cpMainControlPanel.ConversionEnabled && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
                    result = await Utils.VideoUtil.DownloadPreferredYtdl(playlistItem.Url, streamType, targetPlaylist.PlaylistData.Title.Replace(" ", ""));
                else
                    result = await Utils.VideoUtil.DownloadBestYtdl(playlistItem.Url, streamType, cpMainControlPanel.EmbedThumbnail, targetPlaylist.PlaylistData.Title.Replace(" ", ""));

                if (!result.Success && result.Data != "canceled")
                {
                    MsgBox.Show("Download Failed\n" + String.Join("\n", result.ErrorOutput));
                    break;
                }

                if(!result.Success && result.Data == "canceled")
                {
                    cpMainControlPanel.HideListProgress();
                    break;
                }
                var dlLog = new DownloadLog(playlistItem.Url,
                    DownloadType.YouTube,
                    streamType,
                    DateTime.Now,
                    result.Data,
                    pendingDL
                    )
                {
                    InSubFolder = AppSettings.Default.General.CreateFolderForPlaylists,
                    PlaylistTitle = targetPlaylist.PlaylistData.Title,
                    PlaylistUrl = targetPlaylist.PlaylistData.WebpageUrl 
                };
                if (cpMainControlPanel.LimitsEnabled)
                {
                    dlLog.MaxResolution = cpMainControlPanel.MaxResolution;
                    dlLog.MaxFileSize = cpMainControlPanel.MaxFilesize;
                }
                initialDLLocation = result.Data;

                await Historian.RecordDownload(dlLog);
                gcHistory.DataSource = Historian.DownloadHistory;
                refreshHistory();

                cpMainControlPanel.ShowProgress();
                downloaded++;
                cpMainControlPanel.UpdateListProgress(downloaded);
            }

            if(downloaded > 0)
            {
                cpMainControlPanel.ShowDownloadLocation(initialDLLocation);
                if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                {
                    string argument = "/select, \"" + initialDLLocation + "\"";

                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
            }

            cpMainControlPanel.HideProgress();
            cpMainControlPanel.HideListProgress();
            VideoUtil.Running = false;
            ipMainInput.marqeeMain.Hide();
            ipMainInput.marqeeMain.Text = "";
            cpMainControlPanel.DownloadSelectionVisible = false;
            cpMainControlPanel.DownloadAudioVisible = true;
            cpMainControlPanel.DownloadBestVisible = true;
            this.cpMainControlPanel.btnCancelProcess.Visible = false;    
            this.UseWaitCursor = false;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = false;
            return;
        }        
        
        private async void ytdlDownloadBest(string url, Classes.StreamType streamType = Classes.StreamType.AudioAndVideo)
        {            
            this.currentDownload = HtmlUtil.CheckUrl(ipMainInput.URL);
            if (this.currentDownload == DownloadType.Unknown && AppSettings.Default.General.ShowHostWarning)
            {
                DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YTR.Controls.Icon.Warning, FormStartPosition.CenterParent);
                if (res == DialogResult.No)
                    return;
            }

            VideoUtil.Running = true;
            this.UseWaitCursor = true;
            cpMainControlPanel.DownloadSelectionVisible = false;
            cpMainControlPanel.DownloadAudioVisible = false;
            cpMainControlPanel.DownloadBestVisible = false;
            this.cpMainControlPanel.btnCancelProcess.Visible = false;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
            ipMainInput.marqeeMain.Text = "Sending Download Request..";
            ipMainInput.marqeeMain.Show();
            int[] crops = null;
            TimeSpan? start = null;
            TimeSpan? duration = null;
            VideoFormat? videoFormat = null;
            AudioFormat? audioFormat = null;
            VideoData mediaData = null;

            try
            {
                mediaData = await VideoUtil.GetVideoData(url, false);
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }

            if (cpMainControlPanel.SegmentEnabled)
            {
                start = cpMainControlPanel.SegmentStart;
                duration = AppSettings.Default.Layout.SegmentControlMode == SegmentControlMode.Duration ? cpMainControlPanel.SegmentDuration : cpMainControlPanel.SegmentDuration - cpMainControlPanel.SegmentStart;
            }

            if (streamType == Classes.StreamType.AudioAndVideo && cpMainControlPanel.CropEnabled && cpMainControlPanel.ValidCrops())
            {
                crops = new int[] { Convert.ToInt32(cpMainControlPanel.CropTop), Convert.ToInt32(cpMainControlPanel.CropBottom), Convert.ToInt32(cpMainControlPanel.CropLeft), 
                    Convert.ToInt32(cpMainControlPanel.CropRight) };
            }

            if (cpMainControlPanel.ConversionEnabled)
            {
                videoFormat = cpMainControlPanel.ConvertVideoFormat == null ? VideoFormat.UNSPECIFIED : cpMainControlPanel.ConvertVideoFormat;
                audioFormat = cpMainControlPanel.ConvertAudioFormat == null ? AudioFormat.UNSPECIFIED : cpMainControlPanel.ConvertAudioFormat;
                if(videoFormat == VideoFormat.GIF)
                {
                    DialogResult confirm = MsgBox.Show($"GIF Conversion is limited to 60 seconds.\nOnly the first 60 seconds will be downloaded.\n\nContinue?", "GIF Duration", Buttons.YesNo, YTR.Controls.Icon.Exclamation);
                    if (confirm == DialogResult.No)
                    {
                        this.UseWaitCursor = false;
                        VideoUtil.Running = false;
                        ipMainInput.marqeeMain.Hide();
                        ipMainInput.marqeeMain.Text = "";
                        cpMainControlPanel.DownloadSelectionVisible = false;
                        this.cpMainControlPanel.btnCancelProcess.Visible = false;
                        return;
                    }
                }
            }
            PendingDownload pendingDL = new PendingDownload()
            {
                Url = url,
                Start = start,
                Duration = duration,
                Crops = crops,
                VideoConversionFormat = videoFormat,
                AudioConversionFormat = audioFormat
            };

            int maxRes = cpMainControlPanel.LimitsEnabled ? cpMainControlPanel.MaxResolutionValue : AppSettings.Default.General.MaxResolutionValue;
            int maxSize = cpMainControlPanel.LimitsEnabled ? cpMainControlPanel.MaxFilesize : AppSettings.Default.General.MaxFilesizeBest;

            string mainFormatString = "bestvideo{0}{1}+bestaudio/best{0}{1}";
            string audioFormatString = "bestaudio{0}";
            string finalFormatString = String.Format(mainFormatString,
                maxRes > 0 ? $"[height<={maxRes}]" : "",
                maxSize > 0 ? $"[filesize<={maxSize}M]" : "");

            string finalAudioFormatString = String.Format(audioFormatString,
                maxSize > 0 ? $"[filesize<={maxSize}M]" : "");

            RunResult<string> result = null;
            if (cpMainControlPanel.PostProcessingEnabled)
            {
                if (streamType != Classes.StreamType.Audio)
                {                    
                    pendingDL.Format = finalFormatString;
                    IConversion conversion = await VideoUtil.PrepareBestYtdlConversion(url, finalFormatString, start, duration, 
                        cpMainControlPanel.ConversionEnabled && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, crops, videoFormat == null ? VideoFormat.UNSPECIFIED : (VideoFormat)videoFormat, 
                        audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat, false, processOutput);
                    if (conversion != null)
                    {
                        string destination = conversion.OutputFilePath;
                        conversion.OnProgress += Conversion_OnProgress;
                        try
                        {
                            VideoUtil.CancellationTokenSource = new System.Threading.CancellationTokenSource();
                            cpMainControlPanel.ShowProgress();
                            await conversion.Start(VideoUtil.CancellationTokenSource.Token);
                            result = new RunResult<string>(true, new string[] { }, destination);
                            cpMainControlPanel.HideProgress();
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.ToLower() != "a task was canceled.")
                                ExceptionHandler.LogFFmpegException(ex, true, url);
                            result = new RunResult<string>(false, new string[] { ex.Message }, null);
                        }
                    }
                    else
                    {
                        result = new RunResult<string>(false, new string[] { "Conversion Failed" }, null);
                    }
                }
                else
                {
                    pendingDL.Format = finalAudioFormatString;
                    IConversion conversion = await VideoUtil.PrepareBestYtdlConversion(url, finalAudioFormatString, start, duration, 
                        cpMainControlPanel.ConversionEnabled && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, null, VideoFormat.UNSPECIFIED, 
                        audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat, cpMainControlPanel.EmbedThumbnail, processOutput);
                    if (conversion != null)
                    {
                        string destination = conversion.OutputFilePath;
                        conversion.OnProgress += Conversion_OnProgress;
                        try
                        {
                            VideoUtil.CancellationTokenSource = new System.Threading.CancellationTokenSource();
                            await conversion.Start(VideoUtil.CancellationTokenSource.Token);
                            result = new RunResult<string>(true, new string[] { }, destination);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.ToLower() != "a task was canceled.")
                                ExceptionHandler.LogFFmpegException(ex, true, url);
                            result = new RunResult<string>(false, new string[] { ex.Message }, null);
                        }
                    }
                    else
                    {
                        result = new RunResult<string>(false, new string[] { "Conversion Failed" }, null);
                    }
                }
            }
            else
            {
                pendingDL.Format = streamType == Classes.StreamType.AudioAndVideo ? finalFormatString : finalAudioFormatString;
                cpMainControlPanel.ShowProgress();
                RunResult<string> test = null;
                if (cpMainControlPanel.ConversionEnabled && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)                
                    test = await Utils.VideoUtil.DownloadPreferredYtdl(VideoUtil.ConvertToYouTubeLink(url).Url, streamType);
                else 
                    test = await Utils.VideoUtil.DownloadBestYtdl(VideoUtil.ConvertToYouTubeLink(url).Url, streamType, cpMainControlPanel.EmbedThumbnail);

                if (!test.Success)
                {                  
                    // FALLBACK IS CURRENTLY LIMITED TO REDDIT
                    // THIS SECTION MAY NEED TO BE EXPANDED FOR OTHER SUPPORTED HOSTS

                    if (this.currentDownload == DownloadType.Reddit)
                    {
                        var data = await Utils.VideoUtil.GetVideoData(VideoUtil.ConvertToYouTubeLink(url).Url);
                        List<YTDLFormatData> converted = new List<YTDLFormatData>();
                        if (data.Formats != null && data.Formats.Count() > 0)
                        {
                            YTR.Classes.StreamType detect = AppSettings.DetectStreamTypeFromExtension(data.Extension);
                            if (detect != Classes.StreamType.Unknown)
                            {                                
                                if (cpMainControlPanel.ConversionEnabled && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
                                    test = await Utils.VideoUtil.DownloadPreferredYtdl(VideoUtil.ConvertToYouTubeLink(url).Url, detect);
                                else 
                                    test = await Utils.VideoUtil.DownloadBestYtdl(VideoUtil.ConvertToYouTubeLink(url).Url, detect, cpMainControlPanel.EmbedThumbnail);
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
            if (!result.Success && result.Data != "canceled")
            {
                MsgBox.Show("Download Failed\n" + String.Join("\n", result.ErrorOutput));
            }
            VideoUtil.Running = false;
            ipMainInput.marqeeMain.Hide();
            ipMainInput.marqeeMain.Text = ""; 
            cpMainControlPanel.DownloadSelectionVisible = false;
            cpMainControlPanel.DownloadAudioVisible = true;
            cpMainControlPanel.DownloadBestVisible = true;
            this.cpMainControlPanel.btnCancelProcess.Visible = false;
            var dlLog = new DownloadLog(VideoUtil.ConvertToYouTubeLink(url).Url,
                mediaData?.Title,
                currentDownload,
                streamType,
                DateTime.Now,
                result.Data,
                pendingDL)
            {
                Format = pendingDL.Format
            };

            await Historian.RecordDownload(dlLog);
            if (result.Success)
            {
                cpMainControlPanel.ShowDownloadLocation(result.Data);
                if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                {
                    string argument = "/select, \"" + result.Data + "\"";

                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
            }
            gcHistory.DataSource = Historian.DownloadHistory;
            refreshHistory();
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
                DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YTR.Controls.Icon.Warning, FormStartPosition.CenterParent);
                if (res == DialogResult.No)
                    return;
            }

            cpMainControlPanel.SetProcessingLimits();

            VideoUtil.Running = true;
            this.UseWaitCursor = true;
            (this.tcMainTabControl.SelectedPage as CustomTabFormPage).IsLocked = true;
            cpMainControlPanel.HideDownloadLocation();
            ipMainInput.marqeeMain.Text = "Preparing Download..";
            ipMainInput.marqeeMain.Show();
            VideoFormat? videoFormat = null;
            AudioFormat? audioFormat = null;

            RunResult<string> result = null;
            cpMainControlPanel.DownloadSelectionVisible = false;
            this.cpMainControlPanel.btnCancelProcess.Visible = true;

            PendingDownload pendingDL = null;

            bool preferredVideo = cpMainControlPanel.CurrentFormatPair.VideoFormat == null ? false : cpMainControlPanel.CurrentFormatPair.VideoFormat.Extension.ToUpper() == AppSettings.Default.Advanced.PreferredVideoFormat.ToString();
            bool preferredAudio = cpMainControlPanel.CurrentFormatPair.AudioFormat == null ? false : cpMainControlPanel.CurrentFormatPair.AudioFormat.Extension.ToUpper() == AppSettings.Default.Advanced.PreferredAudioFormat.ToString();

            if (cpMainControlPanel.PostProcessingEnabled 
                || (cpMainControlPanel.CurrentFormatPair.Type == Classes.StreamType.Audio && cpMainControlPanel.CurrentFormatPair.AudioFormat != null && cpMainControlPanel.CurrentFormatPair.RedditAudioFormat == null && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat && !preferredAudio)
                || (cpMainControlPanel.CurrentFormatPair.Type != Classes.StreamType.Video && ((cpMainControlPanel.CurrentFormatPair.VideoFormat != null && cpMainControlPanel.CurrentFormatPair.VideoFormat.RedditAudioFormat != null) || (cpMainControlPanel.CurrentFormatPair.AudioFormat != null && cpMainControlPanel.CurrentFormatPair.AudioFormat.RedditAudioFormat != null)))
                || (cpMainControlPanel.CurrentFormatPair.VideoFormat != null && cpMainControlPanel.CurrentFormatPair.VideoFormat.VideoCodec != "gif" && cpMainControlPanel.ConversionEnabled && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat && !preferredVideo))
            {
                if (cpMainControlPanel.SegmentEnabled && cpMainControlPanel.SegmentDuration == TimeSpan.Zero)
                {
                    this.UseWaitCursor = false;
                    VideoUtil.Running = false;
                    ipMainInput.marqeeMain.Hide();
                    ipMainInput.marqeeMain.Text = ""; 
                    cpMainControlPanel.DownloadSelectionVisible = false;                    
                    this.cpMainControlPanel.btnCancelProcess.Visible = false;
                    MsgBox.Show("Please specify a valid duration for the segment", "Invalid Duration");
                    return;
                }

                int[] crops = null;

                if (cpMainControlPanel.CropEnabled && cpMainControlPanel.ValidCrops())
                {
                    crops = new int[] { Convert.ToInt32(cpMainControlPanel.CropTop), Convert.ToInt32(cpMainControlPanel.CropBottom), Convert.ToInt32(cpMainControlPanel.CropLeft), 
                        Convert.ToInt32(cpMainControlPanel.CropRight) };
                }

                TimeSpan? start = null;
                TimeSpan? duration = null;
                if (cpMainControlPanel.SegmentEnabled)
                {
                    start = cpMainControlPanel.SegmentStart;
                    duration = AppSettings.Default.Layout.SegmentControlMode == SegmentControlMode.Duration ? cpMainControlPanel.SegmentDuration : cpMainControlPanel.SegmentDuration - cpMainControlPanel.SegmentStart;
                }

                if (cpMainControlPanel.ConversionEnabled)
                {
                    videoFormat = cpMainControlPanel.ConvertVideoFormat;
                    audioFormat = cpMainControlPanel.ConvertAudioFormat;

                    if (videoFormat == VideoFormat.GIF)
                    {
                        if (cpMainControlPanel.SegmentEnabled && duration != null && ((TimeSpan)duration).TotalSeconds > 60)
                        {
                            DialogResult confirm = MsgBox.Show($"You have specified a duration of {((TimeSpan)duration).TotalSeconds} seconds.\nGIF Conversion is limited to 60 seconds.\n\nContinue?", "GIF Duration", Buttons.YesNo, YTR.Controls.Icon.Exclamation);
                            if (confirm == DialogResult.No)
                            {
                                this.UseWaitCursor = false;
                                VideoUtil.Running = false;
                                ipMainInput.marqeeMain.Hide();
                                ipMainInput.marqeeMain.Text = "";
                                cpMainControlPanel.DownloadSelectionVisible = false;
                                this.cpMainControlPanel.btnCancelProcess.Visible = false;
                                return;
                            }
                        }
                        else if (!cpMainControlPanel.SegmentEnabled && 
                            cpMainControlPanel.CurrentFormatPair.VideoFormat != null && 
                            cpMainControlPanel.CurrentFormatPair.VideoFormat.Duration != null &&
                            ((TimeSpan)cpMainControlPanel.CurrentFormatPair.VideoFormat.Duration).TotalSeconds > 60)
                        {
                            DialogResult confirm = MsgBox.Show($"GIF Conversion is limited to 60 seconds.\nOnly the first 60 seconds will be downloaded.\n\nContinue?", "GIF Duration", Buttons.YesNo, YTR.Controls.Icon.Exclamation);
                            if(confirm == DialogResult.No)
                            {
                                this.UseWaitCursor = false;
                                VideoUtil.Running = false;
                                ipMainInput.marqeeMain.Hide();
                                ipMainInput.marqeeMain.Text = "";
                                cpMainControlPanel.DownloadSelectionVisible = false;
                                this.cpMainControlPanel.btnCancelProcess.Visible = false;
                                return;
                            }
                        }
                    }
                }

                pendingDL = new PendingDownload()
                {
                    Url = VideoUtil.ConvertToYouTubeLink(ipMainInput.URL).Url,
                    Start = start,
                    Duration = duration,
                    Crops = crops,
                    VideoConversionFormat = videoFormat,
                    AudioConversionFormat = audioFormat
                };                

                IConversion conversion = await Utils.VideoUtil.PrepareYoutubeConversion(VideoUtil.ConvertToYouTubeLink(ipMainInput.URL).Url, cpMainControlPanel.CurrentFormatPair, 
                    start, duration, cpMainControlPanel.ConversionEnabled && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, crops, videoFormat == null ? VideoFormat.UNSPECIFIED : (VideoFormat)videoFormat, 
                    audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat);
                if (conversion != null)
                {
                    string destination = conversion.OutputFilePath;
                    conversion.OnProgress += Conversion_OnProgress;
                    cpMainControlPanel.ShowProgress();

                    try
                    {
                        VideoUtil.CancellationTokenSource = new System.Threading.CancellationTokenSource();
                        await conversion.Start(VideoUtil.CancellationTokenSource.Token);

                        if (cpMainControlPanel.EmbedThumbnail)
                        {
                            if (cpMainControlPanel.ConversionEnabled && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
                                audioFormat = AppSettings.Default.Advanced.PreferredAudioFormat;

                            var data = await VideoUtil.GetVideoData(VideoUtil.ConvertToYouTubeLink(ipMainInput.URL).Url);
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
                        if (ex.Message.ToLower() != "a task was canceled.")
                            ExceptionHandler.LogFFmpegException(ex, true, VideoUtil.ConvertToYouTubeLink(ipMainInput.URL).Url);
                    }
                }
                else
                {
                    result = new RunResult<string>(false, new string[] { "Conversion Failed" }, null);
                }
                cpMainControlPanel.HideProgress();
            }
            else
            {
                pendingDL = new PendingDownload()
                {
                    Url = VideoUtil.ConvertToYouTubeLink(ipMainInput.URL).Url,
                    VideoConversionFormat = videoFormat,
                    AudioConversionFormat = audioFormat
                };

                cpMainControlPanel.ShowProgress();
                result = await Utils.VideoUtil.DownloadYTDLFormat(VideoUtil.ConvertToYouTubeLink(cpMainControlPanel.CurrentFormatPair.VideoCodec == "gif" ? cpMainControlPanel.CurrentFormatPair.VideoFormat.Url : ipMainInput.URL).Url, cpMainControlPanel.CurrentFormatPair, cpMainControlPanel.EmbedThumbnail);
                cpMainControlPanel.HideProgress();
                if (!result.Success)
                {
                    YTRErrorMessageBox eb = new YTRErrorMessageBox(String.Join("\n", result.ErrorOutput), "Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                    eb.ShowDialog();
                }
            }
            pendingDL.Format = cpMainControlPanel.CurrentFormatPair.FormatId;

            YTR.Classes.StreamType t = Classes.StreamType.Audio;
            if (cpMainControlPanel.CurrentFormatPair.AudioCodec == "none")
                t = Classes.StreamType.Video;
            else if (cpMainControlPanel.CurrentFormatPair.AudioFormat != null && (cpMainControlPanel.CurrentFormatPair.VideoFormat == null || (cpMainControlPanel.CurrentFormatPair.VideoFormat != null && cpMainControlPanel.CurrentFormatPair.VideoFormat.Resolution == "audio only")))
                t = Classes.StreamType.Audio;
            else
                t = Classes.StreamType.AudioAndVideo;
            ipMainInput.marqeeMain.Hide();
            ipMainInput.marqeeMain.Text = "";
            cpMainControlPanel.DownloadSelectionVisible = true;
            this.cpMainControlPanel.btnCancelProcess.Visible = false;
            VideoUtil.Running = false;
            if (result.Success)
            {
                var dlLog = await Task.Run(() =>
                {
                    return new DownloadLog(
                        VideoUtil.ConvertToYouTubeLink(ipMainInput.URL).Url,
                        this.videoInfoPanel.Title,
                        this.currentDownload,
                        t, DateTime.Now,
                        result.Data,
                        pendingDL)
                    {
                        Format = cpMainControlPanel.CurrentFormatPair.FormatId,
                        FormatPair = cpMainControlPanel.CurrentFormatPair
                    };
                });
                await Historian.RecordDownload(dlLog);
                cpMainControlPanel.ShowDownloadLocation(result.Data);
                if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                {
                    string argument = "/select, \"" + result.Data + "\"";

                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
            }
            gcHistory.DataSource = Historian.DownloadHistory;
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
                        var tsCheck = TimeSpan.TryParse(e.Value.ToString(), out TimeSpan ts);
                        if (tsCheck) 
                            e.DisplayText = ts.ToString(@"hh\:mm\:ss");
                        var fCheck = float.TryParse(e.Value.ToString(), out float f);
                        if (fCheck)
                        {
                            TimeSpan t = TimeSpan.FromSeconds(f);
                            e.DisplayText = t.ToString(@"hh\:mm\:ss");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                if (ex.Message.ToLower() != "a task was canceled.")
                    ExceptionHandler.LogException(ex);
            }
        }
        private void gvFormats_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (AppSettings.Default.Layout.FormatMode == FormatMode.Custom || ipMainInput.ListMode == ListMode.List)
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
            if (fd.Type == Classes.StreamType.Video || fd.Type == Classes.StreamType.AudioAndVideo)
            {
                cpMainControlPanel.SetCurrentFormats(fd);
                if (fd.Width != null && fd.Height != null)
                {
                    videoInfoPanel.UseMediaSize = new Size((int)fd.Width, (int)fd.Height);
                    videoInfoPanel.ShowCropButton(true);
                }
                else
                {
                    videoInfoPanel.UseMediaSize = new Size(0, 0);
                    videoInfoPanel.ShowCropButton(false);
                }
            }
            else if (fd.Type == Classes.StreamType.Audio)
                cpMainControlPanel.SetCurrentFormats(null, fd);
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
                YTR.Classes.StreamType? type = e.CellValue as Classes.StreamType?;
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

            if (ipMainInput.ListMode == ListMode.List)
            {
                var selectedItemRows = gvFormats.GetSelectedRows();
                int selected = selectedItemRows.Length;

                cpMainControlPanel.DownloadSelectionVisible = false;
                cpMainControlPanel.DownloadBestVisible = true;
                cpMainControlPanel.DownloadAudioVisible = true;

                List<YTDLPlaylistData> selectedItems = new List<YTDLPlaylistData>();
                foreach (int i in selectedItemRows)
                {
                    selectedItems.Add((YTDLPlaylistData)gvFormats.GetRow(i));
                }
                cpMainControlPanel.SetCurrentPlaylistItems(playlistItemCollection.PlaylistData, selectedItems);
                cpMainControlPanel.SetSelectionText("Playlist Download" + (selected > 0 ? $" ({selected} Items)" : ""));
                return;
            }
            else
            {
                int handle = e.ControllerRow;
                YTDLFormatData selection = (YTDLFormatData)gvFormats.GetRow(handle);

                if (selection == null) return;

                if (e.Action == System.ComponentModel.CollectionChangeAction.Add)
                {
                    deselecting = true;
                    if (selection.Type == Classes.StreamType.Audio)
                    {
                        if (cpMainControlPanel.CurrentFormatPair.Type == Classes.StreamType.AudioAndVideo)
                        {
                            if (selectedAudioIndex >= 0 && selectedAudioIndex != handle)
                            {
                                gvFormats.UnselectRow(selectedAudioIndex);
                            }
                            else
                            {
                                gvFormats.UnselectRow(handle);
                                selectedAudioIndex = -1;
                                deselecting = false;
                                return;
                            }
                            deselecting = false;
                        }
                        else
                        {
                            if (selectedAudioIndex >= 0)
                                gvFormats.UnselectRow(selectedAudioIndex);
                        }
                        selectedAudioIndex = handle;
                    }
                    else
                    {
                        if (selectedVideoIndex >= 0)
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
                else if (e.Action == System.ComponentModel.CollectionChangeAction.Remove)
                {
                    if (selection.Type == Classes.StreamType.Audio)
                        selectedAudioIndex = -1;
                    else
                        selectedVideoIndex = -1;
                    cpMainControlPanel.RemoveCurrentFormat(selection.Type);
                }

                if (cpMainControlPanel.CurrentFormatPair != null)
                {
                    if (cpMainControlPanel.CurrentFormatPair.Width != null && cpMainControlPanel.CurrentFormatPair.Height != null)
                    {
                        videoInfoPanel.UseMediaSize = new Size((int)cpMainControlPanel.CurrentFormatPair.Width, (int)cpMainControlPanel.CurrentFormatPair.Height);
                        videoInfoPanel.ShowCropButton(true);
                    }
                    else
                    {
                        videoInfoPanel.UseMediaSize = new Size(0, 0);
                        videoInfoPanel.ShowCropButton(false);
                    }
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
        }

        private void gvFormats_RowClick(object sender, RowClickEventArgs e)
        {
            bool test = gvFormats.IsValidRowHandle(e.RowHandle);
            if (!gvFormats.IsValidRowHandle(e.RowHandle) || gvFormats.IsGroupRow(e.RowHandle)) return;
            GridHitInfo hit = gvFormats.CalcHitInfo(e.Location);
            
            if (!hit.InRow || hit.Column.FieldName == "Selected") return;

            if (ipMainInput.ListMode == ListMode.Format)
            {
                e.Handled = true;
                if (selectedAudioIndex == e.RowHandle || selectedVideoIndex == e.RowHandle)
                    gvFormats.UnselectRow(e.RowHandle);
                else
                    gvFormats.SelectRow(e.RowHandle);
            }
            else if (ipMainInput.ListMode == ListMode.List)
            {
                e.Handled = true;
                if (gvFormats.IsRowSelected(e.RowHandle))
                {
                    gvFormats.UnselectRow(e.RowHandle);
                }
                else
                {
                    gvFormats.SelectRow(e.RowHandle);
                }
            }
        }

        private void cancelProcessButtons_MouseMove(object sender, EventArgs e)
        {
            if (this.UseWaitCursor) { this.UseWaitCursor = false; }
        }

        private void cancelProcessButtons_MouseLeave(object sender, EventArgs e)
        {
            if (VideoUtil.Running) { this.UseWaitCursor = true; }
        }

        private void cpMainControlPanel_CancelProcess_Click(object sender, EventArgs e)
        {
            VideoUtil.CancellationTokenSource.Cancel();
            ipMainInput.marqeeMain.Hide();
            ipMainInput.marqeeMain.Text = "";
        }

        private void cpMainControlPanel_ReDownload_Click(object sender, EventArgs e)
        {
            if (TargetLog != null)
            {
                ipMainInput.URL = TargetLog.Url;

                if (TargetLog.Start != null && TargetLog.Duration != null)
                {
                    cpMainControlPanel.EnableToggle(true, false, false, true);
                    cpMainControlPanel.SegmentStart = (TimeSpan)TargetLog.Start;
                    cpMainControlPanel.SegmentDuration = TargetLog.SegmentMode == SegmentControlMode.Duration ? (TimeSpan)TargetLog.Duration : (TimeSpan)TargetLog.Start + (TimeSpan)TargetLog.Duration;
                }
                if (TargetLog.Crops != null && TargetLog.Crops.Length > 0)
                {
                    cpMainControlPanel.EnableToggle(false, true, false, true);
                    cpMainControlPanel.CropTop = TargetLog.Crops[0].ToString();
                    cpMainControlPanel.CropBottom = TargetLog.Crops[1].ToString();
                    cpMainControlPanel.CropLeft = TargetLog.Crops[2].ToString();
                    cpMainControlPanel.CropRight = TargetLog.Crops[3].ToString();
                }
                if ((TargetLog.VideoConversionFormat != null && TargetLog.VideoConversionFormat != VideoFormat.UNSPECIFIED)
                    || (TargetLog.AudioConversionFormat != null && TargetLog.AudioConversionFormat != AudioFormat.UNSPECIFIED))
                {
                    cpMainControlPanel.EnableToggle(false, false, true, true);
                    cpMainControlPanel.ConvertVideoFormat = TargetLog.VideoConversionFormat;
                }
                if (TargetLog.MaxResolution != null && TargetLog.MaxResolution != Resolution.ANY)
                {
                    cpMainControlPanel.EnableToggle(false, false, false, true);
                    cpMainControlPanel.MaxResolution = TargetLog.MaxResolution;
                }
                if(TargetLog.MaxFileSize != null && TargetLog.MaxFileSize > 0)
                {
                    cpMainControlPanel.EnableToggle(false, false, false, true);
                    cpMainControlPanel.MaxFilesize = (int)TargetLog.MaxFileSize;
                }

                if (TargetLog.FormatPair == null)
                {
                    ytdlDownloadBest(TargetLog.Url, TargetLog.StreamType);
                }
                else
                {
                    cpMainControlPanel.SetCurrentFormatPair(TargetLog.FormatPair);                    

                    ytdlDownloadSelection();
                }

            }
            else
                MsgBox.Show("Error reading log info");
        }

        private void cpMainControlPanel_NewDownload_Click(object sender, EventArgs e)
        {
            if (TargetLog != null)
                ipMainInput.URL = TargetLog.Url;
            else
                MsgBox.Show("Error reading log info");
        }

        private async void loadThumbnailsAsync()
        {
            foreach(YTDLPlaylistData item in this.playlistItemCollection.Items)
            {
                if (VideoUtil.CancellationTokenSource.Token.IsCancellationRequested) break;
                if (!string.IsNullOrEmpty(item.ThumbUrl))
                {
                    var imagebytes = await HttpUtil.GetImageAsByteArrayAsync(item.ThumbUrl);
                    if (imagebytes != null)
                    {
                        using (MemoryStream ms = new MemoryStream(imagebytes))
                        {
                            item.ThumbnailImage = Image.FromStream(ms);
                            gcFormats.RefreshDataSource();
                        }
                    }
                }
            }
            return;
        }

        bool allSelected = false;
        private void btnPLSelectAll_Click(object sender, EventArgs e)
        {
            if(ipMainInput.ListMode != ListMode.List) return;
            if (allSelected)
            {
                gvFormats.ClearSelection();
                allSelected = false;
                btnPLSelectAll.Text = "Select All";
            }
            else
            {
                gvFormats.SelectAll();
                allSelected = true;
                btnPLSelectAll.Text = "Clear Selection";
            }
        }

        private void cpMainControlPanel_Controls_Updated(object sender, EventArgs e)
        {
            updateControlPanelDisplay();
        }

        public void UpdateControlPanelDisplay()
        {
            updateControlPanelDisplay();
        }

        private bool adjustedSplitter = false;
        private void updateControlPanelDisplay()
        {
            if (cpMainControlPanel.TotalControlSize.Height > sccMainSplitter.Panel2.Size.Height)
            {
                if (pnlScrollableControls.VerticalScroll.Visible && !adjustedSplitter)
                {
                    sccMainSplitter.SplitterPosition = sccMainSplitter.SplitterPosition - 25;
                    adjustedSplitter = true;
                }
                cpMainControlPanel.Dock = DockStyle.Top;
                cpMainControlPanel.MinimumSize = cpMainControlPanel.TotalControlSize;
                cpMainControlPanel.AdjustControls(pnlScrollableControls.Height);
            }
            else
            {
                if (!pnlScrollableControls.VerticalScroll.Visible && adjustedSplitter)
                {
                    sccMainSplitter.SplitterPosition = sccMainSplitter.SplitterPosition + 25;
                    adjustedSplitter = false;
                }
                cpMainControlPanel.Dock = DockStyle.Fill;
                cpMainControlPanel.MinimumSize = new Size(0, 0);
                cpMainControlPanel.AdjustControls(pnlScrollableControls.Height);
            }
        }

        private void videoInfoPanel_Crop_Click(object sender, EventArgs e)
        {
            using (var cropForm = new CropForm(videoInfoPanel.CurrentImage.Clone() as Image, videoInfoPanel.UseMediaSize))
            {
                cropForm.StartPosition = FormStartPosition.CenterScreen;
                DialogResult cropRes = cropForm.ShowDialog();
                if (cropRes == DialogResult.OK && cropForm.Crops != null)
                {
                    if(!cpMainControlPanel.CropEnabled)
                        cpMainControlPanel.EnableToggle(false, true, false, true, false);

                    cpMainControlPanel.CropTop = cropForm.Crops[0].ToString();
                    cpMainControlPanel.CropBottom = cropForm.Crops[1].ToString();
                    cpMainControlPanel.CropLeft = cropForm.Crops[2].ToString();
                    cpMainControlPanel.CropRight = cropForm.Crops[3].ToString();
                }
                else if (cropRes == DialogResult.Cancel)
                {
                    return;
                }
            }
        }

        private void gvFormats_RowCountChanged(object sender, EventArgs e)
        {
            videoInfoPanel.ShowCropButton(
                (gvFormats.RowCount == 0 && cpMainControlPanel.CurrentFormatPair == null)
                || (gvFormats.RowCount > 0 && cpMainControlPanel.CurrentFormatPair != null
                && (cpMainControlPanel.CurrentFormatPair.Type == Classes.StreamType.Video || cpMainControlPanel.CurrentFormatPair.Type == Classes.StreamType.AudioAndVideo)));
        }

        private int selectedHistoryIndex = -1;
        private void gcHistory_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                GridHitInfo hitInfo = gvHistory.CalcHitInfo(e.Location);
                if (hitInfo != null && hitInfo.InDataRow)
                {
                    selectedHistoryIndex = hitInfo.RowHandle;
                    historyPopup.ShowPopup(Control.MousePosition);
                }
            }
        }

        private void gvHistory_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Logging.DownloadLog row = gvHistory.GetRow(gvHistory.FocusedRowHandle) as Logging.DownloadLog;
                if (row != null && row.FileExists)
                {
                    openFileLocation(row.DownloadLocation);
                }
            }
            catch (Exception ex)
            {
                Logging.ExceptionHandler.LogException(ex);
            }
        }
        private void openFileLocation(string path)
        {
            string argument = "/select, \"" + path + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void bbiReDownload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cpMainControlPanel_ReDownload_Click(sender, e);
        }

        private void bbiNewDownload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cpMainControlPanel_NewDownload_Click(sender, e);
        }

        private void historyTooltip_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            GridHitInfo hitInfo = gvHistory.CalcHitInfo(e.ControlMousePosition);
            if (hitInfo != null && hitInfo.InRowCell)
            {
                object o;
                if (hitInfo.Column.FieldName == "FileExists")
                {
                    bool fileExits = Convert.ToBoolean(gvHistory.GetRowCellValue(hitInfo.RowHandle, "FileExists"));
                    o = $"{hitInfo.HitTest}{hitInfo.RowHandle}";
                    e.Info = new DevExpress.Utils.ToolTipControlInfo(o, fileExits ? "File Exists" : "File Not Found");
                }
                if (hitInfo.Column.FieldName == "AdditionalSettings")
                {
                    string details = "";
                    o = $"{hitInfo.HitTest}{hitInfo.RowHandle}";
                    var row = gvHistory.GetRow(hitInfo.RowHandle) as DownloadLog;
                    if (!string.IsNullOrEmpty(row.PlaylistUrl))
                    {
                        details += $"Playlist: {row.PlaylistTitle}\nPlaylist URL: {row.PlaylistUrl}\n";
                    }

                    details += $"Title: {row.Title}\nURL: {row.Url}\nFormat: {row.Format}\n";

                    if (row.AdditionalSettings)
                    {
                        if (row.Start != null && row.Duration != null)
                        {
                            details += $"Segment Start: {(TimeSpan)row.Start} - Duration: {(TimeSpan)row.Duration}\n";
                        }
                        if (row.Crops != null && row.Crops.Length > 0)
                        {
                            details += $"Crop: Top ({row.Crops[0]}) Bottom ({row.Crops[1]}) Left ({row.Crops[2]}) Right ({row.Crops[3]})\n";
                        }
                        if (row.VideoConversionFormat != null)
                        {
                            details += $"Convert Video: {row.VideoConversionFormat}\n";
                        }
                        if (row.AudioConversionFormat != null)
                        {
                            details += $"Convert Audio: {row.AudioConversionFormat}\n";
                        }
                        if (row.MaxResolution != null)
                        {
                            details += $"Max Resolution: {row.MaxResolution.ToFriendlyString(false, false)}\n";
                        }
                    }
                    e.Info = new DevExpress.Utils.ToolTipControlInfo(o, details);
                }
            }
        }

        public void PopulateHistoryColumns()
        {
            gvHistory.Columns.Clear();
            gvHistory.PopulateColumns();
            gvHistory.Columns["FileExists"].VisibleIndex = 0;
            gvHistory.Columns["DownloadType"].VisibleIndex = 1;
            gvHistory.Columns["DownloadLocation"].VisibleIndex = 2;
            gvHistory.Columns["AdditionalSettings"].VisibleIndex = 3;
            gvHistory.Columns["AdditionalSettings"].MaxWidth = 25;
            gvHistory.Columns["AdditionalSettings"].ColumnEdit = repPostProcessed;
            gvHistory.Columns["AdditionalSettings"].OptionsColumn.ShowCaption = false;
            gvHistory.Columns["FileExists"].ColumnEdit = repFileExists;
            gvHistory.Columns["FileExists"].FieldName = "FileExists";
            gvHistory.Columns["FileExists"].OptionsColumn.ShowCaption = false;
            gvHistory.Columns["FileExists"].MinWidth = 18;
            gvHistory.Columns["FileExists"].MaxWidth = 25;
            gvHistory.Columns["FileExists"].Width = 18;
            gvHistory.Columns["FileExists"].ToolTip = "File Exists?";
            gvHistory.Columns["DownloadType"].Width = 50;
            gvHistory.Columns["DownloadType"].MaxWidth = 50;
            gvHistory.Columns["DownloadType"].Caption = "Type";
            gvHistory.Columns["Url"].Visible = false;
            gvHistory.Columns["InSubFolder"].Visible = false;
            gvHistory.Columns["PlaylistTitle"].Visible = false;
            gvHistory.Columns["PlaylistUrl"].Visible = false;
            gvHistory.Columns["TimeLogged"].Visible = false;
            gvHistory.Columns["StreamType"].Visible = false;
            gvHistory.Columns["Downloaded"].Visible = false;
            gvHistory.Columns["Format"].Visible = false;
            gvHistory.Columns["FormatPair"].Visible = false;
            gvHistory.Columns["Start"].Visible = false;
            gvHistory.Columns["Duration"].Visible = false;
            gvHistory.Columns["Crops"].Visible = false;
            gvHistory.Columns["VideoConversionFormat"].Visible = false;
            gvHistory.Columns["AudioConversionFormat"].Visible = false;
            gvHistory.Columns["MaxResolution"].Visible = false;
            gvHistory.Columns["MaxFileSize"].Visible = false;
            gvHistory.Columns["SegmentMode"].Visible = false;
            gvHistory.Columns["Title"].Visible = false;
            gvHistory.RefreshData();
        }

        private void btnShowHideHistory_Click(object sender, EventArgs e)
        {
            AppSettings.Default.General.CollapseHistoryPanel = !AppSettings.Default.General.CollapseHistoryPanel;
            AppSettings.Default.Save();
            updateHistoryPanel();
        }

        private async void btnClearHistory_Click(object sender, EventArgs e)
        {
            await Logging.Historian.CleanHistory(Logging.DownloadCategory.All);
            refreshHistory();
            MsgBox.Show("Download History Cleared", FormStartPosition.CenterParent);
        }

        private async void btnDelVidDLs_Click(object sender, EventArgs e)
        {
            DialogResult res = MsgBox.Show("Files for all recorded Video downloads will be deleted and Video download logs will be removed.\n\nContinue?", "Delete Downloaded Files", YTR.Controls.Buttons.OKCancel, YTR.Controls.Icon.Warning, FormStartPosition.CenterParent);
            if (res == DialogResult.OK)
            {
                await Logging.Historian.CleanHistory(Logging.DownloadCategory.Video, Logging.DownloadCategory.Video);
                refreshHistory();
                MsgBox.Show("Downloads Cleared", FormStartPosition.CenterParent);
            }
        }

        private async void btnDelAudDLs_Click(object sender, EventArgs e)
        {
            DialogResult res = MsgBox.Show("Files for all recorded Audio downloads will be deleted and Audio download logs will be removed.\n\nContinue?", "Delete Downloaded Files", YTR.Controls.Buttons.OKCancel, YTR.Controls.Icon.Warning, FormStartPosition.CenterParent);
            if (res == DialogResult.OK)
            {
                await Logging.Historian.CleanHistory(Logging.DownloadCategory.Audio, Logging.DownloadCategory.Audio);
                refreshHistory();
                MsgBox.Show("Downloads Cleared", FormStartPosition.CenterParent);
            }
        }

        private async void btnDelAllDLs_Click(object sender, EventArgs e)
        {
            DialogResult res = MsgBox.Show("Files for all recorded downloads will be deleted and download history will be reset.\n\nContinue?", "Delete Downloaded Files", YTR.Controls.Buttons.OKCancel, YTR.Controls.Icon.Warning, FormStartPosition.CenterParent);
            if (res == DialogResult.OK)
            {
                await Logging.Historian.CleanHistory(Logging.DownloadCategory.All, Logging.DownloadCategory.All);
                refreshHistory();
                MsgBox.Show("Downloads Cleared", FormStartPosition.CenterParent);
            }
        }
    }
}