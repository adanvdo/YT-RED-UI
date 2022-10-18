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
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            this.pgPropertyGridPanel = new DevExpress.XtraVerticalGrid.PGPanel();
            this.pgcPropertyGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            this.repButtonEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repPopup = new DevExpress.XtraEditors.Repository.RepositoryItemPopupContainerEdit();
            this.pgcDescription = new DevExpress.XtraVerticalGrid.PropertyDescriptionControl();
            ((System.ComponentModel.ISupportInitialize)(this.pgPropertyGridPanel)).BeginInit();
            this.pgPropertyGridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pgcPropertyGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repButtonEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPopup)).BeginInit();
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
            this.pgPropertyGridPanel.Size = new System.Drawing.Size(673, 483);
            this.pgPropertyGridPanel.TabIndex = 1;
            // 
            // pgcPropertyGrid
            // 
            this.pgPropertyGridPanel.SetColumn(this.pgcPropertyGrid, 0);
            this.pgcPropertyGrid.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgcPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgcPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.pgcPropertyGrid.Name = "pgcPropertyGrid";
            this.pgcPropertyGrid.OptionsBehavior.ResizeHeaderPanel = false;
            this.pgcPropertyGrid.OptionsBehavior.ResizeRowHeaders = false;
            this.pgcPropertyGrid.OptionsBehavior.ResizeRowValues = false;
            this.pgcPropertyGrid.OptionsView.AllowReadOnlyRowAppearance = DevExpress.Utils.DefaultBoolean.True;
            this.pgcPropertyGrid.OptionsView.AllowRowHeaderReadOnlyAppearance = DevExpress.Utils.DefaultBoolean.False;
            this.pgcPropertyGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repButtonEdit,
            this.repPopup});
            this.pgPropertyGridPanel.SetRow(this.pgcPropertyGrid, 0);
            this.pgcPropertyGrid.ShowButtonMode = DevExpress.XtraVerticalGrid.ShowButtonModeEnum.ShowAlways;
            this.pgcPropertyGrid.Size = new System.Drawing.Size(667, 408);
            this.pgcPropertyGrid.TabIndex = 0;
            this.pgcPropertyGrid.UseDisabledStatePainter = false;
            this.pgcPropertyGrid.CustomPropertyDescriptors += new DevExpress.XtraVerticalGrid.Events.CustomPropertyDescriptorsEventHandler(this.pgcPropertyGrid_CustomPropertyDescriptors);
            this.pgcPropertyGrid.CustomRecordCellEdit += new DevExpress.XtraVerticalGrid.Events.GetCustomRowCellEditEventHandler(this.pgcPropertyGrid_CustomRecordCellEdit);
            // 
            // repButtonEdit
            // 
            this.repButtonEdit.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.repButtonEdit.AutoHeight = false;
            editorButtonImageOptions2.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            editorButtonImageOptions2.SvgImage = global::YT_RED.Properties.Resources.loading_1;
            editorButtonImageOptions2.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
            editorButtonImageOptions2.SvgImageSize = new System.Drawing.Size(20, 20);
            editorButtonImageOptions3.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            editorButtonImageOptions3.Location = DevExpress.XtraEditors.ImageLocation.Default;
            editorButtonImageOptions3.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.Full;
            this.repButtonEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Check for Update", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "Downloading..", -1, true, false, false, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.repButtonEdit.ContextImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.repButtonEdit.Name = "repButtonEdit";
            this.repButtonEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.repButtonEdit.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repButtonEdit_ButtonPressed);
            // 
            // repPopup
            // 
            this.repPopup.AutoHeight = false;
            this.repPopup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)});
            this.repPopup.Name = "repPopup";
            this.repPopup.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.repPopup_CustomDisplayText);
            // 
            // pgcDescription
            // 
            this.pgcDescription.AutoHeight = true;
            this.pgPropertyGridPanel.SetColumn(this.pgcDescription, 0);
            this.pgcDescription.Location = new System.Drawing.Point(3, 417);
            this.pgcDescription.Name = "pgcDescription";
            this.pgcDescription.PropertyGrid = this.pgcPropertyGrid;
            this.pgPropertyGridPanel.SetRow(this.pgcDescription, 1);
            this.pgcDescription.Size = new System.Drawing.Size(667, 63);
            this.pgcDescription.TabIndex = 1;
            // 
            // PropertyGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pgPropertyGridPanel);
            this.Name = "PropertyGrid";
            this.Size = new System.Drawing.Size(673, 483);
            ((System.ComponentModel.ISupportInitialize)(this.pgPropertyGridPanel)).EndInit();
            this.pgPropertyGridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pgcPropertyGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repButtonEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repPopup)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraVerticalGrid.PGPanel pgPropertyGridPanel;
        protected DevExpress.XtraVerticalGrid.PropertyGridControl pgcPropertyGrid;
        protected DevExpress.XtraVerticalGrid.PropertyDescriptionControl pgcDescription;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repButtonEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemPopupContainerEdit repPopup;
    }
}
