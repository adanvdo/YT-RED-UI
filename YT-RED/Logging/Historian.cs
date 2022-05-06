﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using YT_RED.Settings;

namespace YT_RED.Logging
{
    public static class Historian
    {
        private static string historyFile = "history.json";
        private static bool historyWasLoaded = false;
        public static List<DownloadLog> DownloadHistory;

        public static void Init()
        {
            DownloadHistory = new List<DownloadLog>();
        }

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

        public static async Task CleanHistory(bool fullErase = false, bool deleteFiles = false)
        {
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

                if(deleteFiles)
                {
                    foreach(var log in DownloadHistory)
                    {
                        if(File.Exists(log.DownloadLocation))
                        {
                            File.Delete(log.DownloadLocation);
                        }
                    }
                }

                if (fullErase)
                {
                    DownloadHistory.Clear();
                }
                else
                {
                    DownloadHistory.RemoveAll(h => h.Downloaded.Date < DateTime.Today.AddDays(-AppSettings.Default.General.HistoryAge).Date);
                }
                await SaveHistory();
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
                    DownloadHistory = new List<DownloadLog>();
                    DownloadHistory.Add(dlLog);
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
                await Task.Run(() => File.WriteAllText(historyFile, json));
                return true;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return false;
        }
    }
}
