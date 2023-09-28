using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YTR.Classes;
using YTR.Logging;
using YTR.Settings;

namespace YTR.Utils
{
    public static class WebUtil
    {
        public static async Task<MemoryStream> GetStreamFromUrl(string url)
        {
            Uri uri = new Uri(url);
            byte[] dataBytes = null;
            using (var client = new WebClient())
            {
                dataBytes = await client.DownloadDataTaskAsync(uri.AbsoluteUri);
            }
            return new MemoryStream(dataBytes);
        }

        public static async Task<bool> DownloadFile(string url, string destination)
        {
            try
            {
                using (var client = new WebClient())
                {
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
