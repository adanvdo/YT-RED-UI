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
	public class AppSettings
	{
		private const string SettingsFile = "settings.json";

		private static AppSettings _default;
		public static AppSettings Default
		{
			get
			{
				if (_default == null)
				{
					if (File.Exists(SettingsFile))
					{
						try
						{
							// load settings from disk
							var json = File.ReadAllText(SettingsFile);
							_default = JsonConvert.DeserializeObject<AppSettings>(json);

							// re-save the file so we add any new settings
							_default.Save();
						}
						catch (Exception ex)
						{
							// do something
						}
					}

					// if the default settings are still null, we failed to parse the file (or it didn't exist)
					if (_default == null)
					{
						_default = new AppSettings();
						_default.Save();
					}

				}
				return _default;
			}
		}	
		
		public GeneralSettings General { get; set; }

		public AdvancedSettings Advanced { get; set; }

		public static VideoFormat VideoFormatFromExtension(string extension)
        {
			switch(extension.ToLower())
            {
				case "flv":
					return VideoFormat.FLV;
				case "mkv":
					return VideoFormat.MKV;
				case "mp4":
					return VideoFormat.MP4;
				case "ogg":
					return VideoFormat.OGG;
				case "webm":
					return VideoFormat.WEBM;
				case "":
					return VideoFormat.UNSPECIFIED;
				default:
					return VideoFormat.BESTVIDEO;
            }
        }

		public static VideoFormat ConvertMergeFormatToVideoFormat(YoutubeDLSharp.Options.DownloadMergeFormat downloadMergeFormat)
        {
			switch(downloadMergeFormat)
            {
				case YoutubeDLSharp.Options.DownloadMergeFormat.Flv:
					return VideoFormat.FLV;
				case YoutubeDLSharp.Options.DownloadMergeFormat.Mkv:
					return VideoFormat.MKV;
				case YoutubeDLSharp.Options.DownloadMergeFormat.Mp4:
					return VideoFormat.MP4;
				case YoutubeDLSharp.Options.DownloadMergeFormat.Ogg:
					return VideoFormat.OGG;
				case YoutubeDLSharp.Options.DownloadMergeFormat.Unspecified:
					return VideoFormat.UNSPECIFIED;
				case YoutubeDLSharp.Options.DownloadMergeFormat.Webm:
					return VideoFormat.WEBM;
				default:
					return VideoFormat.BESTVIDEO;
            }
        }

		public static AudioFormat AudioFormatFromExtension(string extension)
        {
			switch(extension.ToLower())
            {
				case "aac":
					return AudioFormat.AAC;
				case "flac":
					return AudioFormat.FLAC;
				case "m4a":
					return AudioFormat.M4A;
				case "mp3":
					return AudioFormat.MP3;
				case "opus":
					return AudioFormat.OPUS;
				case "vorbis":
					return AudioFormat.VORBIS;
				case "wav":
					return AudioFormat.WAV;
				default:
					return AudioFormat.BEST;
			}
        }

		public static AudioFormat ConvertAudioConversionFormatToAudioFormat(YoutubeDLSharp.Options.AudioConversionFormat audioConversionFormat)
        {
			switch(audioConversionFormat)
            {
				case YoutubeDLSharp.Options.AudioConversionFormat.Aac:
					return AudioFormat.AAC;
				case YoutubeDLSharp.Options.AudioConversionFormat.Flac:
					return AudioFormat.FLAC;
				case YoutubeDLSharp.Options.AudioConversionFormat.M4a:
					return AudioFormat.M4A;
				case YoutubeDLSharp.Options.AudioConversionFormat.Mp3:
					return AudioFormat.MP3;
				case YoutubeDLSharp.Options.AudioConversionFormat.Opus:
					return AudioFormat.OPUS;
				case YoutubeDLSharp.Options.AudioConversionFormat.Vorbis:
					return AudioFormat.VORBIS;
				case YoutubeDLSharp.Options.AudioConversionFormat.Wav:
					return AudioFormat.WAV;
				case YoutubeDLSharp.Options.AudioConversionFormat.Best:
					return AudioFormat.BEST;
				default:
					return AudioFormat.BEST;
            }
        }

		public FeatureSettings[] AllSettings => new FeatureSettings[] { General, Advanced};
		public AppSettings() 
		{
			General = new GeneralSettings();
			Advanced = new AdvancedSettings();
		}

		/// <summary>
		/// Reloads the settings from disk and re-initializes the Default settings.
		/// </summary>
		public static void ReloadFromDisk()
		{
			_default = null;
		}

		public void Save()
		{
			var json = JsonConvert.SerializeObject(this, Formatting.Indented);

			try
			{
				File.WriteAllText(SettingsFile, json);
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.Message);
			}
		}
	}

	public enum AudioFormat
    {
		MP3 = 0,
		M4A = 1,
		AAC = 2,
		OGG = 3,
		WAV = 4,
		FLAC = 5,
		OPUS = 6,
		VORBIS = 7,
		BEST = 8
    }

    public enum VideoFormat
    {
		MP4 = 0,
		WEBM = 1,
		FLV = 2,
		MKV = 3,
		OGG = 4,
		UNSPECIFIED = 5,
		BESTVIDEO = 6
	}

	public enum Resolution
    {
		BEST = 0,
		_2160p = 1,
		_1080p = 2,
		_720p = 3,
		_480p = 4,
		_360p = 5,
		_240p = 6
    }
}
