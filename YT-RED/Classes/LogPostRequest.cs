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

        public LogPostRequest()
        {

        }

        public LogPostRequest(string machineId, DateTime logTime, string logText)
        {
            MachineID = machineId;
            //0000-00-00 00:00:00
            LogTime = logTime.ToString("yyyy-MM-dd HH:mm:ss");
            LogText = logText.Replace(@"\", @"\\").Replace("\n", "\\n");
        }
    }
}
