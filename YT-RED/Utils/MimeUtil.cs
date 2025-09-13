using MimeDetective;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YTR.Extensions;
using YoutubeDLSharp.Metadata;
using YTR.Classes;
using YTR.Logging;

namespace YTR.Utils
{
    public static class MimeUtil
    {
        /// <summary>
        /// Returns a list of supported YTRThumbnailData objects by checking their MIME types and file extensions.
        /// </summary>
        /// <param name="thumbnailData"></param>
        /// <returns></returns>
        public static async Task<List<YTRThumbnailData>> GetSupportedYTRThumbnailDataAsync(ThumbnailData[] thumbnailData)
        {
            var ytrThumbData = YTRThumbnailData.ConvertFromThumbnailDataArray(thumbnailData, SortPriority.Resolution);
            foreach (YTRThumbnailData td in ytrThumbData)
            {
                var (byteArray, contentType) = await HttpUtil.DownloadImageSimpleAsync(td.Url);
                if (string.IsNullOrEmpty(contentType))
                {
                    td.MimeType = await GetMimeTypeFromBytesAsync(byteArray);
                }
                else
                {
                    td.MimeType = await GetMimeTypeFromContentTypeAsync(contentType);
                }

                var extensions = await GetExtensionsFromMimeTypeAsync(td.MimeType);
                if (extensions?.Count > 0)
                {
                    td.IsWebp = extensions.Contains(".WEBP") || extensions.Contains("WEBP");
                    td.IsSupported = td.IsWebp || MimeUtil.ImageExtensions.Any(e => extensions.Any(me => me.ToLower() == e.ToLower()));
                }
            }

            return YTRThumbnailData.UpdateThumbnailOrder(ytrThumbData.Where(t => t.IsSupported)).ToList();
        }

        /// <summary>
        /// Returns a list of file extensions associated with the given MIME type.
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetExtensionsFromMimeTypeAsync(string mimeType)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var lookup = new MimeTypeToFileExtensionLookupBuilder()
                    {
                        Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
                    }.Build();

                    var extensions = lookup.TryGetValues(mimeType).Select(m => m.Extension).ToList();
                    return extensions;
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        /// <summary>
        /// Returns the MIME type from a given content type string.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<string> GetMimeTypeFromContentTypeAsync(string contentType)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var mediaType = new System.Net.Mime.ContentType(contentType);
                    string mimeType = mediaType.MediaType;
                    return mimeType;
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        /// <summary>
        /// Inspects the byte array to determine its MIME type.
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        public static async Task<string> GetMimeTypeFromBytesAsync(byte[] imageBytes)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var inspector = new ContentInspectorBuilder()
                    {
                        Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
                    }.Build();

                    var results = inspector.Inspect(imageBytes);
                    string mimeType = results.FirstOrDefault()?.Definition.File.MimeType ?? null;
                    return mimeType;
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        public static List<string> ImageExtensions = new List<string>()
        {
            "jpeg",
            "jpg",
            "png",
            "gif",
            "bmp"
        };
    }
}
