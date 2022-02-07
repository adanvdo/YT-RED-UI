namespace YT_RED.Controls
{
    partial class PropertyGrid
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
            this.pgPropertyGridPanel = new DevExpress.XtraVerticalGrid.PGPanel();
            this.pgcPropertyGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            this.pgcDescription = new DevExpress.XtraVerticalGrid.PropertyDescriptionControl();
            ((System.ComponentModel.ISupportInitialize)(this.pgPropertyGridPanel)).BeginInit();
            this.pgPropertyGridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pgcPropertyGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // pgPropertyGridPanel
            // 
            this.pgPropertyGridPanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 1F)});
            this.pgPropertyGridPanel.Controls.Add(this.pgcPropertyGrid);
            this.pgPropertyGridPanel.Controls.Add(this.pgcDescription);
            this.pgPropertyGridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgPropertyGridPanel.Location = new System.Drawing.Point(0, 0);
            this.pgPropertyGridPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pgPropertyGridPanel.Name = "pgPropertyGridPanel";
            this.pgPropertyGridPanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 1F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 1F)});
            this.pgPropertyGridPanel.Size = new System.Drawing.Size(540, 462);
            this.pgPropertyGridPanel.TabIndex = 1;
            // 
            // pgcPropertyGrid
            // 
            this.pgPropertyGridPanel.SetColumn(this.pgcPropertyGrid, 0);
            this.pgcPropertyGrid.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgcPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgcPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.pgcPropertyGrid.Name = "pgcPropertyGrid";
            this.pgcPropertyGrid.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            this.pgPropertyGridPanel.SetRow(this.pgcPropertyGrid, 0);
            this.pgcPropertyGrid.Size = new System.Drawing.Size(534, 350);
            this.pgcPropertyGrid.TabIndex = 0;
            // 
            // pgcDescription
            // 
            this.pgcDescription.AutoHeight = true;
            this.pgPropertyGridPanel.SetColumn(this.pgcDescription, 0);
            this.pgcDescription.Location = new System.Drawing.Point(3, 359);
            this.pgcDescription.Name = "pgcDescription";
            this.pgcDescription.PropertyGrid = this.pgcPropertyGrid;
            this.pgPropertyGridPanel.SetRow(this.pgcDescription, 1);
            this.pgcDescription.Size = new System.Drawing.Size(534, 100);
            this.pgcDescription.TabIndex = 1;
            // 
            // PropertyGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pgPropertyGridPanel);
            this.Name = "PropertyGrid";
            this.Size = new System.Drawing.Size(540, 462);
            ((System.ComponentModel.ISupportInitialize)(this.pgPropertyGridPanel)).EndInit();
            this.pgPropertyGridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pgcPropertyGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraVerticalGrid.PGPanel pgPropertyGridPanel;
        protected DevExpress.XtraVerticalGrid.PropertyGridControl pgcPropertyGrid;
        protected DevExpress.XtraVerticalGrid.PropertyDescriptionControl pgcDescription;
    }
}
