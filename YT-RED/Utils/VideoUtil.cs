using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Newtonsoft.Json;

namespace YT_RED.Utils
{
    public static class VideoUtil
    {
        public static string GetRedditVideoID(string m3u8URL)
        {
            string id = m3u8URL.Replace(@"https://v.redd.it/", "");
            int endex = id.IndexOf('/');
            id = id.Substring(0, endex);
            return id;
        }

        public static async Task<IMediaInfo> ParseM3U8(string url)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(url);
            return mediaInfo;
        }

        public static async Task<string> ManifestStream(IMediaInfo mediaInfo)
        {
            string dlPath = @"C:\\Users\jesse\downloads";
            string outputName = Path.Combine(dlPath, Path.ChangeExtension(Path.GetTempFileName(), ".mp4"));

            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.h264);
            IStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(AudioCodec.mp3);

            var conversion = FFmpeg.Conversions.New()
                .AddStream(audioStream, videoStream)
                .SetOutput(outputName);

            await conversion.Start();
            return outputName;
        }


    }
}
