using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using YT_RED.Logging;
using YT_RED.Settings;
using YT_RED.Controls;

namespace YT_RED.Utils
{
    public static class UpdateHelper
    {

        private static string updateDirectory = "";
        public static async Task<string> GetUpdateDirectoryAsync()
        {
            updateDirectory = Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Updates");
            try
            {
                await Task.Run(() =>
                {
                    if (!Directory.Exists(updateDirectory))
                    {
                        Directory.CreateDirectory(updateDirectory);
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return updateDirectory;
        }

        public static async Task<string> DownloadReleaseAsync(string releaseUrl, DownloadProgressChangedEventHandler progressChanged)
        {
            if (string.IsNullOrEmpty(releaseUrl) || progressChanged == null) throw new ArgumentNullException();
            
            Uri uri = new Uri(releaseUrl);

            string filePath = Path.Combine(await GetUpdateDirectoryAsync(), uri.Segments.Last());
            try
            {
                bool exists = await Task.Run(() => File.Exists(filePath));

                if (exists)
                {
                    DialogResult res = MsgBox.Show($"The selected release has already been downloaded.\nAre you sure you want to download it again?", "Existing File", Buttons.YesNo, Icon.Question, System.Windows.Forms.FormStartPosition.CenterParent);
                    if (res == DialogResult.No)
                        return "cancelled";
                    await Task.Run(() => File.Delete(filePath));
                }

                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += progressChanged;
                    await client.DownloadFileTaskAsync(releaseUrl, filePath);
                }
                return filePath;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        public static async Task<YT_RED.Classes.Release> GetLatestRelease(Settings.ReleaseChannel channel)
        {
            try
            {
                HttpResponseMessage getLatest = await HttpUtil.Get("getlatest", $"channel={(int)channel}");
                if (getLatest != null && getLatest.IsSuccessStatusCode)
                {
                    string content = await getLatest.Content.ReadAsStringAsync();
                    if (content != "Unknown")
                    {
                        YT_RED.Classes.Release release = JsonConvert.DeserializeObject<Classes.Release>(content);
                        return release;
                    }
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        public static async Task<bool> ReplaceUpdater()
        {
            bool result = false;
            try
            {
                await Task.Delay(1000);
                await Task.Run(() =>
                {
                    string path = AppSettings.Default.General.ExeDirectoryPath;
                    string oldPath = Path.Combine(path, "YT-RED_Updater.exe");
                    string newPath = Path.Combine(path, "YT-RED_Updater.exe.new");
                    if (File.Exists(newPath))
                    {
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                        File.Move(newPath, oldPath);
                    }                    
                    result = true;
                });
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return result;
        }

        public static async Task<bool> ReplaceZipDependency()
        {
            bool result = false;
            try
            {
                await Task.Run(() =>
                {
                    string path = AppSettings.Default.General.ExeDirectoryPath;
                    string oldPath = Path.Combine(path, "Ionic.Zip.Reduced.dll");
                    string newPath = Path.Combine(path, "Ionic.Zip.Reduced.dll.new");
                    if (File.Exists(newPath))
                    {
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                        File.Move(newPath, oldPath);
                    }
                    result = true;
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return result;
        }

        public static async Task<bool> DeleteRemnants()
        {
            bool result = false;
            try
            {
                await Task.Run(() =>
                {
                    DirectoryInfo backup = new DirectoryInfo(Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Backup"));
                    if (backup.Exists)
                    {
                        foreach (FileInfo f in backup.GetFiles().Where(bf => !bf.Name.EndsWith(".json")))
                        {
                            f.Delete();
                        }
                    }
                    if (File.Exists(Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Updates", "DeletePending.bat")))
                        File.Delete(Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Updates", "DeletePending.bat"));
                    result = true;
                });
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return result;
        }
    }
}
