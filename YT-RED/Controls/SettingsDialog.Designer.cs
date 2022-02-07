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
            this.propertyGridPanel = new DevExpress.XtraVerticalGrid.PGPanel();
            this.pgSettingsGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            this.propertyDescription = new DevExpress.XtraVerticalGrid.PropertyDescriptionControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.propertyGridPanel)).BeginInit();
            this.propertyGridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pgSettingsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGridPanel
            // 
            this.propertyGridPanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 1F)});
            this.propertyGridPanel.Controls.Add(this.pgSettingsGrid);
            this.propertyGridPanel.Controls.Add(this.propertyDescription);
            this.propertyGridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridPanel.Location = new System.Drawing.Point(0, 0);
            this.propertyGridPanel.Name = "propertyGridPanel";
            this.propertyGridPanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 1F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 1F)});
            this.propertyGridPanel.Size = new System.Drawing.Size(509, 391);
            this.propertyGridPanel.TabIndex = 1;
            // 
            // pgSettingsGrid
            // 
            this.propertyGridPanel.SetColumn(this.pgSettingsGrid, 0);
            this.pgSettingsGrid.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgSettingsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgSettingsGrid.Location = new System.Drawing.Point(3, 3);
            this.pgSettingsGrid.Name = "pgSettingsGrid";
            this.pgSettingsGrid.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            this.propertyGridPanel.SetRow(this.pgSettingsGrid, 0);
            this.pgSettingsGrid.Size = new System.Drawing.Size(503, 279);
            this.pgSettingsGrid.TabIndex = 0;
            // 
            // propertyDescription
            // 
            this.propertyDescription.AutoHeight = true;
            this.propertyGridPanel.SetColumn(this.propertyDescription, 0);
            this.propertyDescription.Location = new System.Drawing.Point(3, 288);
            this.propertyDescription.Name = "propertyDescription";
            this.propertyDescription.PropertyGrid = this.pgSettingsGrid;
            this.propertyGridPanel.SetRow(this.propertyDescription, 1);
            this.propertyDescription.Size = new System.Drawing.Size(503, 100);
            this.propertyDescription.TabIndex = 1;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnSave);
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 391);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(509, 30);
            this.panelControl1.TabIndex = 2;
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
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 421);
            this.ControlBox = false;
            this.Controls.Add(this.propertyGridPanel);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.propertyGridPanel)).EndInit();
            this.propertyGridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pgSettingsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraVerticalGrid.PGPanel propertyGridPanel;
        private DevExpress.XtraVerticalGrid.PropertyGridControl pgSettingsGrid;
        private DevExpress.XtraVerticalGrid.PropertyDescriptionControl propertyDescription;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}