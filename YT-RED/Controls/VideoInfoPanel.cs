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

namespace YTR.Controls
{
    public partial class VideoInfoPanel : DevExpress.XtraEditors.XtraUserControl
    {
        [Browsable(true)]
        public event EventHandler Crop_Click;

        public string Title
        {
            get { return txtTitle.Text; }
        }

        private Image currentImage;
        public Image CurrentImage
        {
            get { return currentImage; }
        }

        private Size useMediaSize;
        public Size UseMediaSize
        {
            get { return useMediaSize; }
            set { useMediaSize = value; }
        }

        public VideoInfoPanel()
        {
            InitializeComponent();
        }

        public void ShowCropButton(bool show)
        {
            btnCropMedia.Visible = peThumbnail.Image != null && useMediaSize.Width > 0 && useMediaSize.Height > 0 && show;
        }

        public void Clear()
        {
            var old = peThumbnail.Image;
            peThumbnail.Image = null;
            if (old != null) old.Dispose();
            btnCropMedia.Visible = false;
            if(currentImage != null)
                currentImage.Dispose();
            currentImage = null;
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
                    var supportedImage = videoData.Thumbnails.OrderByDescending(tn => tn.Height * tn.Width).FirstOrDefault(tn => !tn.Url.ToLower().EndsWith(".webp"));
                    if (supportedImage != null)
                    {
                        Stream thumbnailStream = await Utils.WebUtil.GetStreamFromUrl(supportedImage.Url);
                        currentImage = Image.FromStream(thumbnailStream, false, true);
                        useMediaSize = currentImage.Size;
                        peThumbnail.Image = currentImage;
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

        private void btnCropMedia_Click(object sender, EventArgs e)
        {
            if(Crop_Click != null)
                Crop_Click(sender, e);
        }
    }
}
