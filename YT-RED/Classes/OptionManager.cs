using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YT_RED.Settings;

namespace YT_RED.Classes
{
    public class OptionManager
    {
        public DownloadOption Segment { get; set; }
        public DownloadOption Crop { get; set; }
        public DownloadOption Convert { get; set; }
        public DownloadOption Restrictions { get; set; }
        public DownloadOption PrependImage { get; set; }
        public DownloadOption ExternalAudio { get; set; }
        public DownloadOption ExternalImage { get; set; }
        public bool VisibleOption { get
            {
                return Segment.Visible || Crop.Visible || Convert.Visible || Restrictions.Visible || PrependImage.Visible || ExternalAudio.Visible || ExternalImage.Visible;
            } 
        }

        public OptionManager() 
        {
            Segment = new DownloadOption(OptionType.Segment);
            Crop = new DownloadOption(OptionType.Crop);
            Convert = new DownloadOption(OptionType.Convert);
            Restrictions = new DownloadOption(OptionType.Restrictions);
            PrependImage = new DownloadOption(OptionType.Prepend);
            ExternalAudio = new DownloadOption(OptionType.Audio);
            ExternalImage = new DownloadOption(OptionType.Image);
        }

        public void Load()
        {
            Segment = AppSettings.Default.General.SegmentOption;
            Crop = AppSettings.Default.General.CropOption;
            Convert = AppSettings.Default.General.ConvertOption;
            Restrictions = AppSettings.Default.General.RestrictionOption;
            PrependImage = AppSettings.Default.General.PrependOption;
            ExternalAudio = AppSettings.Default.General.AudioOption;
            ExternalImage = AppSettings.Default.General.ImageOption;
        }


        public void Validate()
        {
            if ((int)PrependImage.Parameters["duration"] < 1)
            {
                PrependImage.Parameters["duration"] = "1";
            }
        }

        public void Save()
        {
            Settings.AppSettings.Default.General.SegmentOption = Segment;
            Settings.AppSettings.Default.General.CropOption = Crop;
            Settings.AppSettings.Default.General.ConvertOption = Convert;
            Settings.AppSettings.Default.General.RestrictionOption = Restrictions;
            Settings.AppSettings.Default.General.PrependOption = PrependImage;
            Settings.AppSettings.Default.General.AudioOption = ExternalAudio;
            Settings.AppSettings.Default.General.ImageOption = ExternalImage;
            Settings.AppSettings.Default.Save();
        }
    }
}
