using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeDLSharp.Metadata;
using YTR.Classes;
using YTR.Utils;

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

        public bool EnableCropButton
        {
            get { return btnCropMedia.Enabled; }
            set { btnCropMedia.Enabled = value;}
        }

        public VideoInfoPanel()
        {
            InitializeComponent();
        }

        public void QualifyCropButton(bool show)
        {
            if (!show)
            {
                btnCropMedia.Visible = false;
            }
            else if (currentImage != null && useMediaSize.Width > 0 && useMediaSize.Height > 0)
            {
                AspectRatio videoAR = AspectRatio.FromDimensions(useMediaSize);
                AspectRatio thumbAR = AspectRatio.FromDimensions(currentImage.Width, currentImage.Height);
                btnCropMedia.Visible = peThumbnail.Image != null && videoAR.ToDecimal() == thumbAR.ToDecimal();
            }
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
            bool handled = false;

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
                    List<YTRThumbnailData> ytrThumbnails = new List<YTRThumbnailData>();
                    ytrThumbnails = await MimeUtil.GetSupportedYTRThumbnailDataAsync(videoData.Thumbnails);                    
                    
                    YTRThumbnailData supportedYTRImage = null;
                    for (int i = 0; i < ytrThumbnails.Count; i++)
                    {
                        supportedYTRImage = ytrThumbnails[i];
                        try
                        {
                            await loadThumbnail(supportedYTRImage);
                            handled = true;
                            break;
                        }
                        catch (WebException ex)
                        {
                        }
                    }
                }
            }

            if (!handled)
            {
                currentImage = Properties.Resources.nothumb;
                useMediaSize = currentImage.Size;
                peThumbnail.Image = currentImage;
            }
        }

        private async Task loadThumbnail(YTRThumbnailData thumbData)
        {            
            if (thumbData != null)
            {                
                using Stream sourceStream = await Utils.HttpUtil.GetStreamFromUrl(thumbData.Url);
                if(sourceStream == null)
                    throw new WebException("Could not get stream from URL");

                if (thumbData.IsWebp)
                {
                    using Stream convertedStream = await ImageUtil.WebpToPngStream(sourceStream);
                    currentImage = Image.FromStream(convertedStream, false, true);
                }
                else
                {
                    currentImage = Image.FromStream(sourceStream, false, true);
                }
                useMediaSize = currentImage.Size;
                peThumbnail.Image = currentImage;
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
