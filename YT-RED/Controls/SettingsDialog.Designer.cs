namespace YT_RED.Controls
{
    partial class SettingsDialog
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
            this.components = new System.ComponentModel.Container();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnClearHistory = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.tcSettingsTabControl = new DevExpress.XtraTab.XtraTabControl();
            this.settingsPopup = new DevExpress.XtraBars.PopupMenu(this.components);
            this.settingsBarManager = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.bbiDelVideo = new DevExpress.XtraBars.BarButtonItem();
            this.bbiDelAudio = new DevExpress.XtraBars.BarButtonItem();
            this.bbiDelAll = new DevExpress.XtraBars.BarButtonItem();
            this.ddDeleteDLs = new DevExpress.XtraEditors.DropDownButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcSettingsTabControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsPopup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsBarManager)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.ddDeleteDLs);
            this.panelControl1.Controls.Add(this.btnClearHistory);
            this.panelControl1.Controls.Add(this.btnSave);
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 391);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(509, 30);
            this.panelControl1.TabIndex = 2;
            // 
            // btnClearHistory
            // 
            this.btnClearHistory.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClearHistory.Location = new System.Drawing.Point(282, 2);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new System.Drawing.Size(75, 26);
            this.btnClearHistory.TabIndex = 2;
            this.btnClearHistory.Text = "Clear History";
            this.btnClearHistory.Click += new System.EventHandler(this.btnClearHistory_Click);
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.Location = new System.Drawing.Point(357, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 26);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "OK";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(432, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tcSettingsTabControl
            // 
            this.tcSettingsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcSettingsTabControl.Location = new System.Drawing.Point(0, 0);
            this.tcSettingsTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.tcSettingsTabControl.Name = "tcSettingsTabControl";
            this.tcSettingsTabControl.Size = new System.Drawing.Size(509, 391);
            this.tcSettingsTabControl.TabIndex = 3;
            // 
            // settingsPopup
            // 
            this.settingsPopup.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiDelVideo),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiDelAudio),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbiDelAll)});
            this.settingsPopup.Manager = this.settingsBarManager;
            this.settingsPopup.Name = "settingsPopup";
            // 
            // settingsBarManager
            // 
            this.settingsBarManager.DockControls.Add(this.barDockControlTop);
            this.settingsBarManager.DockControls.Add(this.barDockControlBottom);
            this.settingsBarManager.DockControls.Add(this.barDockControlLeft);
            this.settingsBarManager.DockControls.Add(this.barDockControlRight);
            this.settingsBarManager.Form = this;
            this.settingsBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbiDelVideo,
            this.bbiDelAudio,
            this.bbiDelAll});
            this.settingsBarManager.MaxItemId = 3;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.settingsBarManager;
            this.barDockControlTop.Size = new System.Drawing.Size(509, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 421);
            this.barDockControlBottom.Manager = this.settingsBarManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(509, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.settingsBarManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 421);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(509, 0);
            this.barDockControlRight.Manager = this.settingsBarManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 421);
            // 
            // bbiDelVideo
            // 
            this.bbiDelVideo.Caption = "Delete Video Downloads";
            this.bbiDelVideo.Id = 0;
            this.bbiDelVideo.Name = "bbiDelVideo";
            this.bbiDelVideo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiDelVideo_ItemClick);
            // 
            // bbiDelAudio
            // 
            this.bbiDelAudio.Caption = "Delete Audio Downloads";
            this.bbiDelAudio.Id = 1;
            this.bbiDelAudio.Name = "bbiDelAudio";
            this.bbiDelAudio.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiDelAudio_ItemClick);
            // 
            // bbiDelAll
            // 
            this.bbiDelAll.Caption = "Delete All Downloads";
            this.bbiDelAll.Id = 2;
            this.bbiDelAll.Name = "bbiDelAll";
            this.bbiDelAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiDelAll_ItemClick);
            // 
            // ddDeleteDLs
            // 
            this.ddDeleteDLs.AllowFocus = false;
            this.ddDeleteDLs.Dock = System.Windows.Forms.DockStyle.Right;
            this.ddDeleteDLs.DropDownControl = this.settingsPopup;
            this.ddDeleteDLs.Location = new System.Drawing.Point(147, 2);
            this.ddDeleteDLs.MenuManager = this.settingsBarManager;
            this.ddDeleteDLs.Name = "ddDeleteDLs";
            this.ddDeleteDLs.Size = new System.Drawing.Size(135, 26);
            this.ddDeleteDLs.TabIndex = 4;
            this.ddDeleteDLs.Text = "Delete Downloads";
            this.ddDeleteDLs.Click += new System.EventHandler(this.ddDeleteDLs_Click);
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 421);
            this.ControlBox = false;
            this.Controls.Add(this.tcSettingsTabControl);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tcSettingsTabControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsPopup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsBarManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraTab.XtraTabControl tcSettingsTabControl;
        private DevExpress.XtraEditors.SimpleButton btnClearHistory;
        private DevExpress.XtraBars.PopupMenu settingsPopup;
        private DevExpress.XtraEditors.DropDownButton ddDeleteDLs;
        private DevExpress.XtraBars.BarButtonItem bbiDelVideo;
        private DevExpress.XtraBars.BarButtonItem bbiDelAudio;
        private DevExpress.XtraBars.BarButtonItem bbiDelAll;
        private DevExpress.XtraBars.BarManager settingsBarManager;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}