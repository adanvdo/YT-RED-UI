using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;

namespace YT_RED_Updater
{
    public static class Updater
    {
        public static string ExtractionFolder = string.Empty;
        public static string Package = string.Empty;
        private static string BaseDir = string.Empty;
        public static async Task<string> Validate(DirectoryInfo baseDir, FileInfo package)
        {
            string message = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    if (!baseDir.Exists)
                        message = $"Invalid Directory: {baseDir.FullName}";

                    if (!package.Exists || package.Extension != ".zip")
                        message = $"Invalid Package: {package.FullName}";
                    BaseDir = baseDir.FullName;
                    Package = package.FullName;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            });
            return message;
        }

        public static async Task<ProcessResult> EndRunningProcesses()
        {
            ProcessResult result = new ProcessResult();
            await Task.Run(() =>
            {
                try
                {
                    Process[] running = Process.GetProcessesByName("YT-RED");
                    int killed = 0;
                    foreach (var process in running)
                    {
                        process.Kill();
                        killed++;
                    }
                    string kref = killed > 1 ? "Processes" : "Process";
                    result.Output = $"Killed {killed} {kref}";
                }
                catch (Exception ex)
                {
                    result.Error = ex.Message;
                }
            });
            return result;
        }

        public static async Task<ProcessResult> ExtractPackage(DirectoryInfo baseDir, FileInfo package, Action<int> reportProgress)
        {
            ProcessResult result = new ProcessResult();

            await Task.Run(() =>
            {
                try
                {
                    using (ZipFile zip = ZipFile.Read(package.FullName))
                    {
                        ExtractionFolder = Path.Combine(baseDir.FullName, "Updates", package.Name.Replace(".zip", ""));
                        if (Directory.Exists(ExtractionFolder))
                            Directory.Delete(ExtractionFolder, true);

                        Directory.CreateDirectory(ExtractionFolder);

                        List<ZipEntry> entries = zip.Entries.Where(e => (e.IsDirectory && e.FileName != "YT-RED/") || !e.IsDirectory).ToList();
                        decimal total = entries.Count;
                        decimal extracted = 0;
                        int percentage = 0;

                        foreach (ZipEntry entry in entries)
                        {
                            entry.FileName = entry.FileName.Replace("YT-RED/", "");
                            entry.Extract(ExtractionFolder);
                            extracted++;
                            percentage = Convert.ToInt32(((extracted / total) * 100));
                            reportProgress(percentage);
                        }

                        result.Output = ExtractionFolder;
                    }
                }
                catch (Exception ex)
                {
                    result.Error = ex.Message;
                    return;
                }
            });

            return result;
        }

        public static async Task<ProcessResult> BackupCurrent(Action<int> reportProgress)
        {
            ProcessResult result = new ProcessResult();
            await Task.Run(() =>
            {
                try
                {
                    if (Directory.Exists(Path.Combine(BaseDir, "Backup")))
                        Directory.Delete(Path.Combine(BaseDir, "Backup"), true);

                    Directory.CreateDirectory(Path.Combine(BaseDir, "Backup"));

                    DirectoryInfo baseDir = new DirectoryInfo(BaseDir);
                    List<DirectoryInfo> dirs = baseDir.GetDirectories("*", SearchOption.AllDirectories)
                        .Where(di => di.Name != "ErrorLogs"
                            && !di.FullName.Contains(@"\ErrorLogs\")
                            && di.Name != "Backup"
                            && !di.FullName.Contains(@"\Backup\")
                            && di.Name != "Updates"
                            && !di.FullName.Contains(@"\Updates\")
                        )
                        .ToList();
                    List<FileInfo> files = baseDir.GetFiles("*", SearchOption.AllDirectories)
                        .Where(fi => fi.Directory.Name != "ErrorLogs"
                            && !fi.DirectoryName.Contains(@"\ErrorLogs\")
                            && fi.Directory.Name != "Updates"
                            && !fi.DirectoryName.Contains(@"\Updates\")
                            && fi.Directory.Name != "Backup"
                            && !fi.DirectoryName.Contains(@"\Backup\")
                        )
                        .ToList();

                    decimal total = dirs.Count + files.Count;
                    decimal completed = 0;
                    int percentComplete = 0;

                    //Now Create all of the directories
                    foreach (string dirPath in dirs.Select(d => d.FullName))
                    {
                        Directory.CreateDirectory(dirPath.Replace(BaseDir, Path.Combine(BaseDir, "Backup")));
                        completed++;
                        percentComplete = Convert.ToInt32((completed / total) * 100);
                        reportProgress(percentComplete);
                    }

                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in files.Select(f => f.FullName))
                    {
                        File.Copy(newPath, newPath.Replace(BaseDir, Path.Combine(BaseDir, "Backup")), true);
                        completed++;
                        percentComplete = Convert.ToInt32((completed / total) * 100);
                        reportProgress(percentComplete);
                    }
                    result.Output = "Backup Complete";
                }
                catch (Exception ex)
                {
                    result.Error = ex.Message;
                }
            });
            return result;
        }

        public static async Task<ProcessResult> CleanBaseDirectory(Action<int> reportProgress)
        {
            ProcessResult result = new ProcessResult();
            try
            {
                await Task.Run(() =>
                {
                    DirectoryInfo baseDir = new DirectoryInfo(BaseDir);
                    List<FileInfo> files = baseDir.GetFiles("*", SearchOption.AllDirectories)
                        .Where(f => !f.FullName.EndsWith("YT-RED_Updater.exe")
                            && !f.FullName.EndsWith("Ionic.Zip.Reduced.dll")
                            && !f.Name.EndsWith(".json")
                            && f.Directory.Name != "ErrorLogs"
                            && !f.FullName.Contains(@"\ErrorLogs\")
                            && f.Directory.Name != "Updates"
                            && !f.FullName.Contains(@"\Updates\")
                            && f.Directory.Name != "Backup"
                            && !f.FullName.Contains(@"\Backup\")
                        )
                        .ToList();

                    decimal total = files.Count;
                    decimal completed = 0;
                    int percentComplete = 0;

                    foreach (FileInfo f in files)
                    {
                        f.Delete();
                        completed++;
                        percentComplete = Convert.ToInt32((completed / total) * 100);
                        reportProgress(percentComplete);
                    }

                    result.Output = "Completed";
                });
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            return result;
        }

        public static async Task<ProcessResult> DeleteUpdateFiles(Action<int> reportProgress)
        {
            ProcessResult result = new ProcessResult();
            await Task.Run(() =>
            {
                try
                {
                    reportProgress(50);
                    DirectoryInfo dir = new DirectoryInfo(ExtractionFolder);
                    if (dir.Exists)
                    {
                        dir.Delete(true);
                    }
                    if (!Program.devRun)
                    {
                        FileInfo[] files = dir.Parent.GetFiles();
                        foreach (FileInfo f in files)
                        {
                            try
                            {
                                f.Delete();
                            }
                            catch (Exception ex)
                            {
                                System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
                            }
                        }
                    }
                    result.Output = "Completed";
                    reportProgress(100);
                }
                catch (Exception ex)
                {
                    result.Error = ex.Message;
                }
            });
            return result;
        }

        public static async Task<ProcessResult> CopyUpdateFiles(Action<int> reportCopyProgress, bool copyUpdater = false)
        {
            ProcessResult result = new ProcessResult();
            await Task.Run(() =>
            {
                try
                {
                    List<string> dirs = new List<string>();
                    List<string> files = new List<string>();

                    if (copyUpdater)
                    {
                        dirs = Directory.GetDirectories(ExtractionFolder, "*", SearchOption.AllDirectories).ToList();
                        files = Directory.GetFiles(ExtractionFolder, "*", SearchOption.AllDirectories).ToList();
                    }
                    else
                    {
                        dirs = Directory.GetDirectories(ExtractionFolder, "*", SearchOption.AllDirectories).ToList();
                        files = Directory.GetFiles(ExtractionFolder, "*", SearchOption.AllDirectories).Where(f => !f.EndsWith("YT-RED_Updater.exe")).ToList();
                    }

                    decimal total = dirs.Count + files.Count;
                    decimal completed = 0;
                    int percentComplete = 0;

                    //Now Create all of the directories
                    foreach (string dirPath in dirs)
                    {
                        if (!Directory.Exists(dirPath.Replace(ExtractionFolder, BaseDir)))
                            Directory.CreateDirectory(dirPath.Replace(ExtractionFolder, BaseDir));
                        completed++;
                        percentComplete = Convert.ToInt32((completed / total) * 100);
                        reportCopyProgress(percentComplete);
                    }

                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in files)
                    {
                        if (newPath.EndsWith("YT-RED_Updater.exe"))
                        {
                            File.Copy(newPath, $"{newPath.Replace(ExtractionFolder, BaseDir)}.new");
                        }
                        else if (newPath.EndsWith("Ionic.Zip.Reduced.dll"))
                        {
                            File.Copy(newPath, $"{newPath.Replace(ExtractionFolder, BaseDir)}.new");
                        }
                        else
                        {
                            File.Copy(newPath, newPath.Replace(ExtractionFolder, BaseDir), true);
                        }
                        completed++;
                        percentComplete = Convert.ToInt32((completed / total) * 100);
                        reportCopyProgress(percentComplete);
                    }
                    result.Output = $"Copied {total} files";
                }
                catch (Exception ex)
                {
                    string message = ex.Message + "\n\nManual Update Required. Update File Location:\n" + ExtractionFolder;
                    result.Error = message;
                }
            });
            return result;
        }
    }

    public class ProcessResult
    {
        public string Output { get; set; }

        public string Error { get; set; }

        public ProcessResult()
        {
            Output = string.Empty;
            Error = string.Empty;
        }

        public ProcessResult(string message, string error)
        {
            Output = message;
            Error = error;
        }
    }
}
