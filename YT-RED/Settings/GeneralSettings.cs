using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YT_RED.Settings
{
	public class GeneralSettings : FeatureSettings
	{
		public override AppFeature Feature => AppFeature.General;


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

		[Category("Downloads")]
		[DisplayName("Video Download Path")]
		[Description("The destination folder for downloaded video files")]
		[EditorAttribute(typeof(System.Windows.Forms.FolderBrowserDialog), typeof(System.Drawing.Design.UITypeEditor))]
		[JsonProperty("video_dl_path")]
		public string VideoDownloadPath { get; set; }

		[Category("Downloads")]
		[DisplayName("Audio Download Path")]
		[Description("The destination folder for downloaded audio files")]
		[EditorAttribute(typeof(System.Windows.Forms.FolderBrowserDialog), typeof(System.Drawing.Design.UITypeEditor))]
		[JsonProperty("audio_dl_path")]
		public string AudioDownloadPath { get; set; }

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
			EnableDownloadHistory = true;
			HistoryAge = 30;
			RedditSampleUrl = @"https://www.reddit.com/r/PraiseTheCameraMan/comments/sj7iwr/couldnt_be_more_perfect/";
			RedditMediaURLPrefix = @"https://v.redd.it/";
			RedditDefaultVideoSuffix = @"/DASH_{0}.mp4?source=fallback";
			RedditDefaultAudioSuffix = @"/DASH_audio.mp4?source=fallback";
			YouTubeSampleUrl = @"https://www.youtube.com/watch?v=dCAORZphnlY";
			VideoDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
			AudioDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
		}

		public override async Task<string> ValidateSettings()
        {
			if (string.IsNullOrEmpty(VideoDownloadPath))
				return "You must specify a video download folder";
			if (string.IsNullOrEmpty(AudioDownloadPath))
				return "You must specify an audio download folder";
			return await base.ValidateSettings();
        }
	}
}
