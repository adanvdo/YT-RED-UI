using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YT_RED.Classes;
using YT_RED.Settings;

namespace YT_RED.Logging
{
    public class PendingDownload
    {
        public string Url { get; set; }
        public string Format { get; set; }
        public TimeSpan? Start { get; set; } = null;
        public TimeSpan? Duration { get; set; } = null;
        public int[] Crops { get; set; } = null;
        public VideoFormat? VideoConversionFormat { get; set; } = null;
        public AudioFormat? AudioConversionFormat { get; set; } = null;

        public string? PrependImagePath { get; set; } = null;
        public int? PrependDuration { get; set; } = null;
        public MediaDuration? PrependDurationType { get; set; } = null;

        public string? ExternalAudioPath { get; set; } = null;
        public TimeSpan? AudioStartTime { get; set; } = null;
        public TimeSpan? AudioDuration { get; set; } = null;

        public string? ExternalImagePath { get; set; } = null;
        public TargetResolution? ImageTargetResolution { get; set; } = null;

        public PendingDownload() { }
    }
}