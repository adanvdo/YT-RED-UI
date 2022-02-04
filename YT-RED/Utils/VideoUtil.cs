using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Newtonsoft.Json;
using YT_RED.Settings;

namespace YT_RED.Utils
{
    public static class VideoUtil
    {
        public static string GetRedditVideoID(string m3u8URL)
        {
            if (m3u8URL == null)
                throw new ArgumentNullException("m3U8 URL was null");
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

        public static async Task<IConversion> PrepareDashConversion(string videoUrl, string audioUrl)
        {
            string outputDir = AppSettings.Default.General.VideoDownloadPath;
            string fileName = DateTime.Now.ToString("MMddyyyyhhmmss") + ".mp4";
            string outputFile = Path.Combine(outputDir, fileName);

            IMediaInfo videoInfo = await FFmpeg.GetMediaInfo(videoUrl);
            IMediaInfo audioInfo = await FFmpeg.GetMediaInfo(audioUrl);
            IStream v = videoInfo.VideoStreams.FirstOrDefault();
            IStream a = audioInfo.AudioStreams.FirstOrDefault();

            var convert = FFmpeg.Conversions.New()
                .AddStream(v, a)
                .SetOutput(outputFile);
            return convert;
        }

        public static IConversion PrepareConversion(Classes.ResultStream stream)
        {
            string outputDir = stream.StreamType == Classes.StreamType.Audio ? AppSettings.Default.General.AudioDownloadPath : AppSettings.Default.General.VideoDownloadPath;
            string fileName = DateTime.Now.ToString("MMddyyyyhhmmss")+ (stream.StreamType == Classes.StreamType.Audio ? ".m4a" : ".mp4");
            string outputFile = Path.Combine(outputDir, fileName);

            if(stream.StreamType == Classes.StreamType.AudioAndVideo)
            {
                var convert = FFmpeg.Conversions.New()
                    .AddStream(stream.AudioStream, stream.VideoStream)
                    .SetOutput(outputFile);
                return convert;
            }
            else if (stream.StreamType == Classes.StreamType.Video)
            {
                var convert = FFmpeg.Conversions.New()
                    .AddStream(stream.VideoStream)
                    .SetOutput(outputFile);
                return convert;
            }
            else {
                var convert = FFmpeg.Conversions.New()
                    .AddStream(stream.AudioStream)
                    .SetOutput(outputFile);
                return convert;
            }
        }

        public static async Task<List<Classes.ResultStream>> ConsolidateStreams(List<Classes.RedditStream> streams)
        {
            return await Task.Run(() =>
            {
                List<Classes.ResultStream> consolidated = new List<Classes.ResultStream>();
                List<Classes.RedditStream> video = streams.Where(s => s.StreamType == Classes.StreamType.Video).ToList();
                List<Classes.RedditStream> audio = streams.Where(s => s.StreamType == Classes.StreamType.Audio).ToList();

                int row = 1;
                foreach (Classes.RedditStream vs in video)
                {
                    consolidated.Add(new Classes.ResultStream(vs) { Row = row });
                    row++;
                }

                foreach (Classes.RedditStream s in audio)
                {
                    foreach (Classes.RedditStream vs in video)
                    {
                        consolidated.Add(new Classes.ResultStream(vs, s) { Row = row });
                        row++;
                    }
                }

                foreach (Classes.RedditStream vs in audio)
                {
                    consolidated.Add(new Classes.ResultStream(vs) { Row = row });
                    row++;
                }

                return consolidated;
            });
        }

        public static string RedditAudioUrl(string id)
        {
            return $@"{YT_RED.Settings.AppSettings.Default.General.RedditMediaURLPrefix}{id}{YT_RED.Settings.AppSettings.Default.General.RedditDefaultAudioSuffix}";
        }
    }
}
