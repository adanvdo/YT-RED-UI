namespace YT_RED
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tcMainTabControl = new DevExpress.XtraBars.TabFormControl();
            this.bsiMessage = new DevExpress.XtraBars.BarStaticItem();
            this.skinBarSubItem1 = new DevExpress.XtraBars.SkinBarSubItem();
            this.bbiSettings = new DevExpress.XtraBars.BarButtonItem();
            this.tfpMain = new YT_RED.Controls.CustomTabFormPage();
            this.tabFormContentContainer1 = new DevExpress.XtraBars.TabFormContentContainer();
            this.sccMainSplitter = new DevExpress.XtraEditors.SplitContainerControl();
            this.gcFormats = new DevExpress.XtraGrid.GridControl();
            this.gvFormats = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.pnlPlaylistControls = new DevExpress.XtraEditors.PanelControl();
            this.btnPLSelectAll = new DevExpress.XtraEditors.SimpleButton();
            this.lblSelectAFormat = new DevExpress.XtraEditors.LabelControl();
            this.videoInfoPanel = new YT_RED.Controls.VideoInfoPanel();
            this.marqueeProgressBarControl1 = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.ipMainInput = new YT_RED.Controls.InputPanel();
            this.cpMainControlPanel = new YT_RED.Controls.ControlPanel();
            this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.taskBarMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsiDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiExit = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.tcMainTabControl)).BeginInit();
            this.tabFormContentContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sccMainSplitter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sccMainSplitter.Panel1)).BeginInit();
            this.sccMainSplitter.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sccMainSplitter.Panel2)).BeginInit();
            this.sccMainSplitter.Panel2.SuspendLayout();
            this.sccMainSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcFormats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvFormats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPictureEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlPlaylistControls)).BeginInit();
            this.pnlPlaylistControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.marqueeProgressBarControl1.Properties)).BeginInit();
            this.taskBarMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMainTabControl
            // 
            this.tcMainTabControl.AllowMoreTabsButton = DevExpress.Utils.DefaultBoolean.False;
            this.tcMainTabControl.AllowMoveTabsToOuterForm = false;
            this.tcMainTabControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bsiMessage,
            this.skinBarSubItem1,
            this.bbiSettings});
            this.tcMainTabControl.Location = new System.Drawing.Point(0, 0);
            this.tcMainTabControl.Name = "tcMainTabControl";
            this.tcMainTabControl.Pages.Add(this.tfpMain);
            this.tcMainTabControl.SelectedPage = this.tfpMain;
            this.tcMainTabControl.ShowAddPageButton = false;
            this.tcMainTabControl.Size = new System.Drawing.Size(1188, 66);
            this.tcMainTabControl.TabForm = this;
            this.tcMainTabControl.TabIndex = 0;
            this.tcMainTabControl.TabRightItemLinks.Add(this.bsiMessage);
            this.tcMainTabControl.TabRightItemLinks.Add(this.skinBarSubItem1);
            this.tcMainTabControl.TabRightItemLinks.Add(this.bbiSettings);
            this.tcMainTabControl.TabStop = false;
            // 
            // bsiMessage
            // 
            this.bsiMessage.Id = 2;
            this.bsiMessage.Name = "bsiMessage";
            // 
            // skinBarSubItem1
            // 
            this.skinBarSubItem1.AllowSerializeChildren = DevExpress.Utils.DefaultBoolean.False;
            this.skinBarSubItem1.Hint = "Skins";
            this.skinBarSubItem1.Id = 3;
            this.skinBarSubItem1.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.skinBarSubItem1.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("skinBarSubItem1.ImageOptions.SvgImage")));
            this.skinBarSubItem1.Name = "skinBarSubItem1";
            this.skinBarSubItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // bbiSettings
            // 
            this.bbiSettings.Hint = "Settings";
            this.bbiSettings.Id = 0;
            this.bbiSettings.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.bbiSettings.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("bbiSettings.ImageOptions.SvgImage")));
            this.bbiSettings.Name = "bbiSettings";
            this.bbiSettings.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.bbiSettings.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiSettings_ItemClick);
            // 
            // tfpMain
            // 
            this.tfpMain.ContentContainer = this.tabFormContentContainer1;
            this.tfpMain.Name = "tfpMain";
            this.tfpMain.ShowCloseButton = DevExpress.Utils.DefaultBoolean.False;
            this.tfpMain.Text = "Download";
            // 
            // tabFormContentContainer1
            // 
            this.tabFormContentContainer1.Controls.Add(this.sccMainSplitter);
            this.tabFormContentContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFormContentContainer1.Location = new System.Drawing.Point(0, 66);
            this.tabFormContentContainer1.Name = "tabFormContentContainer1";
            this.tabFormContentContainer1.Size = new System.Drawing.Size(1188, 717);
            this.tabFormContentContainer1.TabIndex = 0;
            // 
            // sccMainSplitter
            // 
            this.sccMainSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sccMainSplitter.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
            this.sccMainSplitter.Location = new System.Drawing.Point(0, 0);
            this.sccMainSplitter.Margin = new System.Windows.Forms.Padding(0);
            this.sccMainSplitter.Name = "sccMainSplitter";
            this.sccMainSplitter.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            // 
            // sccMainSplitter.Panel1
            // 
            this.sccMainSplitter.Panel1.Controls.Add(this.gcFormats);
            this.sccMainSplitter.Panel1.Controls.Add(this.pnlPlaylistControls);
            this.sccMainSplitter.Panel1.Controls.Add(this.videoInfoPanel);
            this.sccMainSplitter.Panel1.Controls.Add(this.marqueeProgressBarControl1);
            this.sccMainSplitter.Panel1.Controls.Add(this.ipMainInput);
            this.sccMainSplitter.Panel1.MinSize = 482;
            this.sccMainSplitter.Panel1.Text = "Panel1";
            // 
            // sccMainSplitter.Panel2
            // 
            this.sccMainSplitter.Panel2.Controls.Add(this.cpMainControlPanel);
            this.sccMainSplitter.Panel2.MinSize = 323;
            this.sccMainSplitter.Panel2.Text = "Panel2";
            this.sccMainSplitter.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.sccMainSplitter.Size = new System.Drawing.Size(1188, 717);
            this.sccMainSplitter.SplitterPosition = 839;
            this.sccMainSplitter.TabIndex = 0;
            this.sccMainSplitter.SplitterMoved += new System.EventHandler(this.sccMainSplitter_SplitterMoved);
            this.sccMainSplitter.Resize += new System.EventHandler(this.sccMainSplitter_Resize);
            // 
            // gcFormats
            // 
            this.gcFormats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcFormats.Location = new System.Drawing.Point(0, 298);
            this.gcFormats.MainView = this.gvFormats;
            this.gcFormats.Name = "gcFormats";
            this.gcFormats.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repPictureEdit});
            this.gcFormats.Size = new System.Drawing.Size(839, 419);
            this.gcFormats.TabIndex = 0;
            this.gcFormats.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvFormats});
            // 
            // gvFormats
            // 
            this.gvFormats.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            this.gvFormats.GridControl = this.gcFormats;
            this.gvFormats.Name = "gvFormats";
            this.gvFormats.OptionsBehavior.AlignGroupSummaryInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.gvFormats.OptionsBehavior.Editable = false;
            this.gvFormats.OptionsCustomization.AllowGroup = false;
            this.gvFormats.OptionsDetail.ShowDetailTabs = false;
            this.gvFormats.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            this.gvFormats.OptionsSelection.CheckBoxSelectorField = "Selected";
            this.gvFormats.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvFormats.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gvFormats.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
            this.gvFormats.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.gvFormats.OptionsSelection.ShowCheckBoxSelectorInPrintExport = DevExpress.Utils.DefaultBoolean.False;
            this.gvFormats.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Fast;
            this.gvFormats.OptionsView.ColumnAutoWidth = false;
            this.gvFormats.OptionsView.ShowDetailButtons = false;
            this.gvFormats.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.gvFormats.OptionsView.ShowGroupPanel = false;
            this.gvFormats.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gvFormats_RowClick);
            this.gvFormats.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gvFormats_CustomDrawCell);
            this.gvFormats.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gvFormats_SelectionChanged);
            this.gvFormats.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvFormats_FocusedRowChanged);
            this.gvFormats.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvFormats_CustomColumnDisplayText);
            // 
            // repPictureEdit
            // 
            this.repPictureEdit.AllowAnimationOnValueChanged = DevExpress.Utils.DefaultBoolean.True;
            this.repPictureEdit.AllowDisposeImage = true;
            this.repPictureEdit.AllowFocused = false;
            this.repPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
            this.repPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
            this.repPictureEdit.AllowZoom = DevExpress.Utils.DefaultBoolean.False;
            this.repPictureEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.repPictureEdit.Name = "repPictureEdit";
            this.repPictureEdit.ReadOnly = true;
            this.repPictureEdit.ShowEditMenuItem = DevExpress.Utils.DefaultBoolean.False;
            this.repPictureEdit.ShowMenu = false;
            this.repPictureEdit.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.False;
            this.repPictureEdit.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            // 
            // pnlPlaylistControls
            // 
            this.pnlPlaylistControls.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlPlaylistControls.Controls.Add(this.btnPLSelectAll);
            this.pnlPlaylistControls.Controls.Add(this.lblSelectAFormat);
            this.pnlPlaylistControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPlaylistControls.Location = new System.Drawing.Point(0, 273);
            this.pnlPlaylistControls.Margin = new System.Windows.Forms.Padding(0);
            this.pnlPlaylistControls.Name = "pnlPlaylistControls";
            this.pnlPlaylistControls.Size = new System.Drawing.Size(839, 25);
            this.pnlPlaylistControls.TabIndex = 3;
            // 
            // btnPLSelectAll
            // 
            this.btnPLSelectAll.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnPLSelectAll.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnPLSelectAll.ImageOptions.SvgImage = global::YT_RED.Properties.Resources.checkbox;
            this.btnPLSelectAll.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
            this.btnPLSelectAll.ImageOptions.SvgImageSize = new System.Drawing.Size(18, 18);
            this.btnPLSelectAll.Location = new System.Drawing.Point(0, 0);
            this.btnPLSelectAll.Margin = new System.Windows.Forms.Padding(0);
            this.btnPLSelectAll.Name = "btnPLSelectAll";
            this.btnPLSelectAll.Size = new System.Drawing.Size(110, 25);
            this.btnPLSelectAll.TabIndex = 0;
            this.btnPLSelectAll.Text = "Select All";
            this.btnPLSelectAll.Visible = false;
            this.btnPLSelectAll.Click += new System.EventHandler(this.btnPLSelectAll_Click);
            // 
            // lblSelectAFormat
            // 
            this.lblSelectAFormat.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblSelectAFormat.Appearance.Options.UseFont = true;
            this.lblSelectAFormat.Appearance.Options.UseTextOptions = true;
            this.lblSelectAFormat.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSelectAFormat.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSelectAFormat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelectAFormat.Location = new System.Drawing.Point(0, 0);
            this.lblSelectAFormat.Margin = new System.Windows.Forms.Padding(0);
            this.lblSelectAFormat.Name = "lblSelectAFormat";
            this.lblSelectAFormat.Size = new System.Drawing.Size(839, 25);
            this.lblSelectAFormat.TabIndex = 0;
            this.lblSelectAFormat.Text = "Select a Video and/or Audio Format";
            // 
            // videoInfoPanel
            // 
            this.videoInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.videoInfoPanel.Location = new System.Drawing.Point(0, 123);
            this.videoInfoPanel.Margin = new System.Windows.Forms.Padding(0);
            this.videoInfoPanel.Name = "videoInfoPanel";
            this.videoInfoPanel.Size = new System.Drawing.Size(839, 150);
            this.videoInfoPanel.TabIndex = 2;
            this.videoInfoPanel.Visible = false;
            // 
            // marqueeProgressBarControl1
            // 
            this.marqueeProgressBarControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.marqueeProgressBarControl1.EditValue = 0;
            this.marqueeProgressBarControl1.Location = new System.Drawing.Point(0, 103);
            this.marqueeProgressBarControl1.Name = "marqueeProgressBarControl1";
            this.marqueeProgressBarControl1.Properties.AllowFocused = false;
            this.marqueeProgressBarControl1.Properties.ShowTitle = true;
            this.marqueeProgressBarControl1.Size = new System.Drawing.Size(839, 20);
            this.marqueeProgressBarControl1.TabIndex = 0;
            this.marqueeProgressBarControl1.Visible = false;
            // 
            // ipMainInput
            // 
            this.ipMainInput.AutoSize = true;
            this.ipMainInput.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ipMainInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.ipMainInput.ListMode = YT_RED.Classes.ListMode.Format;
            this.ipMainInput.Location = new System.Drawing.Point(0, 0);
            this.ipMainInput.Margin = new System.Windows.Forms.Padding(0);
            this.ipMainInput.MinimumSize = new System.Drawing.Size(400, 100);
            this.ipMainInput.Name = "ipMainInput";
            this.ipMainInput.ShowCrab = false;
            this.ipMainInput.Size = new System.Drawing.Size(839, 103);
            this.ipMainInput.TabIndex = 1;
            this.ipMainInput.URL = "";
            this.ipMainInput.ListFormats_Click += new System.EventHandler(this.ipMainInput_ListFormats_Click);
            this.ipMainInput.ResetList_Click += new System.EventHandler(this.ipMainInput_ResetList_Click);
            this.ipMainInput.Url_Changed += new System.EventHandler(this.ipMainInput_Url_Changed);
            this.ipMainInput.Crab_Click += new System.EventHandler(this.ipMainInput_Crab_Click);
            this.ipMainInput.Url_KeyDown += new System.Windows.Forms.KeyEventHandler(this.ipMainInput_Url_KeyDown);
            this.ipMainInput.ListFormats_MouseMove += new System.EventHandler(this.cancelProcessButtons_MouseMove);
            this.ipMainInput.ListFormats_MouseLeave += new System.EventHandler(this.cancelProcessButtons_MouseLeave);
            // 
            // cpMainControlPanel
            // 
            this.cpMainControlPanel.ConvertAudioFormat = null;
            this.cpMainControlPanel.ConvertVideoFormat = null;
            this.cpMainControlPanel.CropBottom = "0";
            this.cpMainControlPanel.CropLeft = "0";
            this.cpMainControlPanel.CropRight = "0";
            this.cpMainControlPanel.CropTop = "0";
            this.cpMainControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cpMainControlPanel.DownloadAudioVisible = true;
            this.cpMainControlPanel.DownloadBestVisible = true;
            this.cpMainControlPanel.DownloadSelectionVisible = false;
            this.cpMainControlPanel.Location = new System.Drawing.Point(0, 0);
            this.cpMainControlPanel.MaxFilesize = 0;
            this.cpMainControlPanel.MaxResolution = null;
            this.cpMainControlPanel.MinimumSize = new System.Drawing.Size(323, 0);
            this.cpMainControlPanel.Name = "cpMainControlPanel";
            this.cpMainControlPanel.SegmentDuration = System.TimeSpan.Parse("00:00:01");
            this.cpMainControlPanel.SegmentStart = System.TimeSpan.Parse("00:00:00");
            this.cpMainControlPanel.Size = new System.Drawing.Size(323, 717);
            this.cpMainControlPanel.TabIndex = 2;
            this.cpMainControlPanel.Cancel_MouseMove += new System.EventHandler(this.cancelProcessButtons_MouseMove);
            this.cpMainControlPanel.Cancel_MouseLeave += new System.EventHandler(this.cancelProcessButtons_MouseLeave);
            this.cpMainControlPanel.DownloadSelection_Click += new System.EventHandler(this.cpMainControlPanel_DownloadSelection_Click);
            this.cpMainControlPanel.DownloadAudio_Click += new System.EventHandler(this.cpMainControlPanel_DownloadAudio_Click);
            this.cpMainControlPanel.DownloadBest_Click += new System.EventHandler(this.cpMainControlPanel_DownloadBest_Click);
            this.cpMainControlPanel.CancelProcess_Click += new System.EventHandler(this.cpMainControlPanel_CancelProcess_Click);
            this.cpMainControlPanel.ReDownload_Click += new System.EventHandler(this.cpMainControlPanel_ReDownload_Click);
            this.cpMainControlPanel.NewDownload_Click += new System.EventHandler(this.cpMainControlPanel_NewDownload_Click);
            // 
            // toolTipController
            // 
            this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController_GetActiveObjectInfo);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipText = "Quick Download is now enabled.\r\nConfigure Hotkey in Advanced Settings.\r\nDouble Cl" +
    "ick to open YT-RED";
            this.notifyIcon.BalloonTipTitle = "YT-RED is Still Running";
            this.notifyIcon.ContextMenuStrip = this.taskBarMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "YT-RED";
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // taskBarMenu
            // 
            this.taskBarMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiDownload,
            this.tsiSettings,
            this.tsiExit});
            this.taskBarMenu.Name = "taskBarMenu";
            this.taskBarMenu.Size = new System.Drawing.Size(163, 70);
            // 
            // tsiDownload
            // 
            this.tsiDownload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsiDownload.Name = "tsiDownload";
            this.tsiDownload.Size = new System.Drawing.Size(162, 22);
            this.tsiDownload.Text = "Quick Download";
            this.tsiDownload.Click += new System.EventHandler(this.tsiDownload_Click);
            // 
            // tsiSettings
            // 
            this.tsiSettings.Name = "tsiSettings";
            this.tsiSettings.Size = new System.Drawing.Size(162, 22);
            this.tsiSettings.Text = "Settings";
            this.tsiSettings.Click += new System.EventHandler(this.tsiSettings_Click);
            // 
            // tsiExit
            // 
            this.tsiExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsiExit.Name = "tsiExit";
            this.tsiExit.Size = new System.Drawing.Size(162, 22);
            this.tsiExit.Text = "Exit";
            this.tsiExit.Click += new System.EventHandler(this.tsiExit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1188, 783);
            this.Controls.Add(this.tabFormContentContainer1);
            this.Controls.Add(this.tcMainTabControl);
            this.DoubleBuffered = true;
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("MainForm.IconOptions.Icon")));
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("MainForm.IconOptions.Image")));
            this.MinimumSize = new System.Drawing.Size(823, 664);
            this.Name = "MainForm";
            this.TabFormControl = this.tcMainTabControl;
            this.Text = "YT-RED";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.tcMainTabControl)).EndInit();
            this.tabFormContentContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sccMainSplitter.Panel1)).EndInit();
            this.sccMainSplitter.Panel1.ResumeLayout(false);
            this.sccMainSplitter.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sccMainSplitter.Panel2)).EndInit();
            this.sccMainSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sccMainSplitter)).EndInit();
            this.sccMainSplitter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcFormats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvFormats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPictureEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlPlaylistControls)).EndInit();
            this.pnlPlaylistControls.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.marqueeProgressBarControl1.Properties)).EndInit();
            this.taskBarMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        protected DevExpress.XtraBars.TabFormControl tcMainTabControl;
        private DevExpress.XtraBars.BarButtonItem bbiSettings;
        private DevExpress.XtraBars.BarStaticItem bsiMessage;
        private DevExpress.XtraBars.SkinBarSubItem skinBarSubItem1;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip taskBarMenu;
        private System.Windows.Forms.ToolStripMenuItem tsiDownload;
        private System.Windows.Forms.ToolStripMenuItem tsiExit;
        private System.Windows.Forms.ToolStripMenuItem tsiSettings;
        private DevExpress.Utils.ToolTipController toolTipController;
        private YT_RED.Controls.CustomTabFormPage tfpMain;
        private DevExpress.XtraBars.TabFormContentContainer tabFormContentContainer1;
        private DevExpress.XtraEditors.SplitContainerControl sccMainSplitter;
        private Controls.InputPanel ipMainInput;
        private DevExpress.XtraEditors.LabelControl lblSelectAFormat;
        private DevExpress.XtraGrid.GridControl gcFormats;
        private DevExpress.XtraGrid.Views.Grid.GridView gvFormats;
        private DevExpress.XtraEditors.MarqueeProgressBarControl marqueeProgressBarControl1;
        private Controls.ControlPanel cpMainControlPanel;
        private Controls.VideoInfoPanel videoInfoPanel;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repPictureEdit;
        private DevExpress.XtraEditors.PanelControl pnlPlaylistControls;
        private DevExpress.XtraEditors.SimpleButton btnPLSelectAll;
    }
}

