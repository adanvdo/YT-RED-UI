using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YT_RED_UI.Classes
{
    public class RedditStream
    {
        [JsonProperty("StreamType")]
        public StreamType StreamType { get; set; }
        public StreamVideoType VideoType { get; set; }
        [JsonProperty("Width")]
        public int Width { get; set; }
        [JsonProperty("Height")]
        public int Height { get; set; }
        [JsonProperty("Framerate")]
        public decimal FrameRate { get; set; }
        [JsonProperty("Codec")]
        public string Codec { get; set; }
        [JsonProperty("Path")]
        public string Path { get; set; }
        [JsonProperty("Duration")]
        public TimeSpan Duration { get; set; }

        public RedditStream()
        {
        }

        public RedditStream(StreamType t, StreamVideoType vt, int w, int h, decimal fr, string codec, string path, string duration)
        {
            StreamType = t;
            VideoType = vt;
            Width = w;
            Height = h;
            FrameRate = fr;
            Codec = codec;
            Path = path;
            TimeSpan interval = TimeSpan.ParseExact(duration, @"h\:mm\:ss\.fff", CultureInfo.CurrentCulture, TimeSpanStyles.AssumeNegative);
            Duration = interval;
        }
    }

    public enum StreamType
    {
        Video = 0,
        Audio = 1
    }
}
