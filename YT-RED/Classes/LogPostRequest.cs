using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YT_RED.Classes
{
    public class LogPostRequest
    {
        [JsonProperty("machineId")]
        public string MachineID { get; set; }

        [JsonProperty("logTime")]
        public string LogTime { get; set; }

        [JsonProperty("logText")]
        public string LogText { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        public LogPostRequest()
        {
            Version = Settings.AppSettings.Default.About.Version;
        }

        public LogPostRequest(string machineId, DateTime logTime, string logText)
        {
            MachineID = machineId;
            LogTime = logTime.ToString();
            LogText = logText.Replace(@"\", @"\\").Replace("\n", "\\n");
            Version = Settings.AppSettings.Default.About.Version;
        }
    }
}
