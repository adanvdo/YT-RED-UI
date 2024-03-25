namespace YTR.Controls
{
    partial class ReleaseItem
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
            this.btnDownload64 = new DevExpress.XtraEditors.SimpleButton();
            this.lblVersionX64 = new DevExpress.XtraEditors.LabelControl();
            this.pnlX64 = new DevExpress.XtraEditors.PanelControl();
            this.pnlX86 = new DevExpress.XtraEditors.PanelControl();
            this.lblVersionX86 = new DevExpress.XtraEditors.LabelControl();
            this.btnDownload86 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pnlX64)).BeginInit();
            this.pnlX64.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlX86)).BeginInit();
            this.pnlX86.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDownload64
            // 
            this.btnDownload64.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDownload64.Location = new System.Drawing.Point(141, 2);
            this.btnDownload64.Margin = new System.Windows.Forms.Padding(0);
            this.btnDownload64.Name = "btnDownload64";
            this.btnDownload64.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnDownload64.Size = new System.Drawing.Size(82, 21);
            this.btnDownload64.TabIndex = 0;
            this.btnDownload64.Text = "Download";
            this.btnDownload64.Click += new System.EventHandler(this.btnDownload64_Click);
            // 
            // lblVersionX64
            // 
            this.lblVersionX64.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblVersionX64.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblVersionX64.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVersionX64.Location = new System.Drawing.Point(2, 2);
            this.lblVersionX64.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersionX64.Name = "lblVersionX64";
            this.lblVersionX64.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblVersionX64.Size = new System.Drawing.Size(139, 21);
            this.lblVersionX64.TabIndex = 1;
            // 
            // pnlX64
            // 
            this.pnlX64.Controls.Add(this.lblVersionX64);
            this.pnlX64.Controls.Add(this.btnDownload64);
            this.pnlX64.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlX64.Location = new System.Drawing.Point(0, 0);
            this.pnlX64.Margin = new System.Windows.Forms.Padding(0);
            this.pnlX64.MaximumSize = new System.Drawing.Size(0, 25);
            this.pnlX64.MinimumSize = new System.Drawing.Size(225, 25);
            this.pnlX64.Name = "pnlX64";
            this.pnlX64.Size = new System.Drawing.Size(225, 25);
            this.pnlX64.TabIndex = 2;
            // 
            // pnlX86
            // 
            this.pnlX86.Controls.Add(this.lblVersionX86);
            this.pnlX86.Controls.Add(this.btnDownload86);
            this.pnlX86.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlX86.Location = new System.Drawing.Point(0, 25);
            this.pnlX86.Margin = new System.Windows.Forms.Padding(0);
            this.pnlX86.MaximumSize = new System.Drawing.Size(0, 25);
            this.pnlX86.MinimumSize = new System.Drawing.Size(225, 25);
            this.pnlX86.Name = "pnlX86";
            this.pnlX86.Size = new System.Drawing.Size(225, 25);
            this.pnlX86.TabIndex = 3;
            // 
            // lblVersionX86
            // 
            this.lblVersionX86.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblVersionX86.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblVersionX86.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVersionX86.Location = new System.Drawing.Point(2, 2);
            this.lblVersionX86.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersionX86.Name = "lblVersionX86";
            this.lblVersionX86.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblVersionX86.Size = new System.Drawing.Size(139, 21);
            this.lblVersionX86.TabIndex = 1;
            // 
            // btnDownload86
            // 
            this.btnDownload86.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDownload86.Location = new System.Drawing.Point(141, 2);
            this.btnDownload86.Margin = new System.Windows.Forms.Padding(0);
            this.btnDownload86.Name = "btnDownload86";
            this.btnDownload86.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnDownload86.Size = new System.Drawing.Size(82, 21);
            this.btnDownload86.TabIndex = 0;
            this.btnDownload86.Text = "Download";
            this.btnDownload86.Click += new System.EventHandler(this.btnDownload86_Click);
            // 
            // ReleaseItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.pnlX86);
            this.Controls.Add(this.pnlX64);
            this.MinimumSize = new System.Drawing.Size(225, 25);
            this.Name = "ReleaseItem";
            this.Size = new System.Drawing.Size(225, 50);
            ((System.ComponentModel.ISupportInitialize)(this.pnlX64)).EndInit();
            this.pnlX64.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlX86)).EndInit();
            this.pnlX86.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnDownload64;
        private DevExpress.XtraEditors.LabelControl lblVersionX64;
        private DevExpress.XtraEditors.PanelControl pnlX64;
        private DevExpress.XtraEditors.PanelControl pnlX86;
        private DevExpress.XtraEditors.LabelControl lblVersionX86;
        private DevExpress.XtraEditors.SimpleButton btnDownload86;
    }
}
