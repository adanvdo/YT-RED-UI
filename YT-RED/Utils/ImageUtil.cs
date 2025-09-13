using SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using YTR.Logging;

namespace YTR.Utils
{
    public static class ImageUtil
    {
        public static async Task<byte[]> WebpToPng(byte[] webpBytes)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using SKData data = SKData.CreateCopy(webpBytes);
                    using SKBitmap bmp = SKBitmap.Decode(data);
                    using SKImage img = SKImage.FromBitmap(bmp);
                    using SKData pngData = img.Encode(SKEncodedImageFormat.Png, 100);
                    return pngData.ToArray();
                });
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        /// <summary>
        /// Convert a webp image stream to a png image stream
        /// </summary>
        /// <param name="webpStream"></param>
        /// <returns></returns>
        public static async Task<MemoryStream> WebpToPngStream(Stream webpStream)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using SKBitmap bmp = SKBitmap.Decode(webpStream);
                    using SKImage img = SKImage.FromBitmap(bmp);
                    using SKData pngData = img.Encode(SKEncodedImageFormat.Png, 100);
                    return new MemoryStream(pngData.ToArray());
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }
    }
}
