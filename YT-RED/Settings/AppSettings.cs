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
		BESTAUDIO = 5
    }

    public enum VideoFormat
    {
		MP4 = 0,
		WEBM = 1,
		FLV = 2,
		BESTVIDEO = 3
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
