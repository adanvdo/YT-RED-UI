using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Svg;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private TrayForm activeTrayForm = null;
        private PlaylistItemCollection playlistItemCollection = null;


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
                    YT_RED.Controls.MsgBox.Show($"Failed to Replace Updater", "Something Went Wrong", FormStartPosition.CenterParent);
                }
            }

            if (this.updated || Program.updated)
            {
                bool replaceDependency = await UpdateHelper.ReplaceZipDependency();
                bool deleteBackup = await UpdateHelper.DeleteRemnants();
            }
        }

        private void applyLayout(Settings.LayoutArea layoutArea = LayoutArea.All)
        {
            this.UseWaitCursor = true;
            cpMainControlPanel.gcHistory.Visible = AppSettings.Default.General.EnableDownloadHistory;

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
            cpMainControlPanel.gcHistory.DataSource = Historian.DownloadHistory;
            refreshHistory();
            applyLayout(LayoutArea.Panels);
            cpMainControlPanel.UpdatePanelStates();
            if (res == DialogResult.OK)
            {
                bsiMessage.Caption = "Settings Saved";
                cpMainControlPanel.gcHistory.Visible = AppSettings.Default.General.EnableDownloadHistory;
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
            if(playlistItemCollection != null)
            {
                playlistItemCollection.Dispose();
                playlistItemCollection = null;
            }

            gcFormats.DataSource = null;
            gvFormats.RefreshData();
            cpMainControlPanel.CurrentFormatPair.Clear();
            cpMainControlPanel.ResetControls();
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
                if(link.Type == YoutubeLinkType.Playlist)
                {
                    cpMainControlPanel.DisableToggle(true, true, true);                    
                    cpMainControlPanel.SetSelectionText("Playlist Download");
                    cpMainControlPanel.btnDownloadBest.Text = "DOWNLOAD ALL [audio+video]       ";
                    cpMainControlPanel.btnDownloadAudio.Text = "DOWNLOAD ALL AUDIO     ";
                    ipMainInput.ListMode = ListMode.List;
                }
                else
                {
                    ipMainInput.ListMode = ListMode.Format;
                    if (gvFormats.GetSelectedRows().Length < 1)
                    {
                        cpMainControlPanel.DownloadSelectionVisible = false;
                        cpMainControlPanel.DownloadBestVisible = true;
                        cpMainControlPanel.DownloadAudioVisible = true;
                        return;
                    }
                    cpMainControlPanel.EnableToggle(true, true, true);
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
            cpMainControlPanel.ResetControls();
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
                        cpMainControlPanel.ResetControls();
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
            int originalIndex = cpMainControlPanel.gvHistory.FocusedRowHandle;
            cpMainControlPanel.gcHistory.DataSource = null;
            cpMainControlPanel.gcHistory.DataSource = Historian.DownloadHistory;
            cpMainControlPanel.PopulateHistoryColumns();
            cpMainControlPanel.gvHistory.FocusedRowHandle = originalIndex;
            selectedYTLog = cpMainControlPanel.gvHistory.GetRow(originalIndex) as DownloadLog;
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
                    DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
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
                        YT_RED.Controls.Icon.Warning, 
                        FormStartPosition.CenterParent);
                    if(res == DialogResult.No) { return; }
                }
                else
                {
                    MsgBox.Show("Empty or Invalid Playlist", "Error", Buttons.OK, YT_RED.Controls.Icon.Error, FormStartPosition.CenterParent);
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

            string mainFormatString = "bestvideo{0}{1}+bestaudio/best{0}{1}";
            string audioFormatString = "bestaudio{0}";
            string finalFormatString = String.Format(mainFormatString,
                AppSettings.Default.General.MaxResolutionValue > 0 ? $"[height<={AppSettings.Default.General.MaxResolutionValue}]" : "",
                AppSettings.Default.General.MaxFilesizeBest > 0 ? $"[filesize<={AppSettings.Default.General.MaxFilesizeBest}M]" : "");

            string finalAudioFormatString = String.Format(audioFormatString,
                AppSettings.Default.General.MaxFilesizeBest > 0 ? $"[filesize<={AppSettings.Default.General.MaxFilesizeBest}M]" : "");

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
                if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
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
                    pendingDL);
                initialDLLocation = result.Data;

                await Historian.RecordDownload(dlLog);
                cpMainControlPanel.gcHistory.DataSource = Historian.DownloadHistory;
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
                DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
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

            if (cpMainControlPanel.SegmentEnabled)
            {
                start = cpMainControlPanel.SegmentStart;
                duration = cpMainControlPanel.SegmentDuration;
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

            string mainFormatString = "bestvideo{0}{1}+bestaudio/best{0}{1}";
            string audioFormatString = "bestaudio{0}";
            string finalFormatString = String.Format(mainFormatString,
                AppSettings.Default.General.MaxResolutionValue > 0 ? $"[height<={AppSettings.Default.General.MaxResolutionValue}]" : "",
                AppSettings.Default.General.MaxFilesizeBest > 0 ? $"[filesize<={AppSettings.Default.General.MaxFilesizeBest}M]" : "");

            string finalAudioFormatString = String.Format(audioFormatString,
                AppSettings.Default.General.MaxFilesizeBest > 0 ? $"[filesize<={AppSettings.Default.General.MaxFilesizeBest}M]" : "");

            RunResult<string> result = null;
            if (cpMainControlPanel.PostProcessingEnabled)
            {
                if (streamType != Classes.StreamType.Audio)
                {                    
                    pendingDL.Format = finalFormatString;
                    IConversion conversion = await VideoUtil.PrepareBestYtdlConversion(url, finalFormatString, start, duration, 
                        AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, crops, videoFormat == null ? VideoFormat.UNSPECIFIED : (VideoFormat)videoFormat, 
                        audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat, false, processOutput);
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
                        ExceptionHandler.LogFFmpegException(ex);
                        result = new RunResult<string>(false, new string[] { ex.Message }, null);
                    }
                }
                else
                {
                    pendingDL.Format = finalAudioFormatString;
                    IConversion conversion = await VideoUtil.PrepareBestYtdlConversion(url, finalAudioFormatString, start, duration, 
                        AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, null, VideoFormat.UNSPECIFIED, 
                        audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat, cpMainControlPanel.EmbedThumbnail, processOutput);
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
                        ExceptionHandler.LogFFmpegException(ex);
                        result = new RunResult<string>(false, new string[] { ex.Message }, null);
                    }
                }
            }
            else
            {
                pendingDL.Format = streamType == Classes.StreamType.AudioAndVideo ? finalFormatString : finalAudioFormatString;
                cpMainControlPanel.ShowProgress();
                RunResult<string> test = null;
                if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)                
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
                            YT_RED.Classes.StreamType detect = AppSettings.DetectStreamTypeFromExtension(data.Extension);
                            if (detect != Classes.StreamType.Unknown)
                            {                                
                                if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
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
            cpMainControlPanel.gcHistory.DataSource = Historian.DownloadHistory;
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
                DialogResult res = MsgBox.ShowUrlCheckWarning("The URL entered is not from a supported host. Downloads from this URL may fail or result in errors.\n\nContinue?", "Unrecognized URL", Buttons.YesNo, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
                if (res == DialogResult.No)
                    return;
            }

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
                || (cpMainControlPanel.CurrentFormatPair.VideoFormat != null && cpMainControlPanel.CurrentFormatPair.VideoFormat.VideoCodec != "gif" && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat && !preferredVideo))
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
                    duration = cpMainControlPanel.SegmentDuration;
                }

                if (cpMainControlPanel.ConversionEnabled)
                {
                    videoFormat = cpMainControlPanel.ConvertVideoFormat;
                    audioFormat = cpMainControlPanel.ConvertAudioFormat;
                } 
                else if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
                {
                    videoFormat = AppSettings.Default.Advanced.PreferredVideoFormat;
                    audioFormat = AppSettings.Default.Advanced?.PreferredAudioFormat;
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
                    start, duration, AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat, crops, videoFormat == null ? VideoFormat.UNSPECIFIED : (VideoFormat)videoFormat, 
                    audioFormat == null ? AudioFormat.UNSPECIFIED : (AudioFormat)audioFormat);
                string destination = conversion.OutputFilePath;
                conversion.OnProgress += Conversion_OnProgress;
                cpMainControlPanel.ShowProgress();

                try
                {
                    VideoUtil.CancellationTokenSource = new System.Threading.CancellationTokenSource();
                    await conversion.Start(VideoUtil.CancellationTokenSource.Token);

                    if (cpMainControlPanel.EmbedThumbnail)
                    {
                        if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
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
                    ExceptionHandler.LogFFmpegException(ex);
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

            YT_RED.Classes.StreamType t = Classes.StreamType.Audio;
            if (cpMainControlPanel.CurrentFormatPair.AudioCodec == "none")
                t = Classes.StreamType.Video;
            else if (cpMainControlPanel.CurrentFormatPair.AudioFormat != null || (cpMainControlPanel.CurrentFormatPair.VideoFormat != null && cpMainControlPanel.CurrentFormatPair.VideoFormat.Resolution == "audio only"))
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
                cpMainControlPanel.SetCurrentFormats(fd);
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
                            gvFormats.UnselectRow(handle);
                            deselecting = false;
                            return;
                        }

                        if (selectedAudioIndex >= 0)
                            gvFormats.UnselectRow(selectedAudioIndex);
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
            if (cpMainControlPanel.TargetLog != null)
            {
                ipMainInput.URL = cpMainControlPanel.TargetLog.Url;

                if (cpMainControlPanel.TargetLog.Start != null && cpMainControlPanel.TargetLog.Duration != null)
                {
                    cpMainControlPanel.EnableToggle(true, false, false, true);
                    cpMainControlPanel.SegmentStart = (TimeSpan)cpMainControlPanel.TargetLog.Start;
                    cpMainControlPanel.SegmentDuration = (TimeSpan)cpMainControlPanel.TargetLog.Duration;
                }
                if (cpMainControlPanel.TargetLog.Crops != null && cpMainControlPanel.TargetLog.Crops.Length > 0)
                {
                    cpMainControlPanel.EnableToggle(false, true, false, true);
                    cpMainControlPanel.CropTop = cpMainControlPanel.TargetLog.Crops[0].ToString();
                    cpMainControlPanel.CropBottom = cpMainControlPanel.TargetLog.Crops[1].ToString();
                    cpMainControlPanel.CropLeft = cpMainControlPanel.TargetLog.Crops[2].ToString();
                    cpMainControlPanel.CropRight = cpMainControlPanel.TargetLog.Crops[3].ToString();
                }
                if((cpMainControlPanel.TargetLog.VideoConversionFormat != null && cpMainControlPanel.TargetLog.VideoConversionFormat != VideoFormat.UNSPECIFIED)
                    || (cpMainControlPanel.TargetLog.AudioConversionFormat != null && cpMainControlPanel.TargetLog.AudioConversionFormat != AudioFormat.UNSPECIFIED))
                {
                    cpMainControlPanel.EnableToggle(false, false, true, true);
                    cpMainControlPanel.ConvertVideoFormat = cpMainControlPanel.TargetLog.VideoConversionFormat;
                }

                if (cpMainControlPanel.TargetLog.FormatPair == null)
                {
                    ytdlDownloadBest(cpMainControlPanel.TargetLog.Url, cpMainControlPanel.TargetLog.Type);
                }
                else
                {
                    cpMainControlPanel.SetCurrentFormatPair(cpMainControlPanel.TargetLog.FormatPair);                    

                    ytdlDownloadSelection();
                }

            }
            else
                MsgBox.Show("Error reading log info");
        }

        private void cpMainControlPanel_NewDownload_Click(object sender, EventArgs e)
        {
            if (cpMainControlPanel.TargetLog != null)
                ipMainInput.URL = cpMainControlPanel.TargetLog.Url;
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
    }
}