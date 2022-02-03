using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YT_RED.Classes;
using Newtonsoft.Json;

namespace YT_RED.Logging
{
    public class DownloadLog
    {
        [JsonProperty("filename")]
        public string FileName { get; set; }
        [JsonProperty("type")]
        public StreamType Type { get; set; }
        [JsonProperty("downloaded")]
        public DateTime Downloaded { get; set; }
        [JsonProperty("location")]
        public string DownloadLocation { get; set; }
        [JsonProperty("time_logged")]
        public DateTime TimeLogged { get; set; }

        public DownloadLog()
        { }

        public DownloadLog(string fileName, StreamType type, DateTime downloaded, string location)
        {
            FileName = fileName;
            Type = type;
            Downloaded = downloaded;
            DownloadLocation = location; 
            TimeLogged = DateTime.Now;
        }
    }
}
