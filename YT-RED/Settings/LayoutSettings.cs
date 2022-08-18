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

        [Category("Panels")]
        [DisplayName("Input Panel Position")]
        [Description("Changes the position of the Url Input Panel")]
        [JsonProperty("input_pnl_pos")]
        public VerticalPanelPosition InputPanelPosition { get; set; }

        [Category("Panels")]
        [DisplayName("Control Panel Position")]
        [Description("Changes the position of the Filter and Download Control Panel")]
        [JsonProperty("ctrl_pnl_pos")]
        public HorizontalPanelPosition ControlPanelPosition { get; set; }

        [Category("Format List")]
        [DisplayName("Format Selection Mode")]
        [Description("Preset: Generates a list of preset video+audio formats\n"
            + "Custom: Allows user to select the available video and audio format of their choice")]
        [JsonProperty("format_mode")]
        public FormatMode FormatMode { get; set; }

        public LayoutSettings()
        {
            this.InputPanelPosition = VerticalPanelPosition.Top;
            this.ControlPanelPosition = HorizontalPanelPosition.Right;
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
        FormatList = 1,
        Panels = 2
    }

    public enum HorizontalPanelPosition
    {        
        Left = 0,
        Right = 1
    }

    public enum VerticalPanelPosition
    {
        Top = 0,
        Bottom = 1
    }
}
