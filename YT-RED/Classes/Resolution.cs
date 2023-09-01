using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YT_RED.Classes
{
    public enum Resolution
    {
        [Description("480p")]
        SD = 0,
        [Description("720p")]
        HD720p = 1,
        [Description("1080p")]
        HD1080p = 2,
        [Description("2160p")]
        UHD2160p = 3,
        [Description("Any")]
        ANY = 4
    }

    public enum TargetResolution
    {
        [Description("640x480")]
        SD = 0,
        [Description("1280x720")]
        HD720p = 1,
        [Description("1920x1080")]
        HD1080p = 2,
        [Description("3840x2160")]
        UHD2160p = 3,
        [Description("Source")]
        Source = 4
    }
}
