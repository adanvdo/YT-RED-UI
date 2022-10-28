using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp.Metadata;
using YT_RED.Utils;

namespace YT_RED.Classes
{
    public class YTDLPlaylistData : VideoData, IDisposable
    {
        public bool Selected { get; set; }
        private string thumbUrl = null;
        public string ThumbUrl { get { return thumbUrl; } }
        public Image ThumbnailImage { get; set; }
        
        public YTDLPlaylistData() : base() 
        { }

        public YTDLPlaylistData(VideoData sourceData) : base()
        {
            ID = sourceData.ID;
            Url = sourceData.Url;
            Title = sourceData.Title;
            Description = sourceData.Description;
            Duration = sourceData.Duration;
            Thumbnails = sourceData.Thumbnails;
            var useThumb = sourceData.Thumbnails.FirstOrDefault(tn => !tn.Url.ToLower().EndsWith(".webp"));
            if (useThumb != null) this.thumbUrl = useThumb.Url;
        }

        public void Dispose()
        {
            if(ThumbnailImage != null)
            {
                ThumbnailImage.Dispose();
            }
        }
    }

    public class PlaylistItemCollection : IDisposable
    {
        public VideoData PlaylistData { get; set; }

        public int Count
        {
            get
            {
                return Items == null ? 0 : Items.Count;
            }
        }
        public List<YTDLPlaylistData> Items { get; set; }

        public PlaylistItemCollection() : this(null, new List<YTDLPlaylistData>()) { }

        public PlaylistItemCollection(VideoData playlistVideoData, List<YTDLPlaylistData> items)
        {
            PlaylistData = playlistVideoData;
            Items = items;
        }

        public PlaylistItemCollection(VideoData playlistVideoData)
        {
            PlaylistData = playlistVideoData;
            Items = playlistVideoData.Entries.Select(e => new YTDLPlaylistData(e)).Where(pd => pd.Title.ToLower() != "[deleted video]" && pd.Title.ToLower() != "[private video]").ToList();
        }        

        public async Task LoadThumbnails()
        {
            try
            {
                foreach (var item in Items)
                {
                    if (!string.IsNullOrEmpty(item.ThumbUrl))
                    {
                        var imageBytes = await HttpUtil.GetImageAsByteArrayAsync(item.ThumbUrl);
                        MemoryStream ms = new MemoryStream(imageBytes);
                        item.ThumbnailImage = Image.FromStream(ms);
                    }
                }
            }
            catch(Exception ex)
            {
                Logging.ExceptionHandler.LogException(ex);
            }
        }

        public void Dispose()
        {
            foreach(YTDLPlaylistData item in Items)
            {
                item.Dispose();
            }
        }
    }
}
