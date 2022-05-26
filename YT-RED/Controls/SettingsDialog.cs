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
                MsgBox.Show(validate, "Invalid Settings", Buttons.OK, YT_RED.Controls.Icon.Exclamation);
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
            await Logging.Historian.CleanHistory(Logging.DownloadCategory.All);
            MsgBox.Show("Download History Cleared", FormStartPosition.CenterParent);
        }

        private async void bbiDelAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult res = MsgBox.Show("Files for all recorded downloads will be deleted\nand download history will be reset.\n\nContinue?", "Delete Downloaded Files", YT_RED.Controls.Buttons.OKCancel, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
            if (res == DialogResult.OK)
            {
                await Logging.Historian.CleanHistory(Logging.DownloadCategory.All, Logging.DownloadCategory.All);
                MsgBox.Show("Downloads Cleared", FormStartPosition.CenterParent);
            }
        }

        private async void bbiDelAudio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult res = MsgBox.Show("Files for all recorded Audio downloads will be deleted\nand Audio download logs will be removed.\n\nContinue?", "Delete Downloaded Files", YT_RED.Controls.Buttons.OKCancel, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
            if (res == DialogResult.OK)
            {
                await Logging.Historian.CleanHistory(Logging.DownloadCategory.Audio, Logging.DownloadCategory.Audio);
                MsgBox.Show("Downloads Cleared", FormStartPosition.CenterParent);
            }
        }

        private async void bbiDelVideo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult res = MsgBox.Show("Files for all recorded Video downloads will be deleted\nand Video download logs will be removed.\n\nContinue?", "Delete Downloaded Files", YT_RED.Controls.Buttons.OKCancel, YT_RED.Controls.Icon.Warning, FormStartPosition.CenterParent);
            if (res == DialogResult.OK)
            {
                await Logging.Historian.CleanHistory(Logging.DownloadCategory.Video, Logging.DownloadCategory.Video);
                MsgBox.Show("Downloads Cleared", FormStartPosition.CenterParent);
            }
        }

        private void ddDeleteDLs_Click(object sender, EventArgs e)
        {
            ddDeleteDLs.ShowDropDown();
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