using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YT_RED.Settings;

namespace YT_RED.Logging
{
    public static class Historian
    {
        private const string historyFile = "history.json";
        private static bool historyWasLoaded = false;
        public static List<DownloadLog> DownloadHistory = new List<DownloadLog>();

        public static async Task<bool> LoadDownloadHistory()
        {
            try
            {
                if (File.Exists(historyFile))
                {
                    var json = await Task.Run(() => File.ReadAllText(historyFile));
                    List<DownloadLog> history = JsonConvert.DeserializeObject<List<DownloadLog>>(json);
                    DownloadHistory = history;
                    historyWasLoaded = true;
                    return true;
                }

                await Task.Run(() => File.Create(historyFile));
                return true;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message); 
            }
            return false;
        }

        private static async Task CleanHistory(bool fullErase = false)
        {
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

                if (fullErase)
                {
                    DownloadHistory.Clear();
                }
                else
                {
                    DownloadHistory.RemoveAll(h => h.Downloaded.Date < DateTime.Today.AddDays(-AppSettings.Default.General.HistoryAge).Date);
                }
                bool saved = await SaveHistory();            
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        public static async Task<bool> RecordDownload(DownloadLog dlLog)
        {
            try
            {
                DownloadHistory.Add(dlLog);
                bool saved = await SaveHistory();
                return true; 
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return false;
        }

        public static async Task<bool> SaveHistory()
        {
            try
            {
                await CleanHistory();
                var json = JsonConvert.SerializeObject(DownloadHistory, Formatting.Indented);
                await Task.Run(() => File.WriteAllText(historyFile, json));
                return true;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return false;
        }
    }
}
