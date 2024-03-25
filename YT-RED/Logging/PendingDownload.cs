using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YTR.Settings;

namespace YTR.Logging
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

        public PendingDownload() { }
    }
}
