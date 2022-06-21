using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YT_RED.Classes
{
    public class FFmpegParam
    {
        public ParamType Type { get; set; }
        public string Value { get; set; }

        public FFmpegParam(ParamType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    public enum ParamType
    {
        StartTime = 0,
        Duration = 1,
        EndTime = 2,
        Size = 3,
        VideoOutFormat = 4,
        AudioOutFormat = 5,
        Crop = 6,
        ID3Thumbnail = 7
    }
}
