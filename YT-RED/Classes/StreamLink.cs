using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YT_RED_UI.Classes
{
    public class StreamLink
    {
        public StreamVideoType VideoType { get; set; }
        public string StreamUrl { get; set; }

        public StreamLink()
        {
            StreamUrl = string.Empty;
        }

        public StreamLink(StreamVideoType videoType, string url)
        {            
            VideoType = videoType;
            StreamUrl = url;
        }
    }

    public enum StreamVideoType
    {
        HLS = 0,
        DASH = 1,
        UNKNOWN = 2
    }
}
