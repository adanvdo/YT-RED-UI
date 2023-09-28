using System;
using System.IO;
using YTR.Classes;
using Newtonsoft.Json;
using YTR.Settings;

namespace YTR.Logging
{
    public class DownloadLog
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("in_subfolder")]
        public bool InSubFolder { get; set; } = false;
        [JsonProperty("playlist_title")]
        public string PlaylistTitle { get; set; } = "";
        [JsonProperty("playlist_url")]
        public string PlaylistUrl { get; set; } = "";
        [JsonProperty("dl_type")]
        public DownloadType DownloadType { get; set; }
        [JsonProperty("type")]
        public StreamType StreamType { get; set; }
        [JsonProperty("downloaded")]
        public DateTime Downloaded { get; set; }
        [JsonProperty("location")]
        public string DownloadLocation { get; set; }
        [JsonProperty("time_logged")]
        public DateTime TimeLogged { get; set; }
        [JsonProperty("format")]
        public string Format { get; set; }
        [JsonIgnore]
        public bool AdditionalSettings
        {
            get { return Start != null || Duration != null || Crops != null || VideoConversionFormat != null || AudioConversionFormat != null || MaxResolution != null || MaxFileSize != null; }
        }
        [JsonProperty("format_pair")]
        public YTDLFormatPair FormatPair { get; set; }
        [JsonProperty("start")]
        public TimeSpan? Start { get; set; } = null;
        [JsonProperty("duration")]
        public TimeSpan? Duration { get; set; } = null;
        [JsonProperty("crops")]
        public int[] Crops { get; set; } = null;
        [JsonProperty("video_conversion")]
        public VideoFormat? VideoConversionFormat { get; set; } = null;
        [JsonProperty("audio_conversion")]
        public AudioFormat? AudioConversionFormat { get; set; } = null;
        [JsonProperty("max_resolution")]
        public Resolution? MaxResolution { get; set; } = null;
        [JsonProperty("max_filesize")]
        public int? MaxFileSize { get; set; } = null;

        [JsonIgnore]
        public bool FileExists
        {
            get
            {
                return !string.IsNullOrEmpty(DownloadLocation) && File.Exists(DownloadLocation);
            }
        }

        [JsonProperty("segment_mode")]
        public SegmentControlMode SegmentMode { get; set; } = SegmentControlMode.Duration;

        public DownloadLog()
        { }

        public DownloadLog(string url, DownloadType dlType, StreamType type, DateTime downloaded, string location, PendingDownload pendingDownload = null)
        {
            Url = url;
            DownloadType = dlType;
            StreamType = type;
            Downloaded = downloaded;
            DownloadLocation = location; 
            TimeLogged = DateTime.Now;
            if(pendingDownload != null)
            {
                Format = pendingDownload.Format;
                Start = pendingDownload.Start;
                Duration = pendingDownload.Duration;
                Crops = pendingDownload.Crops;
                VideoConversionFormat = pendingDownload.VideoConversionFormat;
                AudioConversionFormat = pendingDownload.AudioConversionFormat;
            }
            SegmentMode = AppSettings.Default.Layout.SegmentControlMode;
        }
    }
}
