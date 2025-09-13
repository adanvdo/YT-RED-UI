using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp.Metadata;

namespace YTR.Classes
{
    public class YTRThumbnailData : YoutubeDLSharp.Metadata.ThumbnailData
    {
        public string MimeType { get; set; }
        public bool IsSupported { get; set; } = false;
        public bool IsWebp { get; set; } = false;
        public int Order { get; set; }
        public YTRThumbnailData() : base() { }
        public YTRThumbnailData(string mimeType) : base()
        {
            this.MimeType = mimeType;
        }
        public YTRThumbnailData(ThumbnailData thumbnailData): base()
        {
            base.Width = thumbnailData.Width;
            base.Height = thumbnailData.Height;
            base.ID = thumbnailData.ID;
            base.Url = thumbnailData.Url;
            base.Resolution = thumbnailData.Resolution;
            base.Filesize = thumbnailData.Filesize;
            base.Preference = thumbnailData.Preference;
            this.MimeType = null;
            this.Order = -1;
        }

        public static List<YTRThumbnailData> ConvertFromThumbnailDataArray(ThumbnailData[] thumbnailDataArray, SortPriority sortPriority = SortPriority.Preference, Expression<Func<ThumbnailData, bool>> thumbnailPredicate = null)
        {
            IOrderedEnumerable<ThumbnailData> thumbs = sortPriority == SortPriority.Preference ? 
                thumbnailDataArray.OrderByDescending(tn => tn.Preference) :
                thumbnailDataArray.OrderByDescending(tn => tn.Resolution).ThenByDescending(tn => tn.Preference);
            if(thumbnailPredicate == null)
                return thumbs.Select((tn, i) => new YTRThumbnailData(tn) { Order = i }).ToList();
            else 
                return thumbs.AsQueryable().Where(thumbnailPredicate).Select((tn, i) => new YTRThumbnailData(tn) { Order = i }).ToList();
        }

        public static List<YTRThumbnailData> UpdateThumbnailOrder(IEnumerable<YTRThumbnailData> ytrThumbnailData)
        {
            var l = ytrThumbnailData.ToList();
            for(int i = 0; i < l.Count; i++)
            {
                l[i].Order = i;
            }
            return l;
        }
    }

    public enum SortPriority
    {
        Preference = 0,
        Resolution = 1
    }
}
