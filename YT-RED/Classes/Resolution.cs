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
}
