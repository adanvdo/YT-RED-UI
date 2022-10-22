using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using YT_RED.Utils;

namespace YT_RED.Settings
{
    public class GeneralSettings : FeatureSettings
	{
		public override AppFeature Feature => AppFeature.General;

        [Browsable(false)]
        [JsonProperty("setting_tab")]
		public AppFeature ActiveFeatureTab { get; set; }

        [Browsable(false)]
        [JsonProperty("show_host_warning")]
		public bool ShowHostWarning { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public string ExeDirectoryPath { get; set; }

		[Category("Error Logs")]
		[DisplayName("Error Log Path")]
		[Description("The destination folder to store error logs")]
		[EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[JsonProperty("error_logs")]
		public string ErrorLogPath { get; set; }

		[Browsable(false)]
		[JsonProperty("active_skin")]
		public string ActiveSkin { get; set; }

        [Browsable(false)]
        [JsonProperty("skin_palette")]
		public string SkinPalette { get; set; }

        [Category("Downloads")]
        [DisplayName("Best Resolution Max")]
        [Description("The maximum resolution allowed when using \"Download Best\"")]
        [JsonProperty("history_enabled")]
		public Classes.Resolution MaxResolutionBest { get; set; }

        [Category("Downloads")]
		[DisplayName("Enable Download History")]
		[Description("Enable YT-RED to keep a list of downloads for quick access on the Home screen")]
		[DefaultValue(true)]
		[JsonProperty("history_enabled")]
		public bool EnableDownloadHistory { get; set; }

		[Category("Downloads")]
		[DisplayName("History Age")]
		[Description("The number of days to keep download history if it is enabled")]
		[DefaultValue(30)]
		[JsonProperty("history_age")]
		public int HistoryAge { get; set; }

		[Category("Downloads")]
		[DisplayName("Auto-Open Download Location")]
		[Description("Automatically Open Completed Downloads in File Explorer")]
		[DefaultValue(false)]
		[JsonProperty("auto_open")]
		public bool AutomaticallyOpenDownloadLocation { get; set; }

        [Category("Downloads")]
        [DisplayName("Use Title As File Name")]
        [Description("Use the video title as the filename for all downloads (only available for non-processed downloads)\nA unique filename will be generated if this is disabled, or any post-processing features are used")]
        [JsonProperty("use_title_filename")]
        public bool UseTitleAsFileName { get; set; }

		[Category("Downloads")]
		[DisplayName("Video Download Path")]
		[Description("The destination folder for downloaded video files")]
		[EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[JsonProperty("video_dl_path")]
		public string VideoDownloadPath { get; set; }

		[Category("Downloads")]
		[DisplayName("Audio Download Path")]
		[Description("The destination folder for downloaded audio files")]
		[EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[JsonProperty("audio_dl_path")]
		public string AudioDownloadPath { get; set; }

		[Browsable(false)]
		[JsonProperty("use_preferred_format")]
		public bool UsePreferredFormat { get; set; }

		#region reddit
		[Browsable(false)]
		[JsonIgnore]
		public string RedditSampleUrl { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public string RedditMediaURLPrefix { get; set; }
		#endregion

		#region youtube
		[Browsable(false)]
		[JsonIgnore]
		public string YouTubeSampleUrl { get; set; } = @"";
        #endregion;

        public GeneralSettings()
        {
            try
            {
				ExeDirectoryPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
				ErrorLogPath = Path.Combine(ExeDirectoryPath, "ErrorLogs");
			}
			catch(Exception ex)
            {
				Logging.ExceptionHandler.LogException(ex);
				ErrorLogPath = "./ErrorLogs";
			}
			ActiveFeatureTab = AppFeature.General;
			ShowHostWarning = true;
			ActiveSkin = "WXI";
			SkinPalette = "Darkness";
			MaxResolutionBest = Classes.Resolution.ANY;
			EnableDownloadHistory = true;
			HistoryAge = 30;
			AutomaticallyOpenDownloadLocation = false;
			RedditSampleUrl = @"https://www.reddit.com/r/PraiseTheCameraMan/comments/sj7iwr/couldnt_be_more_perfect/";
			RedditMediaURLPrefix = @"https://v.redd.it/";
			YouTubeSampleUrl = @"https://www.youtube.com/watch?v=dCAORZphnlY";
			UseTitleAsFileName = false;
			VideoDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
			AudioDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
			UsePreferredFormat = false;
		}

		public override async Task<string> ValidateSettings()
        {
			if (string.IsNullOrEmpty(ErrorLogPath))
				return "Error Log Path is required";
			if (string.IsNullOrEmpty(VideoDownloadPath))
				return "You must specify a video download folder";
			if (string.IsNullOrEmpty(AudioDownloadPath))
				return "You must specify an audio download folder";
			return await base.ValidateSettings();
        }
	}	
}
