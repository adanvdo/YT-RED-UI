using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xabe.FFmpeg;

namespace YTR.Classes
{
    public class MediaStream
    {
        [JsonProperty("StreamType")]
        public StreamType StreamType { get; set; }
        public StreamPlaylistType PlaylistType { get; set; }
        [JsonProperty("Width")]
        public int Width { get; set; }
        [JsonProperty("Height")]
        public int Height { get; set; }
        [JsonProperty("Framerate")]
        public decimal FrameRate { get; set; }
        [JsonProperty("Ratio")]
        public string Ratio { get; set; }
        [JsonProperty("Codec")]
        public string Codec { get; set; }
        [JsonProperty("Path")]
        public string Path { get; set; }
        [JsonProperty("Duration")]
        public TimeSpan Duration { get; set; }
        [JsonProperty("Bitrate")]
        public int Bitrate { get; set; }
        [JsonProperty("Channels")]
        public int Channels { get; set; }
        [JsonProperty("SampleRate")]
        public int SampleRate { get; set; }
        
        public IStream DeliverStream { get; set; }

        public MediaStream()
        {
        }
    }    
}
