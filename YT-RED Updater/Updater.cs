using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using Ionic.Zip;

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
                            entry.FileName = entry.FileName.Replace($"YTR/", "");
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
                        System.IO.File.Copy(newPath, newPath.Replace(BaseDir, Path.Combine(BaseDir, "Backup")), true);
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
                Tuple<List<DirectoryInfo>, List<FileInfo>> pendingDeletes = await Task.Run(async () =>
                {
                    List<DirectoryInfo> failedDirs = new List<DirectoryInfo>();
                    List<FileInfo> failedFiles = new List<FileInfo>();

                    DirectoryInfo baseDir = new DirectoryInfo(BaseDir);
                    List<DirectoryInfo> folders = new List<DirectoryInfo>();
                    DirectoryInfo backupDir = new DirectoryInfo(Path.Combine(baseDir.FullName, "Backup"));
                    decimal total = 0;
                    decimal completed = 0;
                    int percentComplete = 0;
                    if (backupDir.Exists)
                    {
                        folders = backupDir.GetDirectories("*", SearchOption.TopDirectoryOnly).ToList();
                        if(folders.Count > 0)
                        {
                            total = folders.Count;
                            foreach(var di in folders)
                            {
                                FolderActionResult tryDelete = await FileHelper.DeleteFolder(di);
                                if (!tryDelete.Success)
                                    failedDirs.Add(di);
                                completed++;
                                percentComplete = Convert.ToInt32((completed / total) * 100);
                                reportProgress(percentComplete);
                            }
                        }
                    }

                    total = 0;
                    completed = 0;
                    percentComplete = 0;
                    List<FileInfo> files = baseDir.GetFiles("*", SearchOption.AllDirectories)
                        .Where(f => !f.FullName.EndsWith("YTR_Updater.exe")
                            && !f.FullName.EndsWith("Ionic.Zip.Reduced.dll")
                            && !f.Name.EndsWith(".json")
                            && f.Directory.Name != "ErrorLogs"
                            && !f.FullName.Contains(@"\ErrorLogs\")
                            && f.Directory.Name != "Updates"
                            && !f.FullName.Contains(@"\Updates\")
                            && f.Directory.Name != "Backup"
                            && !f.FullName.Contains(@"\Backup")
                        )
                        .ToList();

                    total = files.Count;

                    foreach (FileInfo f in files)
                    {
                        FileActionResult tryDelete = await FileHelper.DeleteFile(f);
                        if (!tryDelete.Success)
                            failedFiles.Add(f);
                        completed++;
                        percentComplete = Convert.ToInt32((completed / total) * 100);
                        reportProgress(percentComplete);
                    }

                    result.Output = "Completed";
                    return Tuple.Create(failedDirs, failedFiles);
                });

                result.PendingFolders = pendingDeletes.Item1;
                result.PendingFiles = pendingDeletes.Item2;
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
                        if (!Directory.Exists(dir.FullName.Replace(extractionFolder.FullName, BaseDir)))
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
                            System.IO.File.Copy(newFile.FullName, $"{dest}.new");
                        }
                        else
                        {
                            System.IO.File.Copy(newFile.FullName, dest, true);
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

        public static async Task<ProcessResult> SearchAndReplaceShortcuts(Action<int> reportProgress, string newApplicationPath)
        {
            ProcessResult result = new ProcessResult();
            await Task.Run(() =>
            {
                try
                {
                    var checkFolders = new List<string>() {
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)
                    };

                    int processed = 0;
                    WshShell shell = new WshShell();
                    foreach (string folder in checkFolders)
                    {
                        DirectoryInfo dir = new DirectoryInfo(folder);
                        if (dir.Exists) {
                            var shortcuts = dir.GetFiles().Where(f => f.Extension.ToLower() == ".lnk");
                            foreach(var sc in shortcuts)
                            {
                                string lnkLocation = string.Empty;
                                IWshShortcut oldLink = (IWshShortcut)shell.CreateShortcut(sc.FullName);
                                if (oldLink.TargetPath.Contains($"YT-RED.exe"))
                                {
                                    lnkLocation = sc.FullName;
                                    sc.Delete();
                                
                                    IWshShortcut newLink = (IWshShortcut)shell.CreateShortcut(lnkLocation.Replace("YT-RED", "YTR"));
                                    newLink.TargetPath = newApplicationPath;
                                    newLink.Description = $"New YTR Shortcut";
                                    newLink.Save();
                                    processed++;
                                }
                            }
                        }
                    }
                    result.Output = $"Replaced {processed} Shortcuts";
                }
                catch (Exception ex)
                {
                    result.Error = "Failed to Update Existing Shortcuts\n\n" + ex.ToString();
                }
            });
            return result;
        }            
    }

    public class ProcessResult
    {
        public string Output { get; set; }

        public List<FileInfo> PendingFiles { get; set; }
        public List<DirectoryInfo> PendingFolders { get; set; }
        public string Error { get; set; }

        public ProcessResult()
        {
            Output = string.Empty;
            PendingFiles = null;
            PendingFolders = null;
            Error = string.Empty;
        }       
    }
}
