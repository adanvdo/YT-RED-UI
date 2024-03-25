using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTR.Classes
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
        Size = 2,
        VideoOutFormat = 3,
        AudioOutFormat = 4,
        Crop = 5,
        ID3Thumbnail = 6,
        Framerate = 7,
        VideoCodec = 8,
        PixelFormat = 9,
        VF = 10,
        Loop = 11,
        ANULLSRC = 12,
        Map = 13,
        Input = 14
    }
}
