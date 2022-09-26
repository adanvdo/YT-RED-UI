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
        private bool loading = true;
        public SettingsDialog(string selectPage = "")
        {
            InitializeComponent();
            Init();
            if (!string.IsNullOrEmpty(selectPage))
            {
                var page = this.tcSettingsTabControl.TabPages.Where(tpg => tpg.Name == $"tpg{selectPage}").ToArray()[0];
                if(page != null) 
                    this.tcSettingsTabControl.SelectedTabPage = page;
            }
        }

        private async void Init()
        {
            createFeatureOptionsPages();
            DevExpress.XtraTab.XtraTabPage active = this.tcSettingsTabControl.TabPages.FirstOrDefault(tpg => tpg.Name == $"tpg{AppSettings.Default.General.ActiveFeatureTab}");
            this.tcSettingsTabControl.SelectedTabPage = active;
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
                if (setting.Feature == AppFeature.About)
                {
                    propertyGrid.Grid.OptionsView.ShowFocusedFrame = false;
                    propertyGrid.Grid.OptionsSelectionAndFocus.EnableAppearanceFocusedRow = false;
                }

                var tabPage = new DevExpress.XtraTab.XtraTabPage();
                tabPage.Controls.Add(propertyGrid);
                tabPage.Name = $"tpg{setting.Feature}";
                tabPage.Text = setting.Feature.ToFriendlyString().Replace("&", "&&");

                this.tcSettingsTabControl.TabPages.Add(tabPage);

                propertyGrid.SelectedObject = setting;
            }

            loading = false;
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
            if (this.tcSettingsTabControl.SelectedTabPage.Name == $"tpg{AppFeature.About}")
            {
                PropertyGrid pg = (PropertyGrid)this.tcSettingsTabControl.SelectedTabPage.Controls.Find($"pg{AppFeature.About}", true).FirstOrDefault();
                if (pg.IsBusy)
                {
                    MsgBox.Show("A task is in progress. Please wait.");
                    return;
                }
            }
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

        private void tcSettingsTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (loading || e.Page == null) return;
            bool parseFeature = Enum.TryParse(e.Page.Name.Remove(0, 3), out AppFeature featureTab);
            if (parseFeature)
            {
                AppSettings.Default.General.ActiveFeatureTab = featureTab;
            }
            else AppSettings.Default.General.ActiveFeatureTab = AppFeature.General;
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