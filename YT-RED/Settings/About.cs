using System;
using DevExpress.Utils;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading.Tasks;

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
        public string Build { get; set; }

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

        public About()
        {
            Version = "0.0.0.0";
            Build = "Beta";
            GitHub = @"https://github.com/adanvdo/YT-RED-UI";
            Contact = @"jesse@jamgalactic.com";
        }
    }

    public enum ReleaseChannel
    {
        Stable = 0,
        Beta = 1,
        Alpha = 2
    }
}
