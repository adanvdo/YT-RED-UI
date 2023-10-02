using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using YTR.Logging;
using YTR.Settings;
using YTR.Controls;
using SevenZipExtractor;
using YTR.Classes;
using System.Diagnostics;

namespace YTR.Utils
{
    public static class UpdateHelper
    {

        #region YTR UPDATES

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

        public static async Task<YTR.Classes.Release> GetLatestRelease(ReleaseChannel channel)
        {
            try
            {
                HttpResponseMessage getLatest = await HttpUtil.Get("getlatest", $"channel={(int)channel}");
                if (getLatest != null && getLatest.IsSuccessStatusCode)
                {
                    string content = await getLatest.Content.ReadAsStringAsync();
                    if (content != "Unknown")
                    {
                        YTR.Classes.Release release = JsonConvert.DeserializeObject<Classes.Release>(content);
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
                    string oldPath = Path.Combine(path, "YTR_Updater.exe");
                    string newPath = Path.Combine(path, "YTR_Updater.exe.new");
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

        #endregion

        #region DEPENDENCY UPDATES

        public static async Task<string[]> GetLocalAppVersions()
        {
            return await Task.Run(() =>
            {
                try
                {
                    DirectoryInfo resources = new DirectoryInfo(Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Resources", "App"));
                    var ytdlpInfo = FileVersionInfo.GetVersionInfo(Path.Combine(resources.FullName, "yt-dlp.exe"));
                    var ffmpegInfo = FileVersionInfo.GetVersionInfo(Path.Combine(resources.FullName, "ffmpeg.exe"));
                    return new string[] { ytdlpInfo.FileVersion, ffmpegInfo.FileVersion };
                }
                catch(Exception ex)
                {
                    ExceptionHandler.LogException(ex);
                }
                return null;
            });
        }

        public static async Task<string> GetLatestYtdlpVersionNumber()
        {
            var response = await HttpUtil.SendGetRequest(AppSettings.Default.General.YtdlpVersionUrl, "request");
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    List<GithubTag> tags = JsonConvert.DeserializeObject<List<GithubTag>>(content);
                    return tags.FirstOrDefault().Name;
                }
                catch(Exception ex)
                {
                    ExceptionHandler.LogException(ex);
                }
            }
            return null;
        }

        public static async Task<string> GetLatestFfmpegVersionNumber()
        {
            var response = await HttpUtil.SendGetRequest(AppSettings.Default.General.FfmpegVersionUrl);
            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            return null;
        }

        public static async Task<DirectoryInfo> PrepareTempDirectory()
        {
            DirectoryInfo tempDir = await Task.Run(() =>
            {
                DirectoryInfo resources = new DirectoryInfo(Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Resources", "App"));
                if (resources.Exists)
                {
                    DirectoryInfo temp = new DirectoryInfo(Path.Combine(resources.FullName, "Temp"));
                    if (!temp.Exists)
                    {
                        temp.Create();
                    }
                    return temp;
                }
                return null;
            });
            return tempDir;
        }

        public static async Task<string> InstallFfmpeg()
        {
            string result = "";
            string package = Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Resources", "App", "Temp", "ffmpeg-git-essentials.7z");
            if (File.Exists(package))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        using (var archive = new ArchiveFile(package))
                        {
                            var execs = archive.Entries.Where(e => !e.IsFolder && (e.FileName.EndsWith("ffmpeg.exe") || e.FileName.EndsWith("ffprobe.exe")));
                            foreach(var entry in execs)
                            {
                                string path = Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Resources", "App", "Temp", entry.FileName.Substring(entry.FileName.IndexOf("bin\\") + 4, entry.FileName.Length - (entry.FileName.IndexOf("bin\\") + 4)));
                                entry.Extract(path);
                            }
                            
                            string target1 = Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Resources", "App", "ffmpeg.exe");
                            string target2 = Path.Combine(Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Resources", "App", "ffprobe.exe"));
                            if (File.Exists(target1))
                                File.Delete(target1);
                            if (File.Exists(target2))
                                File.Delete(target2);

                            File.Move(Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Resources", "App", "Temp", "ffmpeg.exe"), target1);
                            File.Move(Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Resources", "App", "Temp", "ffprobe.exe"), target2);
                        }
                    });
                    result = "Installation Complete";
                }
                catch(Exception ex)
                {
                    ExceptionHandler.LogException(ex);
                    result = "Installation Failed";
                }
            }
            else
            {
                result = "Installation Failed";
            }
            return result;
        }

        public static async Task<bool> CleanUpFFMPEG()
        {
            string package = Path.Combine(AppSettings.Default.General.ExeDirectoryPath, "Resources", "App", "Temp", "ffmpeg-git-essentials.7z");
            return await Task.Run(() =>
            {
                if (File.Exists(package))
                {
                    try
                    {
                        File.Delete(package);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogException(ex);
                        return false;
                    }
                }
                return true;
            });
        }

        public static async Task<string> UpdateFfmpeg(System.Net.DownloadProgressChangedEventHandler progressChanged)
        {
            string result = "";
            try
            {
                DirectoryInfo temp = await PrepareTempDirectory();
                if (temp != null)
                {
                    await Task.Run(() =>
                    {
                        FileInfo f = new FileInfo(Path.Combine(temp.FullName, "ffmpeg-git-essentials.7z"));
                        if (f.Exists)
                        {
                            f.Delete();
                        }
                    });

                    string downloadPath = Path.Combine(temp.FullName, "ffmpeg-git-essentials.7z");
                    bool dl = await WebUtil.DownloadFileWithProgress(AppSettings.Default.General.FfmpegUrl, downloadPath, progressChanged);
                    if (dl)
                    {
                        result = "Download Complete";
                    }
                    else
                    {
                        result = "Download Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
                result = "Download Failed";
            }
            return result;
        }

        public static async Task<string> UpdateYTDLP(System.Net.DownloadProgressChangedEventHandler progressChanged)
        {
            string result = "";
            try
            {
                DirectoryInfo temp = await PrepareTempDirectory();
                if (temp != null)
                {
                    await Task.Run(() =>
                    {
                        FileInfo f = new FileInfo(Path.Combine(temp.FullName, "yt-dlp.exe"));
                        if (f.Exists)
                        {
                            f.Delete();
                        }

                        FileInfo f86 = new FileInfo(Path.Combine(temp.FullName, "yt-dlp_x86.exe"));
                        if (f86.Exists)
                        {
                            f86.Delete();
                        }
                    });

                    string downloadPath1 = Path.Combine(temp.FullName, $"yt-dlp_x86.exe");
                    string downloadPath2 = Path.Combine(temp.FullName, $"yt-dlp.exe");
                    var Task1 = WebUtil.DownloadFileWithProgress(string.Format(AppSettings.Default.General.YtdlpUrl, "_x86"), downloadPath1, progressChanged);
                    var Task2 = WebUtil.DownloadFileWithProgress(string.Format(AppSettings.Default.General.YtdlpUrl, ""), downloadPath2, progressChanged);
                    var dlResult = await Task.WhenAll(Task1, Task2);
                    if (dlResult[0] && dlResult[1])
                    {
                        DirectoryInfo appDir = temp.Parent;
                        if (appDir.Exists)
                        {
                            string targetPath1 = Path.Combine(appDir.FullName, "yt-dlp_x86.exe");
                            if (File.Exists(targetPath1))
                            {
                                File.Delete(targetPath1);
                            }
                            File.Move(downloadPath1, targetPath1);

                            string targetPath2 = Path.Combine(appDir.FullName, "yt-dlp.exe");
                            if (File.Exists(targetPath2))
                            {
                                File.Delete(targetPath2);
                            }
                            File.Move(downloadPath2, targetPath2);

                            result = "Download Complete";
                        }
                        else
                        {
                            result = "Download Failed";
                        }
                    }
                    else
                    {
                        result = "Download Failed";
                    }
                }
            }
            catch( Exception ex)
            {
                ExceptionHandler.LogException(ex);
                result = "Download Failed";
            }
            return result;
        }

        #endregion
    }
}
