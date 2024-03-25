using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YTR.Utils;

namespace YTR.Classes
{
    public class YoutubeLink
    {
        public YoutubeLinkType Type { get; set; }
        public string Url { get; set; }

        public YoutubeLink(YoutubeLinkType type, string url)
        {
            Type = type;
            Url = url;
        }
    }
}
