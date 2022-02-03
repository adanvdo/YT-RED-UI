using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YT_RED.Classes
{
    public class RedditStream
    {
        [JsonProperty("StreamType")]
        public StreamType StreamType { get; set; }
        [JsonProperty("VideoType")]
        public StreamPlaylistType? VideoType { get; set; }

        [JsonProperty("Width")]
        public int? Width { get; set; }
        [JsonProperty("Height")]
        public int? Height { get; set; }
        [JsonProperty("Framerate")]
        public decimal? FrameRate { get; set; }
        [JsonProperty("Codec")]
        public string VideoCodec { get; set; }
        [JsonProperty("Path")]
        public string VideoPath { get; set; }
        [JsonProperty("VideoDuration")]
        public TimeSpan? VideoDuration { get; set; }
        [JsonProperty("Channels")]
        public int? Channels { get; set; }
        [JsonProperty("SampleRate")]
        public int? SampleRate { get; set; }
        [JsonProperty("AudioCodec")]
        public string AudioCodec { get; set; }
        [JsonProperty("AudioDuration")]
        public TimeSpan? AudioDuration { get; set; }
        [JsonProperty("AudioPath")]
        public string AudioPath { get; set; }


        public RedditStream()
        {
        }

        public RedditStream(StreamPlaylistType vt, int w, int h, decimal fr, string videoCodec, string videoPath, string videoDuration)
        {
            StreamType = StreamType.Video;
            VideoType = vt;
            Width = w;
            Height = h;
            FrameRate = fr;
            VideoCodec = videoCodec;
            VideoPath = videoPath;
            TimeSpan interval = TimeSpan.ParseExact(videoDuration, @"h\:mm\:ss\.fff", CultureInfo.CurrentCulture, TimeSpanStyles.AssumeNegative);
            VideoDuration = interval;
            Channels = null;
            SampleRate = null;
            AudioCodec = null;
            AudioDuration = null;
            AudioPath = null;
        }

        public RedditStream(int channels, int sampleRate, string audioCodec, TimeSpan audioDuration, string audioPath)
        {
            StreamType = StreamType.Audio;
            Channels = channels;
            SampleRate = sampleRate;
            AudioCodec = audioCodec;
            AudioDuration = audioDuration;
            AudioPath = audioPath;
            VideoType = null;
            Width = null;
            Height = null;
            FrameRate = null;
            VideoCodec = null;
            VideoPath = null;
            VideoDuration = null;
        }

        public RedditStream(StreamPlaylistType vt, int w, int h, decimal fr, string videoCodec, string videoPath, string videoDuration
            , int channels, int sampleRate, string audioCodec, TimeSpan audioDuration, string audioPath)
        {
            StreamType = StreamType.VideoAndAudio;
            VideoType = vt;
            Width = w;
            Height = h;
            FrameRate = fr;
            VideoCodec = videoCodec;
            VideoPath = videoPath;
            TimeSpan interval = TimeSpan.ParseExact(videoDuration, @"h\:mm\:ss\.fff", CultureInfo.CurrentCulture, TimeSpanStyles.AssumeNegative);
            VideoDuration = interval;
            Channels = channels;
            SampleRate = sampleRate;
            AudioCodec = audioCodec;
            AudioDuration = audioDuration;
            AudioPath = audioPath;
        }
    }

    public enum StreamType
    {
        Video = 0,
        Audio = 1,
        VideoAndAudio = 2
    }
}
