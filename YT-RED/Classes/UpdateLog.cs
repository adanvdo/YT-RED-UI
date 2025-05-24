using Newtonsoft.Json;
using System;
using YTR.Converters;

namespace YTR.Classes
{
    public class UpdateLog
    {
        [JsonProperty("updated"), JsonConverter(typeof(DateFormatConverter), "MM/dd/yyyy")]
        public DateTime Updated { get; set; }
        [JsonProperty("ytdlpversion")]
        public string YTDLPVersion { get; set; }
        [JsonProperty("ffmpegversion")]
        public string FFMPEGVersion { get; set; }

        public UpdateLog() { }
    }

    
}
