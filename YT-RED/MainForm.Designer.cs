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
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.tfpHome = new YT_RED.Controls.CustomTabFormPage();
            this.tabFormContentContainer1 = new DevExpress.XtraBars.TabFormContentContainer();
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
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.lblDLLocation = new DevExpress.XtraEditors.LabelControl();
            this.pbDownloadProgress = new DevExpress.XtraEditors.ProgressBarControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.lblSelectionText = new DevExpress.XtraEditors.LabelControl();
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
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDownloadProgress.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tabFormControl1
            // 
            this.tabFormControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1});
            this.tabFormControl1.Location = new System.Drawing.Point(0, 0);
            this.tabFormControl1.Name = "tabFormControl1";
            this.tabFormControl1.Pages.Add(this.tfpHome);
            this.tabFormControl1.Pages.Add(this.tfpYouTube);
            this.tabFormControl1.Pages.Add(this.tfpReddit);
            this.tabFormControl1.SelectedPage = this.tfpReddit;
            this.tabFormControl1.Size = new System.Drawing.Size(1059, 50);
            this.tabFormControl1.TabForm = this;
            this.tabFormControl1.TabIndex = 0;
            this.tabFormControl1.TabRightItemLinks.Add(this.barButtonItem1);
            this.tabFormControl1.TabStop = false;
            this.tabFormControl1.PageClosing += new DevExpress.XtraBars.PageClosingEventHandler(this.tabFormControl1_PageClosing);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Settings";
            this.barButtonItem1.Hint = "Program Settings";
            this.barButtonItem1.Id = 2;
            this.barButtonItem1.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("barButtonItem1.ImageOptions.SvgImage")));
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // tfpHome
            // 
            this.tfpHome.ContentContainer = this.tabFormContentContainer1;
            this.tfpHome.Name = "tfpHome";
            this.tfpHome.Text = "Home";
            // 
            // tabFormContentContainer1
            // 
            this.tabFormContentContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFormContentContainer1.Location = new System.Drawing.Point(0, 50);
            this.tabFormContentContainer1.Name = "tabFormContentContainer1";
            this.tabFormContentContainer1.Size = new System.Drawing.Size(1059, 678);
            this.tabFormContentContainer1.TabIndex = 1;
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
            this.gcReddit.Visible = false;
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
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.lblDLLocation);
            this.panelControl2.Controls.Add(this.pbDownloadProgress);
            this.panelControl2.Controls.Add(this.simpleButton1);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(0, 20);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(325, 176);
            this.panelControl2.TabIndex = 4;
            // 
            // lblDLLocation
            // 
            this.lblDLLocation.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblDLLocation.Appearance.Options.UseFont = true;
            this.lblDLLocation.Appearance.Options.UseTextOptions = true;
            this.lblDLLocation.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblDLLocation.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblDLLocation.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblDLLocation.Location = new System.Drawing.Point(2, 164);
            this.lblDLLocation.Name = "lblDLLocation";
            this.lblDLLocation.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.lblDLLocation.Size = new System.Drawing.Size(321, 10);
            this.lblDLLocation.TabIndex = 3;
            this.lblDLLocation.Click += new System.EventHandler(this.lblDLLocation_Click);
            // 
            // pbDownloadProgress
            // 
            this.pbDownloadProgress.Location = new System.Drawing.Point(15, 76);
            this.pbDownloadProgress.Name = "pbDownloadProgress";
            this.pbDownloadProgress.Properties.ShowTitle = true;
            this.pbDownloadProgress.Size = new System.Drawing.Size(298, 31);
            this.pbDownloadProgress.TabIndex = 1;
            this.pbDownloadProgress.Visible = false;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.simpleButton1.Appearance.Options.UseFont = true;
            this.simpleButton1.Enabled = false;
            this.simpleButton1.Location = new System.Drawing.Point(106, 20);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(112, 32);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "DOWNLOAD";
            this.simpleButton1.Visible = false;
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
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
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbDownloadProgress.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private YT_RED.Controls.CustomTabFormPage tfpHome;
        private DevExpress.XtraBars.TabFormContentContainer tabFormContentContainer1;
        private YT_RED.Controls.CustomTabFormPage tfpYouTube;
        private DevExpress.XtraBars.TabFormContentContainer tabFormContentContainer2;
        private YT_RED.Controls.CustomTabFormPage tfpReddit;
        private DevExpress.XtraBars.TabFormContentContainer tabFormContentContainer3;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
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
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.ProgressBarControl pbDownloadProgress;
        private DevExpress.XtraEditors.LabelControl lblDLLocation;
    }
}

