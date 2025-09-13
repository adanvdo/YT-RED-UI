using YoutubeDLSharp.Metadata;
using YTR.Classes;

namespace YTR.Extensions
{
    public static class ThumbnailDataExtensions
    {
        public static YTRThumbnailData ToYTRThumbnailData(this ThumbnailData thumbnailData, string mimeType = null)
        {
            var thumb = new YTRThumbnailData()
            {
                Filesize = thumbnailData.Filesize,
                Height = thumbnailData.Height,
                Width = thumbnailData.Width,
                ID = thumbnailData.ID,
                Preference = thumbnailData.Preference,
                Resolution = thumbnailData.Resolution,
                Url = thumbnailData.Url,
                MimeType = mimeType
            };
            return thumb;
        }
    }
}
