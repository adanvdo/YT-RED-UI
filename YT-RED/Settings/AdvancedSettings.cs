using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YT_RED.Settings
{
    public class AdvancedSettings : FeatureSettings
    {
        public override AppFeature Feature => AppFeature.Advanced;

        [Category("Updates")]
        [DisplayName("Release Channel")]
        [Description("The type of releases to include when checking for updates.\nSelecting the Beta channel will include Beta and Stable. Alpha will include all releases.")]
        [JsonProperty("release_channel")]
        public ReleaseChannel Channel { get; set; }

        [Category("Hotkeys")]
        [DisplayName("Enable Hotkeys")]
        [Description(@"Enables and Registers Hotkey for Quick Download Feature
YT-RED will not register any Hotkeys or access your clipboard data if this is disabled")]
        [DefaultValue(false)]
        [JsonProperty("enable_hotkeys")]
        public bool EnableHotKeys { get; set; }

        [Category("Hotkeys")]
        [DisplayName("Quick Download Hotkey")]
        [Description(@"Register a Hotkey to perform Quick Downloads without needing YT-RED open in the foreground.

How it works:
Highlight the YouTube or Reddit Media URL in your browser address bar and press your configured Hotkey.
YT-DLP will store the highlighted URL on your clipboard, and use the clipboard value to start a download from the System Tray.
If there is existing text in your clipboard, YT-RED will restore it after starting the download.
")]
        [JsonProperty("dl_hotkey")]
        public Shortcut DownloadHotKey { get; set; }

        [Category("Processing")]
        [DisplayName("Generate Missing Metadata")]
        [Description("When enabled, YT-RED will analyze fetched formats that are missing useful Metadata.\n"
            +"(Only applies when loading a format list)")]
        [JsonProperty("fetch_metadata")]
        public bool GetMissingMetadata { get; set; }

        [Category("Processing")]
        [DisplayName("Always Convert Format")]
        [Description("Always convert video and audio downloads to your preferred format")]
        [JsonProperty("always_convert")]
        public bool AlwaysConvertToPreferredFormat { get; set; }

        [Category("Processing")]
        [DisplayName("Preferred Video Format")]
        [Description("Prefer this format when downloading \"Preferred\" video")]
        [JsonProperty("preferred_video_format")]
        public VideoFormat PreferredVideoFormat { get; set; }

        [Category("Processing")]
        [DisplayName("Preferred Audio Format")]
        [Description("Prefer this format when downloading \"preferred\" audio")]
        [JsonProperty("preferred_audio_format")]
        public AudioFormat PreferredAudioFormat { get; set; }

        [Category("Debug")]
        [DisplayName("Enable Verbose Output")]
        [Description("When enabled, displays full downloader arguments when starting a download")]
        [JsonProperty("verbose_output")]
        public bool VerboseOutput { get; set; }

        public AdvancedSettings()
        {
            Channel = ReleaseChannel.Stable;
            EnableHotKeys = false;
            DownloadHotKey = Shortcut.None;
            GetMissingMetadata = true;
            AlwaysConvertToPreferredFormat = false;
            PreferredVideoFormat = VideoFormat.UNSPECIFIED;
            PreferredAudioFormat = AudioFormat.UNSPECIFIED;
            VerboseOutput = false;
        }

        public override async Task<string> ValidateSettings()
        {
            return await base.ValidateSettings();
        }
    }
}
