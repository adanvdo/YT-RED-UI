using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp.Metadata;

namespace YT_RED.Classes
{
    public class YTDLPlaylistData : VideoData
    {
        public bool Selected { get; set; }
        public YTDLPlaylistData() : base() 
        { }

        public YTDLPlaylistData(VideoData sourceData) : base()
        {
            Url = sourceData.Url;
            Title = sourceData.Title;
            Duration = sourceData.Duration;
        }
    }

    public class PlaylistItemCollection
    {
        public VideoData PlaylistData { get; set; }

        public List<YTDLPlaylistData> Items { get; set; }

        public PlaylistItemCollection() : this(new List<YTDLPlaylistData>()) { }

        public PlaylistItemCollection(List<YTDLPlaylistData> items)
        {
            PlaylistData = null;
            Items = items;
        }

        public PlaylistItemCollection(VideoData playlistVideoData)
        {
            PlaylistData = playlistVideoData;
            Items = playlistVideoData.Entries.Select(e => new YTDLPlaylistData(e)).ToList();
        }
    }
}
