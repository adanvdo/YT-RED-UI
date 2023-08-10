namespace YT_RED.Controls
{
    partial class YTRErrorMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YTRErrorMessageBox));
            this.pnlButtons = new DevExpress.XtraEditors.PanelControl();
            this.btnUpload = new DevExpress.XtraEditors.SimpleButton();
            this.btnRetry = new DevExpress.XtraEditors.SimpleButton();
            this.btnYes = new DevExpress.XtraEditors.SimpleButton();
            this.btnNo = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.memoMessage = new DevExpress.XtraEditors.MemoEdit();
            this.uploadMarquee = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.lblIndicator = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoMessage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uploadMarquee.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnUpload);
            this.pnlButtons.Controls.Add(this.btnRetry);
            this.pnlButtons.Controls.Add(this.btnYes);
            this.pnlButtons.Controls.Add(this.btnNo);
            this.pnlButtons.Controls.Add(this.btnOk);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 129);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(383, 30);
            this.pnlButtons.TabIndex = 0;
            // 
            // btnUpload
            // 
            this.btnUpload.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnUpload.Location = new System.Drawing.Point(-69, 2);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnUpload.Size = new System.Drawing.Size(75, 26);
            this.btnUpload.TabIndex = 5;
            this.btnUpload.Text = "Upload Logs";
            this.btnUpload.Visible = false;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnRetry
            // 
            this.btnRetry.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRetry.Location = new System.Drawing.Point(6, 2);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnRetry.Size = new System.Drawing.Size(75, 26);
            this.btnRetry.TabIndex = 4;
            this.btnRetry.Text = "Retry";
            this.btnRetry.Visible = false;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnYes
            // 
            this.btnYes.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnYes.Location = new System.Drawing.Point(81, 2);
            this.btnYes.Name = "btnYes";
            this.btnYes.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnYes.Size = new System.Drawing.Size(75, 26);
            this.btnYes.TabIndex = 3;
            this.btnYes.Text = "Yes";
            this.btnYes.Visible = false;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnNo.Location = new System.Drawing.Point(156, 2);
            this.btnNo.Name = "btnNo";
            this.btnNo.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnNo.Size = new System.Drawing.Size(75, 26);
            this.btnNo.TabIndex = 2;
            this.btnNo.Text = "No";
            this.btnNo.Visible = false;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnOk
            // 
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOk.Location = new System.Drawing.Point(231, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnOk.Size = new System.Drawing.Size(75, 26);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.Visible = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(306, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // memoMessage
            // 
            this.memoMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoMessage.Location = new System.Drawing.Point(0, 0);
            this.memoMessage.Name = "memoMessage";
            this.memoMessage.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.memoMessage.Properties.Appearance.Options.UseFont = true;
            this.memoMessage.Properties.Appearance.Options.UseTextOptions = true;
            this.memoMessage.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.memoMessage.Properties.Padding = new System.Windows.Forms.Padding(5, 10, 5, 10);
            this.memoMessage.Properties.ReadOnly = true;
            this.memoMessage.Properties.UseReadOnlyAppearance = false;
            this.memoMessage.Size = new System.Drawing.Size(383, 91);
            this.memoMessage.TabIndex = 1;
            this.memoMessage.EditValueChanged += new System.EventHandler(this.memoMessage_EditValueChanged);
            this.memoMessage.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.memoMessage_EditValueChanging);
            // 
            // uploadMarquee
            // 
            this.uploadMarquee.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.uploadMarquee.EditValue = 0;
            this.uploadMarquee.Location = new System.Drawing.Point(0, 91);
            this.uploadMarquee.Name = "uploadMarquee";
            this.uploadMarquee.Properties.AllowFocused = false;
            this.uploadMarquee.Properties.ShowTitle = true;
            this.uploadMarquee.Size = new System.Drawing.Size(383, 18);
            this.uploadMarquee.TabIndex = 2;
            this.uploadMarquee.Visible = false;
            // 
            // lblIndicator
            // 
            this.lblIndicator.Appearance.Options.UseTextOptions = true;
            this.lblIndicator.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblIndicator.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblIndicator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblIndicator.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblIndicator.Location = new System.Drawing.Point(0, 109);
            this.lblIndicator.Name = "lblIndicator";
            this.lblIndicator.Size = new System.Drawing.Size(383, 20);
            this.lblIndicator.TabIndex = 3;
            this.lblIndicator.Visible = false;
            // 
            // YTRErrorMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(383, 159);
            this.ControlBox = false;
            this.Controls.Add(this.memoMessage);
            this.Controls.Add(this.uploadMarquee);
            this.Controls.Add(this.lblIndicator);
            this.Controls.Add(this.pnlButtons);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("YTRErrorMessageBox.IconOptions.Icon")));
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("YTRErrorMessageBox.IconOptions.Image")));
            this.Name = "YTRErrorMessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ErrorMessageBox";
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoMessage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uploadMarquee.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlButtons;
        private DevExpress.XtraEditors.MemoEdit memoMessage;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnYes;
        private DevExpress.XtraEditors.SimpleButton btnNo;
        private DevExpress.XtraEditors.SimpleButton btnRetry;
        private DevExpress.XtraEditors.SimpleButton btnUpload;
        private DevExpress.XtraEditors.MarqueeProgressBarControl uploadMarquee;
        private DevExpress.XtraEditors.LabelControl lblIndicator;
    }
}