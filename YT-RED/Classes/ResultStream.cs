using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xabe.FFmpeg;

namespace YT_RED.Classes
{
    public class ResultStream
    {
        public int Row { get; set; }
        public StreamType StreamType { get; set; }
        public StreamPlaylistType PlaylistType { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string FrameRate { get; set; }
        public string VideoCodec { get; set; }
        public string VideoPath { get; set; }
        public TimeSpan Duration { get; set; }
        public string AudioCodec { get; set; }
        public string AudioPath { get; set; }
        public string AudioBitrate { get; set; }
        public string AudioChannels { get; set; }
        public string AudioSampleRate { get; set; }

        public IStream VideoStream { get; set; }
        public IStream AudioStream { get; set; }        

        public ResultStream(MediaStream stream)
        {
            Row = -1;
            StreamType = stream.StreamType;
            PlaylistType = stream.PlaylistType;
            Duration = stream.Duration;
            if (stream.StreamType == StreamType.Video)
            {
                VideoStream = stream.DeliverStream;
                Width = stream.Width.ToString();
                Height = stream.Height.ToString();
                FrameRate = stream.FrameRate.ToString();
                VideoCodec = stream.Codec;
                VideoPath = stream.Path;
                AudioCodec = String.Empty;
                AudioPath = String.Empty;
                AudioBitrate = String.Empty;
                AudioChannels = String.Empty;
                AudioSampleRate = String.Empty;
            }
            else if(stream.StreamType == StreamType.Audio)
            {
                AudioStream = stream.DeliverStream;
                Width = String.Empty;
                Height = String.Empty;
                FrameRate = string.Empty;
                VideoCodec = String.Empty;
                VideoPath = String.Empty;
                AudioCodec = stream.Codec;
                AudioPath = stream.Path;
                AudioBitrate = stream.Bitrate.ToString();
                AudioChannels = stream.Channels.ToString();
                AudioSampleRate = stream.SampleRate.ToString();
            }

        }

        public ResultStream(MediaStream video, MediaStream audio)
        {
            Row = -1;
            VideoStream = video.DeliverStream;
            AudioStream = audio.DeliverStream;
            StreamType = StreamType.AudioAndVideo;
            PlaylistType = video.PlaylistType;
            Width = video.Width.ToString();
            Height= video.Height.ToString();
            FrameRate= video.FrameRate.ToString();
            VideoCodec= video.Codec;
            VideoPath= video.Path;
            AudioCodec = audio.Codec;
            AudioPath= audio.Path;
            AudioBitrate = audio.Bitrate.ToString();
            AudioChannels= audio.Channels.ToString();
            AudioSampleRate= audio.SampleRate.ToString();
        }
    }
}
