using DevExpress.XtraEditors;
using DevExpress.XtraVerticalGrid;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using YT_RED.Settings;
using YT_RED.Logging;
using YT_RED.Utils;

namespace YT_RED.Controls
{
    public partial class PropertyGrid : DevExpress.XtraEditors.XtraUserControl
    {
        public bool IsBusy { get { return loadingUpdateInfo || downloadingUpdate; } }
        private bool loadingUpdateInfo = false;
        private bool pendingUpdateInfo = false;
        private bool downloadingUpdate = false;
        private Classes.Release pendingReleaseInfo = null;
        private Version currentVersion = null;
        private int indicatorStep = 0;

        private System.Windows.Forms.Timer indicatorTimer;

		public string GridName { get { return this.pgcPropertyGrid.Name; } set { this.pgcPropertyGrid.Name = value; } }

		public int GridTabIndex { get { return this.pgcPropertyGrid.TabIndex; } set { this.pgcPropertyGrid.TabIndex = value; } }

		public object SelectedObject { get { return this.pgcPropertyGrid.SelectedObject; } set { this.pgcPropertyGrid.SelectedObject = value; } }

		public PropertyGridControl Grid { get { return this.pgcPropertyGrid; } set { this.pgcPropertyGrid = value; } }

		public PropertyGrid()
		{
			InitializeComponent();
            indicatorTimer = new Timer();
            indicatorTimer.Interval = 250;
            indicatorTimer.Tick += IndicatorTimer_Tick;
            indicatorTimer.Enabled = true;
		}

        private void IndicatorTimer_Tick(object sender, EventArgs e)
        {
            repButtonEdit.Buttons[0].Visible = !loadingUpdateInfo && !downloadingUpdate;
            repButtonEdit.Buttons[1].Visible = loadingUpdateInfo;
            repButtonEdit.Buttons[2].Visible = downloadingUpdate;
            if (loadingUpdateInfo)
            {
                switch (indicatorStep)
                {
                    case 0:
                        repButtonEdit.Buttons[1].ImageOptions.SvgImage = Properties.Resources.loading_1;
                        indicatorStep = 1;
                        break;
                    case 1:
                        repButtonEdit.Buttons[1].ImageOptions.SvgImage = Properties.Resources.loading_2;
                        indicatorStep = 2;
                        break;
                    case 2:
                        repButtonEdit.Buttons[1].ImageOptions.SvgImage = Properties.Resources.loading_3;
                        indicatorStep = 3;
                        break;
                    case 3:
                        repButtonEdit.Buttons[1].ImageOptions.SvgImage = Properties.Resources.loading_4;
                        indicatorStep = 4;
                        break;
                    case 4:
                        repButtonEdit.Buttons[1].ImageOptions.SvgImage = Properties.Resources.loading_5;
                        indicatorStep = 5;
                        break;
                    case 5:
                        repButtonEdit.Buttons[1].ImageOptions.SvgImage = Properties.Resources.loading_6;
                        indicatorStep = 6;
                        break;
                    case 6:
                        repButtonEdit.Buttons[1].ImageOptions.SvgImage = Properties.Resources.loading_7;
                        indicatorStep = 7;
                        break;
                    case 7:
                        repButtonEdit.Buttons[1].ImageOptions.SvgImage = Properties.Resources.loading_8;
                        indicatorStep = 8;
                        break;
                    case 8:
                        repButtonEdit.Buttons[1].ImageOptions.SvgImage = Properties.Resources.loading_1;
                        indicatorStep = 1;
                        break;

                }
            }
        }

        private void pgcPropertyGrid_CustomPropertyDescriptors(object sender, DevExpress.XtraVerticalGrid.Events.CustomPropertyDescriptorsEventArgs e)
        {
            if(e.Properties.Find("FormatMode", false) != null)
            {
                this.pgcPropertyGrid.OptionsBehavior.PropertySort = DevExpress.XtraVerticalGrid.PropertySort.NoSort;
                e.Properties = e.Properties.Sort(new string[] { "InputPanelPosition", "ControlPanelPosition", "FormatMode" });
            }
            if (e.Properties.Find("VerboseOutput", false) != null)
            {
                this.pgcPropertyGrid.OptionsBehavior.PropertySort = DevExpress.XtraVerticalGrid.PropertySort.NoSort;
                e.Properties = e.Properties.Sort(new string[] { "Channel", "EnableHotKeys", "DownloadHotKey", "GetMissingMetadata", "AlwaysConvertToPreferredFormat",
                    "PreferredVideoFormat", "PreferredAudioFormat", "VerboseOutput" });
            }
			if(e.Properties.Find("Version", false) != null)
            {
				this.pgcPropertyGrid.OptionsBehavior.PropertySort = DevExpress.XtraVerticalGrid.PropertySort.NoSort;
				e.Properties = e.Properties.Sort(new string[] { "Version", "Build", "GitHub", "Contact" });
            } 
			if(e.Properties.Find("UseTitleAsFileName", false) != null)
            {
				this.pgcPropertyGrid.OptionsBehavior.PropertySort = DevExpress.XtraVerticalGrid.PropertySort.NoSort;
				e.Properties = e.Properties.Sort(new string[] { "UseTitleAsFileName", "AudioDownloadPath", "VideoDownloadPath", "MaxResolutionBest", "MaxFilesizeBest", "AutomaticallyOpenDownloadLocation", "EnableDownloadHistory", "HistoryAge", "ErrorLogPath" });
            }
        }

        private void pgcPropertyGrid_CustomRecordCellEdit(object sender, DevExpress.XtraVerticalGrid.Events.GetCustomRowCellEditEventArgs e)
        {
			Settings.FeatureSettings setting = pgcPropertyGrid.GetRecordObject(e.RecordIndex) as Settings.FeatureSettings;
			if(setting != null)
            {
				Settings.About aboutSettings = setting as Settings.About;
                if (aboutSettings != null)
                {
                    if (e.Row.Index == 0)
                    {
                        if (!downloadingUpdate && pendingUpdateInfo)
                        {
                            PopupContainerControl popup = new PopupContainerControl();
                            popup.Margin = new Padding(0);
                            ReleaseItem item = new ReleaseItem(pendingReleaseInfo);
                            item.Download64Click += Item_Download64Click;
                            item.Download86Click += Item_Download86Click;
                            item.Dock = DockStyle.Top;
                            popup.Controls.Add(item);
                            popup.Height = item.Height;
                            repPopup.PopupControl = popup;
                            e.RepositoryItem = repPopup;
                        }                        
                        else
                        {
                            e.RepositoryItem = repButtonEdit;
                        }
                    }
                    else
                    {
                        e.Row.Properties.AllowEdit = false;
                    }
                }
            }
        }

        private async void repButtonEdit_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {     
            if(!pendingUpdateInfo)
            {
                loadingUpdateInfo = true;
                repButtonEdit.Buttons[0].Visible = !loadingUpdateInfo;
                repButtonEdit.Buttons[1].Visible = loadingUpdateInfo;
                pendingReleaseInfo = await UpdateHelper.GetLatestRelease(Settings.AppSettings.Default.Advanced.Channel);
                if (pendingReleaseInfo != null)
                {
                    currentVersion = new Version(Settings.AppSettings.Default.About.Version);
                    if (pendingReleaseInfo.Version > currentVersion)
                    {   
                        pendingUpdateInfo = true;
                        pgcPropertyGrid.Refresh();
                    }
                    else
                    {
                        repButtonEdit.Buttons[0].Caption = "Up To Date!";
                    }
                }
                loadingUpdateInfo = false;
                if (!pendingUpdateInfo)
                {
                    await Task.Delay(3000);
                    repButtonEdit.Buttons[0].Caption = "Check for Update";
                }
            }
        }

        private void repPopup_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            e.DisplayText = $"{currentVersion}          Available: {pendingReleaseInfo.Version}-{pendingReleaseInfo.Channel}";
        }

        private async void Item_Download86Click(object sender, EventArgs e)
        {
            await performDownloadAsync(pendingReleaseInfo.x86Url);
        }

        private async void Item_Download64Click(object sender, EventArgs e)
        {
            await performDownloadAsync(pendingReleaseInfo.x64Url);
        }

        private async Task performDownloadAsync(string url)
        {
            ((PopupContainerEdit)pgcPropertyGrid.ActiveEditor).ClosePopup();
            pgcPropertyGrid.Focus();
            downloadingUpdate = true;
            pgcPropertyGrid.Refresh();
            string download = await UpdateHelper.DownloadReleaseAsync(url, new System.Net.DownloadProgressChangedEventHandler(progressChanged));
            if (download != null)
            {
                downloadComplete(download);
            }
        }

        private void downloadComplete(string location)
        {
            downloadingUpdate = false;
            pendingUpdateInfo = false;
            loadingUpdateInfo = false;
            repButtonEdit.Buttons[2].Caption = $"Downloading.. ";
            pgcPropertyGrid.Refresh();

            if (location == "cancelled")
                return;

            if (pendingReleaseInfo.ManualInstallRequired)
            {
                DialogResult res = MsgBox.Show("Update Package Downloaded, but a Manual Install is required.\nOpen Download Location?", "Download Complete", Buttons.YesNo, Icon.Question, FormStartPosition.CenterParent);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(location);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogException(ex);
                    }
                }
            }
            else
            {
                DialogResult res = MsgBox.Show("Update Package Downloaded\nInstall Update Now?", "Download Complete", Buttons.YesNo, Icon.Question, FormStartPosition.CenterParent);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        string args = $"-dir={Settings.AppSettings.Default.General.ExeDirectoryPath} -pkg={location} -skin={Settings.AppSettings.Default.General.ActiveSkin} -pal={AppSettings.Default.General.SkinPalette}";
                        if (pendingReleaseInfo.ReplaceUpdater)
                            args += " -updater";
                        string updaterProcess = System.IO.Path.Combine(Settings.AppSettings.Default.General.ExeDirectoryPath, "YT-RED_Updater.exe");
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(
                            updaterProcess,
                            args
                        );
                        System.Diagnostics.Process.Start(startInfo);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogException(ex);
                    }
                }
                else
                {
                    MsgBox.Show($"Update Package Location:\n{location}", "Download Location", Buttons.OK, Icon.Info, FormStartPosition.CenterParent);
                }
            }
        }

        private void progressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            repButtonEdit.Buttons[2].Caption = $"Downloading.. {e.ProgressPercentage}%";
        }
    }
}
