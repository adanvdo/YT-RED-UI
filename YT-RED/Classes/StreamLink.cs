using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YT_RED.Classes
{
    public class StreamLink
    {
        public StreamPlaylistType PlaylistType { get; set; }
        public string StreamUrl { get; set; }

        public StreamLink()
        {
            StreamUrl = string.Empty;
        }

        public StreamLink(StreamPlaylistType videoType, string url)
        {            
            PlaylistType = videoType;
            StreamUrl = url;
        }
    }

    public enum StreamPlaylistType
    {
        HLS = 0,
        DASH = 1,
        UNKNOWN = 2
    }
}
