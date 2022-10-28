using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YT_RED.Classes;
using YT_RED.Logging;
using YT_RED.Settings;

namespace YT_RED.Utils
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
    }
}
