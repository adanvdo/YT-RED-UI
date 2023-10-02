using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace YTR_Updater
{
    public static class FileHelper
    {
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

        public static async Task<ProcessResult> CreateFileUpdateBatch(List<FileInfo> files, string launchArgs, string appBasePath)
        {
            ProcessResult res = new ProcessResult();
            string batPath = $@"{appBasePath}\Updates\DeletePending.bat";
            try
            {
                await Task.Run(() =>
                {
                    using (StreamWriter writer = File.CreateText(batPath))
                    {
                        writer.WriteLine("TIMEOUT /T 2");
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
            catch (Exception ex)
            {
                res.Error = ex.Message;
            }
            return res;
        }

        public static async Task<ProcessResult> CopyFilesRecursively(string sourcePath, string targetPath)
        {
            ProcessResult res = new ProcessResult();
            try 
            {
                string result = await Task.Run(() =>
                {
                    int files = 0;
                    int dirs = 0;

                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                    {
                        string tDir = dirPath.Replace(sourcePath, targetPath);
                        if (!Directory.Exists(tDir))
                        {
                            Directory.CreateDirectory(tDir);
                            dirs++;
                        }
                    }

                    //Copy all the files & Replaces any files with the same name
                    foreach (string file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                    {
                        string tf = file.Replace(sourcePath, targetPath);
                        File.Copy(file, tf, true);
                        files++;
                    }
                    return $"Copied {files} files and {dirs} directories";
                });
                res.Output = result;
            }
            catch (Exception ex)
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
}
