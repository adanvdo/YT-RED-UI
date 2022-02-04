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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabFormControl1 = new DevExpress.XtraBars.TabFormControl();
            this.tfpYouTube = new YT_RED.Controls.CustomTabFormPage();
            this.tabFormContentContainer2 = new DevExpress.XtraBars.TabFormContentContainer();
            this.tfpReddit = new YT_RED.Controls.CustomTabFormPage();
            this.tabFormContentContainer3 = new DevExpress.XtraBars.TabFormContentContainer();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.gcReddit = new DevExpress.XtraGrid.GridControl();
            this.gvReddit = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblSelect = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.redditListMarquee = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.btnRedditList = new DevExpress.XtraEditors.SimpleButton();
            this.btnRedditDefault = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtRedditPost = new DevExpress.XtraEditors.TextEdit();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.gcHistory = new DevExpress.XtraGrid.GridControl();
            this.gvHistory = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.btnRedDL = new DevExpress.XtraEditors.SimpleButton();
            this.pbDownloadProgress = new DevExpress.XtraEditors.ProgressBarControl();
            this.btnDownloadReddit = new DevExpress.XtraEditors.SimpleButton();
            this.lblSelectionText = new DevExpress.XtraEditors.LabelControl();
            this.tfpHome = new YT_RED.Controls.CustomTabFormPage();
            this.tabFormContentContainer1 = new DevExpress.XtraBars.TabFormContentContainer();
            this.gridPanel = new DevExpress.XtraVerticalGrid.PGPanel();
            this.settingsGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            this.pgToolbar1 = new DevExpress.XtraVerticalGrid.PGToolbar();
            this.pgCategoryCheckButton1 = new DevExpress.XtraVerticalGrid.PGCategoryCheckButton();
            this.pgAlphabeticalCheckButton1 = new DevExpress.XtraVerticalGrid.PGAlphabeticalCheckButton();
            this.pgSeparatorControl1 = new DevExpress.XtraVerticalGrid.PGSeparatorControl();
            this.pgDescriptionCheckButton1 = new DevExpress.XtraVerticalGrid.PGDescriptionCheckButton();
            this.propertyDescriptionControl1 = new DevExpress.XtraVerticalGrid.PropertyDescriptionControl();
            this.pgSearchControl1 = new DevExpress.XtraVerticalGrid.PGSearchControl();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.btnSaveSettings = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.tabFormControl1)).BeginInit();
            this.tabFormContentContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).BeginInit();
            this.splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).BeginInit();
            this.splitContainerControl1.Panel2.SuspendLayout();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcReddit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvReddit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.redditListMarquee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRedditPost.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDownloadProgress.Properties)).BeginInit();
            this.tabFormContentContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPanel)).BeginInit();
            this.gridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pgToolbar1)).BeginInit();
            this.pgToolbar1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pgSeparatorControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pgSearchControl1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabFormControl1
            // 
            this.tabFormControl1.Location = new System.Drawing.Point(0, 0);
            this.tabFormControl1.Name = "tabFormControl1";
            this.tabFormControl1.Pages.Add(this.tfpYouTube);
            this.tabFormControl1.Pages.Add(this.tfpReddit);
            this.tabFormControl1.Pages.Add(this.tfpHome);
            this.tabFormControl1.SelectedPage = this.tfpReddit;
            this.tabFormControl1.Size = new System.Drawing.Size(1059, 50);
            this.tabFormControl1.TabForm = this;
            this.tabFormControl1.TabIndex = 0;
            this.tabFormControl1.TabStop = false;
            this.tabFormControl1.PageClosing += new DevExpress.XtraBars.PageClosingEventHandler(this.tabFormControl1_PageClosing);
            this.tabFormControl1.SelectedPageChanged += new DevExpress.XtraBars.TabFormSelectedPageChangedEventHandler(this.tabFormControl1_SelectedPageChanged);
            // 
            // tfpYouTube
            // 
            this.tfpYouTube.ContentContainer = this.tabFormContentContainer2;
            this.tfpYouTube.Name = "tfpYouTube";
            this.tfpYouTube.Text = "YouTube";
            // 
            // tabFormContentContainer2
            // 
            this.tabFormContentContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFormContentContainer2.Location = new System.Drawing.Point(0, 50);
            this.tabFormContentContainer2.Name = "tabFormContentContainer2";
            this.tabFormContentContainer2.Size = new System.Drawing.Size(1059, 678);
            this.tabFormContentContainer2.TabIndex = 2;
            // 
            // tfpReddit
            // 
            this.tfpReddit.ContentContainer = this.tabFormContentContainer3;
            this.tfpReddit.Name = "tfpReddit";
            this.tfpReddit.Text = "Reddit";
            // 
            // tabFormContentContainer3
            // 
            this.tabFormContentContainer3.Controls.Add(this.splitContainerControl1);
            this.tabFormContentContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFormContentContainer3.Location = new System.Drawing.Point(0, 50);
            this.tabFormContentContainer3.Name = "tabFormContentContainer3";
            this.tabFormContentContainer3.Size = new System.Drawing.Size(1059, 678);
            this.tabFormContentContainer3.TabIndex = 3;
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            this.splitContainerControl1.Panel1.Controls.Add(this.gcReddit);
            this.splitContainerControl1.Panel1.Controls.Add(this.lblSelect);
            this.splitContainerControl1.Panel1.Controls.Add(this.panelControl1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            this.splitContainerControl1.Panel2.Controls.Add(this.gcHistory);
            this.splitContainerControl1.Panel2.Controls.Add(this.panelControl2);
            this.splitContainerControl1.Panel2.Controls.Add(this.lblSelectionText);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(1059, 678);
            this.splitContainerControl1.SplitterPosition = 729;
            this.splitContainerControl1.TabIndex = 0;
            // 
            // gcReddit
            // 
            this.gcReddit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcReddit.Location = new System.Drawing.Point(0, 161);
            this.gcReddit.MainView = this.gvReddit;
            this.gcReddit.Name = "gcReddit";
            this.gcReddit.Size = new System.Drawing.Size(729, 517);
            this.gcReddit.TabIndex = 2;
            this.gcReddit.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvReddit});
            // 
            // gvReddit
            // 
            this.gvReddit.GridControl = this.gcReddit;
            this.gvReddit.Name = "gvReddit";
            this.gvReddit.OptionsBehavior.AlignGroupSummaryInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.gvReddit.OptionsBehavior.Editable = false;
            this.gvReddit.OptionsCustomization.AllowColumnMoving = false;
            this.gvReddit.OptionsCustomization.AllowGroup = false;
            this.gvReddit.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvReddit.OptionsDetail.ShowDetailTabs = false;
            this.gvReddit.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvReddit.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gvReddit.OptionsView.ShowDetailButtons = false;
            this.gvReddit.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.gvReddit.OptionsView.ShowGroupPanel = false;
            this.gvReddit.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvReddit_FocusedRowChanged);
            // 
            // lblSelect
            // 
            this.lblSelect.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblSelect.Appearance.Options.UseFont = true;
            this.lblSelect.Appearance.Options.UseTextOptions = true;
            this.lblSelect.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSelect.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSelect.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSelect.Location = new System.Drawing.Point(0, 133);
            this.lblSelect.Name = "lblSelect";
            this.lblSelect.Size = new System.Drawing.Size(729, 28);
            this.lblSelect.TabIndex = 3;
            this.lblSelect.Text = "Select a Video and/or Audio Format";
            this.lblSelect.Visible = false;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.redditListMarquee);
            this.panelControl1.Controls.Add(this.btnRedditList);
            this.panelControl1.Controls.Add(this.btnRedditDefault);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.txtRedditPost);
            this.panelControl1.Controls.Add(this.pictureEdit1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(729, 133);
            this.panelControl1.TabIndex = 0;
            // 
            // redditListMarquee
            // 
            this.redditListMarquee.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.redditListMarquee.EditValue = 0;
            this.redditListMarquee.Location = new System.Drawing.Point(5, 116);
            this.redditListMarquee.Name = "redditListMarquee";
            this.redditListMarquee.Size = new System.Drawing.Size(719, 10);
            this.redditListMarquee.TabIndex = 5;
            this.redditListMarquee.Visible = false;
            // 
            // btnRedditList
            // 
            this.btnRedditList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRedditList.Location = new System.Drawing.Point(490, 85);
            this.btnRedditList.Name = "btnRedditList";
            this.btnRedditList.Size = new System.Drawing.Size(122, 25);
            this.btnRedditList.TabIndex = 4;
            this.btnRedditList.Text = "List Available Formats";
            this.btnRedditList.Click += new System.EventHandler(this.btnRedditList_Click);
            // 
            // btnRedditDefault
            // 
            this.btnRedditDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRedditDefault.Location = new System.Drawing.Point(618, 85);
            this.btnRedditDefault.Name = "btnRedditDefault";
            this.btnRedditDefault.Size = new System.Drawing.Size(101, 25);
            this.btnRedditDefault.TabIndex = 3;
            this.btnRedditDefault.Text = "Download Default";
            this.btnRedditDefault.Click += new System.EventHandler(this.btnRedditDefault_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(31, 57);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(93, 16);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Video Permalink";
            // 
            // txtRedditPost
            // 
            this.txtRedditPost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRedditPost.Location = new System.Drawing.Point(130, 54);
            this.txtRedditPost.Name = "txtRedditPost";
            this.txtRedditPost.Properties.AutoHeight = false;
            this.txtRedditPost.Size = new System.Drawing.Size(594, 25);
            this.txtRedditPost.TabIndex = 1;
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.EditValue = global::YT_RED.Properties.Resources.reddit;
            this.pictureEdit1.Location = new System.Drawing.Point(7, 6);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.pictureEdit1.Size = new System.Drawing.Size(112, 45);
            this.pictureEdit1.TabIndex = 0;
            // 
            // gcHistory
            // 
            this.gcHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcHistory.Location = new System.Drawing.Point(0, 161);
            this.gcHistory.MainView = this.gvHistory;
            this.gcHistory.Name = "gcHistory";
            this.gcHistory.Size = new System.Drawing.Size(325, 517);
            this.gcHistory.TabIndex = 5;
            this.gcHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvHistory});
            // 
            // gvHistory
            // 
            this.gvHistory.GridControl = this.gcHistory;
            this.gvHistory.Name = "gvHistory";
            this.gvHistory.OptionsBehavior.AlignGroupSummaryInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.gvHistory.OptionsBehavior.Editable = false;
            this.gvHistory.OptionsCustomization.AllowColumnMoving = false;
            this.gvHistory.OptionsCustomization.AllowGroup = false;
            this.gvHistory.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvHistory.OptionsDetail.ShowDetailTabs = false;
            this.gvHistory.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvHistory.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gvHistory.OptionsView.ShowDetailButtons = false;
            this.gvHistory.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.gvHistory.OptionsView.ShowGroupPanel = false;
            this.gvHistory.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvHistory_FocusedRowChanged);
            this.gvHistory.DoubleClick += new System.EventHandler(this.gvHistory_DoubleClick);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.btnRedDL);
            this.panelControl2.Controls.Add(this.pbDownloadProgress);
            this.panelControl2.Controls.Add(this.btnDownloadReddit);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(0, 20);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(325, 141);
            this.panelControl2.TabIndex = 4;
            // 
            // btnRedDL
            // 
            this.btnRedDL.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnRedDL.Location = new System.Drawing.Point(2, 113);
            this.btnRedDL.Name = "btnRedDL";
            this.btnRedDL.Size = new System.Drawing.Size(321, 26);
            this.btnRedDL.TabIndex = 2;
            this.btnRedDL.Visible = false;
            this.btnRedDL.Click += new System.EventHandler(this.btnRedDL_Click);
            // 
            // pbDownloadProgress
            // 
            this.pbDownloadProgress.Location = new System.Drawing.Point(15, 65);
            this.pbDownloadProgress.Name = "pbDownloadProgress";
            this.pbDownloadProgress.Properties.ShowTitle = true;
            this.pbDownloadProgress.Size = new System.Drawing.Size(298, 31);
            this.pbDownloadProgress.TabIndex = 1;
            this.pbDownloadProgress.Visible = false;
            // 
            // btnDownloadReddit
            // 
            this.btnDownloadReddit.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnDownloadReddit.Appearance.Options.UseFont = true;
            this.btnDownloadReddit.Enabled = false;
            this.btnDownloadReddit.Location = new System.Drawing.Point(105, 18);
            this.btnDownloadReddit.Name = "btnDownloadReddit";
            this.btnDownloadReddit.Size = new System.Drawing.Size(112, 32);
            this.btnDownloadReddit.TabIndex = 0;
            this.btnDownloadReddit.Text = "DOWNLOAD";
            this.btnDownloadReddit.Visible = false;
            this.btnDownloadReddit.Click += new System.EventHandler(this.btnDownloadReddit_Click);
            // 
            // lblSelectionText
            // 
            this.lblSelectionText.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblSelectionText.Appearance.Options.UseFont = true;
            this.lblSelectionText.Appearance.Options.UseTextOptions = true;
            this.lblSelectionText.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSelectionText.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblSelectionText.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSelectionText.Location = new System.Drawing.Point(0, 0);
            this.lblSelectionText.Name = "lblSelectionText";
            this.lblSelectionText.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.lblSelectionText.Size = new System.Drawing.Size(325, 20);
            this.lblSelectionText.TabIndex = 3;
            // 
            // tfpHome
            // 
            this.tfpHome.ContentContainer = this.tabFormContentContainer1;
            this.tfpHome.Name = "tfpHome";
            this.tfpHome.Text = "Settings";
            // 
            // tabFormContentContainer1
            // 
            this.tabFormContentContainer1.Controls.Add(this.gridPanel);
            this.tabFormContentContainer1.Controls.Add(this.panelControl3);
            this.tabFormContentContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFormContentContainer1.Location = new System.Drawing.Point(0, 50);
            this.tabFormContentContainer1.Name = "tabFormContentContainer1";
            this.tabFormContentContainer1.Size = new System.Drawing.Size(1059, 678);
            this.tabFormContentContainer1.TabIndex = 1;
            // 
            // gridPanel
            // 
            this.gridPanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 1F)});
            this.gridPanel.Controls.Add(this.settingsGrid);
            this.gridPanel.Controls.Add(this.pgToolbar1);
            this.gridPanel.Controls.Add(this.propertyDescriptionControl1);
            this.gridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel.Location = new System.Drawing.Point(0, 33);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 1F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 1F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 1F)});
            this.gridPanel.Size = new System.Drawing.Size(1059, 645);
            this.gridPanel.TabIndex = 1;
            // 
            // settingsGrid
            // 
            this.gridPanel.SetColumn(this.settingsGrid, 0);
            this.settingsGrid.Cursor = System.Windows.Forms.Cursors.Default;
            this.settingsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsGrid.Location = new System.Drawing.Point(3, 31);
            this.settingsGrid.Name = "settingsGrid";
            this.settingsGrid.OptionsFind.FindFilterColumns = "Caption;SearchTags";
            this.settingsGrid.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            this.gridPanel.SetRow(this.settingsGrid, 1);
            this.settingsGrid.Size = new System.Drawing.Size(1053, 505);
            this.settingsGrid.TabIndex = 0;
            // 
            // pgToolbar1
            // 
            this.gridPanel.SetColumn(this.pgToolbar1, 0);
            this.pgToolbar1.Controls.Add(this.pgCategoryCheckButton1);
            this.pgToolbar1.Controls.Add(this.pgAlphabeticalCheckButton1);
            this.pgToolbar1.Controls.Add(this.pgSeparatorControl1);
            this.pgToolbar1.Controls.Add(this.pgDescriptionCheckButton1);
            this.pgToolbar1.Controls.Add(this.pgSearchControl1);
            this.pgToolbar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgToolbar1.Location = new System.Drawing.Point(0, 2);
            this.pgToolbar1.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.pgToolbar1.Name = "pgToolbar1";
            this.gridPanel.SetRow(this.pgToolbar1, 0);
            this.pgToolbar1.Size = new System.Drawing.Size(1059, 24);
            this.pgToolbar1.TabIndex = 1;
            // 
            // pgCategoryCheckButton1
            // 
            this.pgCategoryCheckButton1.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("pgCategoryCheckButton1.ImageOptions.SvgImage")));
            this.pgCategoryCheckButton1.Location = new System.Drawing.Point(2, 0);
            this.pgCategoryCheckButton1.Name = "pgCategoryCheckButton1";
            this.pgCategoryCheckButton1.PropertyGrid = this.settingsGrid;
            this.pgCategoryCheckButton1.Size = new System.Drawing.Size(26, 24);
            this.pgCategoryCheckButton1.TabIndex = 0;
            // 
            // pgAlphabeticalCheckButton1
            // 
            this.pgAlphabeticalCheckButton1.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("pgAlphabeticalCheckButton1.ImageOptions.SvgImage")));
            this.pgAlphabeticalCheckButton1.Location = new System.Drawing.Point(32, 0);
            this.pgAlphabeticalCheckButton1.Name = "pgAlphabeticalCheckButton1";
            this.pgAlphabeticalCheckButton1.PropertyGrid = this.settingsGrid;
            this.pgAlphabeticalCheckButton1.Size = new System.Drawing.Size(26, 24);
            this.pgAlphabeticalCheckButton1.TabIndex = 1;
            this.pgAlphabeticalCheckButton1.TabStop = false;
            // 
            // pgSeparatorControl1
            // 
            this.pgSeparatorControl1.Location = new System.Drawing.Point(63, 2);
            this.pgSeparatorControl1.Name = "pgSeparatorControl1";
            this.pgSeparatorControl1.Size = new System.Drawing.Size(2, 20);
            this.pgSeparatorControl1.TabIndex = 2;
            // 
            // pgDescriptionCheckButton1
            // 
            this.pgDescriptionCheckButton1.DescriptionControl = this.propertyDescriptionControl1;
            this.pgDescriptionCheckButton1.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("pgDescriptionCheckButton1.ImageOptions.SvgImage")));
            this.pgDescriptionCheckButton1.Location = new System.Drawing.Point(70, 0);
            this.pgDescriptionCheckButton1.Name = "pgDescriptionCheckButton1";
            this.pgDescriptionCheckButton1.PropertyGrid = this.settingsGrid;
            this.pgDescriptionCheckButton1.Size = new System.Drawing.Size(26, 24);
            this.pgDescriptionCheckButton1.TabIndex = 3;
            // 
            // propertyDescriptionControl1
            // 
            this.propertyDescriptionControl1.AutoHeight = true;
            this.gridPanel.SetColumn(this.propertyDescriptionControl1, 0);
            this.propertyDescriptionControl1.Location = new System.Drawing.Point(3, 542);
            this.propertyDescriptionControl1.Name = "propertyDescriptionControl1";
            this.propertyDescriptionControl1.PropertyGrid = this.settingsGrid;
            this.gridPanel.SetRow(this.propertyDescriptionControl1, 2);
            this.propertyDescriptionControl1.Size = new System.Drawing.Size(1053, 100);
            this.propertyDescriptionControl1.TabIndex = 2;
            // 
            // pgSearchControl1
            // 
            this.pgSearchControl1.Client = this.settingsGrid;
            this.pgSearchControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgSearchControl1.Location = new System.Drawing.Point(100, 2);
            this.pgSearchControl1.Name = "pgSearchControl1";
            this.pgSearchControl1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Repository.ClearButton(),
            new DevExpress.XtraEditors.Repository.SearchButton()});
            this.pgSearchControl1.Properties.Client = this.settingsGrid;
            this.pgSearchControl1.Size = new System.Drawing.Size(957, 20);
            this.pgToolbar1.SetStretched(this.pgSearchControl1, true);
            this.pgSearchControl1.TabIndex = 2;
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.btnSaveSettings);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl3.Location = new System.Drawing.Point(0, 0);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(1059, 33);
            this.panelControl3.TabIndex = 2;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSaveSettings.Location = new System.Drawing.Point(982, 2);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(75, 29);
            this.btnSaveSettings.TabIndex = 0;
            this.btnSaveSettings.Text = "Save";
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 728);
            this.Controls.Add(this.tabFormContentContainer3);
            this.Controls.Add(this.tabFormControl1);
            this.Name = "MainForm";
            this.TabFormControl = this.tabFormControl1;
            this.Text = "YT-RED";
            ((System.ComponentModel.ISupportInitialize)(this.tabFormControl1)).EndInit();
            this.tabFormContentContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).EndInit();
            this.splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).EndInit();
            this.splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcReddit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvReddit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.redditListMarquee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRedditPost.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbDownloadProgress.Properties)).EndInit();
            this.tabFormContentContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridPanel)).EndInit();
            this.gridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.settingsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pgToolbar1)).EndInit();
            this.pgToolbar1.ResumeLayout(false);
            this.pgToolbar1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pgSeparatorControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pgSearchControl1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private YT_RED.Controls.CustomTabFormPage tfpHome;
        private DevExpress.XtraBars.TabFormContentContainer tabFormContentContainer1;
        private YT_RED.Controls.CustomTabFormPage tfpYouTube;
        private DevExpress.XtraBars.TabFormContentContainer tabFormContentContainer2;
        private YT_RED.Controls.CustomTabFormPage tfpReddit;
        private DevExpress.XtraBars.TabFormContentContainer tabFormContentContainer3;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.MarqueeProgressBarControl redditListMarquee;
        private DevExpress.XtraEditors.SimpleButton btnRedditList;
        private DevExpress.XtraEditors.SimpleButton btnRedditDefault;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtRedditPost;
        private DevExpress.XtraGrid.GridControl gcReddit;
        private DevExpress.XtraGrid.Views.Grid.GridView gvReddit;
        private DevExpress.XtraEditors.LabelControl lblSelect;
        protected DevExpress.XtraBars.TabFormControl tabFormControl1;
        private DevExpress.XtraEditors.LabelControl lblSelectionText;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btnDownloadReddit;
        private DevExpress.XtraEditors.ProgressBarControl pbDownloadProgress;
        private DevExpress.XtraGrid.GridControl gcHistory;
        private DevExpress.XtraGrid.Views.Grid.GridView gvHistory;
        private DevExpress.XtraEditors.SimpleButton btnRedDL;
        private DevExpress.XtraVerticalGrid.PGPanel gridPanel;
        private DevExpress.XtraVerticalGrid.PropertyGridControl settingsGrid;
        private DevExpress.XtraVerticalGrid.PGToolbar pgToolbar1;
        private DevExpress.XtraVerticalGrid.PGCategoryCheckButton pgCategoryCheckButton1;
        private DevExpress.XtraVerticalGrid.PGAlphabeticalCheckButton pgAlphabeticalCheckButton1;
        private DevExpress.XtraVerticalGrid.PGSeparatorControl pgSeparatorControl1;
        private DevExpress.XtraVerticalGrid.PGDescriptionCheckButton pgDescriptionCheckButton1;
        private DevExpress.XtraVerticalGrid.PropertyDescriptionControl propertyDescriptionControl1;
        private DevExpress.XtraVerticalGrid.PGSearchControl pgSearchControl1;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.SimpleButton btnSaveSettings;
    }
}

