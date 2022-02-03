using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YT_RED_UI.Settings
{
	public class GeneralSettings : FeatureSettings
	{
		public override AppFeature Feature => AppFeature.General;

		[Category("Downloads")]
		[DisplayName("Video Download Path")]
		[Description("The destination folder for downloaded video files")]
		[EditorAttribute(typeof(System.Windows.Forms.FolderBrowserDialog), typeof(System.Drawing.Design.UITypeEditor))]
		[DefaultValue(@"C:\Users\%USERNAME%\Downloads")]
		[JsonProperty("video_dl_path")]
		public string VideoDownloadPath { get; set; }

		[Category("Downloads")]
		[DisplayName("Audio Download Path")]
		[Description("The destination folder for downloaded audio files")]
		[EditorAttribute(typeof(System.Windows.Forms.FolderBrowserDialog), typeof(System.Drawing.Design.UITypeEditor))]
		[DefaultValue(@"C:\Users\%USERNAME%\Downloads")]
		[JsonProperty("audio_dl_path")]
		public string AudioDownloadPath { get; set; }

		#region reddit
		[Browsable(false)]
		[JsonIgnore]
		public string RedditSampleUrl { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public string RedditMediaURLPrefix { get; set; }

		[Category("Defaults")]
		[DisplayName("Reddit Preferred Video Resolution")]
		[Description("The preferred resolution for default reddit downloads")]
		[DefaultValue(Resolution.x1080)]
		[JsonProperty("reddit_res")]
		public Resolution RedditPreferredVideoResolution { get; set; }

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

		[Category("Defaults")]
		[DisplayName("YouTube Preferred Video Resolution")]
		[Description("The preferred resolution for default youtube downloads")]
		[DefaultValue(Resolution.BEST)]
		[JsonProperty("youtube_res")]
		public Resolution YouTubePreferredVideoResolution { get; set; }

		#endregion;

		public GeneralSettings()
        {
			RedditSampleUrl = @"https://www.reddit.com/r/PraiseTheCameraMan/comments/sj7iwr/couldnt_be_more_perfect/";
			RedditMediaURLPrefix = @"https://v.redd.it/";
			RedditPreferredVideoResolution = Resolution.x1080;
			RedditDefaultVideoSuffix = @"/DASH_{0}.mp4?source=fallback";
			RedditDefaultAudioSuffix = @"/DASH_audio.mp4?source=fallback";
			YouTubePreferredVideoResolution = Resolution.BEST;
			VideoDownloadPath = @"C:\Users\%USERNAME%\Downloads";
			AudioDownloadPath = @"C:\Users\%USERNAME%\Downloads";
		}

		public override async Task<string> ValidateSettings()
        {
			return string.Empty;
        }
	}
}
