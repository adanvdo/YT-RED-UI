namespace YT_RED.Controls
{
    partial class CropForm
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
            this.pnlMainPanel = new DevExpress.XtraEditors.PanelControl();
            this.pnlCropView = new DevExpress.XtraEditors.PanelControl();
            this.pePictureEdit = new DevExpress.XtraEditors.PictureEdit();
            this.pnlControlBar = new DevExpress.XtraEditors.PanelControl();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMainPanel)).BeginInit();
            this.pnlMainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCropView)).BeginInit();
            this.pnlCropView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pePictureEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlControlBar)).BeginInit();
            this.pnlControlBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMainPanel
            // 
            this.pnlMainPanel.Controls.Add(this.pnlCropView);
            this.pnlMainPanel.Controls.Add(this.pnlControlBar);
            this.pnlMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainPanel.Location = new System.Drawing.Point(0, 0);
            this.pnlMainPanel.Name = "pnlMainPanel";
            this.pnlMainPanel.Size = new System.Drawing.Size(595, 390);
            this.pnlMainPanel.TabIndex = 0;
            // 
            // pnlCropView
            // 
            this.pnlCropView.Controls.Add(this.pePictureEdit);
            this.pnlCropView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCropView.Location = new System.Drawing.Point(2, 2);
            this.pnlCropView.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCropView.Name = "pnlCropView";
            this.pnlCropView.Size = new System.Drawing.Size(591, 356);
            this.pnlCropView.TabIndex = 1;
            // 
            // pePictureEdit
            // 
            this.pePictureEdit.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pePictureEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pePictureEdit.Location = new System.Drawing.Point(2, 2);
            this.pePictureEdit.Name = "pePictureEdit";
            this.pePictureEdit.Properties.AllowFocused = false;
            this.pePictureEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
            this.pePictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pePictureEdit.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pePictureEdit.Properties.ShowEditMenuItem = DevExpress.Utils.DefaultBoolean.False;
            this.pePictureEdit.Properties.ShowMenu = false;
            this.pePictureEdit.Properties.ShowScrollBars = true;
            this.pePictureEdit.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
            this.pePictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.pePictureEdit.Size = new System.Drawing.Size(587, 352);
            this.pePictureEdit.TabIndex = 0;
            this.pePictureEdit.Paint += new System.Windows.Forms.PaintEventHandler(this.pePictureEdit_Paint);
            this.pePictureEdit.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pePictureEdit_MouseDown);
            this.pePictureEdit.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pePictureEdit_MouseMove);
            this.pePictureEdit.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pePictureEdit_MouseUp);
            // 
            // pnlControlBar
            // 
            this.pnlControlBar.Controls.Add(this.btnOk);
            this.pnlControlBar.Controls.Add(this.btnCancel);
            this.pnlControlBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControlBar.Location = new System.Drawing.Point(2, 358);
            this.pnlControlBar.Margin = new System.Windows.Forms.Padding(0);
            this.pnlControlBar.Name = "pnlControlBar";
            this.pnlControlBar.Size = new System.Drawing.Size(591, 30);
            this.pnlControlBar.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOk.Location = new System.Drawing.Point(439, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnOk.Size = new System.Drawing.Size(75, 26);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(514, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CropForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 390);
            this.ControlBox = false;
            this.Controls.Add(this.pnlMainPanel);
            this.IconOptions.ShowIcon = false;
            this.Name = "CropForm";
            this.Text = "Crop Media";
            ((System.ComponentModel.ISupportInitialize)(this.pnlMainPanel)).EndInit();
            this.pnlMainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlCropView)).EndInit();
            this.pnlCropView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pePictureEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlControlBar)).EndInit();
            this.pnlControlBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlMainPanel;
        private DevExpress.XtraEditors.PanelControl pnlCropView;
        private DevExpress.XtraEditors.PanelControl pnlControlBar;
        private DevExpress.XtraEditors.PictureEdit pePictureEdit;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}