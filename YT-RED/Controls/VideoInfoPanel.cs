using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeDLSharp.Metadata;

namespace YT_RED.Controls
{
    public partial class VideoInfoPanel : DevExpress.XtraEditors.XtraUserControl
    {
        public VideoInfoPanel()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            var old = peThumbnail.Image;
            peThumbnail.Image = null;
            if (old != null) old.Dispose();
            clearText();
        }

        public async Task Populate(VideoData videoData)
        {
            Clear();

            if (videoData != null) 
            {
                txtTitle.Text = string.IsNullOrEmpty(videoData.Title) ? "Unknown Title" : videoData.Title.ToUpper();
                txtDuration.Text = videoData.Duration != null ? $"Duration: {TimeSpan.FromSeconds((float)videoData.Duration)}" : "Duration: unknown";
                if (string.IsNullOrEmpty(videoData.Description))
                {
                    txtDescription.Properties.ScrollBars = ScrollBars.None;
                }
                else
                {
                    txtDescription.Properties.ScrollBars = ScrollBars.Vertical;
                    txtDescription.Lines = videoData.Description.Split('\n');
                }

                if(videoData.Thumbnails != null && videoData.Thumbnails.Length > 0)
                {
                    var supportedImage = videoData.Thumbnails.FirstOrDefault(tn => !tn.Url.ToLower().EndsWith(".webp"));
                    if (supportedImage != null)
                    {
                        Stream thumbnailStream = await Utils.WebUtil.GetStreamFromUrl(supportedImage.Url);
                        peThumbnail.Image = Image.FromStream(thumbnailStream, false, true);
                    }
                }
            }
        }

        private void clearText()
        {
            txtTitle.Text = string.Empty;
            txtDuration.Text = string.Empty;
            txtDescription.Lines = new string[] { };
        }

        private void peThumbnail_LoadCompleted(object sender, EventArgs e)
        {
        }
    }
}
