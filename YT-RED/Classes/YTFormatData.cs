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
        public TimeSpan? Duration { get; set; }

        public YTDLFormatData() : base()
        { }

        public YTDLFormatData(FormatData formatData, float? duration) : base()
        {
            this.Url = formatData.Url;
            this.ManifestUrl = formatData.ManifestUrl;
            this.Extension = formatData.Extension;
            this.Format = formatData.Format;
            this.FormatId = formatData.FormatId;
            this.FormatNote = formatData.FormatNote;
            this.Duration = duration.HasValue ? (TimeSpan?)TimeSpan.FromSeconds((double)duration) : null;
            this.Width = formatData.Width;
            this.Height = formatData.Height;
            this.Resolution = formatData.Resolution;
            this.Bitrate = formatData.Bitrate;
            this.AudioBitrate = formatData.AudioBitrate;
            this.AudioCodec = formatData.AudioCodec;
            this.AudioSamplingRate = formatData.AudioSamplingRate;
            this.VideoBitrate = formatData.VideoBitrate;
            this.FrameRate = formatData.FrameRate;
            this.VideoCodec = formatData.VideoCodec;
            this.ContainerFormat = formatData.ContainerFormat;
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
