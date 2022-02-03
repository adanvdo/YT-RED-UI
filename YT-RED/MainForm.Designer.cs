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
            this.tabFormControl1 = new DevExpress.XtraBars.TabFormControl();
            this.tabFormContentContainer1 = new DevExpress.XtraBars.TabFormContentContainer();
            this.tfpHome = new DevExpress.XtraBars.TabFormPage();
            this.tabFormContentContainer2 = new DevExpress.XtraBars.TabFormContentContainer();
            this.tfpYouTube = new DevExpress.XtraBars.TabFormPage();
            this.tabFormContentContainer3 = new DevExpress.XtraBars.TabFormContentContainer();
            this.tfpReddit = new DevExpress.XtraBars.TabFormPage();
            ((System.ComponentModel.ISupportInitialize)(this.tabFormControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabFormControl1
            // 
            this.tabFormControl1.Location = new System.Drawing.Point(0, 0);
            this.tabFormControl1.Name = "tabFormControl1";
            this.tabFormControl1.Pages.Add(this.tfpHome);
            this.tabFormControl1.Pages.Add(this.tfpYouTube);
            this.tabFormControl1.Pages.Add(this.tfpReddit);
            this.tabFormControl1.SelectedPage = this.tfpReddit;
            this.tabFormControl1.Size = new System.Drawing.Size(945, 50);
            this.tabFormControl1.TabForm = this;
            this.tabFormControl1.TabIndex = 0;
            this.tabFormControl1.TabStop = false;
            // 
            // tabFormContentContainer1
            // 
            this.tabFormContentContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFormContentContainer1.Location = new System.Drawing.Point(0, 50);
            this.tabFormContentContainer1.Name = "tabFormContentContainer1";
            this.tabFormContentContainer1.Size = new System.Drawing.Size(945, 529);
            this.tabFormContentContainer1.TabIndex = 1;
            // 
            // tfpHome
            // 
            this.tfpHome.ContentContainer = this.tabFormContentContainer1;
            this.tfpHome.Name = "tfpHome";
            this.tfpHome.Text = "Home";
            // 
            // tabFormContentContainer2
            // 
            this.tabFormContentContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFormContentContainer2.Location = new System.Drawing.Point(0, 50);
            this.tabFormContentContainer2.Name = "tabFormContentContainer2";
            this.tabFormContentContainer2.Size = new System.Drawing.Size(945, 529);
            this.tabFormContentContainer2.TabIndex = 2;
            // 
            // tfpYouTube
            // 
            this.tfpYouTube.ContentContainer = this.tabFormContentContainer2;
            this.tfpYouTube.Name = "tfpYouTube";
            this.tfpYouTube.Text = "YouTube";
            // 
            // tabFormContentContainer3
            // 
            this.tabFormContentContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabFormContentContainer3.Location = new System.Drawing.Point(0, 50);
            this.tabFormContentContainer3.Name = "tabFormContentContainer3";
            this.tabFormContentContainer3.Size = new System.Drawing.Size(945, 529);
            this.tabFormContentContainer3.TabIndex = 3;
            // 
            // tfpReddit
            // 
            this.tfpReddit.ContentContainer = this.tabFormContentContainer3;
            this.tfpReddit.Name = "tfpReddit";
            this.tfpReddit.Text = "Reddit";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 579);
            this.Controls.Add(this.tabFormContentContainer3);
            this.Controls.Add(this.tabFormControl1);
            this.Name = "MainForm";
            this.TabFormControl = this.tabFormControl1;
            this.Text = "YT-RED";
            ((System.ComponentModel.ISupportInitialize)(this.tabFormControl1)).EndInit();
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
    }
}

