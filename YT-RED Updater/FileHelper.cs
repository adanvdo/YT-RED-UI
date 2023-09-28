using DevExpress.Data.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace YTR_Updater
{
    public static class FileHelper
    {
        public static async Task<FolderActionResult> DeleteFolder(DirectoryInfo directory)
        {
            FolderActionResult result = await Task.Run(() =>
            {
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
                    ManagementObjectCollection collection = searcher.Get();
                    string username = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];

                    if (!directory.Exists)
                    {
                        return new FolderActionResult(false, directory, "Folder Not Found");
                    }
                    else
                    {
                        DirectorySecurity acl = directory.GetAccessControl(AccessControlSections.All);
                        AuthorizationRule[] rules = new AuthorizationRule[] { };
                        acl.GetAccessRules(true, true, typeof(NTAccount)).CopyTo(rules, 0);
                        var filterRules = rules.Where(r => r.IdentityReference.Value.Equals(username, StringComparison.CurrentCultureIgnoreCase));
                        foreach (var filterRule in filterRules)
                        {
                            var fsar = (FileSystemAccessRule)filterRule;

                            if ((fsar.FileSystemRights & FileSystemRights.ReadData) > 0 && fsar.AccessControlType == AccessControlType.Deny)
                                return new FolderActionResult(false, directory, "Read Failed");
                            if ((fsar.FileSystemRights & FileSystemRights.WriteData) > 0 && fsar.AccessControlType == AccessControlType.Deny)
                                return new FolderActionResult(false, directory, "Write Failed");
                        }
                        
                        directory.Delete();
                        return new FolderActionResult(true, directory);
                    }
                }
                catch (IOException ex)
                {
                    return new FolderActionResult(false, directory, ex.Message);
                }
                catch (Exception ex)
                {
                    return new FolderActionResult(false, directory, ex.Message);
                }
            });
            return result;
        }

        public static async Task<FileActionResult> DeleteFile(FileInfo fileInfo)
        {
            FileActionResult result = await Task.Run(() =>
            {
                try
                {
                    if (!fileInfo.Exists)
                    {
                        return new FileActionResult(false, fileInfo, "File Not Found");
                    }
                    else
                    {
                        using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open))
                        {
                            if (!fs.CanRead) { return new FileActionResult(false, fileInfo, "Read Failed"); }
                            if (!fs.CanWrite) { return new FileActionResult(false, fileInfo, "Write Failed"); }
                        }
                    }

                    fileInfo.Delete();
                    return new FileActionResult(true, fileInfo);
                }
                catch (IOException ex)
                {
                    return new FileActionResult(false, fileInfo, ex.Message);
                }
                catch (Exception ex)
                {
                    return new FileActionResult(false, fileInfo, ex.Message);
                }
            });
            return result;
        }

        public static async Task<ProcessResult> CreateFileUpdateBatch(DirectoryInfo appBase, List<FileInfo> files, List<DirectoryInfo> dirs, string launchArgs)
        {
            ProcessResult res = new ProcessResult();
            string batPath = Path.Combine(appBase.FullName, "Updates","DeletePending.bat");
            try
            {
                await Task.Run(() =>
                {
                    using (StreamWriter writer = File.CreateText(batPath))
                    {
                        writer.WriteLine("TIMEOUT /T 2");

                        foreach (DirectoryInfo dir in dirs)
                        {
                            writer.WriteLine($"rmdir /s /q \"{dir.FullName}\"");
                        }
                        
                        foreach (FileInfo fileInfo in files)
                        {
                            writer.WriteLine($"del \"{fileInfo.FullName}\"");
                            writer.WriteLine($"ren \"{fileInfo.FullName}.new\" \"{fileInfo.Name}\"");
                        }
                        
                        writer.WriteLine(launchArgs);
                    }
                });
                res.Output = batPath;
            }
            catch(Exception ex)
            {
                res.Error = ex.Message;
            }
            return res;
        }
    }

    public class FileActionResult
    {
        public bool Success { get; set; }
        public FileInfo File { get; set; }
        public string Error { get; set; }

        public FileActionResult(bool success, FileInfo file, string error = null)
        {
            Success = success;
            File = file;
            Error = error;
        }
    }

    public class FolderActionResult
    {
        public bool Success { get; set; }
        public DirectoryInfo Folder { get; set; }
        public string Error { get; set; }

        public FolderActionResult(bool success, DirectoryInfo folder, string error = null)
        {
            Success = success;
            Folder = folder;
            Error = error;
        }
    }
}
