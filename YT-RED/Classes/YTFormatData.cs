using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp.Metadata;


namespace YT_RED.Classes
{
    public class YTDLFormatData : FormatData
    {
        public FormatData RedditAudioFormat { get; set; }

        public TimeSpan? Duration { get; set; }

        public StreamType Type
        {
            get
            {
                if ((this.VideoCodec == null || this.VideoCodec == "none") && (this.AudioCodec != null && this.AudioCodec != "none"))
                    return StreamType.Audio;
                else if (this.VideoCodec != null && this.AudioCodec != null && this.AudioCodec != "none")
                    return StreamType.AudioAndVideo;
                else return StreamType.Video;
            }
        }

        public YTDLFormatData() : base()
        { }

        public YTDLFormatData(YoutubeDLSharp.Metadata.VideoData gifData, Xabe.FFmpeg.IMediaInfo iMediaInfo = null)
        {
            this.Url = gifData.Url;
            this.RedditAudioFormat = null;
            if(iMediaInfo != null && iMediaInfo.VideoStreams.Count() > 0)
            {
                this.Duration = (TimeSpan?)iMediaInfo.Duration;
                var stream = iMediaInfo.VideoStreams.FirstOrDefault();
                this.VideoCodec = "gif";
                this.Width = stream.Width;
                this.Height = stream.Height;
                this.FileSize = (long?)iMediaInfo.Size;
                this.FrameRate = (float?)stream.Framerate;
            }
            else
            {
                this.Duration = gifData.Duration.HasValue ? (TimeSpan?)TimeSpan.FromSeconds((double)gifData.Duration) : null;
            }
            this.AudioCodec = "none";
            this.ManifestUrl = String.Empty;
            this.Extension = gifData.Extension;
            this.Format = iMediaInfo != null ? $"GIF - {this.Width}x{this.Height}" : "Unknown";
            this.FormatId = "gif";
            this.FormatNote = "Animated GIF";
            this.ContainerFormat = "gif";

        }

        public YTDLFormatData(FormatData formatData, float? duration, FormatData redditAudioFormat = null) : base()
        {
            this.Url = formatData.Url;
            this.RedditAudioFormat = redditAudioFormat;
            this.ManifestUrl = formatData.ManifestUrl;
            this.Extension = formatData.Extension;
            this.Format = formatData.Format;
            this.FormatId = formatData.FormatId;
            if (redditAudioFormat != null)
            {
                this.Format += $" + {redditAudioFormat.Format.Replace(" - audio only (audio 0)", "").Replace(" - audio only (DASH audio)", "")}";
                this.FormatId += $"+{redditAudioFormat.FormatId}";
            }
            this.FormatNote = formatData.FormatNote;
            this.Duration = duration.HasValue ? (TimeSpan?)TimeSpan.FromSeconds((double)duration) : null;
            this.Width = formatData.Width;
            this.Height = formatData.Height;
            this.Resolution = formatData.Resolution;
            this.Bitrate = formatData.Bitrate;
            this.AudioBitrate = redditAudioFormat != null ? redditAudioFormat.AudioBitrate : formatData.AudioBitrate;
            this.AudioCodec = redditAudioFormat != null ? redditAudioFormat.AudioCodec : formatData.AudioCodec;
            this.AudioSamplingRate = redditAudioFormat != null ? redditAudioFormat.AudioSamplingRate : formatData.AudioSamplingRate;
            this.VideoBitrate = formatData.VideoBitrate;
            this.FrameRate = formatData.FrameRate;
            this.VideoCodec = formatData.VideoCodec;
            this.ContainerFormat = string.IsNullOrEmpty(formatData.ContainerFormat) ? this.Extension : formatData.ContainerFormat;
            this.FileSize = formatData.FileSize;
            this.ApproximateFileSize = formatData.ApproximateFileSize;
            this.PlayerUrl = formatData.PlayerUrl;
            this.Protocol = formatData.Protocol;
            this.FragmentBaseUrl = formatData.FragmentBaseUrl;
            this.Preference = formatData.Preference;
            this.Language = formatData.Language;
            this.LanguagePreference = formatData.LanguagePreference;
            this.Quality = formatData.Quality;
            this.SourcePreference = formatData.SourcePreference;
            this.StretchedRatio = formatData.StretchedRatio;
            this.NoResume = formatData.NoResume;
        }

        public static List<string> ExcludeFormatIDs = new List<string>()
        {
            "sb0",
            "sb1",
            "sb2",
            "http-240p",
            "http-360p",
            "http-540p",
            "http-720p",
            "http-1080p"
        };
    }    
}
