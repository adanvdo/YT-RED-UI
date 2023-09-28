using System;
using DevExpress.Utils;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading.Tasks;
using YT_RED.Classes;

namespace YT_RED.Settings
{
    public class About : FeatureSettings
    {       
        public override AppFeature Feature => AppFeature.About;

        private Version version;        

        [Category("About")]
        [DisplayName("Version")]
        [Description("Current Version")]
        [JsonIgnore]
        public string Version { get { return version.ToString(); } set { version = new System.Version(value); } }

        [Category("About")]
        [DisplayName("Build")]
        [Description("Current Build")]
        [JsonIgnore]
        public string Build
        {
            get { return ReleaseChannel.ToString(); }
            set { ReleaseChannel = EnumExtensions.ToEnum<ReleaseChannel>(value); }
        }

        [Browsable(false)]
        [JsonIgnore]
        public ReleaseChannel ReleaseChannel { get; set; }

        [Category("About")]
        [DisplayName("GitHub")]
        [Description("GitHub Repository")]
        [JsonIgnore]
        public string GitHub { get; set; }

        [Category("About")]
        [DisplayName("Contact")]
        [Description("Developer Contact")]
        [JsonIgnore]
        public string Contact { get; set; }

        [Category("About")]
        [DisplayName("Dependencies")]
        [Description("YT-RED depends on the YT-DLP and FFMPEG Libraries for dowloading and processing media.\nIf you suddenly start getting errors during downloads, these tools may need to be updated.")]
        [JsonIgnore]
        public string Dependencies { get; set; }

        public About()
        {
            Version = "0.0.0.0";
            ReleaseChannel = ReleaseChannel.Alpha;
            GitHub = @"https://github.com/adanvdo/YT-RED-UI";
            Contact = @"jesse@jamgalactic.com";
            Dependencies = "YT-DLP + FFMPEG";
        }
    }    
}
