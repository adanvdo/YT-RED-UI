using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YT_RED.Settings;

namespace YT_RED.Logging
{
    public static class Historian
    {
        private static string historyFile = "history.json";
        public static bool Loaded { get { return historyWasLoaded; } }
        private static bool historyWasLoaded = false;
        public static List<DownloadLog> DownloadHistory = new List<DownloadLog>();

        public static async Task<bool> LoadDownloadHistory()
        {
            if(DownloadHistory == null)
                DownloadHistory = new List<DownloadLog>();

            FileInfo hinfo = new FileInfo(Assembly.GetEntryAssembly().Location);
            historyFile = Path.Combine(hinfo.DirectoryName, "history.json");

            try
            {
                if (File.Exists(historyFile))
                {
                    var json = await Task.Run(() => File.ReadAllText(historyFile));
                    List<DownloadLog> history = JsonConvert.DeserializeObject<List<DownloadLog>>(json);
                    if (history != null)
                        DownloadHistory = history;
                    else
                        DownloadHistory = new List<DownloadLog>();
                    historyWasLoaded = true;
                    return true;
                }

                await Task.Run(() => File.Create(historyFile));
                return true;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return false;
        }

        public static async Task CleanHistory(DownloadCategory deleteLogs = DownloadCategory.None, DownloadCategory deleteDownloads = DownloadCategory.None)
        {
            bool hasChanges = false;
            if (DownloadHistory == null)
                DownloadHistory = new List<DownloadLog>();
            try
            {
                if (!historyWasLoaded)
                {
                    bool loadHistory = await LoadDownloadHistory();
                    if (!loadHistory)
                    {
                        return;
                    }
                }

                if(deleteDownloads != DownloadCategory.None)
                {
                    List<DownloadLog> filterLogs;
                    if(deleteDownloads == DownloadCategory.Video)
                    {
                        filterLogs = DownloadHistory.Where(dl => dl.StreamType == Classes.StreamType.Video || dl.StreamType == Classes.StreamType.AudioAndVideo).ToList();
                    }
                    else if (deleteDownloads == DownloadCategory.Audio)
                    {
                        filterLogs = DownloadHistory.Where(dl => dl.StreamType == Classes.StreamType.Audio).ToList();
                    }
                    else
                    {
                        filterLogs = DownloadHistory;
                    }

                    if(filterLogs.Count > 0)
                    {
                        hasChanges = true;
                    }

                    foreach(var log in filterLogs)
                    {
                        if (log.InSubFolder)
                        {
                            string subFolder = new FileInfo(log.DownloadLocation).Directory.FullName;
                            if(Directory.Exists(subFolder))
                                Directory.Delete(subFolder, true);
                        } 
                        else if(File.Exists(log.DownloadLocation))
                        {
                            File.Delete(log.DownloadLocation);
                        }
                    }
                }

                if (deleteLogs == DownloadCategory.All)
                {
                    DownloadHistory.Clear();
                    hasChanges = true;
                }
                else if(deleteLogs == DownloadCategory.Video)
                {
                    DownloadHistory.RemoveAll(h => h.StreamType == Classes.StreamType.Video || h.StreamType == Classes.StreamType.AudioAndVideo);
                    hasChanges = true;
                }
                else if (deleteLogs == DownloadCategory.Audio)
                {
                    DownloadHistory.RemoveAll(h => h.StreamType == Classes.StreamType.Audio);
                    hasChanges = true;
                }
                else 
                {
                    if (DownloadHistory.Where(h => h.Downloaded.Date < DateTime.Today.AddDays(-AppSettings.Default.General.HistoryAge).Date).Count() > 0)
                    {
                        DownloadHistory.RemoveAll(h => h.Downloaded.Date < DateTime.Today.AddDays(-AppSettings.Default.General.HistoryAge).Date);
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    await SaveHistory();
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
        }

        public static async Task<bool> RecordDownload(DownloadLog dlLog)
        {
            if (!AppSettings.Default.General.EnableDownloadHistory)
                return false;
            try
            {
                if (DownloadHistory == null || DownloadHistory.Count == 0)
                {
                    DownloadHistory = new List<DownloadLog>
                    {
                        dlLog
                    };
                } 
                else DownloadHistory.Insert(0, dlLog);
                bool saved = await SaveHistory();
                return saved;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return false;
        }

        public static async Task<bool> SaveHistory()
        {
            try
            {
                var json = JsonConvert.SerializeObject(DownloadHistory, Formatting.Indented);
                FileInfo hf = new FileInfo(historyFile);
                if(await IsFileLocked(hf))
                {
                    return false;
                }
                await Task.Run(() => File.WriteAllText(historyFile, json));
                return true;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return false;
        }

        private static async Task<bool> IsFileLocked(FileInfo file)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        stream.Close();
                    }
                }
                catch (IOException)
                {
                    //the file is unavailable because it is:
                    //still being written to
                    //or being processed by another thread
                    //or does not exist (has already been processed)
                    return true;
                }

                //file is not locked
                return false;
            });
        }
    }

    public enum DownloadCategory
    {
        All = 0,
        Video = 1,
        Audio = 2,
        None = 3
    }
}
