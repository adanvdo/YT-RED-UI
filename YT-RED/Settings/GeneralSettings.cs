using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using YT_RED.Classes;

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

		[Category("Download Restrictions")]
		[DisplayName("Enforce Restrictions")]
		[Description("Always Enable Restrictions For New Downloads\n(Can be toggled off per-download in Control Panel)")]
		[DefaultValue(true)]
		[JsonProperty("enforce_restrictions")]
		public bool EnforceRestrictions { get; set; }

        [Category("Download Restrictions")]
        [DisplayName("Best Resolution Max")]
        [Description("The maximum resolution allowed when using \"Download Best\"")]
		[DefaultValue(Classes.ResolutionFilter.ANY)]
        [JsonProperty("best_max_res")]
        public Classes.ResolutionFilter MaxResolutionBest { get; set; }

		[Browsable(false)]
		public int MaxResolutionValue
		{
			get
			{
				switch (MaxResolutionBest)
				{
					case Classes.ResolutionFilter.SD:
						return 480;
					case Classes.ResolutionFilter.HD720p:
						return 720;
					case Classes.ResolutionFilter.HD1080p:
						return 1080;
					case Classes.ResolutionFilter.UHD2160p:
						return 2160;
					case Classes.ResolutionFilter.ANY:
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
		[Description("Enable YT-RED to keep a list of downloads for quick access on the Home screen")]
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

        [Category("Downloads")]
        [DisplayName("Create Folder For Playlist Downloads")]
        [Description("When true, a subfolder will be created for media downloaded from a Youtube playlist")]
        [JsonProperty("playlist_folders")]
        public bool CreateFolderForPlaylists { get; set; }

		[Browsable(false)]
		[JsonProperty("use_preferred_format")]
		public bool UsePreferredFormat { get; set; }

		[Browsable(false)]
		[JsonProperty("segment_option")]
		public DownloadOption SegmentOption { get; set; }
        [Browsable(false)]
        [JsonProperty("crop_option")]
        public DownloadOption CropOption { get; set; }
        [Browsable(false)]
        [JsonProperty("convert_option")]
		public DownloadOption ConvertOption { get; set;}
        [Browsable(false)]
        [JsonProperty("restriction_option")]
		public DownloadOption RestrictionOption { get; set;}
        [Browsable(false)]
        [JsonProperty("prepend_option")]
		public DownloadOption PrependOption { get; set; }
        [Browsable(false)]
        [JsonProperty("audio_option")]
		public DownloadOption AudioOption { get; set; }
        [Browsable(false)]
        [JsonProperty("image_option")]
		public DownloadOption ImageOption { get; set;}
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
			EnforceRestrictions = false;
			MaxResolutionBest = Classes.ResolutionFilter.ANY;
			MaxFilesizeBest = 0;
			EnableDownloadHistory = true;
			HistoryAge = 30;
			AutomaticallyOpenDownloadLocation = false;
			RedditSampleUrl = @"https://www.reddit.com/r/PraiseTheCameraMan/comments/sj7iwr/couldnt_be_more_perfect/";
			RedditMediaURLPrefix = @"https://v.redd.it/";
			YouTubeSampleUrl = @"https://www.youtube.com/watch?v=dCAORZphnlY";
			UseTitleAsFileName = false;
			VideoDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
			AudioDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
			CreateFolderForPlaylists = true;
			UsePreferredFormat = false;
			SegmentOption = new DownloadOption(OptionType.Segment);
			CropOption = new DownloadOption(OptionType.Crop);
			ConvertOption = new DownloadOption(OptionType.Convert);
			RestrictionOption = new DownloadOption(OptionType.Restrictions);
			PrependOption = new DownloadOption(OptionType.Prepend);
			AudioOption = new DownloadOption(OptionType.Audio);
			ImageOption = new DownloadOption(OptionType.Image);
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
