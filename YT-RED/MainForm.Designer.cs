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
            this.tfpHome = new DevExpress.XtraBars.TabFormPage();
            this.tabFormContentContainer1 = new DevExpress.XtraBars.TabFormContentContainer();
            this.tfpYouTube = new DevExpress.XtraBars.TabFormPage();
            this.tabFormContentContainer2 = new DevExpress.XtraBars.TabFormContentContainer();
            this.tfpReddit = new DevExpress.XtraBars.TabFormPage();
            this.tabFormContentContainer3 = new DevExpress.XtraBars.TabFormContentContainer();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.gcRedditAudio = new DevExpress.XtraGrid.GridControl();
            this.gvRedditAudio = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.redditListMarquee = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.btnRedditList = new DevExpress.XtraEditors.SimpleButton();
            this.btnRedditDefault = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtRedditPost = new DevExpress.XtraEditors.TextEdit();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            ((System.ComponentModel.ISupportInitialize)(this.tabFormControl1)).BeginInit();
            this.tabFormContentContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).BeginInit();
            this.splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcRedditAudio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvRedditAudio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.redditListMarquee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRedditPost.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
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
            this.tabFormControl1.SelectedPage = this.tfpHome;
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
            this.splitContainerControl1.Panel1.Controls.Add(this.gcRedditAudio);
            this.splitContainerControl1.Panel1.Controls.Add(this.labelControl2);
            this.splitContainerControl1.Panel1.Controls.Add(this.panelControl1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(1059, 678);
            this.splitContainerControl1.SplitterPosition = 569;
            this.splitContainerControl1.TabIndex = 0;
            // 
            // gcRedditAudio
            // 
            this.gcRedditAudio.Dock = System.Windows.Forms.DockStyle.Top;
            this.gcRedditAudio.Location = new System.Drawing.Point(0, 161);
            this.gcRedditAudio.MainView = this.gvRedditAudio;
            this.gcRedditAudio.Name = "gcRedditAudio";
            this.gcRedditAudio.Size = new System.Drawing.Size(569, 200);
            this.gcRedditAudio.TabIndex = 2;
            this.gcRedditAudio.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvRedditAudio});
            this.gcRedditAudio.Visible = false;
            // 
            // gvRedditAudio
            // 
            this.gvRedditAudio.GridControl = this.gcRedditAudio;
            this.gvRedditAudio.Name = "gvRedditAudio";
            this.gvRedditAudio.OptionsBehavior.AlignGroupSummaryInGroupRow = DevExpress.Utils.DefaultBoolean.False;
            this.gvRedditAudio.OptionsBehavior.Editable = false;
            this.gvRedditAudio.OptionsCustomization.AllowColumnMoving = false;
            this.gvRedditAudio.OptionsCustomization.AllowGroup = false;
            this.gvRedditAudio.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvRedditAudio.OptionsDetail.ShowDetailTabs = false;
            this.gvRedditAudio.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvRedditAudio.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gvRedditAudio.OptionsView.ShowDetailButtons = false;
            this.gvRedditAudio.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.gvRedditAudio.OptionsView.ShowGroupPanel = false;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Appearance.Options.UseTextOptions = true;
            this.labelControl2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelControl2.Location = new System.Drawing.Point(0, 133);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(569, 28);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Select a Video and/or Audio Format";
            this.labelControl2.Visible = false;
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
            this.panelControl1.Size = new System.Drawing.Size(569, 133);
            this.panelControl1.TabIndex = 0;
            // 
            // redditListMarquee
            // 
            this.redditListMarquee.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.redditListMarquee.EditValue = 0;
            this.redditListMarquee.Location = new System.Drawing.Point(5, 116);
            this.redditListMarquee.Name = "redditListMarquee";
            this.redditListMarquee.Size = new System.Drawing.Size(559, 10);
            this.redditListMarquee.TabIndex = 5;
            // 
            // btnRedditList
            // 
            this.btnRedditList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRedditList.Location = new System.Drawing.Point(330, 85);
            this.btnRedditList.Name = "btnRedditList";
            this.btnRedditList.Size = new System.Drawing.Size(122, 25);
            this.btnRedditList.TabIndex = 4;
            this.btnRedditList.Text = "List Available Formats";
            this.btnRedditList.Click += new System.EventHandler(this.btnRedditList_Click);
            // 
            // btnRedditDefault
            // 
            this.btnRedditDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRedditDefault.Location = new System.Drawing.Point(458, 85);
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
            this.txtRedditPost.Size = new System.Drawing.Size(434, 25);
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 728);
            this.Controls.Add(this.tabFormContentContainer1);
            this.Controls.Add(this.tabFormControl1);
            this.Name = "MainForm";
            this.TabFormControl = this.tabFormControl1;
            this.Text = "YT-RED";
            ((System.ComponentModel.ISupportInitialize)(this.tabFormControl1)).EndInit();
            this.tabFormContentContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).EndInit();
            this.splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcRedditAudio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvRedditAudio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.redditListMarquee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRedditPost.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.TabFormControl tabFormControl1;
        private DevExpress.XtraBars.TabFormPage tfpHome;
        private DevExpress.XtraBars.TabFormContentContainer tabFormContentContainer1;
        private DevExpress.XtraBars.TabFormPage tfpYouTube;
        private DevExpress.XtraBars.TabFormContentContainer tabFormContentContainer2;
        private DevExpress.XtraBars.TabFormPage tfpReddit;
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
        private DevExpress.XtraGrid.GridControl gcRedditAudio;
        private DevExpress.XtraGrid.Views.Grid.GridView gvRedditAudio;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}

