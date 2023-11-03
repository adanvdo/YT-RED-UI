using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YTR.Classes
{
    public class UpdateLog
    {
        [JsonProperty("updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("ytdlpversion")]
        public string YTDLPVersion { get; set; }
        [JsonProperty("ffmpegversion")]
        public string FFMPEGVersion { get; set; }

        public UpdateLog() { }
    }
}
