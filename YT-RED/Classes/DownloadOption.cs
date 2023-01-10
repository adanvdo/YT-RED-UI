using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace YT_RED.Classes
{
    public class DownloadOption
    {
        [JsonProperty("option_type")]
        public OptionType Type { get; set; }
        [JsonProperty("option_visible")]
        public bool Visible { get; set; } = false;
        [JsonProperty("option_enabled")]
        public bool Enabled { get; set; } = false;
        [JsonIgnore]
        public bool FunctionallyEnabled
        {
            get
            {
                return Visible && Enabled;
            }
        }
        [JsonIgnore]
        public Dictionary<string, object> Parameters { get; set; }

        public DownloadOption(OptionType type)
        {
            Type = type;
            Parameters = new Dictionary<string, object>();
            initTypes();
        }

        private void initTypes()
        {
            switch(Type)
            {
                case OptionType.Segment:
                    Parameters["start"] = TimeSpan.Zero;
                    Parameters["duration"] = TimeSpan.Zero;
                    break;
                case OptionType.Crop:
                    Parameters["top"] = "0";
                    Parameters["bottom"] = "0";
                    Parameters["left"] = "0";
                    Parameters["right"] = "0";
                    break;
                case OptionType.Convert:
                    Parameters["video"] = Settings.VideoFormat.UNSPECIFIED;
                    Parameters["audio"] = Settings.AudioFormat.UNSPECIFIED;
                    break;
                case OptionType.Restrictions:
                    Parameters["maxres"] = Resolution.ANY;
                    Parameters["maxsize"] = "0";
                    break;
                case OptionType.Prepend:
                    Parameters["path"] = string.Empty;
                    Parameters["duration"] = "2";
                    Parameters["dtype"] = MediaDuration.Frames;
                    break;
                case OptionType.Audio:
                    Parameters["path"] = string.Empty;
                    Parameters["start"] = TimeSpan.Zero;
                    Parameters["duration"] = TimeSpan.Zero;
                    break;
                case OptionType.Image:
                    Parameters["path"] = string.Empty;
                    break;
            }
        }

    }

    public enum OptionType
    {
        Segment = 0,
        Crop = 1,
        Convert = 2,
        Restrictions = 3,
        Prepend = 4,
        Audio = 5,
        Image = 6
    }
}
