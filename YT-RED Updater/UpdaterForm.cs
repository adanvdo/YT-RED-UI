using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YT_RED_Updater
{
    public partial class UpdaterForm : DevExpress.XtraEditors.XtraForm
    {
        private DirectoryInfo appBase;
        private FileInfo updatePackage;
        private bool requiresUpdaterReplacement = false;

        public UpdaterForm()
        {
            InitializeComponent();
            lblMessage.Text = "YT-RED Updater cannot be run manually";
        }

        public UpdaterForm(string appBaseDirectory, string updatePackagePath, string skin, string palette, bool includeUpdater)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(appBaseDirectory) || string.IsNullOrEmpty(updatePackagePath))
            {
                lblMessage.Text = "Missing/Invalid Parameters\n";
                return;
            }

            try
            {
                marquee.Visible = true;
                progress.Visible = true;
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(skin, palette);
                this.appBase = new DirectoryInfo(appBaseDirectory);
                this.updatePackage = new FileInfo(updatePackagePath);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                return;
            }

            RunUpdater(includeUpdater);
        }

        public async void RunUpdater(bool updateUpdater)
        {
            marquee.Properties.ShowTitle = true;
            progress.Properties.ShowTitle = true;
            marquee.Text = "Validating Update";
            marquee.Properties.Stopped = false;
            string validate = await Updater.Validate(appBase, updatePackage);
            marquee.Text = "";
            marquee.Properties.Stopped = true;
            if (!string.IsNullOrEmpty(validate))
            {
                marquee.Properties.ShowTitle = false;
                progress.Properties.ShowTitle = false;
                lblMessage.Text = validate;
                return;
            }

            marquee.Text = "Extracting Update";
            marquee.Properties.Stopped = false;
            progress.Position = 0;
            ProcessResult extract = await Updater.ExtractPackage(this.appBase, this.updatePackage, reportProgress);
            progress.Position = 0;
            marquee.Text = "";
            marquee.Properties.Stopped = true;
            if(!string.IsNullOrEmpty(extract.Error))
            {
                marquee.Properties.ShowTitle = false;
                progress.Properties.ShowTitle = false;
                lblMessage.Text = $"A Task Failed\n{extract.Error}\nYou may close this dialog";
                return;
            }

            marquee.Text = "Closing YT-RED";
            marquee.Properties.Stopped = false;
            ProcessResult kill = await Updater.EndRunningProcesses();
            marquee.Text = "";
            marquee.Properties.Stopped = true;
            if (!string.IsNullOrEmpty(kill.Error))
            {
                marquee.Properties.ShowTitle = false;
                progress.Properties.ShowTitle = false;
                lblMessage.Text = $"A Task Failed\n{extract.Error}\nYou may close this dialog";
                return;
            }

            marquee.Text = "Creating Backup";
            marquee.Properties.Stopped = false;
            progress.Position = 0;
            ProcessResult backup = await Updater.BackupCurrent(reportProgress);
            progress.Position = 0;
            marquee.Text = "";
            marquee.Properties.Stopped = true;
            if (!string.IsNullOrEmpty(backup.Error))
            {
                marquee.Properties.ShowTitle = false;
                progress.Properties.ShowTitle = false;
                lblMessage.Text = $"A Task Failed\n{backup.Error}\nYou may close this dialog";
                return;
            }

            marquee.Text = "Cleaning Up Files";
            marquee.Properties.Stopped = false;
            progress.Position = 0;
            ProcessResult clean = await Updater.CleanBaseDirectory(reportProgress);
            progress.Position = 0;
            marquee.Text = "";
            marquee.Properties.Stopped = true;
            if (!string.IsNullOrEmpty(clean.Error))
            {
                marquee.Properties.ShowTitle = false;
                progress.Properties.ShowTitle = false;
                lblMessage.Text = $"A Task Failed\n{clean.Error}\nYou may close this dialog";
                return;
            }

            marquee.Text = "Installing Update";
            marquee.Properties.Stopped = false;
            progress.Position = 0;
            ProcessResult install = await Updater.CopyUpdateFiles(reportProgress, updateUpdater);
            progress.Position = 0;
            marquee.Text = "";
            marquee.Properties.Stopped = true;
            if (!string.IsNullOrEmpty(install.Error))
            {
                marquee.Properties.ShowTitle = false;
                progress.Properties.ShowTitle = false;
                lblMessage.Text = $"A Task Failed\n{install.Error}\nYou may close this dialog";
                return;
            }

            marquee.Text = "Removing Temporary Files";
            marquee.Properties.Stopped = false;
            progress.Position = 0;
            ProcessResult delete = await Updater.DeleteUpdateFiles(reportProgress);
            progress.Position = 0;
            marquee.Text = "";
            marquee.Properties.Stopped = true;
            if (!string.IsNullOrEmpty(delete.Error))
            {
                marquee.Properties.ShowTitle = false;
                progress.Properties.ShowTitle = false;
                lblMessage.Text = $"A Task Failed\n{delete.Error}\nYou may close this dialog";
                return;
            }

            marquee.Text = "Launching YT-RED";
            marquee.Properties.Stopped = false;
            try
            {
                ProcessStartInfo processStart;
                if (requiresUpdaterReplacement)
                    processStart = new ProcessStartInfo(Path.Combine(this.appBase.FullName, "YT-RED.exe"), "-updater");
                else
                    processStart = new ProcessStartInfo(Path.Combine(this.appBase.FullName, "YT-RED.exe"));
                Process.Start(processStart);
                await Task.Delay(1000);
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message;
                return;
            }
            Environment.Exit(0);
        }

        private void reportProgress(int percent)
        {
            if (progress.InvokeRequired)
            {
                progress.Invoke(new MethodInvoker(delegate
                {
                    progress.Position = percent;
                    progress.Refresh();
                }));
            }
            else
            {
                progress.Position = percent;
                progress.Refresh();
            }
        }      

    }
}
