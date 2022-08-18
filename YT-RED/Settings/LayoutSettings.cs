using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace YT_RED.Settings
{
    public class LayoutSettings : FeatureSettings
    {
        public override AppFeature Feature => AppFeature.Layout;

        [Category("Format List")]
        [DisplayName("Format Selection Mode")]
        [Description("Controls the Selection Mode of the Format Grid when listing formats.\n"
            + "Preset Mode: Generates a list of preset video+audio formats\n"
            + "Custom Mode: Allows user to select the available video and audio format of their choice")]
        [JsonProperty("format_mode")]
        public FormatMode FormatMode { get; set; }

        public LayoutSettings()
        {
            this.FormatMode = FormatMode.Preset;
        }

        public override async Task<string> ValidateSettings()
        {
            return await base.ValidateSettings();
        }
    }

    public enum FormatMode
    {
        Preset = 0,
        Custom = 1
    }

    public enum LayoutArea
    {
        All = 0,
        FormatList = 1
    }
}
