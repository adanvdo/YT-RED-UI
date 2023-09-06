namespace YT_RED.Controls
{
    partial class VideoInfoPanel
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
            this.lblMediaInfo = new DevExpress.XtraEditors.LabelControl();
            this.pnlMediaInfo = new DevExpress.XtraEditors.PanelControl();
            this.btnCropMedia = new DevExpress.XtraEditors.SimpleButton();
            this.pnlTextPanel = new DevExpress.XtraEditors.PanelControl();
            this.txtDescription = new DevExpress.XtraEditors.MemoEdit();
            this.txtDuration = new DevExpress.XtraEditors.TextEdit();
            this.txtTitle = new DevExpress.XtraEditors.TextEdit();
            this.peThumbnail = new DevExpress.XtraEditors.PictureEdit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMediaInfo)).BeginInit();
            this.pnlMediaInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTextPanel)).BeginInit();
            this.pnlTextPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDuration.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peThumbnail.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMediaInfo
            // 
            this.lblMediaInfo.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblMediaInfo.Appearance.Options.UseFont = true;
            this.lblMediaInfo.Appearance.Options.UseTextOptions = true;
            this.lblMediaInfo.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblMediaInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMediaInfo.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblMediaInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMediaInfo.Location = new System.Drawing.Point(0, 0);
            this.lblMediaInfo.Margin = new System.Windows.Forms.Padding(0);
            this.lblMediaInfo.Name = "lblMediaInfo";
            this.lblMediaInfo.Size = new System.Drawing.Size(572, 25);
            this.lblMediaInfo.TabIndex = 2;
            this.lblMediaInfo.Text = "Media Info";
            // 
            // pnlMediaInfo
            // 
            this.pnlMediaInfo.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlMediaInfo.Controls.Add(this.btnCropMedia);
            this.pnlMediaInfo.Controls.Add(this.pnlTextPanel);
            this.pnlMediaInfo.Controls.Add(this.peThumbnail);
            this.pnlMediaInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMediaInfo.Location = new System.Drawing.Point(0, 25);
            this.pnlMediaInfo.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMediaInfo.Name = "pnlMediaInfo";
            this.pnlMediaInfo.Size = new System.Drawing.Size(572, 125);
            this.pnlMediaInfo.TabIndex = 3;
            // 
            // btnCropMedia
            // 
            this.btnCropMedia.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCropMedia.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnCropMedia.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCropMedia.ImageOptions.SvgImage = global::YT_RED.Properties.Resources.crop;
            this.btnCropMedia.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
            this.btnCropMedia.ImageOptions.SvgImageSize = new System.Drawing.Size(28, 28);
            this.btnCropMedia.Location = new System.Drawing.Point(183, 7);
            this.btnCropMedia.Name = "btnCropMedia";
            this.btnCropMedia.Size = new System.Drawing.Size(30, 30);
            this.btnCropMedia.TabIndex = 4;
            this.btnCropMedia.ToolTip = "Crop Media";
            this.btnCropMedia.Visible = false;
            this.btnCropMedia.Click += new System.EventHandler(this.btnCropMedia_Click);
            // 
            // pnlTextPanel
            // 
            this.pnlTextPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlTextPanel.Controls.Add(this.txtDescription);
            this.pnlTextPanel.Controls.Add(this.txtDuration);
            this.pnlTextPanel.Controls.Add(this.txtTitle);
            this.pnlTextPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTextPanel.Location = new System.Drawing.Point(219, 0);
            this.pnlTextPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlTextPanel.Name = "pnlTextPanel";
            this.pnlTextPanel.Size = new System.Drawing.Size(353, 125);
            this.pnlTextPanel.TabIndex = 3;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.EditValue = "";
            this.txtDescription.Location = new System.Drawing.Point(0, 50);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Properties.AllowFocused = false;
            this.txtDescription.Properties.Appearance.Options.UseTextOptions = true;
            this.txtDescription.Properties.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.txtDescription.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.txtDescription.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtDescription.Properties.ReadOnly = true;
            this.txtDescription.Properties.UseReadOnlyAppearance = false;
            this.txtDescription.Size = new System.Drawing.Size(353, 75);
            this.txtDescription.TabIndex = 2;
            // 
            // txtDuration
            // 
            this.txtDuration.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtDuration.EditValue = "Duration:";
            this.txtDuration.Location = new System.Drawing.Point(0, 25);
            this.txtDuration.Name = "txtDuration";
            this.txtDuration.Properties.AllowFocused = false;
            this.txtDuration.Properties.AutoHeight = false;
            this.txtDuration.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.txtDuration.Properties.ReadOnly = true;
            this.txtDuration.Properties.UseReadOnlyAppearance = false;
            this.txtDuration.Size = new System.Drawing.Size(353, 25);
            this.txtDuration.TabIndex = 1;
            // 
            // txtTitle
            // 
            this.txtTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtTitle.EditValue = "TITLE";
            this.txtTitle.Location = new System.Drawing.Point(0, 0);
            this.txtTitle.Margin = new System.Windows.Forms.Padding(0);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Properties.AllowFocused = false;
            this.txtTitle.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtTitle.Properties.Appearance.Options.UseFont = true;
            this.txtTitle.Properties.AutoHeight = false;
            this.txtTitle.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.txtTitle.Properties.ReadOnly = true;
            this.txtTitle.Properties.UseReadOnlyAppearance = false;
            this.txtTitle.Size = new System.Drawing.Size(353, 25);
            this.txtTitle.TabIndex = 0;
            // 
            // peThumbnail
            // 
            this.peThumbnail.Dock = System.Windows.Forms.DockStyle.Left;
            this.peThumbnail.Location = new System.Drawing.Point(0, 0);
            this.peThumbnail.Margin = new System.Windows.Forms.Padding(0);
            this.peThumbnail.Name = "peThumbnail";
            this.peThumbnail.Properties.AllowFocused = false;
            this.peThumbnail.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.peThumbnail.Properties.NullText = " ";
            this.peThumbnail.Properties.ReadOnly = true;
            this.peThumbnail.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.peThumbnail.Properties.ShowEditMenuItem = DevExpress.Utils.DefaultBoolean.False;
            this.peThumbnail.Properties.ShowMenu = false;
            this.peThumbnail.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.peThumbnail.Size = new System.Drawing.Size(219, 125);
            this.peThumbnail.TabIndex = 2;
            // 
            // VideoInfoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMediaInfo);
            this.Controls.Add(this.lblMediaInfo);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "VideoInfoPanel";
            this.Size = new System.Drawing.Size(572, 150);
            ((System.ComponentModel.ISupportInitialize)(this.pnlMediaInfo)).EndInit();
            this.pnlMediaInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlTextPanel)).EndInit();
            this.pnlTextPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDuration.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peThumbnail.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblMediaInfo;
        private DevExpress.XtraEditors.PanelControl pnlMediaInfo;
        private DevExpress.XtraEditors.PictureEdit peThumbnail;
        private DevExpress.XtraEditors.PanelControl pnlTextPanel;
        private DevExpress.XtraEditors.TextEdit txtTitle;
        private DevExpress.XtraEditors.TextEdit txtDuration;
        private DevExpress.XtraEditors.MemoEdit txtDescription;
        private DevExpress.XtraEditors.SimpleButton btnCropMedia;
    }
}
