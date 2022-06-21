namespace YT_RED.Controls
{
    partial class MsgBox
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
            this.messagePanel = new DevExpress.XtraEditors.PanelControl();
            this.msgIcon = new DevExpress.XtraEditors.PictureEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.buttonPanel = new DevExpress.XtraEditors.PanelControl();
            this.btnYes = new DevExpress.XtraEditors.SimpleButton();
            this.btnNo = new DevExpress.XtraEditors.SimpleButton();
            this.btnAbort = new DevExpress.XtraEditors.SimpleButton();
            this.btnRetry = new DevExpress.XtraEditors.SimpleButton();
            this.btnIgnore = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.lblCaption = new DevExpress.XtraEditors.LabelControl();
            this.pnlSuppressPanel = new DevExpress.XtraEditors.PanelControl();
            this.chkSuppress = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.messagePanel)).BeginInit();
            this.messagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.msgIcon.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonPanel)).BeginInit();
            this.buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSuppressPanel)).BeginInit();
            this.pnlSuppressPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSuppress.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // messagePanel
            // 
            this.messagePanel.AutoSize = true;
            this.messagePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.messagePanel.Controls.Add(this.msgIcon);
            this.messagePanel.Controls.Add(this.panelControl1);
            this.messagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messagePanel.Location = new System.Drawing.Point(3, 28);
            this.messagePanel.Margin = new System.Windows.Forms.Padding(0);
            this.messagePanel.MinimumSize = new System.Drawing.Size(394, 102);
            this.messagePanel.Name = "messagePanel";
            this.messagePanel.Padding = new System.Windows.Forms.Padding(0, 15, 15, 15);
            this.messagePanel.Size = new System.Drawing.Size(394, 130);
            this.messagePanel.TabIndex = 0;
            // 
            // msgIcon
            // 
            this.msgIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.msgIcon.EditValue = global::YT_RED.Properties.Resources.exclamation;
            this.msgIcon.Location = new System.Drawing.Point(24, 43);
            this.msgIcon.Margin = new System.Windows.Forms.Padding(0);
            this.msgIcon.Name = "msgIcon";
            this.msgIcon.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.msgIcon.Properties.Appearance.BackColor2 = System.Drawing.Color.Transparent;
            this.msgIcon.Properties.Appearance.Options.UseBackColor = true;
            this.msgIcon.Properties.Appearance.Options.UseImage = true;
            this.msgIcon.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.msgIcon.Properties.ContextButtonOptions.AllowGlyphSkinning = true;
            this.msgIcon.Properties.ErrorImageOptions.SvgImage = global::YT_RED.Properties.Resources.highimportance;
            this.msgIcon.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.msgIcon.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.msgIcon.Properties.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
            this.msgIcon.Size = new System.Drawing.Size(40, 40);
            this.msgIcon.TabIndex = 1;
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.lblMessage);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControl1.Location = new System.Drawing.Point(92, 15);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(287, 100);
            this.panelControl1.TabIndex = 0;
            // 
            // lblMessage
            // 
            this.lblMessage.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Appearance.Options.UseFont = true;
            this.lblMessage.Appearance.Options.UseTextOptions = true;
            this.lblMessage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblMessage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMessage.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Location = new System.Drawing.Point(0, 0);
            this.lblMessage.MinimumSize = new System.Drawing.Size(287, 72);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(287, 100);
            this.lblMessage.TabIndex = 0;
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.Location = new System.Drawing.Point(50, 180);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Size = new System.Drawing.Size(8, 8);
            this.pictureEdit1.TabIndex = 1;
            // 
            // buttonPanel
            // 
            this.buttonPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.buttonPanel.Controls.Add(this.btnYes);
            this.buttonPanel.Controls.Add(this.btnNo);
            this.buttonPanel.Controls.Add(this.btnAbort);
            this.buttonPanel.Controls.Add(this.btnRetry);
            this.buttonPanel.Controls.Add(this.btnIgnore);
            this.buttonPanel.Controls.Add(this.btnOk);
            this.buttonPanel.Controls.Add(this.btnCancel);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(3, 182);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(5);
            this.buttonPanel.Size = new System.Drawing.Size(394, 42);
            this.buttonPanel.TabIndex = 2;
            // 
            // btnYes
            // 
            this.btnYes.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnYes.Location = new System.Drawing.Point(-61, 7);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(64, 28);
            this.btnYes.TabIndex = 4;
            this.btnYes.Text = "Yes";
            this.btnYes.Visible = false;
            this.btnYes.Click += new System.EventHandler(this.btnClick);
            // 
            // btnNo
            // 
            this.btnNo.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnNo.Location = new System.Drawing.Point(3, 7);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(64, 28);
            this.btnNo.TabIndex = 5;
            this.btnNo.Text = "No";
            this.btnNo.Visible = false;
            this.btnNo.Click += new System.EventHandler(this.btnClick);
            // 
            // btnAbort
            // 
            this.btnAbort.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAbort.Location = new System.Drawing.Point(67, 7);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(64, 28);
            this.btnAbort.TabIndex = 2;
            this.btnAbort.Text = "Abort";
            this.btnAbort.Visible = false;
            this.btnAbort.Click += new System.EventHandler(this.btnClick);
            // 
            // btnRetry
            // 
            this.btnRetry.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRetry.Location = new System.Drawing.Point(131, 7);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(64, 28);
            this.btnRetry.TabIndex = 3;
            this.btnRetry.Text = "Retry";
            this.btnRetry.Visible = false;
            this.btnRetry.Click += new System.EventHandler(this.btnClick);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnIgnore.Location = new System.Drawing.Point(195, 7);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(64, 28);
            this.btnIgnore.TabIndex = 6;
            this.btnIgnore.Text = "Ignore";
            this.btnIgnore.Visible = false;
            this.btnIgnore.Click += new System.EventHandler(this.btnClick);
            // 
            // btnOk
            // 
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOk.Location = new System.Drawing.Point(259, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(64, 28);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.Visible = false;
            this.btnOk.Click += new System.EventHandler(this.btnClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(323, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnClick);
            // 
            // lblCaption
            // 
            this.lblCaption.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblCaption.Appearance.Options.UseFont = true;
            this.lblCaption.Appearance.Options.UseTextOptions = true;
            this.lblCaption.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.lblCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblCaption.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCaption.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCaption.Location = new System.Drawing.Point(3, 3);
            this.lblCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Padding = new System.Windows.Forms.Padding(10, 2, 10, 0);
            this.lblCaption.Size = new System.Drawing.Size(394, 25);
            this.lblCaption.TabIndex = 3;
            this.lblCaption.Visible = false;
            // 
            // pnlSuppressPanel
            // 
            this.pnlSuppressPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlSuppressPanel.Controls.Add(this.chkSuppress);
            this.pnlSuppressPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSuppressPanel.Location = new System.Drawing.Point(3, 158);
            this.pnlSuppressPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSuppressPanel.Name = "pnlSuppressPanel";
            this.pnlSuppressPanel.Size = new System.Drawing.Size(394, 24);
            this.pnlSuppressPanel.TabIndex = 4;
            this.pnlSuppressPanel.Visible = false;
            // 
            // chkSuppress
            // 
            this.chkSuppress.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkSuppress.Location = new System.Drawing.Point(259, 0);
            this.chkSuppress.Name = "chkSuppress";
            this.chkSuppress.Properties.Caption = "   Don\'t Show Again";
            this.chkSuppress.Size = new System.Drawing.Size(135, 24);
            this.chkSuppress.TabIndex = 0;
            this.chkSuppress.CheckedChanged += new System.EventHandler(this.chkSuppress_CheckedChanged);
            // 
            // MsgBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(400, 227);
            this.Controls.Add(this.messagePanel);
            this.Controls.Add(this.pnlSuppressPanel);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.pictureEdit1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(400, 150);
            this.Name = "MsgBox";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MsgBox";
            ((System.ComponentModel.ISupportInitialize)(this.messagePanel)).EndInit();
            this.messagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.msgIcon.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonPanel)).EndInit();
            this.buttonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSuppressPanel)).EndInit();
            this.pnlSuppressPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkSuppress.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl messagePanel;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.PictureEdit msgIcon;
        private DevExpress.XtraEditors.PanelControl buttonPanel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnYes;
        private DevExpress.XtraEditors.SimpleButton btnNo;
        private DevExpress.XtraEditors.SimpleButton btnAbort;
        private DevExpress.XtraEditors.SimpleButton btnRetry;
        private DevExpress.XtraEditors.LabelControl lblMessage;
        private DevExpress.XtraEditors.SimpleButton btnIgnore;
        private DevExpress.XtraEditors.LabelControl lblCaption;
        private DevExpress.XtraEditors.PanelControl pnlSuppressPanel;
        private DevExpress.XtraEditors.CheckEdit chkSuppress;
    }
}