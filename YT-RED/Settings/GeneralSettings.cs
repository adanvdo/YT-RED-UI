using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace YT_RED.Settings
{
    public class GeneralSettings : FeatureSettings
	{
		public override AppFeature Feature => AppFeature.General;

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

		[Category("Preferences")]
		[DisplayName("Enable Download History")]
		[Description("Enable YT-RED to keep a list of downloads for quick access on the Home screen")]
		[DefaultValue(true)]
		[JsonProperty("history_enabled")]
		public bool EnableDownloadHistory { get; set; }

		[Category("Preferences")]
		[DisplayName("History Age")]
		[Description("The number of days to keep download history if it is enabled")]
		[DefaultValue(30)]
		[JsonProperty("history_age")]
		public int HistoryAge { get; set; }

		[Category("Preferences")]
		[DisplayName("Auto-Open Download Location")]
		[Description("Automatically Open Completed Downloads in File Explorer")]
		[DefaultValue(false)]
		[JsonProperty("auto_open")]
		public bool AutomaticallyOpenDownloadLocation { get; set; }

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

		[Browsable(false)]
		[JsonIgnore]
		public string RedditDefaultVideoSuffix { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public string RedditDefaultAudioSuffix { get; set; }
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
				YT_RED.Controls.YTRErrorMessageBox.ErrorMessageBox(ex).ShowDialog();
				ErrorLogPath = "./ErrorLogs";
			}
			ActiveSkin = "DevExpress Dark Style";
			EnableDownloadHistory = true;
			HistoryAge = 30;
			AutomaticallyOpenDownloadLocation = false;
			RedditSampleUrl = @"https://www.reddit.com/r/PraiseTheCameraMan/comments/sj7iwr/couldnt_be_more_perfect/";
			RedditMediaURLPrefix = @"https://v.redd.it/";
			RedditDefaultVideoSuffix = @"/DASH_{0}.mp4?source=fallback";
			RedditDefaultAudioSuffix = @"/DASH_audio.mp4?source=fallback";
			YouTubeSampleUrl = @"https://www.youtube.com/watch?v=dCAORZphnlY";
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
