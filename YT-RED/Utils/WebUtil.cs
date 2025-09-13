using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using YTR.Logging;

namespace YTR.Utils
{
    public static class WebUtil
    {
        public static async Task<bool> DownloadFileWithProgress(string url, string destination, DownloadProgressChangedEventHandler progressChanged)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += progressChanged;
                    await client.DownloadFileTaskAsync(url, destination);
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return false;
        }
    }
}
