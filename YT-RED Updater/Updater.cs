using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace YTR_Updater
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
                    Process[] running = Process.GetProcessesByName("YTR");
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

                        List<ZipEntry> entries = zip.Entries.Where(e => (e.IsDirectory && e.FileName != "YTR/") || !e.IsDirectory).ToList();
                        decimal total = entries.Count;
                        decimal extracted = 0;
                        int percentage = 0;

                        foreach (ZipEntry entry in entries)
                        {
                            entry.FileName = entry.FileName.Replace("YTR/", "");
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

        public static async Task<ProcessResult> CleanBaseDirectory(Action<int> reportProgress, bool newUpdater = false)
        {
            ProcessResult result = new ProcessResult();
            try
            {
                List<FileInfo> pendingDeletes = await Task.Run(async () =>
                {
                    List<FileInfo> failed = new List<FileInfo>();

                    DirectoryInfo baseDir = new DirectoryInfo(BaseDir);
                    List<FileInfo> files = baseDir.GetFiles("*", SearchOption.AllDirectories)
                        .Where(f => !f.Name.EndsWith(".json")
                            && f.Directory.Name != "ErrorLogs"
                            && !f.FullName.Contains(@"\ErrorLogs\")
                            && f.Directory.Name != "Updates"
                            && !f.FullName.Contains(@"\Updates\")
                            && f.Directory.Name != "Backup"
                            && !f.FullName.Contains(@"\Backup\")
                            && (newUpdater || (!newUpdater && f.Name != "YTR_Updater.exe"))
                        )
                        .ToList();

                    decimal total = files.Count;
                    decimal completed = 0;
                    int percentComplete = 0;

                    foreach (FileInfo f in files)
                    {
                        if (f.FullName.EndsWith("YTR_Updater.exe") || f.FullName.EndsWith("Ionic.Zip.Reduced.dll"))
                        {
                            failed.Add(f);
                        }
                        else
                        {
                            FileActionResult tryDelete = await FileHelper.DeleteFile(f);
                            if (!tryDelete.Success)
                                failed.Add(f);
                        }
                        completed++;
                        percentComplete = Convert.ToInt32((completed / total) * 100);
                        reportProgress(percentComplete);
                    }

                    result.Output = "Completed";
                    return failed;
                });

                result.Pending = pendingDeletes;
            }
            catch (Exception ex)
            {
                result.Error = ex.ToString();
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
                        FileInfo[] files = dir.Parent.GetFiles().Where(f => f.Name != "DeletePending.bat").ToArray();
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

        public static async Task<ProcessResult> CopyUpdateFiles(Action<int> reportCopyProgress, bool copyUpdater = false, List<FileInfo> pendingDelete = null)
        {
            ProcessResult result = new ProcessResult();
            await Task.Run(() =>
            {
                try
                {
                    DirectoryInfo extractionFolder = new DirectoryInfo(ExtractionFolder);

                    List<DirectoryInfo> dirs = new List<DirectoryInfo>();
                    List<FileInfo> files = new List<FileInfo>();

                    if (copyUpdater)
                    {
                        dirs = extractionFolder.GetDirectories("*", SearchOption.AllDirectories).ToList();
                        files = extractionFolder.GetFiles("*", SearchOption.AllDirectories).ToList();
                    }
                    else
                    {
                        dirs = extractionFolder.GetDirectories("*", SearchOption.AllDirectories).ToList();
                        files = extractionFolder.GetFiles("*", SearchOption.AllDirectories).Where(f => f.Name != "YTR_Updater.exe").ToList();
                    }

                    decimal total = dirs.Count + files.Count;
                    decimal completed = 0;
                    int percentComplete = 0;

                    //Now Create all of the directories
                    foreach (DirectoryInfo dir in dirs)
                    {
                        if (!Directory.Exists(dir.FullName.Replace(ExtractionFolder, BaseDir)))
                            Directory.CreateDirectory(dir.FullName.Replace(ExtractionFolder, BaseDir));
                        completed++;
                        percentComplete = Convert.ToInt32((completed / total) * 100);
                        reportCopyProgress(percentComplete);
                    }

                    //Copy all the files & Replaces any files with the same name

                    foreach (FileInfo newFile in files)
                    {
                        string dest = newFile.FullName.Replace(ExtractionFolder, BaseDir);
                        if (newFile.Name == "YTR_Updater.exe"
                            || newFile.Name == "Ionic.Zip.Reduced.dll"
                            || (pendingDelete != null && pendingDelete.Find(fi => fi.Name == newFile.Name) != null))
                        {
                            File.Copy(newFile.FullName, $"{dest}.new");
                        }
                        else
                        {
                            File.Copy(newFile.FullName, dest, true);
                        }
                        completed++;
                        percentComplete = Convert.ToInt32((completed / total) * 100);
                        reportCopyProgress(percentComplete);
                    }
                    result.Output = $"Copied {total - pendingDelete.Count} files";
                }
                catch (Exception ex)
                {
                    string message = ex.ToString() + "\n\nManual Update Required. Update File Location:\n" + ExtractionFolder;
                    result.Error = message;
                }
            });
            return result;
        }
    }

    public class ProcessResult
    {
        public string Output { get; set; }

        public List<FileInfo> Pending { get; set; }
        public string Error { get; set; }

        public ProcessResult()
        {
            Output = string.Empty;
            Pending = null;
            Error = string.Empty;
        }
    }
}
