using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YT_RED.Classes
{
    public class YTDLFormatPair
    {
        [JsonProperty("stream_type")]
        public StreamType Type { get; set; }

        private YTDLFormatData videoFormat;
        [JsonProperty("video_format")]
        public YTDLFormatData VideoFormat
        {
            get { return this.videoFormat; }
            set
            {
                this.videoFormat = value;
                if (this.videoFormat != null && (this.audioFormat != null || (!string.IsNullOrEmpty(this.videoFormat.AudioCodec) && this.videoFormat.AudioCodec != "none")))
                {
                    this.Type = StreamType.AudioAndVideo;
                }
                else if (this.videoFormat != null)
                {
                    this.Type = StreamType.Video;
                }
                else if (this.audioFormat != null)
                {
                    this.Type = StreamType.Audio;
                }
                else this.Type = StreamType.Unknown;
            }
        }

        [JsonIgnore]
        public int? Width
        {
            get {
                if (this.videoFormat != null) return (int)this.videoFormat.Width;
                else return null;
            }
        }

        [JsonIgnore] 
        public int? Height
        {
            get { return (int)this.videoFormat?.Height; }
        }

        private YTDLFormatData audioFormat;
        [JsonProperty("audio_format")]
        public YTDLFormatData AudioFormat
        {
            get { return this.audioFormat; }
            set
            {
                this.audioFormat = value;
                if (this.videoFormat != null && this.audioFormat != null)
                {
                    this.Type = StreamType.AudioAndVideo;
                }
                else if (this.videoFormat != null)
                {
                    this.Type = StreamType.Video;
                }
                else if (this.audioFormat != null)
                {
                    this.Type = StreamType.Audio;
                }
                else this.Type = StreamType.Unknown;
            }
        }

        [JsonProperty("format_id")]
        public string FormatId
        {
            get
            {
                List<string> ids = new List<string>();
                if (this.audioFormat != null) ids.Add(this.audioFormat.FormatId);
                if (this.videoFormat != null) ids.Add(this.videoFormat.FormatId);

                return string.Join("+", ids);
            }
        }

        [JsonIgnore]
        public string Extension
        {
            get
            {
                if (this.videoFormat != null)
                    return this.videoFormat.Extension;

                if (this.audioFormat != null)
                    return this.audioFormat.Extension;

                return string.Empty;
            }
        }

        [JsonIgnore]
        public TimeSpan? Duration
        {
            get
            {
                if (this.videoFormat != null) return this.videoFormat.Duration;
                else if (this.audioFormat != null) return this.audioFormat.Duration;
                return null;
            }
        }

        [JsonIgnore]
        public string VideoCodec
        {
            get
            {
                if (this.videoFormat != null) return this.videoFormat.VideoCodec;
                else if (this.audioFormat != null) return this.audioFormat.VideoCodec;
                else return null;
            }
        }

        [JsonIgnore]
        public string AudioCodec
        {
            get
            {
                if (this.audioFormat != null) return this.audioFormat.AudioCodec;
                else return this.videoFormat.AudioCodec;
            }
        }

        [JsonIgnore]
        public YoutubeDLSharp.Metadata.FormatData RedditAudioFormat
        {
            get
            {
                if (this.audioFormat != null && this.audioFormat.RedditAudioFormat != null)
                    return this.audioFormat.RedditAudioFormat;
                else if (this.videoFormat != null && this.videoFormat.RedditAudioFormat != null)
                    return this.videoFormat.RedditAudioFormat;
                return null;
            }
        }

        [JsonProperty("format_text")]
        public string FormatDisplayText { 
            get
            {
                string text = "";
                if (VideoFormat == null && AudioFormat == null)
                    return text;

                if(VideoFormat != null)
                {
                    text += VideoFormat.Format;
                }

                if(AudioFormat != null)
                {
                    text += VideoFormat != null ? $" + {AudioFormat.Format}" : AudioFormat.Format;
                }
                return text;
            } 
        }

        public void Clear()
        {
            this.Type = StreamType.Unknown;
            this.VideoFormat = null;
            this.AudioFormat = null;
        }

        public bool IsValid()
        {
            return this.videoFormat != null || this.audioFormat != null;
        }

        public YTDLFormatPair()
        {
        }
    }
}
