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

namespace YTR_Updater
{
    public partial class UpdaterForm : DevExpress.XtraEditors.XtraForm
    {
        private DirectoryInfo appBase;
        private FileInfo updatePackage;
        private bool requiresUpdaterReplacement = false;

        public UpdaterForm()
        {
            InitializeComponent();
            lblMessage.Text = "YTR Updater cannot be run manually";
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
                this.requiresUpdaterReplacement = includeUpdater;
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
                lblMessage.Text = $"Extraction Failed\n{extract.Error}\nYou may close this dialog";
                return;
            }

            marquee.Text = "Closing YTR";
            marquee.Properties.Stopped = false;
            ProcessResult kill = await Updater.EndRunningProcesses();
            marquee.Text = "";
            marquee.Properties.Stopped = true;
            if (!string.IsNullOrEmpty(kill.Error))
            {
                marquee.Properties.ShowTitle = false;
                progress.Properties.ShowTitle = false;
                lblMessage.Text = $"Failed to End Process\n{extract.Error}\nYou may close this dialog";
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
                lblMessage.Text = $"Backup Failed\n{backup.Error}\nYou may close this dialog";
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
                lblMessage.Text = $"Folder Preparation Failed\n{clean.Error}\nYou may close this dialog";
                return;
            }

            ProcessResult createPostBat = null;
            
            string launchArg = requiresUpdaterReplacement ? "-updater -updated" : "-updated";
            createPostBat = await FileHelper.CreateFileUpdateBatch(clean.Pending, $"{Path.Combine(this.appBase.FullName, "YTR.exe")} {launchArg}");
            if (!string.IsNullOrEmpty(createPostBat.Error))
            {
                marquee.Properties.ShowTitle = false;
                progress.Properties.ShowTitle = false;
                lblMessage.Text = $"Failed to create post-update script\n{clean.Error}\nYou may close this dialog";
                return;
            }
            

            marquee.Text = "Installing Update";
            marquee.Properties.Stopped = false;
            progress.Position = 0;
            ProcessResult install = await Updater.CopyUpdateFiles(reportProgress, updateUpdater, clean.Pending);
            progress.Position = 0;
            marquee.Text = "";
            marquee.Properties.Stopped = true;
            if (!string.IsNullOrEmpty(install.Error))
            {
                marquee.Properties.ShowTitle = false;
                progress.Properties.ShowTitle = false;
                lblMessage.Text = $"Install Failed\n{install.Error}\nYou may close this dialog";
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
                lblMessage.Text = $"Cleanup Failed\n{delete.Error}\nYou may close this dialog";
                return;
            }

            marquee.Text = "Launching YTR";
            marquee.Properties.Stopped = false;
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = createPostBat.Output;
                p.Start();
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
