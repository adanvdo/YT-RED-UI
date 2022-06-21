
namespace YT_RED.Controls
{
    partial class TrayForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrayForm));
            this.pcMainPanel = new DevExpress.XtraEditors.PanelControl();
            this.progressPanel = new DevExpress.XtraEditors.PanelControl();
            this.btnOpenDL = new DevExpress.XtraEditors.SimpleButton();
            this.pgTrayProgress = new DevExpress.XtraEditors.ProgressBarControl();
            this.progressMarquee = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.pcTop = new DevExpress.XtraEditors.PanelControl();
            this.btnGo = new DevExpress.XtraEditors.SimpleButton();
            this.txtUrl = new DevExpress.XtraEditors.TextEdit();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.pcMainPanel)).BeginInit();
            this.pcMainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressPanel)).BeginInit();
            this.progressPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pgTrayProgress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressMarquee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcTop)).BeginInit();
            this.pcTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUrl.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pcMainPanel
            // 
            this.pcMainPanel.AutoSize = true;
            this.pcMainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pcMainPanel.Controls.Add(this.progressPanel);
            this.pcMainPanel.Controls.Add(this.pcTop);
            this.pcMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pcMainPanel.Location = new System.Drawing.Point(0, 0);
            this.pcMainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pcMainPanel.MinimumSize = new System.Drawing.Size(362, 70);
            this.pcMainPanel.Name = "pcMainPanel";
            this.pcMainPanel.Size = new System.Drawing.Size(362, 160);
            this.pcMainPanel.TabIndex = 0;
            // 
            // progressPanel
            // 
            this.progressPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.progressPanel.Controls.Add(this.btnOpenDL);
            this.progressPanel.Controls.Add(this.pgTrayProgress);
            this.progressPanel.Controls.Add(this.progressMarquee);
            this.progressPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressPanel.Location = new System.Drawing.Point(2, 76);
            this.progressPanel.Margin = new System.Windows.Forms.Padding(0);
            this.progressPanel.Name = "progressPanel";
            this.progressPanel.Size = new System.Drawing.Size(358, 84);
            this.progressPanel.TabIndex = 1;
            // 
            // btnOpenDL
            // 
            this.btnOpenDL.Appearance.Options.UseTextOptions = true;
            this.btnOpenDL.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.btnOpenDL.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.btnOpenDL.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOpenDL.Location = new System.Drawing.Point(0, 56);
            this.btnOpenDL.Name = "btnOpenDL";
            this.btnOpenDL.Size = new System.Drawing.Size(358, 25);
            this.btnOpenDL.TabIndex = 4;
            this.btnOpenDL.Visible = false;
            this.btnOpenDL.Click += new System.EventHandler(this.btnOpenDL_Click);
            // 
            // pgTrayProgress
            // 
            this.pgTrayProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.pgTrayProgress.Location = new System.Drawing.Point(0, 25);
            this.pgTrayProgress.MaximumSize = new System.Drawing.Size(358, 31);
            this.pgTrayProgress.Name = "pgTrayProgress";
            this.pgTrayProgress.Properties.ShowTitle = true;
            this.pgTrayProgress.Size = new System.Drawing.Size(358, 31);
            this.pgTrayProgress.TabIndex = 0;
            this.pgTrayProgress.Visible = false;
            // 
            // progressMarquee
            // 
            this.progressMarquee.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressMarquee.EditValue = 0;
            this.progressMarquee.Location = new System.Drawing.Point(0, 0);
            this.progressMarquee.MaximumSize = new System.Drawing.Size(358, 25);
            this.progressMarquee.Name = "progressMarquee";
            this.progressMarquee.Properties.ShowTitle = true;
            this.progressMarquee.Size = new System.Drawing.Size(358, 25);
            this.progressMarquee.TabIndex = 3;
            this.progressMarquee.Visible = false;
            // 
            // pcTop
            // 
            this.pcTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pcTop.Controls.Add(this.btnGo);
            this.pcTop.Controls.Add(this.txtUrl);
            this.pcTop.Controls.Add(this.btnClose);
            this.pcTop.Controls.Add(this.labelControl1);
            this.pcTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pcTop.Location = new System.Drawing.Point(2, 2);
            this.pcTop.Name = "pcTop";
            this.pcTop.Size = new System.Drawing.Size(358, 74);
            this.pcTop.TabIndex = 3;
            // 
            // btnGo
            // 
            this.btnGo.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnGo.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnGo.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnGo.ImageOptions.SvgImage")));
            this.btnGo.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
            this.btnGo.ImageOptions.SvgImageSize = new System.Drawing.Size(23, 23);
            this.btnGo.Location = new System.Drawing.Point(330, 35);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(25, 25);
            this.btnGo.TabIndex = 3;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(10, 36);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Properties.AutoHeight = false;
            this.txtUrl.Size = new System.Drawing.Size(314, 25);
            this.txtUrl.TabIndex = 0;
            this.txtUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUrl_KeyDown);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnClose.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClose.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnClose.ImageOptions.SvgImage")));
            this.btnClose.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
            this.btnClose.ImageOptions.SvgImageSize = new System.Drawing.Size(25, 25);
            this.btnClose.Location = new System.Drawing.Point(330, 2);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(26, 26);
            this.btnClose.TabIndex = 2;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.labelControl1.Location = new System.Drawing.Point(10, 5);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelControl1.Size = new System.Drawing.Size(180, 25);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Media URL";
            // 
            // TrayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(362, 160);
            this.ControlBox = false;
            this.Controls.Add(this.pcMainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("TrayForm.IconOptions.Image")));
            this.IconOptions.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrayForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TrayForm_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.pcMainPanel)).EndInit();
            this.pcMainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.progressPanel)).EndInit();
            this.progressPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pgTrayProgress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressMarquee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcTop)).EndInit();
            this.pcTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtUrl.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pcMainPanel;
        private DevExpress.XtraEditors.PanelControl progressPanel;
        private DevExpress.XtraEditors.ProgressBarControl pgTrayProgress;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtUrl;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.MarqueeProgressBarControl progressMarquee;
        private DevExpress.XtraEditors.PanelControl pcTop;
        private DevExpress.XtraEditors.SimpleButton btnOpenDL;
        private DevExpress.XtraEditors.SimpleButton btnGo;
    }
}