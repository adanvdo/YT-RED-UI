using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace YTR.Settings
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

        [Browsable(false)]
        [JsonProperty("active_skin")]
        public string ActiveSkin { get; set; }

        [Browsable(false)]
        [JsonProperty("skin_palette")]
        public string SkinPalette { get; set; }

        [Category("Error Logs")]
		[DisplayName("Error Log Path")]
		[Description("The destination folder to store error logs")]
		[EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[JsonProperty("error_logs")]
		public string ErrorLogPath { get; set; }

		[Category("Download Restrictions")]
		[DisplayName("Enforce Restrictions")]
		[Description("Always Enable Restrictions For New Downloads\n(Can be toggled off per-download in Control Panel)")]
		[DefaultValue(true)]
		[JsonProperty("enforce_restrictions")]
		public bool EnforceRestrictions { get; set; }

        [Category("Download Restrictions")]
        [DisplayName("Best Resolution Max")]
        [Description("The maximum resolution allowed when using \"Download Best\"")]
		[DefaultValue(Classes.Resolution.ANY)]
        [JsonProperty("best_max_res")]
        public Classes.Resolution MaxResolutionBest { get; set; }

		[Browsable(false)]
		public int MaxResolutionValue
		{
			get
			{
				switch (MaxResolutionBest)
				{
					case Classes.Resolution.SD:
						return 480;
					case Classes.Resolution.HD720p:
						return 720;
					case Classes.Resolution.HD1080p:
						return 1080;
					case Classes.Resolution.UHD2160p:
						return 2160;
					case Classes.Resolution.ANY:
						return 0;
					default:
						return 0;
				}
			}
		}

        [Category("Download Restrictions")]
        [DisplayName("Best Filesize Max")]
        [Description("The maximum filesize in MB allowed when using \"Download Best\"\n0 = Unlimited")]
		[DisplayFormat(DataFormatString = "{0}MB", ApplyFormatInEditMode = false)]
		[DefaultValue(0)]
        [JsonProperty("best_max_size")]
		public int MaxFilesizeBest { get; set; }

        [Category("Download History")]
		[DisplayName("Enable Download History")]
		[Description("Enable YTR to keep a list of downloads for quick access on the Home screen")]
		[DefaultValue(true)]
		[JsonProperty("history_enabled")]
		public bool EnableDownloadHistory { get; set; }

		[Category("Download History")]
		[DisplayName("History Age")]
		[Description("The number of days to keep download history if it is enabled")]
		[DefaultValue(30)]
		[JsonProperty("history_age")]
		public int HistoryAge { get; set; }

		[Category("Downloads")]
		[DisplayName("Auto-Open Download Location")]
		[Description("Automatically Open Completed Downloads in File Explorer")]
		[DefaultValue(true)]
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

        [Category("Downloads")]
        [DisplayName("Create Folder For Playlist Downloads")]
        [Description("When true, a subfolder will be created for media downloaded from a Youtube playlist")]
        [JsonProperty("playlist_folders")]
        public bool CreateFolderForPlaylists { get; set; }

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

		#region Dependencies

		[Browsable(false)]
		[JsonProperty("yt-dlp_local_version")]
		public string YtdlpLocalVersion { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public string YtdlpUrl { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public string YtdlpVersionUrl { get; set; }

		[Browsable(false)]
		[JsonProperty("ffmpeg_local_version")]
		public string FfmpegLocalVersion { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public string FfmpegUrl { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public string FfmpegVersionUrl { get; set; }
        #endregion

        #region Layout
        [Browsable(false)]
		[JsonProperty("segment_collapsed")]
		public bool CollapseSegmentControl { get; set; }

		[Browsable(false)]
		[JsonProperty("crop_collapsed")]
		public bool CollapseCropControl { get; set; }

		[Browsable(false)]
		[JsonProperty("convert_collapsed")]
		public bool CollapseConvertControl { get; set; }

		[Browsable(false)]
		[JsonProperty("limits_collapsed")]
		public bool CollapseLimitsControl { get; set; }

        [Browsable(false)]
		[JsonProperty("history_collapsed")]
        public bool CollapseHistoryPanel { get; set; }
        #endregion

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
			EnforceRestrictions = false;
			MaxResolutionBest = Classes.Resolution.ANY;
			MaxFilesizeBest = 0;
			EnableDownloadHistory = true;
			HistoryAge = 30;
			AutomaticallyOpenDownloadLocation = true;
			RedditSampleUrl = @"https://www.reddit.com/r/PraiseTheCameraMan/comments/sj7iwr/couldnt_be_more_perfect/";
			RedditMediaURLPrefix = @"https://v.redd.it/";
			YouTubeSampleUrl = @"https://www.youtube.com/watch?v=dCAORZphnlY";
			UseTitleAsFileName = false;
			VideoDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
			AudioDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
			CreateFolderForPlaylists = true;
			UsePreferredFormat = false;
			YtdlpLocalVersion = "2023.10.13";
			YtdlpUrl = @"https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp{0}.exe";
			YtdlpVersionUrl = "https://api.github.com/repos/yt-dlp/yt-dlp/tags";
			FfmpegLocalVersion = "6.0";
            FfmpegUrl = @"https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.7z";
			FfmpegVersionUrl = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.7z.ver";
			CollapseSegmentControl = false;
			CollapseCropControl = false;
			CollapseConvertControl = false;
			CollapseLimitsControl = false;
			CollapseHistoryPanel = false;
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
