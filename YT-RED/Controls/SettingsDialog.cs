using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YT_RED.Settings;

namespace YT_RED.Controls
{
    public partial class SettingsDialog : DevExpress.XtraEditors.XtraForm
    {
        public SettingsDialog()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            createFeatureOptionsPages();
            this.tcSettingsTabControl.SelectedTabPage = this.tcSettingsTabControl.TabPages[0];
        }
        private void createFeatureOptionsPages()
        {
            foreach (var setting in Settings.AppSettings.Default.AllSettings)
            {
                var propertyGrid = new PropertyGrid();
                propertyGrid.Dock = DockStyle.Fill;
                propertyGrid.Location = new Point(0, 0);
                propertyGrid.Name = $"pg{setting.Feature}";
                propertyGrid.TabIndex = 99;
                propertyGrid.GridTabIndex = 1;

                var tabPage = new DevExpress.XtraTab.XtraTabPage();
                tabPage.Controls.Add(propertyGrid);
                tabPage.Name = $"tpg{setting.Feature}";
                tabPage.Text = setting.Feature.ToFriendlyString().Replace("&", "&&");
                if (setting.Feature == AppFeature.About)
                {
                    propertyGrid.Grid.OptionsBehavior.Editable = false;                    
                }

                this.tcSettingsTabControl.TabPages.Add(tabPage);

                propertyGrid.SelectedObject = setting;
            }
        }

        private async void saveSettings()
        {
            string validate = await AppSettings.Default.General.ValidateSettings();
            if(!string.IsNullOrEmpty(validate))
            {
                MessageBox.Show(validate, "Invalid Settings", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            AppSettings.Default.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        private async void btnClearHistory_Click(object sender, EventArgs e)
        {
            await Logging.Historian.CleanHistory(true);
            MessageBox.Show("Download History Cleared");
        }

        private async void btnDeleteDownloads_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Files for all recorded downloads will be deleted\nand download history will be reset.\n\nContinue?", "Delete Downloaded Files", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (res == DialogResult.OK)
            {
                await Logging.Historian.CleanHistory(true, true);
            }
            MessageBox.Show("Downloads Cleared");
        }
    }

    public enum OpenPosition
    {
        Center = 0,
        BottomRight = 1,
        BottomLeft = 2,
        TopRight = 3,
        TopLeft = 4,
        Unspecified = 5
    }
}