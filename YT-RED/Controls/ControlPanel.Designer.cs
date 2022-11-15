namespace YT_RED.Controls
{
    partial class ControlPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions1 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions2 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions3 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions4 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            this.lblSelectionText = new DevExpress.XtraEditors.LabelControl();
            this.pnlOptionPanel = new DevExpress.XtraEditors.PanelControl();
            this.gcDLButtons = new DevExpress.XtraEditors.GroupControl();
            this.btnCancelProcess = new DevExpress.XtraEditors.SimpleButton();
            this.btnDownloadBest = new DevExpress.XtraEditors.SimpleButton();
            this.btnDownloadAudio = new DevExpress.XtraEditors.SimpleButton();
            this.btnSelectionDL = new DevExpress.XtraEditors.SimpleButton();
            this.gcDownloadLimits = new DevExpress.XtraEditors.GroupControl();
            this.pnlLimitPanel = new DevExpress.XtraEditors.PanelControl();
            this.txtMaxFilesize = new DevExpress.XtraEditors.TextEdit();
            this.lblMaxFilesize = new DevExpress.XtraEditors.LabelControl();
            this.lblMaxRes = new DevExpress.XtraEditors.LabelControl();
            this.cbMaxRes = new DevExpress.XtraEditors.ComboBoxEdit();
            this.toggleDownloadLimits = new DevExpress.XtraEditors.ToggleSwitch();
            this.gcConvert = new DevExpress.XtraEditors.GroupControl();
            this.hlblOpenSettings = new DevExpress.XtraEditors.HyperlinkLabelControl();
            this.lblAlwaysConvert = new DevExpress.XtraEditors.LabelControl();
            this.pnlConvertPanel = new DevExpress.XtraEditors.PanelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.cbAudioFormat = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cbVideoFormat = new DevExpress.XtraEditors.ComboBoxEdit();
            this.toggleConvert = new DevExpress.XtraEditors.ToggleSwitch();
            this.gcCrop = new DevExpress.XtraEditors.GroupControl();
            this.pnlCropPanel = new DevExpress.XtraEditors.PanelControl();
            this.toggleCrop = new DevExpress.XtraEditors.ToggleSwitch();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.txtCropTop = new DevExpress.XtraEditors.TextEdit();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.txtCropBottom = new DevExpress.XtraEditors.TextEdit();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.txtCropLeft = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtCropRight = new DevExpress.XtraEditors.TextEdit();
            this.gcSegments = new DevExpress.XtraEditors.GroupControl();
            this.lblSegmentDisclaimer = new DevExpress.XtraEditors.LabelControl();
            this.pnlSegPanel = new DevExpress.XtraEditors.PanelControl();
            this.toggleSegment = new DevExpress.XtraEditors.ToggleSwitch();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.tsStart = new DevExpress.XtraEditors.TimeSpanEdit();
            this.tsDuration = new DevExpress.XtraEditors.TimeSpanEdit();
            this.pnlProgressPanel = new DevExpress.XtraEditors.PanelControl();
            this.btnOpenDL = new DevExpress.XtraEditors.SimpleButton();
            this.lblLastDL = new DevExpress.XtraEditors.LabelControl();
            this.pbProgress = new DevExpress.XtraEditors.ProgressBarControl();
            this.pbListProgress = new DevExpress.XtraEditors.ProgressBarControl();
            this.gcHistory = new DevExpress.XtraGrid.GridControl();
            this.gvHistory = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.historyBarManager = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.bbiReDownload = new DevExpress.XtraBars.BarButtonItem();
            this.bbiNewDownload = new DevExpress.XtraBars.BarButtonItem();
            this.repFileExists = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repPostProcessed = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.historyTooltip = new DevExpress.Utils.ToolTipController(this.components);
            this.historyPopup = new DevExpress.XtraBars.PopupMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pnlOptionPanel)).BeginInit();
            this.pnlOptionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcDLButtons)).BeginInit();
            this.gcDLButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcDownloadLimits)).BeginInit();
            this.gcDownloadLimits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlLimitPanel)).BeginInit();
            this.pnlLimitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxFilesize.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbMaxRes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toggleDownloadLimits.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcConvert)).BeginInit();
            this.gcConvert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlConvertPanel)).BeginInit();
            this.pnlConvertPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbAudioFormat.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbVideoFormat.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toggleConvert.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcCrop)).BeginInit();
            this.gcCrop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCropPanel)).BeginInit();
            this.pnlCropPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toggleCrop.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCropTop.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCropBottom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCropLeft.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCropRight.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcSegments)).BeginInit();
            this.gcSegments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSegPanel)).BeginInit();
            this.pnlSegPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSegment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tsStart.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tsDuration.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlProgressPanel)).BeginInit();
            this.pnlProgressPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbListProgress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.historyBarManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repFileExists)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPostProcessed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.historyPopup)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSelectionText
            // 
            this.lblSelectionText.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblSelectionText.Appearance.Options.UseFont = true;
            this.lblSelectionText.Appearance.Options.UseTextOptions = true;
            this.lblSelectionText.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSelectionText.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblSelectionText.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblSelectionText.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSelectionText.Location = new System.Drawing.Point(0, 0);
            this.lblSelectionText.Margin = new System.Windows.Forms.Padding(0);
            this.lblSelectionText.Name = "lblSelectionText";
            this.lblSelectionText.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.lblSelectionText.Size = new System.Drawing.Size(323, 0);
            this.lblSelectionText.TabIndex = 0;
            this.lblSelectionText.TextChanged += new System.EventHandler(this.lblSelectionText_TextChanged);
            // 
            // pnlOptionPanel
            // 
            this.pnlOptionPanel.AutoSize = true;
            this.pnlOptionPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlOptionPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlOptionPanel.Controls.Add(this.gcDLButtons);
            this.pnlOptionPanel.Controls.Add(this.gcDownloadLimits);
            this.pnlOptionPanel.Controls.Add(this.gcConvert);
            this.pnlOptionPanel.Controls.Add(this.gcCrop);
            this.pnlOptionPanel.Controls.Add(this.gcSegments);
            this.pnlOptionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOptionPanel.Location = new System.Drawing.Point(0, 0);
            this.pnlOptionPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlOptionPanel.MinimumSize = new System.Drawing.Size(323, 136);
            this.pnlOptionPanel.Name = "pnlOptionPanel";
            this.pnlOptionPanel.Size = new System.Drawing.Size(323, 722);
            this.pnlOptionPanel.TabIndex = 0;
            // 
            // gcDLButtons
            // 
            this.gcDLButtons.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gcDLButtons.AppearanceCaption.Options.UseFont = true;
            this.gcDLButtons.AutoSize = true;
            this.gcDLButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gcDLButtons.Controls.Add(this.btnCancelProcess);
            this.gcDLButtons.Controls.Add(this.btnDownloadBest);
            this.gcDLButtons.Controls.Add(this.btnDownloadAudio);
            this.gcDLButtons.Controls.Add(this.btnSelectionDL);
            this.gcDLButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.gcDLButtons.Location = new System.Drawing.Point(0, 563);
            this.gcDLButtons.Margin = new System.Windows.Forms.Padding(0);
            this.gcDLButtons.Name = "gcDLButtons";
            this.gcDLButtons.Size = new System.Drawing.Size(323, 159);
            this.gcDLButtons.TabIndex = 0;
            this.gcDLButtons.Text = "Execute Download";
            // 
            // btnCancelProcess
            // 
            this.btnCancelProcess.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnCancelProcess.Appearance.Options.UseFont = true;
            this.btnCancelProcess.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCancelProcess.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnCancelProcess.ImageOptions.SvgImage = global::YT_RED.Properties.Resources.close;
            this.btnCancelProcess.ImageOptions.SvgImageSize = new System.Drawing.Size(25, 25);
            this.btnCancelProcess.Location = new System.Drawing.Point(2, 125);
            this.btnCancelProcess.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancelProcess.Name = "btnCancelProcess";
            this.btnCancelProcess.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCancelProcess.Size = new System.Drawing.Size(319, 32);
            this.btnCancelProcess.TabIndex = 15;
            this.btnCancelProcess.Text = "CANCEL        ";
            this.btnCancelProcess.Visible = false;
            this.btnCancelProcess.Click += new System.EventHandler(this.btnCancelProcess_Click);
            this.btnCancelProcess.MouseLeave += new System.EventHandler(this.btnCancelProcess_MouseLeave);
            this.btnCancelProcess.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnCancelProcess_MouseMove);
            // 
            // btnDownloadBest
            // 
            this.btnDownloadBest.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnDownloadBest.Appearance.Options.UseFont = true;
            this.btnDownloadBest.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDownloadBest.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnDownloadBest.ImageOptions.SvgImage = global::YT_RED.Properties.Resources.VideoSound;
            this.btnDownloadBest.ImageOptions.SvgImageSize = new System.Drawing.Size(25, 25);
            this.btnDownloadBest.Location = new System.Drawing.Point(2, 93);
            this.btnDownloadBest.Margin = new System.Windows.Forms.Padding(0);
            this.btnDownloadBest.Name = "btnDownloadBest";
            this.btnDownloadBest.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnDownloadBest.Size = new System.Drawing.Size(319, 32);
            this.btnDownloadBest.TabIndex = 14;
            this.btnDownloadBest.Text = "DOWNLOAD BEST [audio+video]      ";
            this.btnDownloadBest.Click += new System.EventHandler(this.btnDownloadBest_Click);
            // 
            // btnDownloadAudio
            // 
            this.btnDownloadAudio.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnDownloadAudio.Appearance.Options.UseFont = true;
            this.btnDownloadAudio.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDownloadAudio.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnDownloadAudio.ImageOptions.SvgImage = global::YT_RED.Properties.Resources.sound;
            this.btnDownloadAudio.ImageOptions.SvgImageSize = new System.Drawing.Size(25, 25);
            this.btnDownloadAudio.Location = new System.Drawing.Point(2, 61);
            this.btnDownloadAudio.Margin = new System.Windows.Forms.Padding(0);
            this.btnDownloadAudio.Name = "btnDownloadAudio";
            this.btnDownloadAudio.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnDownloadAudio.Size = new System.Drawing.Size(319, 32);
            this.btnDownloadAudio.TabIndex = 13;
            this.btnDownloadAudio.Text = "DOWNLOAD AUDIO       ";
            this.btnDownloadAudio.Click += new System.EventHandler(this.btnDownloadAudio_Click);
            // 
            // btnSelectionDL
            // 
            this.btnSelectionDL.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnSelectionDL.Appearance.Options.UseFont = true;
            this.btnSelectionDL.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSelectionDL.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnSelectionDL.ImageOptions.SvgImageSize = new System.Drawing.Size(25, 25);
            this.btnSelectionDL.Location = new System.Drawing.Point(2, 29);
            this.btnSelectionDL.Margin = new System.Windows.Forms.Padding(0);
            this.btnSelectionDL.Name = "btnSelectionDL";
            this.btnSelectionDL.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnSelectionDL.Size = new System.Drawing.Size(319, 32);
            this.btnSelectionDL.TabIndex = 12;
            this.btnSelectionDL.Text = "DOWNLOAD SELECTED FORMAT    ";
            this.btnSelectionDL.Visible = false;
            this.btnSelectionDL.Click += new System.EventHandler(this.btnSelectionDL_Click);
            // 
            // gcDownloadLimits
            // 
            this.gcDownloadLimits.AutoSize = true;
            this.gcDownloadLimits.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gcDownloadLimits.CaptionImageOptions.AllowGlyphSkinning = true;
            this.gcDownloadLimits.Controls.Add(this.pnlLimitPanel);
            buttonImageOptions1.SvgImage = global::YT_RED.Properties.Resources.actions_remove;
            buttonImageOptions1.SvgImageSize = new System.Drawing.Size(18, 18);
            this.gcDownloadLimits.CustomHeaderButtons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("", false, buttonImageOptions1, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, true, true, "ytSegment", -1)});
            this.gcDownloadLimits.CustomHeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.gcDownloadLimits.Dock = System.Windows.Forms.DockStyle.Top;
            this.gcDownloadLimits.Location = new System.Drawing.Point(0, 436);
            this.gcDownloadLimits.Margin = new System.Windows.Forms.Padding(0);
            this.gcDownloadLimits.Name = "gcDownloadLimits";
            this.gcDownloadLimits.Size = new System.Drawing.Size(323, 127);
            this.gcDownloadLimits.TabIndex = 6;
            this.gcDownloadLimits.Text = "Best Download Settings";
            this.gcDownloadLimits.Click += new System.EventHandler(this.gcDownloadLimits_Click);
            // 
            // pnlLimitPanel
            // 
            this.pnlLimitPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlLimitPanel.Controls.Add(this.txtMaxFilesize);
            this.pnlLimitPanel.Controls.Add(this.lblMaxFilesize);
            this.pnlLimitPanel.Controls.Add(this.lblMaxRes);
            this.pnlLimitPanel.Controls.Add(this.cbMaxRes);
            this.pnlLimitPanel.Controls.Add(this.toggleDownloadLimits);
            this.pnlLimitPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLimitPanel.Location = new System.Drawing.Point(2, 29);
            this.pnlLimitPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlLimitPanel.Name = "pnlLimitPanel";
            this.pnlLimitPanel.Size = new System.Drawing.Size(319, 96);
            this.pnlLimitPanel.TabIndex = 0;
            // 
            // txtMaxFilesize
            // 
            this.txtMaxFilesize.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtMaxFilesize.EditValue = "0";
            this.txtMaxFilesize.Enabled = false;
            this.txtMaxFilesize.Location = new System.Drawing.Point(162, 61);
            this.txtMaxFilesize.Name = "txtMaxFilesize";
            this.txtMaxFilesize.Properties.AutoHeight = false;
            this.txtMaxFilesize.Properties.DisplayFormat.FormatString = "{0} MB";
            this.txtMaxFilesize.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtMaxFilesize.Properties.EditFormat.FormatString = "{0} MB";
            this.txtMaxFilesize.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtMaxFilesize.Properties.NullText = "0";
            this.txtMaxFilesize.Size = new System.Drawing.Size(142, 25);
            this.txtMaxFilesize.TabIndex = 11;
            this.txtMaxFilesize.TextChanged += new System.EventHandler(this.txtMaxFilesize_TextChanged);
            // 
            // lblMaxFilesize
            // 
            this.lblMaxFilesize.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblMaxFilesize.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblMaxFilesize.Appearance.Options.UseFont = true;
            this.lblMaxFilesize.Location = new System.Drawing.Point(43, 66);
            this.lblMaxFilesize.Name = "lblMaxFilesize";
            this.lblMaxFilesize.Size = new System.Drawing.Size(101, 16);
            this.lblMaxFilesize.TabIndex = 0;
            this.lblMaxFilesize.Text = "Maximum Filesize";
            // 
            // lblMaxRes
            // 
            this.lblMaxRes.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblMaxRes.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblMaxRes.Appearance.Options.UseFont = true;
            this.lblMaxRes.Location = new System.Drawing.Point(26, 35);
            this.lblMaxRes.Name = "lblMaxRes";
            this.lblMaxRes.Size = new System.Drawing.Size(118, 16);
            this.lblMaxRes.TabIndex = 0;
            this.lblMaxRes.Text = "Maximum Resolution";
            // 
            // cbMaxRes
            // 
            this.cbMaxRes.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cbMaxRes.Enabled = false;
            this.cbMaxRes.Location = new System.Drawing.Point(162, 30);
            this.cbMaxRes.Name = "cbMaxRes";
            this.cbMaxRes.Properties.AutoHeight = false;
            this.cbMaxRes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbMaxRes.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbMaxRes.Size = new System.Drawing.Size(142, 25);
            this.cbMaxRes.TabIndex = 10;
            this.cbMaxRes.SelectedIndexChanged += new System.EventHandler(this.cbMaxRes_SelectedIndexChanged);
            // 
            // toggleDownloadLimits
            // 
            this.toggleDownloadLimits.Location = new System.Drawing.Point(7, 3);
            this.toggleDownloadLimits.Name = "toggleDownloadLimits";
            this.toggleDownloadLimits.Properties.AllowFocused = false;
            this.toggleDownloadLimits.Properties.OffText = "Off";
            this.toggleDownloadLimits.Properties.OnText = "On";
            this.toggleDownloadLimits.Size = new System.Drawing.Size(95, 24);
            this.toggleDownloadLimits.TabIndex = 9;
            this.toggleDownloadLimits.Toggled += new System.EventHandler(this.toggleDownloadLimits_Toggled);
            // 
            // gcConvert
            // 
            this.gcConvert.AutoSize = true;
            this.gcConvert.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gcConvert.CaptionImageOptions.AllowGlyphSkinning = true;
            this.gcConvert.Controls.Add(this.hlblOpenSettings);
            this.gcConvert.Controls.Add(this.lblAlwaysConvert);
            this.gcConvert.Controls.Add(this.pnlConvertPanel);
            buttonImageOptions2.SvgImage = global::YT_RED.Properties.Resources.actions_remove;
            buttonImageOptions2.SvgImageSize = new System.Drawing.Size(18, 18);
            this.gcConvert.CustomHeaderButtons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("", false, buttonImageOptions2, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, true, true, "ytSegment", -1)});
            this.gcConvert.CustomHeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.gcConvert.Dock = System.Windows.Forms.DockStyle.Top;
            this.gcConvert.Location = new System.Drawing.Point(0, 275);
            this.gcConvert.Margin = new System.Windows.Forms.Padding(0);
            this.gcConvert.Name = "gcConvert";
            this.gcConvert.Size = new System.Drawing.Size(323, 161);
            this.gcConvert.TabIndex = 0;
            this.gcConvert.Text = "Conversion Override";
            this.gcConvert.Click += new System.EventHandler(this.gcConvert_Click);
            // 
            // hlblOpenSettings
            // 
            this.hlblOpenSettings.Appearance.Options.UseTextOptions = true;
            this.hlblOpenSettings.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.hlblOpenSettings.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.hlblOpenSettings.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.hlblOpenSettings.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.hlblOpenSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.hlblOpenSettings.Location = new System.Drawing.Point(2, 143);
            this.hlblOpenSettings.Name = "hlblOpenSettings";
            this.hlblOpenSettings.Size = new System.Drawing.Size(319, 16);
            this.hlblOpenSettings.TabIndex = 1;
            this.hlblOpenSettings.Text = "Open Settings";
            this.hlblOpenSettings.Visible = false;
            this.hlblOpenSettings.Click += new System.EventHandler(this.hlblOpenSettings_Click);
            // 
            // lblAlwaysConvert
            // 
            this.lblAlwaysConvert.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.lblAlwaysConvert.Appearance.Options.UseForeColor = true;
            this.lblAlwaysConvert.Appearance.Options.UseTextOptions = true;
            this.lblAlwaysConvert.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblAlwaysConvert.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblAlwaysConvert.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAlwaysConvert.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAlwaysConvert.Location = new System.Drawing.Point(2, 96);
            this.lblAlwaysConvert.Name = "lblAlwaysConvert";
            this.lblAlwaysConvert.Padding = new System.Windows.Forms.Padding(3);
            this.lblAlwaysConvert.Size = new System.Drawing.Size(319, 47);
            this.lblAlwaysConvert.TabIndex = 0;
            this.lblAlwaysConvert.Text = "YT-RED is currently set to Always Convert to your \r\nPreferred Video and Audio For" +
    "mat. \r\nThis can be changed in Advanced Settings";
            this.lblAlwaysConvert.Visible = false;
            // 
            // pnlConvertPanel
            // 
            this.pnlConvertPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlConvertPanel.Controls.Add(this.labelControl7);
            this.pnlConvertPanel.Controls.Add(this.labelControl6);
            this.pnlConvertPanel.Controls.Add(this.cbAudioFormat);
            this.pnlConvertPanel.Controls.Add(this.cbVideoFormat);
            this.pnlConvertPanel.Controls.Add(this.toggleConvert);
            this.pnlConvertPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlConvertPanel.Location = new System.Drawing.Point(2, 29);
            this.pnlConvertPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlConvertPanel.Name = "pnlConvertPanel";
            this.pnlConvertPanel.Size = new System.Drawing.Size(319, 67);
            this.pnlConvertPanel.TabIndex = 0;
            // 
            // labelControl7
            // 
            this.labelControl7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Location = new System.Drawing.Point(170, 35);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(32, 16);
            this.labelControl7.TabIndex = 0;
            this.labelControl7.Text = "Audio";
            // 
            // labelControl6
            // 
            this.labelControl6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl6.Appearance.Options.UseFont = true;
            this.labelControl6.Location = new System.Drawing.Point(17, 35);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(32, 16);
            this.labelControl6.TabIndex = 0;
            this.labelControl6.Text = "Video";
            // 
            // cbAudioFormat
            // 
            this.cbAudioFormat.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cbAudioFormat.Enabled = false;
            this.cbAudioFormat.Location = new System.Drawing.Point(208, 32);
            this.cbAudioFormat.Name = "cbAudioFormat";
            this.cbAudioFormat.Properties.AutoHeight = false;
            this.cbAudioFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbAudioFormat.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbAudioFormat.Size = new System.Drawing.Size(100, 25);
            this.cbAudioFormat.TabIndex = 11;
            this.cbAudioFormat.SelectedIndexChanged += new System.EventHandler(this.cbAudioFormat_SelectedIndexChanged);
            // 
            // cbVideoFormat
            // 
            this.cbVideoFormat.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cbVideoFormat.Enabled = false;
            this.cbVideoFormat.Location = new System.Drawing.Point(55, 32);
            this.cbVideoFormat.Name = "cbVideoFormat";
            this.cbVideoFormat.Properties.AutoHeight = false;
            this.cbVideoFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbVideoFormat.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbVideoFormat.Size = new System.Drawing.Size(100, 25);
            this.cbVideoFormat.TabIndex = 10;
            this.cbVideoFormat.SelectedIndexChanged += new System.EventHandler(this.cbVideoFormat_SelectedIndexChanged);
            // 
            // toggleConvert
            // 
            this.toggleConvert.Location = new System.Drawing.Point(7, 3);
            this.toggleConvert.Name = "toggleConvert";
            this.toggleConvert.Properties.AllowFocused = false;
            this.toggleConvert.Properties.OffText = "Off";
            this.toggleConvert.Properties.OnText = "On";
            this.toggleConvert.Size = new System.Drawing.Size(95, 24);
            this.toggleConvert.TabIndex = 9;
            this.toggleConvert.Toggled += new System.EventHandler(this.toggleConvert_Toggled);
            // 
            // gcCrop
            // 
            this.gcCrop.AutoSize = true;
            this.gcCrop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gcCrop.CaptionImageOptions.AllowGlyphSkinning = true;
            this.gcCrop.Controls.Add(this.pnlCropPanel);
            buttonImageOptions3.SvgImage = global::YT_RED.Properties.Resources.actions_remove;
            buttonImageOptions3.SvgImageSize = new System.Drawing.Size(18, 18);
            this.gcCrop.CustomHeaderButtons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("", false, buttonImageOptions3, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, true, true, "ytSegment", -1)});
            this.gcCrop.CustomHeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.gcCrop.Dock = System.Windows.Forms.DockStyle.Top;
            this.gcCrop.Location = new System.Drawing.Point(0, 151);
            this.gcCrop.Margin = new System.Windows.Forms.Padding(0);
            this.gcCrop.Name = "gcCrop";
            this.gcCrop.Size = new System.Drawing.Size(323, 124);
            this.gcCrop.TabIndex = 0;
            this.gcCrop.Text = "Crop Video";
            this.gcCrop.Click += new System.EventHandler(this.gcCrop_Click);
            // 
            // pnlCropPanel
            // 
            this.pnlCropPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlCropPanel.Controls.Add(this.toggleCrop);
            this.pnlCropPanel.Controls.Add(this.labelControl10);
            this.pnlCropPanel.Controls.Add(this.txtCropTop);
            this.pnlCropPanel.Controls.Add(this.labelControl9);
            this.pnlCropPanel.Controls.Add(this.txtCropBottom);
            this.pnlCropPanel.Controls.Add(this.labelControl8);
            this.pnlCropPanel.Controls.Add(this.txtCropLeft);
            this.pnlCropPanel.Controls.Add(this.labelControl5);
            this.pnlCropPanel.Controls.Add(this.txtCropRight);
            this.pnlCropPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCropPanel.Location = new System.Drawing.Point(2, 29);
            this.pnlCropPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCropPanel.Name = "pnlCropPanel";
            this.pnlCropPanel.Size = new System.Drawing.Size(319, 93);
            this.pnlCropPanel.TabIndex = 0;
            // 
            // toggleCrop
            // 
            this.toggleCrop.Location = new System.Drawing.Point(7, 3);
            this.toggleCrop.Name = "toggleCrop";
            this.toggleCrop.Properties.AllowFocused = false;
            this.toggleCrop.Properties.OffText = "Off";
            this.toggleCrop.Properties.OnText = "On";
            this.toggleCrop.Size = new System.Drawing.Size(95, 24);
            this.toggleCrop.TabIndex = 4;
            this.toggleCrop.Toggled += new System.EventHandler(this.toggleCrop_Toggled);
            // 
            // labelControl10
            // 
            this.labelControl10.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelControl10.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl10.Appearance.Options.UseFont = true;
            this.labelControl10.Location = new System.Drawing.Point(173, 63);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(29, 16);
            this.labelControl10.TabIndex = 0;
            this.labelControl10.Text = "Right";
            // 
            // txtCropTop
            // 
            this.txtCropTop.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtCropTop.EditValue = "";
            this.txtCropTop.Enabled = false;
            this.txtCropTop.Location = new System.Drawing.Point(69, 29);
            this.txtCropTop.Name = "txtCropTop";
            this.txtCropTop.Properties.AutoHeight = false;
            this.txtCropTop.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtCropTop.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtCropTop.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.txtCropTop.Properties.MaskSettings.Set("MaskManagerSignature", "allowNull=False");
            this.txtCropTop.Properties.MaskSettings.Set("mask", "d");
            this.txtCropTop.Properties.NullText = "0";
            this.txtCropTop.Properties.UseMaskAsDisplayFormat = true;
            this.txtCropTop.Size = new System.Drawing.Size(75, 25);
            this.txtCropTop.TabIndex = 5;
            // 
            // labelControl9
            // 
            this.labelControl9.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelControl9.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl9.Appearance.Options.UseFont = true;
            this.labelControl9.Location = new System.Drawing.Point(42, 63);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(21, 16);
            this.labelControl9.TabIndex = 0;
            this.labelControl9.Text = "Left";
            // 
            // txtCropBottom
            // 
            this.txtCropBottom.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtCropBottom.EditValue = "";
            this.txtCropBottom.Enabled = false;
            this.txtCropBottom.Location = new System.Drawing.Point(208, 29);
            this.txtCropBottom.Name = "txtCropBottom";
            this.txtCropBottom.Properties.AutoHeight = false;
            this.txtCropBottom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtCropBottom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtCropBottom.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.txtCropBottom.Properties.MaskSettings.Set("MaskManagerSignature", "allowNull=False");
            this.txtCropBottom.Properties.MaskSettings.Set("mask", "d");
            this.txtCropBottom.Properties.NullText = "0";
            this.txtCropBottom.Properties.UseMaskAsDisplayFormat = true;
            this.txtCropBottom.Size = new System.Drawing.Size(75, 25);
            this.txtCropBottom.TabIndex = 6;
            // 
            // labelControl8
            // 
            this.labelControl8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelControl8.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl8.Appearance.Options.UseFont = true;
            this.labelControl8.Location = new System.Drawing.Point(162, 34);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(40, 16);
            this.labelControl8.TabIndex = 0;
            this.labelControl8.Text = "Bottom";
            // 
            // txtCropLeft
            // 
            this.txtCropLeft.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtCropLeft.EditValue = "";
            this.txtCropLeft.Enabled = false;
            this.txtCropLeft.Location = new System.Drawing.Point(69, 60);
            this.txtCropLeft.Name = "txtCropLeft";
            this.txtCropLeft.Properties.AutoHeight = false;
            this.txtCropLeft.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtCropLeft.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtCropLeft.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.txtCropLeft.Properties.MaskSettings.Set("MaskManagerSignature", "allowNull=False");
            this.txtCropLeft.Properties.MaskSettings.Set("mask", "d");
            this.txtCropLeft.Properties.NullText = "0";
            this.txtCropLeft.Properties.UseMaskAsDisplayFormat = true;
            this.txtCropLeft.Size = new System.Drawing.Size(75, 25);
            this.txtCropLeft.TabIndex = 7;
            // 
            // labelControl5
            // 
            this.labelControl5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(41, 34);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(22, 16);
            this.labelControl5.TabIndex = 0;
            this.labelControl5.Text = "Top";
            // 
            // txtCropRight
            // 
            this.txtCropRight.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtCropRight.EditValue = "";
            this.txtCropRight.Enabled = false;
            this.txtCropRight.Location = new System.Drawing.Point(208, 60);
            this.txtCropRight.Name = "txtCropRight";
            this.txtCropRight.Properties.AutoHeight = false;
            this.txtCropRight.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtCropRight.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtCropRight.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            this.txtCropRight.Properties.MaskSettings.Set("MaskManagerSignature", "allowNull=False");
            this.txtCropRight.Properties.MaskSettings.Set("mask", "d");
            this.txtCropRight.Properties.NullText = "0";
            this.txtCropRight.Properties.UseMaskAsDisplayFormat = true;
            this.txtCropRight.Size = new System.Drawing.Size(75, 25);
            this.txtCropRight.TabIndex = 8;
            // 
            // gcSegments
            // 
            this.gcSegments.AutoSize = true;
            this.gcSegments.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gcSegments.CaptionImageOptions.AllowGlyphSkinning = true;
            this.gcSegments.Controls.Add(this.lblSegmentDisclaimer);
            this.gcSegments.Controls.Add(this.pnlSegPanel);
            buttonImageOptions4.SvgImage = global::YT_RED.Properties.Resources.actions_remove;
            buttonImageOptions4.SvgImageSize = new System.Drawing.Size(18, 18);
            this.gcSegments.CustomHeaderButtons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("", false, buttonImageOptions4, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, false, true, "ytSegment", -1)});
            this.gcSegments.CustomHeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.gcSegments.Dock = System.Windows.Forms.DockStyle.Top;
            this.gcSegments.Location = new System.Drawing.Point(0, 0);
            this.gcSegments.Margin = new System.Windows.Forms.Padding(0);
            this.gcSegments.Name = "gcSegments";
            this.gcSegments.Size = new System.Drawing.Size(323, 151);
            this.gcSegments.TabIndex = 0;
            this.gcSegments.Text = "Download Segment";
            this.gcSegments.Click += new System.EventHandler(this.gcSegments_Click);
            // 
            // lblSegmentDisclaimer
            // 
            this.lblSegmentDisclaimer.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.lblSegmentDisclaimer.Appearance.Options.UseForeColor = true;
            this.lblSegmentDisclaimer.Appearance.Options.UseTextOptions = true;
            this.lblSegmentDisclaimer.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSegmentDisclaimer.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblSegmentDisclaimer.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSegmentDisclaimer.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSegmentDisclaimer.Location = new System.Drawing.Point(2, 98);
            this.lblSegmentDisclaimer.Name = "lblSegmentDisclaimer";
            this.lblSegmentDisclaimer.Padding = new System.Windows.Forms.Padding(3);
            this.lblSegmentDisclaimer.Size = new System.Drawing.Size(319, 51);
            this.lblSegmentDisclaimer.TabIndex = 0;
            this.lblSegmentDisclaimer.Text = "CAUTION: Segment Downloads will use CPU/GPU\r\n for encoding and can be resource in" +
    "tensive \r\nwhen downloading Best Audio+Video";
            this.lblSegmentDisclaimer.Visible = false;
            // 
            // pnlSegPanel
            // 
            this.pnlSegPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlSegPanel.Controls.Add(this.toggleSegment);
            this.pnlSegPanel.Controls.Add(this.labelControl3);
            this.pnlSegPanel.Controls.Add(this.labelControl4);
            this.pnlSegPanel.Controls.Add(this.tsStart);
            this.pnlSegPanel.Controls.Add(this.tsDuration);
            this.pnlSegPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSegPanel.Location = new System.Drawing.Point(2, 29);
            this.pnlSegPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSegPanel.Name = "pnlSegPanel";
            this.pnlSegPanel.Size = new System.Drawing.Size(319, 69);
            this.pnlSegPanel.TabIndex = 0;
            // 
            // toggleSegment
            // 
            this.toggleSegment.Location = new System.Drawing.Point(7, 3);
            this.toggleSegment.Name = "toggleSegment";
            this.toggleSegment.Properties.AllowFocused = false;
            this.toggleSegment.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.toggleSegment.Properties.OffText = "Off";
            this.toggleSegment.Properties.OnText = "On";
            this.toggleSegment.Size = new System.Drawing.Size(95, 24);
            this.toggleSegment.TabIndex = 1;
            this.toggleSegment.Toggled += new System.EventHandler(this.toggleSegment_Toggled);
            // 
            // labelControl3
            // 
            this.labelControl3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(10, 36);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(28, 16);
            this.labelControl3.TabIndex = 0;
            this.labelControl3.Text = "Start";
            // 
            // labelControl4
            // 
            this.labelControl4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(160, 36);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 16);
            this.labelControl4.TabIndex = 0;
            this.labelControl4.Text = "Duration";
            // 
            // tsStart
            // 
            this.tsStart.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tsStart.EditValue = System.TimeSpan.Parse("00:00:00");
            this.tsStart.Enabled = false;
            this.tsStart.Location = new System.Drawing.Point(44, 33);
            this.tsStart.Name = "tsStart";
            this.tsStart.Properties.AutoHeight = false;
            this.tsStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.tsStart.Properties.DisplayFormat.FormatString = "{hh}h {mm}m {ss}s";
            this.tsStart.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.tsStart.Properties.EditFormat.FormatString = "{hh}h {mm}m {ss}s";
            this.tsStart.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.tsStart.Properties.MaskSettings.Set("mask", "hh\\h mm\\m ss\\s");
            this.tsStart.Properties.MinValue = System.TimeSpan.Parse("00:00:00");
            this.tsStart.Size = new System.Drawing.Size(100, 25);
            this.tsStart.TabIndex = 2;
            // 
            // tsDuration
            // 
            this.tsDuration.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tsDuration.EditValue = System.TimeSpan.Parse("00:00:01");
            this.tsDuration.Enabled = false;
            this.tsDuration.Location = new System.Drawing.Point(214, 33);
            this.tsDuration.Name = "tsDuration";
            this.tsDuration.Properties.AutoHeight = false;
            this.tsDuration.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.tsDuration.Properties.DisplayFormat.FormatString = "{hh}h {mm}m {ss}s";
            this.tsDuration.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.tsDuration.Properties.EditFormat.FormatString = "{hh}h {mm}m {ss}s";
            this.tsDuration.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.tsDuration.Properties.MaskSettings.Set("mask", "hh\\h mm\\m ss\\s");
            this.tsDuration.Properties.MinValue = System.TimeSpan.Parse("00:00:01");
            this.tsDuration.Size = new System.Drawing.Size(100, 25);
            this.tsDuration.TabIndex = 3;
            // 
            // pnlProgressPanel
            // 
            this.pnlProgressPanel.AutoSize = true;
            this.pnlProgressPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlProgressPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlProgressPanel.Controls.Add(this.btnOpenDL);
            this.pnlProgressPanel.Controls.Add(this.lblLastDL);
            this.pnlProgressPanel.Controls.Add(this.pbProgress);
            this.pnlProgressPanel.Controls.Add(this.pbListProgress);
            this.pnlProgressPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProgressPanel.Location = new System.Drawing.Point(0, 722);
            this.pnlProgressPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProgressPanel.Name = "pnlProgressPanel";
            this.pnlProgressPanel.Size = new System.Drawing.Size(323, 109);
            this.pnlProgressPanel.TabIndex = 0;
            // 
            // btnOpenDL
            // 
            this.btnOpenDL.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOpenDL.Location = new System.Drawing.Point(0, 83);
            this.btnOpenDL.Name = "btnOpenDL";
            this.btnOpenDL.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnOpenDL.Size = new System.Drawing.Size(323, 26);
            this.btnOpenDL.TabIndex = 0;
            this.btnOpenDL.Visible = false;
            this.btnOpenDL.Click += new System.EventHandler(this.btnOpenDL_Click);
            // 
            // lblLastDL
            // 
            this.lblLastDL.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lblLastDL.Appearance.Options.UseFont = true;
            this.lblLastDL.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblLastDL.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblLastDL.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLastDL.Location = new System.Drawing.Point(0, 62);
            this.lblLastDL.Margin = new System.Windows.Forms.Padding(0);
            this.lblLastDL.Name = "lblLastDL";
            this.lblLastDL.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblLastDL.Size = new System.Drawing.Size(323, 21);
            this.lblLastDL.TabIndex = 1;
            this.lblLastDL.Text = "Last Downloaded File";
            this.lblLastDL.Visible = false;
            // 
            // pbProgress
            // 
            this.pbProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbProgress.Location = new System.Drawing.Point(0, 31);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Properties.AllowFocused = false;
            this.pbProgress.Properties.ShowTitle = true;
            this.pbProgress.Size = new System.Drawing.Size(323, 31);
            this.pbProgress.TabIndex = 0;
            this.pbProgress.Visible = false;
            // 
            // pbListProgress
            // 
            this.pbListProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbListProgress.Location = new System.Drawing.Point(0, 0);
            this.pbListProgress.Name = "pbListProgress";
            this.pbListProgress.Properties.AllowFocused = false;
            this.pbListProgress.Properties.PercentView = false;
            this.pbListProgress.Properties.ShowTitle = true;
            this.pbListProgress.Size = new System.Drawing.Size(323, 31);
            this.pbListProgress.TabIndex = 2;
            this.pbListProgress.Visible = false;
            this.pbListProgress.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.pbListProgress_CustomDisplayText);
            // 
            // gcHistory
            // 
            this.gcHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcHistory.Location = new System.Drawing.Point(0, 831);
            this.gcHistory.MainView = this.gvHistory;
            this.gcHistory.MenuManager = this.historyBarManager;
            this.gcHistory.Name = "gcHistory";
            this.gcHistory.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repFileExists,
            this.repPostProcessed});
            this.gcHistory.Size = new System.Drawing.Size(323, 207);
            this.gcHistory.TabIndex = 0;
            this.gcHistory.ToolTipController = this.historyTooltip;
            this.gcHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvHistory});
            this.gcHistory.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gcHistory_MouseClick);
            // 
            // gvHistory
            // 
            this.gvHistory.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gvHistory.GridControl = this.gcHistory;
            this.gvHistory.Name = "gvHistory";
            this.gvHistory.OptionsBehavior.AlignGroupSummaryInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.gvHistory.OptionsBehavior.Editable = false;
            this.gvHistory.OptionsCustomization.AllowColumnMoving = false;
            this.gvHistory.OptionsCustomization.AllowGroup = false;
            this.gvHistory.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvHistory.OptionsDetail.EnableDetailToolTip = true;
            this.gvHistory.OptionsDetail.ShowDetailTabs = false;
            this.gvHistory.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvHistory.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gvHistory.OptionsView.ShowDetailButtons = false;
            this.gvHistory.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.gvHistory.OptionsView.ShowGroupPanel = false;
            this.gvHistory.OptionsView.ShowIndicator = false;
            this.gvHistory.DoubleClick += new System.EventHandler(this.gvHistory_DoubleClick);
            // 
            // historyBarManager
            // 
            this.historyBarManager.DockControls.Add(this.barDockControlTop);
            this.historyBarManager.DockControls.Add(this.barDockControlBottom);
            this.historyBarManager.DockControls.Add(this.barDockControlLeft);
            this.historyBarManager.DockControls.Add(this.barDockControlRight);
            this.historyBarManager.Form = this;
            this.historyBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbiReDownload,
            this.bbiNewDownload});
            this.historyBarManager.MaxItemId = 2;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.historyBarManager;
            this.barDockControlTop.Size = new System.Drawing.Size(323, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 1038);
            this.barDockControlBottom.Manager = this.historyBarManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(323, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.historyBarManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 1038);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(323, 0);
            this.barDockControlRight.Manager = this.historyBarManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 1038);
            // 
            // bbiReDownload
            // 
            this.bbiReDownload.Caption = "Download Again";
            this.bbiReDownload.Id = 0;
            this.bbiReDownload.Name = "bbiReDownload";
            this.bbiReDownload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiReDownload_ItemClick);
            // 
            // bbiNewDownload
            // 
            this.bbiNewDownload.Caption = "Use URL for New Download";
            this.bbiNewDownload.Id = 1;
            this.bbiNewDownload.Name = "bbiNewDownload";
            this.bbiNewDownload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiNewDownload_ItemClick);
            // 
            // repFileExists
            // 
            this.repFileExists.AutoHeight = false;
            this.repFileExists.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Custom;
            this.repFileExists.ImageOptions.SvgImageChecked = global::YT_RED.Properties.Resources.actions_checkcircled;
            this.repFileExists.ImageOptions.SvgImageSize = new System.Drawing.Size(20, 20);
            this.repFileExists.ImageOptions.SvgImageUnchecked = global::YT_RED.Properties.Resources.security_warningcircled2;
            this.repFileExists.Name = "repFileExists";
            // 
            // repPostProcessed
            // 
            this.repPostProcessed.AutoHeight = false;
            this.repPostProcessed.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Custom;
            this.repPostProcessed.CheckBoxOptions.SvgImageSize = new System.Drawing.Size(20, 20);
            this.repPostProcessed.ImageOptions.SvgImageChecked = global::YT_RED.Properties.Resources.functionsinformation;
            this.repPostProcessed.ImageOptions.SvgImageSize = new System.Drawing.Size(20, 20);
            this.repPostProcessed.ImageOptions.SvgImageUnchecked = global::YT_RED.Properties.Resources.about1;
            this.repPostProcessed.Name = "repPostProcessed";
            // 
            // historyTooltip
            // 
            this.historyTooltip.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.historyTooltip_GetActiveObjectInfo);
            // 
            // historyPopup
            // 
            this.historyPopup.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiReDownload),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiNewDownload)});
            this.historyPopup.Manager = this.historyBarManager;
            this.historyPopup.Name = "historyPopup";
            this.historyPopup.CloseUp += new System.EventHandler(this.historyPopup_CloseUp);
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gcHistory);
            this.Controls.Add(this.pnlProgressPanel);
            this.Controls.Add(this.pnlOptionPanel);
            this.Controls.Add(this.lblSelectionText);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.MinimumSize = new System.Drawing.Size(323, 0);
            this.Name = "ControlPanel";
            this.Size = new System.Drawing.Size(323, 1038);
            ((System.ComponentModel.ISupportInitialize)(this.pnlOptionPanel)).EndInit();
            this.pnlOptionPanel.ResumeLayout(false);
            this.pnlOptionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcDLButtons)).EndInit();
            this.gcDLButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcDownloadLimits)).EndInit();
            this.gcDownloadLimits.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlLimitPanel)).EndInit();
            this.pnlLimitPanel.ResumeLayout(false);
            this.pnlLimitPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxFilesize.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbMaxRes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toggleDownloadLimits.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcConvert)).EndInit();
            this.gcConvert.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlConvertPanel)).EndInit();
            this.pnlConvertPanel.ResumeLayout(false);
            this.pnlConvertPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbAudioFormat.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbVideoFormat.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toggleConvert.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcCrop)).EndInit();
            this.gcCrop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlCropPanel)).EndInit();
            this.pnlCropPanel.ResumeLayout(false);
            this.pnlCropPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toggleCrop.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCropTop.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCropBottom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCropLeft.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCropRight.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcSegments)).EndInit();
            this.gcSegments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSegPanel)).EndInit();
            this.pnlSegPanel.ResumeLayout(false);
            this.pnlSegPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toggleSegment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tsStart.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tsDuration.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlProgressPanel)).EndInit();
            this.pnlProgressPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbListProgress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.historyBarManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repFileExists)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPostProcessed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.historyPopup)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblSelectionText;
        private DevExpress.XtraEditors.PanelControl pnlOptionPanel;
        private DevExpress.XtraEditors.GroupControl gcDLButtons;
        private DevExpress.XtraEditors.GroupControl gcCrop;
        private DevExpress.XtraEditors.PanelControl pnlCropPanel;
        private DevExpress.XtraEditors.ToggleSwitch toggleCrop;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private DevExpress.XtraEditors.TextEdit txtCropTop;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.TextEdit txtCropBottom;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.TextEdit txtCropLeft;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txtCropRight;
        private DevExpress.XtraEditors.GroupControl gcSegments;
        private DevExpress.XtraEditors.LabelControl lblSegmentDisclaimer;
        private DevExpress.XtraEditors.PanelControl pnlSegPanel;
        private DevExpress.XtraEditors.ToggleSwitch toggleSegment;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TimeSpanEdit tsStart;
        private DevExpress.XtraEditors.TimeSpanEdit tsDuration;
        private DevExpress.XtraEditors.PanelControl pnlProgressPanel;
        private DevExpress.XtraEditors.SimpleButton btnOpenDL;
        private DevExpress.XtraEditors.ProgressBarControl pbProgress;
        public DevExpress.XtraGrid.GridControl gcHistory;
        public DevExpress.XtraGrid.Views.Grid.GridView gvHistory;
        private DevExpress.XtraEditors.GroupControl gcConvert;
        private DevExpress.XtraEditors.LabelControl lblAlwaysConvert;
        private DevExpress.XtraEditors.PanelControl pnlConvertPanel;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.ComboBoxEdit cbAudioFormat;
        private DevExpress.XtraEditors.ComboBoxEdit cbVideoFormat;
        private DevExpress.XtraEditors.ToggleSwitch toggleConvert;
        private DevExpress.XtraEditors.LabelControl lblLastDL;
        private DevExpress.Utils.ToolTipController historyTooltip;
        public DevExpress.XtraEditors.SimpleButton btnDownloadBest;
        public DevExpress.XtraEditors.SimpleButton btnDownloadAudio;
        public DevExpress.XtraEditors.SimpleButton btnSelectionDL;
        public DevExpress.XtraEditors.SimpleButton btnCancelProcess;
        private DevExpress.XtraBars.BarManager historyBarManager;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.PopupMenu historyPopup;
        private DevExpress.XtraBars.BarButtonItem bbiReDownload;
        private DevExpress.XtraBars.BarButtonItem bbiNewDownload;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repFileExists;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repPostProcessed;
        private DevExpress.XtraEditors.HyperlinkLabelControl hlblOpenSettings;
        private DevExpress.XtraEditors.ProgressBarControl pbListProgress;
        private DevExpress.XtraEditors.GroupControl gcDownloadLimits;
        private DevExpress.XtraEditors.PanelControl pnlLimitPanel;
        private DevExpress.XtraEditors.LabelControl lblMaxFilesize;
        private DevExpress.XtraEditors.LabelControl lblMaxRes;
        private DevExpress.XtraEditors.ComboBoxEdit cbMaxRes;
        private DevExpress.XtraEditors.ToggleSwitch toggleDownloadLimits;
        private DevExpress.XtraEditors.TextEdit txtMaxFilesize;
    }
}
