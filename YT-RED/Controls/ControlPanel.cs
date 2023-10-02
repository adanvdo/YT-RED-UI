using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using YTR.Settings;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using YTR.Logging;
using YTR.Classes;
using DevExpress.XtraEditors;

namespace YTR.Controls
{
    public partial class ControlPanel : DevExpress.XtraEditors.XtraUserControl
    {
        bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
        private string formatWarning = "YTR is currently set to Always Convert to your\nPreferred Video and Audio Format.\nThis can be changed in Advanced Settings";
        private string gifWarning = "GIF Conversion has the following limitations\nMax Size: 600px\nMax Frames: 300\nMax Duration: 60 Seconds\nFramerate is adjusted to meet this criteria";

        [Browsable(false)]
        public bool PostProcessingEnabled
        {
            get
            {
                return SegmentEnabled || CropEnabled || ConversionEnabled;
            }
        }

        [Browsable(false)]
        public bool SegmentEnabled
        {
            get { return toggleSegment.IsOn; }
        }

        [Browsable(false)]
        public bool CropEnabled
        {
            get { return toggleCrop.IsOn; }
        }

        [Browsable(false)]
        public bool ConversionEnabled
        {
            get { return toggleConvert.IsOn; }
        }

        [Browsable(false)]  
        public bool LimitsEnabled
        {
            get { return toggleDownloadLimits.IsOn; }
        }

        [Browsable(true)]
        public event EventHandler Cancel_MouseMove;

        [Browsable(true)]
        public event EventHandler Cancel_MouseLeave;

        private bool convertIntended = false;
        public bool ConvertIntended
        {
            get { return convertIntended; }
            set { convertIntended = value; }
        }

        private VideoFormat intendedVideoFormat = VideoFormat.UNSPECIFIED;
        private bool disableIntendedVideoUpdate = false;
        private AudioFormat intendedAudioFormat = AudioFormat.UNSPECIFIED;
        private bool disableIntendedAudioUpdate = false;

        private Classes.YTDLFormatPair currentFormatPair = new Classes.YTDLFormatPair();

        [Browsable(false)]
        public Classes.YTDLFormatPair CurrentFormatPair
        {
            get { return this.currentFormatPair; }
        }

        private Classes.PlaylistItemCollection currentPlaylistItems = new PlaylistItemCollection();
        [Browsable(false)]
        public Classes.PlaylistItemCollection CurrentPlaylistItems
        {
            get { return this.currentPlaylistItems; }
        }


        public void SetCurrentPlaylistItems(PlaylistItemCollection playlistItemCollection)
        {
            this.currentPlaylistItems = playlistItemCollection;
        }

        public void SetCurrentPlaylistItems(YoutubeDLSharp.Metadata.VideoData videoData, List<YTDLPlaylistData> playlistData)
        {
            this.currentPlaylistItems = new PlaylistItemCollection(videoData, playlistData);
        }

        public void ClearCurrentPlaylistItems()
        {
            this.currentPlaylistItems = new PlaylistItemCollection();
        }

        public void RemoveCurrentFormat(Classes.StreamType type)
        {
            if (type == Classes.StreamType.File || type == Classes.StreamType.Unknown) throw new ArgumentException("invalid type");

            if (type == Classes.StreamType.Audio) this.currentFormatPair.AudioFormat = null;
            else this.currentFormatPair.VideoFormat = null;

            processFormatChange();
        }

        public void SetCurrentFormat(Classes.YTDLFormatData format)
        {
            if (format == null) throw new ArgumentNullException("format");
            if (format.Type == Classes.StreamType.File || format.Type == Classes.StreamType.Unknown) throw new ArgumentException("type is not valid");

            if (format.Type == Classes.StreamType.Video || format.Type == Classes.StreamType.AudioAndVideo)
                this.currentFormatPair.VideoFormat = format;
            else if (format.Type == Classes.StreamType.Audio)
                this.currentFormatPair.AudioFormat = format;

            processFormatChange();
        }

        public void SetCurrentFormats(Classes.YTDLFormatData videoFormat = null, Classes.YTDLFormatData audioFormat = null)
        {
            if (currentFormatPair == null)
                currentFormatPair = new Classes.YTDLFormatPair();

            this.currentFormatPair.VideoFormat = videoFormat;
            this.currentFormatPair.AudioFormat = audioFormat;

            processFormatChange();
        }

        public void SetCurrentFormatPair(YTDLFormatPair formatPair)
        {
            currentFormatPair = formatPair;

            processFormatChange();
        }

        private void processFormatChange()
        {
            bool invalidFormat = currentFormatPair == null || !currentFormatPair.IsValid();
            lblSelectionText.Text = invalidFormat ? "" : currentFormatPair.FormatDisplayText;
            if (currentFormatPair != null && currentFormatPair.IsValid())
            {
                switch (currentFormatPair.Type)
                {
                    case Classes.StreamType.Video:
                        btnSelectionDL.ImageOptions.SvgImage = Properties.Resources.glyph_video;
                        break;
                    case Classes.StreamType.Audio:
                        btnSelectionDL.ImageOptions.SvgImage = Properties.Resources.sound;
                        break;
                    case Classes.StreamType.AudioAndVideo:
                        btnSelectionDL.ImageOptions.SvgImage = Properties.Resources.VideoSound;
                        break;
                }
            }

            lblSelectionText.Refresh();
            formatChanged();
            setProcessingLimits();
        }

        public void SetProcessingLimits()
        {
            this.setProcessingLimits();
        }

        private void setProcessingLimits()
        {
            if (currentFormatPair == null || !currentFormatPair.IsValid())
                return;

            if(currentFormatPair.Type == StreamType.Video || currentFormatPair.Type == StreamType.AudioAndVideo)
            {
                if(CropEnabled)
                {
                    if (currentFormatPair.VideoFormat.Height != null && (currentFormatPair.VideoFormat.Height - TotalVerticalCrop) < 50)
                    {
                        int acceptableCrop = (int)currentFormatPair.VideoFormat.Height - 50;
                        if(Convert.ToInt32(CropTop) > 0 || Convert.ToInt32(CropBottom) > 0)
                        {
                            if(Convert.ToInt32(CropTop) > Convert.ToInt32(CropBottom))
                            {
                                CropTop = (acceptableCrop - Convert.ToInt32(CropBottom)).ToString();
                            }
                            else
                            {
                                CropBottom = (acceptableCrop - Convert.ToInt32(CropTop)).ToString();
                            }
                        }
                    }

                    if(currentFormatPair.VideoFormat.Width != null && (currentFormatPair.VideoFormat.Width - TotalHorizontalCrop) < 50)
                    {
                        int acceptableCrop = (int)currentFormatPair.VideoFormat.Width - 50;
                        if (Convert.ToInt32(CropLeft) > 0 || Convert.ToInt32(CropRight) > 0)
                        {
                            if (Convert.ToInt32(CropLeft) > Convert.ToInt32(CropRight))
                            {
                                CropLeft = (acceptableCrop - Convert.ToInt32(CropRight)).ToString();
                            }
                            else
                            {
                                CropRight = (acceptableCrop - Convert.ToInt32(CropLeft)).ToString();
                            }
                        }
                    }
                }
            }

            if(SegmentEnabled)
            {
                TimeSpan? dur = currentFormatPair.Duration;

                if(dur != null && SegmentDuration.TotalSeconds > 0)
                {
                    if(AppSettings.Default.Layout.SegmentControlMode == SegmentControlMode.EndTime && SegmentDuration > dur)
                    {
                        SegmentDuration = (TimeSpan)dur;
                    }
                    else if(AppSettings.Default.Layout.SegmentControlMode == SegmentControlMode.Duration && SegmentStart + SegmentDuration > dur)
                    {
                        SegmentDuration = (TimeSpan)dur - SegmentStart;
                    }
                }
            }
        }

        [Browsable(false)]
        public bool EmbedThumbnail
        {
            get
            {
                if (currentFormatPair == null || !currentFormatPair.IsValid() || currentFormatPair.Type != Classes.StreamType.Audio)
                    return false;

                if (cbAudioFormat.SelectedItem != null && cbAudioFormat.SelectedItem.ToString().ToLower() != "opus")
                    return true;

                if (currentFormatPair.AudioCodec == "opus" || currentFormatPair.Extension == "webm")
                    return false;

                return true;
            }
        }

        private void formatChanged()
        {
            if (currentFormatPair != null && currentFormatPair.IsValid())
            {
                if (currentFormatPair.VideoFormat == null || currentFormatPair.VideoCodec == "none")
                {
                    cbVideoFormat.Properties.Items.Clear();
                    cbAudioFormat.Properties.Items.Clear();
                    cbAudioFormat.Properties.Items.AddRange(audioFormats.Where(af => af.ToLower() != currentFormatPair.AudioFormat.Extension.ToLower()).ToList());
                    cbVideoFormat.Enabled = false;
                    cbAudioFormat.Enabled = toggleConvert.IsOn;
                    cbVideoFormat.SelectedItem = null;
                    var afs = Enum.GetNames(typeof(AudioFormat)).Cast<string>();
                    disableIntendedAudioUpdate = true;
                    ConvertAudioFormat = AppSettings.Default.Advanced.PreferredAudioFormat.ToFriendlyString(true) == currentFormatPair.AudioFormat.Extension.ToLower()
                        ? SystemCodecMaps.GetAudioFormatByExtension(afs.FirstOrDefault(af => af.ToLower() != currentFormatPair.AudioFormat.Extension.ToLower()))
                        : intendedAudioFormat;
                    disableIntendedAudioUpdate = false;
                    checkRedundancy();
                }
                else
                {
                    cbVideoFormat.Properties.Items.Clear();
                    cbVideoFormat.Properties.Items.AddRange(videoFormats.Where(vf => vf.ToLower() != currentFormatPair.VideoFormat.Extension.ToLower()).ToList());
                    cbAudioFormat.Properties.Items.Clear();
                    cbAudioFormat.Enabled = false;
                    cbVideoFormat.Enabled = toggleConvert.IsOn;
                    cbAudioFormat.SelectedItem = null;
                    var vfs = Enum.GetNames(typeof(VideoFormat)).Cast<string>();
                    disableIntendedVideoUpdate = true;
                    ConvertVideoFormat = AppSettings.Default.Advanced.PreferredVideoFormat.ToFriendlyString(true) == currentFormatPair.VideoFormat.Extension.ToLower()
                        ? SystemCodecMaps.GetVideoFormatByExtension(vfs.FirstOrDefault(vf => vf.ToLower() != currentFormatPair.VideoFormat.Extension.ToLower()))
                        : intendedVideoFormat;
                    disableIntendedVideoUpdate = false;
                    checkRedundancy();
                }
            }
            else
            {
                cbVideoFormat.Properties.Items.Clear();
                cbVideoFormat.Properties.Items.AddRange(videoFormats);
                ConvertVideoFormat = AppSettings.Default.Advanced.PreferredVideoFormat;
                cbAudioFormat.Properties.Items.Clear();
                cbAudioFormat.Properties.Items.AddRange(audioFormats);
                ConvertAudioFormat = AppSettings.Default.Advanced.PreferredAudioFormat;
                cbVideoFormat.Enabled = toggleConvert.IsOn;
                cbAudioFormat.Enabled = toggleConvert.IsOn;
                checkRedundancy();
            }
            cbAudioFormat.Enabled = toggleConvert.IsOn && !AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            cbVideoFormat.Enabled = toggleConvert.IsOn && !AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            hlblOpenSettings.Visible = toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            lblAlwaysConvert.Visible = toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
        }

        private void checkRedundancy()
        {
            if (currentFormatPair != null)
            {
                if (currentFormatPair.VideoFormat != null && toggleConvert.Enabled && cbVideoFormat.SelectedItem != null)
                {
                    if ((this.intendedVideoFormat.ToFriendlyString(true) == currentFormatPair.VideoFormat.Extension.ToLower() && this.toggleConvert.IsOn)
                        || (this.intendedVideoFormat.ToFriendlyString(true) != currentFormatPair.VideoFormat.Extension.ToLower() && !this.toggleConvert.IsOn && this.convertIntended))
                    {
                        this.toggleConvert.Toggle();
                    }
                }
                else if(currentFormatPair.AudioFormat != null && toggleConvert.Enabled && cbAudioFormat.SelectedItem != null)
                {
                    if ((this.intendedAudioFormat.ToFriendlyString(true) == currentFormatPair.AudioFormat.Extension.ToLower() && this.toggleConvert.IsOn)
                        || (this.intendedAudioFormat.ToFriendlyString(true) != currentFormatPair.AudioFormat.Extension.ToLower() && !this.toggleConvert.IsOn && this.convertIntended))
                    {
                        this.toggleConvert.Toggle();
                    }
                }
            }
        }

        public TimeSpan SegmentStart
        {
            get { return tsStart.TimeSpan; }

            set { tsStart.TimeSpan = value; }
        }

        public TimeSpan SegmentDuration
        {
            get { return tsDuration.TimeSpan; }

            set { tsDuration.TimeSpan = value; }
        }

        public void ShowSegmentWarning()
        {
            lblSegmentDisclaimer.Visible = true;
        }

        public void HideSegmentWarning()
        {
            lblSegmentDisclaimer.Visible = false;
        }

        public string CropTop
        {
            get { return txtCropTop.Text; }
            set { txtCropTop.Text = value; }
        }
        public string CropBottom
        {
            get { return txtCropBottom.Text; }
            set { txtCropBottom.Text = value; }
        }
        public string CropLeft
        {
            get { return txtCropLeft.Text; }
            set { txtCropLeft.Text = value; }
        }
        public string CropRight
        {
            get { return txtCropRight.Text; }
            set { txtCropRight.Text = value; }
        }

        public int TotalVerticalCrop
        {
            get { return Convert.ToInt32(txtCropTop.Text) + Convert.ToInt32(txtCropBottom.Text); }
        }

        public int TotalHorizontalCrop
        {
            get { return Convert.ToInt32(txtCropLeft.Text) + Convert.ToInt32(txtCropRight.Text); }
        }

        public Size TotalControlSize
        {
            get
            {

                int totalMinHeight = (gcSegments.Visible ? gcSegments.Height : 0)
                    + (gcCrop.Visible ? gcCrop.Height : 0)
                    + (gcConvert.Visible ? gcConvert.Height : 0)
                    + (gcDownloadLimits.Visible ? gcDownloadLimits.Height : 0)
                    + gcDLButtons.Height
                    + pnlProgressPanel.Height;
                return new Size(this.Size.Width, totalMinHeight);
            }
        }

        public bool ValidCrops()
        {
            return !string.IsNullOrEmpty(CropTop) || !string.IsNullOrEmpty(CropBottom) || !string.IsNullOrEmpty(CropLeft) || !string.IsNullOrEmpty(CropRight);
        }

        [Browsable(false)]
        public Settings.VideoFormat? ConvertVideoFormat
        {
            get
            {
                if (toggleConvert.IsOn && cbVideoFormat.SelectedItem != null && !string.IsNullOrEmpty(cbVideoFormat.SelectedItem.ToString()))
                {
                    Settings.VideoFormat vf = VideoFormat.UNSPECIFIED;
                    bool pf = Enum.TryParse(cbVideoFormat.SelectedItem.ToString(), out VideoFormat pvf);
                    if (pf)
                        vf = pvf;
                    if (vf != Settings.VideoFormat.UNSPECIFIED)
                        return vf;
                }
                return null;
            }
            set
            {
                VideoFormat? v = value;
                if (v != null)
                    cbVideoFormat.SelectedIndex = value == VideoFormat.UNSPECIFIED ? 0 : cbVideoFormat.Properties.Items.IndexOf(value.ToString());
            }
        }

        [Browsable(false)]
        public Settings.AudioFormat? ConvertAudioFormat
        {
            get
            {
                if (toggleConvert.IsOn && cbAudioFormat.SelectedItem != null && !string.IsNullOrEmpty(cbAudioFormat.SelectedItem.ToString()))
                {
                    Settings.AudioFormat af = AudioFormat.UNSPECIFIED;
                    bool pf = Enum.TryParse(cbAudioFormat.SelectedItem.ToString(), out AudioFormat paf);
                    if (pf)
                        af = paf;
                    return af;
                }
                return null;
            }
            set
            {
                AudioFormat? v = value;
                if (v != null)
                    cbAudioFormat.SelectedIndex = value == AudioFormat.UNSPECIFIED ? 0 : cbAudioFormat.Properties.Items.IndexOf(value.ToString());
            }
        }

        [Browsable(false)]
        public Resolution? MaxResolution
        {
            get
            {
                if(toggleDownloadLimits.IsOn && cbMaxRes.SelectedItem != null && !string.IsNullOrEmpty(cbMaxRes.SelectedItem.ToString()))
                {
                    Resolution r = Resolution.ANY;
                    bool pr = Enum.TryParse(cbMaxRes.SelectedItem.ToString(), out Resolution ppr);
                    if (pr)
                        r = ppr;
                    return r;
                }
                return null;
            }
            set
            {
                Resolution? v = value;
                if (v != null)
                    cbMaxRes.SelectedIndex = cbMaxRes.Properties.Items.IndexOf(value.ToString());                    
            }
        }

        [Browsable(false)]
        public int MaxResolutionValue
        {
            get
            {
                switch (MaxResolution)
                {
                    case Classes.Resolution.SD:
                        return 480;
                    case Classes.Resolution.HD720p:
                        return 720;
                    case Classes.Resolution.HD1080p:
                        return 1080;
                    case Classes.Resolution.UHD2160p:
                        return 2160;
                    default:
                        return 0;
                }
            }
        }

        [Browsable(false)]
        public int MaxFilesize
        {
            get
            {
                if(toggleDownloadLimits.IsOn && !string.IsNullOrEmpty(txtMaxFilesize.Text))
                {
                    int max = 0;
                    string val = txtMaxFilesize.Text.Split(' ')[0];
                    bool pi = int.TryParse(val, out int maxSize);
                    if(pi)
                        max = maxSize;
                    return max;
                }
                return 0;
            }
            set
            {
                txtMaxFilesize.Text = value.ToString();
            }
        }

        public bool DownloadSelectionVisible
        {
            get { return btnSelectionDL.Visible; }
            set { btnSelectionDL.Visible = value; }
        }

        public bool DownloadAudioVisible
        {
            get { return btnDownloadAudio.Visible; }
            set { btnDownloadAudio.Visible = value; }
        }

        public bool DownloadBestVisible
        {
            get { return btnDownloadBest.Visible; }
            set { btnDownloadBest.Visible = value; }
        }

        public void UpdateListProgress(int completed = 0, bool hideOnCompletion = true)
        {          

            if (pbListProgress.InvokeRequired)
            {
                Action safeUpdate = delegate
                {
                    pbListProgress.Position = completed;
                    pbListProgress.Tag = completed != currentPlaylistItems.Count ? $"{completed}:{currentPlaylistItems.Count}" : null;
                    pbListProgress.Refresh();
                };
                pbListProgress.Invoke(safeUpdate);
            }
            else
            {
                pbListProgress.Position = completed;
                pbListProgress.Tag = completed != currentPlaylistItems.Count ? $"{completed}:{currentPlaylistItems.Count}" : null;
                pbListProgress.Refresh();
            }
        }

        public void UpdateProgress(int progress = 0, bool hideOnCompletion = true)
        {
            if (pbProgress.InvokeRequired)
            {
                Action safeUpdate = delegate
                {
                    pbProgress.Position = progress;
                    pbProgress.Refresh();
                };
                pbProgress.Invoke(safeUpdate);
            }
            else
            {
                pbProgress.Position = progress;
                pbProgress.Refresh();
            }
        }

        public void ShowListProgress()
        {
            if(pbListProgress.Properties.Maximum != currentPlaylistItems.Count) pbListProgress.Properties.Maximum = currentPlaylistItems.Count;
            pbListProgress.Tag = null;
            pbListProgress.Visible = true;
            pbProgress.Visible = true;
            UpdateListProgress(0);
        }

        public void HideListProgress()
        {
            pbListProgress.Properties.Maximum = 0;
            pbListProgress.Tag = null;
            pbListProgress.Visible = false;
            pbProgress.Visible = false;
        }

        public void ShowProgress()
        {
            pbProgress.Position = 0;
            pbProgress.Visible = true;
        }

        public void HideProgress()
        {
            pbProgress.Visible = false;
        }

        public void ShowDownloadLocation(string fileLocation)
        {
            btnOpenDL.Text = fileLocation;
            btnOpenDL.Visible = true;
            lblLastDL.Visible = true;
        }

        public void HideDownloadLocation()
        {
            btnOpenDL.Visible = false;
            btnOpenDL.Text = string.Empty;
            lblLastDL.Visible = false;
        }

        [Browsable(true)]
        public event EventHandler Controls_Updated;

        [Browsable(true)]
        public event EventHandler DownloadSelection_Click;

        [Browsable(true)]
        public event EventHandler DownloadAudio_Click;

        [Browsable(true)]
        public event EventHandler DownloadBest_Click;

        [Browsable(true)]
        public event EventHandler CancelProcess_Click;

        [Browsable(true)]
        public event EventHandler ReDownload_Click;

        [Browsable(true)]
        public event EventHandler NewDownload_Click;

        private Color toggleForeColor;

        private bool inInit = true;

        private MainForm parentMainForm = null;

        public MainForm ParentMainForm { set { parentMainForm = value; } }

        /// <summary>
        /// Control Constructor
        /// </summary>
        public ControlPanel()
        {
            InitializeComponent();
            toggleForeColor = toggleSegment.ForeColor;
            if (!designMode)
            {
                if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
                {
                    this.intendedAudioFormat = AppSettings.Default.Advanced.PreferredAudioFormat;
                    this.intendedVideoFormat = AppSettings.Default.Advanced.PreferredVideoFormat;
                }
            }
            InitControls();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (parentMainForm != null)
            {
                this.parentMainForm.UpdateControlPanelDisplay();
            }
            base.OnPaint(e);
        }

        public void InitControls()
        {
            if (!designMode)
            {
                videoFormats = new List<string>();
                List<string> vFormats = new List<string>() { "" };
                vFormats.AddRange(Enum.GetNames(typeof(VideoFormat)).Cast<string>());
                videoFormats.AddRange(vFormats.Where(f => f != "UNSPECIFIED"));// && f != "GIF"));
                cbVideoFormat.Properties.Items.AddRange(videoFormats);
                cbVideoFormat.SelectedIndex = 0;
                audioFormats = new List<string>();
                List<string> aFormats = new List<string>() { "" };
                aFormats.AddRange(Enum.GetNames(typeof(AudioFormat)).Cast<string>());
                audioFormats.AddRange(aFormats.Where(f => f != "UNSPECIFIED"));
                cbAudioFormat.Properties.Items.AddRange(audioFormats);
                cbAudioFormat.SelectedIndex = 0;
                cbMaxRes.Properties.Items.AddRange(Utils.VideoUtil.ResolutionList);
                cbMaxRes.SelectedIndex = 4;
                txtMaxFilesize.Text = "0";
                lblDuration.Text = AppSettings.Default.Layout.SegmentControlMode == SegmentControlMode.Duration ? "Duration" : "End";
                inInit = false;
                this.controlsUpdated();
            }
        }

        private List<string> videoFormats;
        private List<string> audioFormats;

        public void ResetControls(bool forFormatList = false)
        {
            toggleSegment.IsOn = false;
            tsStart.TimeSpan = TimeSpan.Zero;
            tsDuration.TimeSpan = TimeSpan.FromSeconds(1);
            toggleCrop.IsOn = false;
            lblSelectionText.Text = String.Empty;
            txtCropBottom.Text = String.Empty;
            txtCropTop.Text = String.Empty;
            txtCropLeft.Text = String.Empty;
            txtCropRight.Text = String.Empty;
            toggleConvert.IsOn = false;
            ShowHideControlGroup(ControlGroups.Segment, true);
            ShowHideControlGroup(ControlGroups.Crop, true);
            DisableToggle(true, true, true, forFormatList);
            EnableToggle(true, true, true, false, !forFormatList);
            ShowHideControlGroup(ControlGroups.Limits, !forFormatList);
            cbVideoFormat.SelectedIndex = 0;
            cbAudioFormat.SelectedIndex = 0;
            cbVideoFormat.Enabled = true;
            cbAudioFormat.Enabled = true;
            
            if (AppSettings.Default.General.EnforceRestrictions && !forFormatList)
            {
                EnableToggle(false, false, false, true, true);
                MaxResolution = AppSettings.Default.General.MaxResolutionBest;
                MaxFilesize = AppSettings.Default.General.MaxFilesizeBest;
            }
            btnSelectionDL.Text = "DOWNLOAD SELECTED FORMAT    ";
            btnDownloadAudio.Text = "DOWNLOAD AUDIO       ";
            btnDownloadBest.Text = "DOWNLOAD BEST [audio+video]      ";
            HideDownloadLocation();
            if (currentPlaylistItems != null)
            {
                currentPlaylistItems.Dispose();
                currentPlaylistItems = null;
            }
            currentFormatPair.Clear();
            formatChanged(); 

            if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat)
            {
                EnableToggle(false, false, true, true, false);
                ConvertVideoFormat = AppSettings.Default.Advanced.PreferredVideoFormat;
                ConvertAudioFormat = AppSettings.Default.Advanced.PreferredAudioFormat;
                cbVideoFormat.Enabled = false;
                cbAudioFormat.Enabled = false;
            }

            lblDuration.Text = AppSettings.Default.Layout.SegmentControlMode == SegmentControlMode.Duration ? "Duration" : "End";

            hlblOpenSettings.Visible = toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            lblAlwaysConvert.Visible = toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            this.controlsUpdated();
        }

        public void AdjustControls(int parentPanelHeight)
        {
            if(TotalControlSize.Height < parentPanelHeight)
            {
                this.Height = parentPanelHeight;
            }
        }

        private void controlsUpdated()
        {
            if(Controls_Updated != null) Controls_Updated(this, EventArgs.Empty);
        }

        public void DisableToggle(bool disableSegment = false, bool disableCrop = false, bool disableConvert = false, bool disableLimits = false)
        {
            if (disableSegment)
            {
                toggleSegment.IsOn = false;
                toggleSegment.Enabled = false;
            }
            if (disableCrop)
            {
                toggleCrop.IsOn = false;
                toggleCrop.Enabled = false;
            }
            if (disableConvert)
            {
                toggleConvert.IsOn = false;
                toggleConvert.Enabled = false;
            }
            if (disableLimits)
            {
                toggleDownloadLimits.IsOn = false;
                toggleDownloadLimits.Enabled = false;
            }
        }

        public void SetSelectionText(string text)
        {
            this.lblSelectionText.Text = text;            
        }

        public void EnableToggle(bool enableSegment = false, bool enableCrop = false, bool enableConvert = false, bool turnOn = false, bool enableLimits = false)
        {
            if (enableSegment)
            {
                toggleSegment.Enabled = true;
                if (turnOn) toggleSegment.IsOn = true;
            }
            if (enableCrop)
            {
                toggleCrop.Enabled = true;
                if (turnOn) toggleCrop.IsOn = true;
            }
            if (enableConvert)
            {
                toggleConvert.Enabled = true;
                if(turnOn) toggleConvert.IsOn = true;
                cbVideoFormat.Enabled = !AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
                cbAudioFormat.Enabled = !AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            }
            if (enableLimits)
            {
                toggleDownloadLimits.Enabled = true;
                if(turnOn) toggleDownloadLimits.IsOn = true;
            }
        }

        private void gcSegments_Click(object sender, EventArgs e)
        {
            ExpandCollapseControlGroup(ControlGroups.Segment);
        }

        private void gcCrop_Click(object sender, EventArgs e)
        {
            ExpandCollapseControlGroup(ControlGroups.Crop);
        }

        private void gcConvert_Click(object sender, EventArgs e)
        {
            ExpandCollapseControlGroup(ControlGroups.Convert);
        }

        private void gcDownloadLimits_Click(object sender, EventArgs e)
        {
            ExpandCollapseControlGroup(ControlGroups.Limits);
        }

        public void ShowHideControlGroup(ControlGroups controlGroup, bool show)
        {
            switch(controlGroup)
            {
                case ControlGroups.Segment:
                    gcSegments.Visible = show; break;
                case ControlGroups.Crop:
                    gcCrop.Visible = show; break;
                case ControlGroups.Convert:
                    gcConvert.Visible = show; break;
                case ControlGroups.Limits:
                    gcDownloadLimits.Visible = show; break;
            }
        }

        public void RestoreControlGroupCollapseStates()
        {
            restoreControlGroupCollapseState(ControlGroups.Segment, AppSettings.Default.General.CollapseSegmentControl);
            restoreControlGroupCollapseState(ControlGroups.Crop, AppSettings.Default.General.CollapseCropControl);
            restoreControlGroupCollapseState(ControlGroups.Convert, AppSettings.Default.General.CollapseConvertControl);
            restoreControlGroupCollapseState(ControlGroups.Limits, AppSettings.Default.General.CollapseLimitsControl);
        }

        private void restoreControlGroupCollapseState(ControlGroups controlGroup, bool collapse)
        {
            switch (controlGroup)
            {
                case ControlGroups.Segment:
                    if (toggleSegment.IsOn) return;
                    if (pnlSegPanel.Visible && collapse)
                    {
                        pnlSegPanel.Visible = false;
                        gcSegments.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else if(!collapse)
                    {
                        pnlSegPanel.Visible = true;
                        gcSegments.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
                case ControlGroups.Crop:
                    if (toggleCrop.IsOn) return;
                    if (pnlCropPanel.Visible && collapse)
                    {
                        pnlCropPanel.Visible = false;
                        gcCrop.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else if (!collapse)
                    {
                        pnlCropPanel.Visible = true;
                        gcCrop.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
                case ControlGroups.Convert:
                    if (toggleConvert.IsOn) return;
                    if (pnlConvertPanel.Visible && collapse)
                    {
                        pnlConvertPanel.Visible = false;
                        gcConvert.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else if (!collapse)
                    {
                        pnlConvertPanel.Visible = true;
                        gcConvert.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
                case ControlGroups.Limits:
                    if (toggleDownloadLimits.IsOn) return;
                    if (pnlLimitPanel.Visible && collapse)
                    {
                        pnlLimitPanel.Visible = false;
                        gcDownloadLimits.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else if (!collapse)
                    {
                        pnlLimitPanel.Visible = true;
                        gcDownloadLimits.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
            }
        }

        public void ExpandCollapseControlGroup(ControlGroups controlGroup)
        {
            switch(controlGroup)
            {
                case ControlGroups.Segment:
                    if (toggleSegment.IsOn) return;
                    if (pnlSegPanel.Visible)
                    {
                        pnlSegPanel.Visible = false;
                        gcSegments.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlSegPanel.Visible = true;
                        gcSegments.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    AppSettings.Default.General.CollapseSegmentControl = !pnlSegPanel.Visible;
                    break;
                case ControlGroups.Crop:
                    if (toggleCrop.IsOn) return;
                    if (pnlCropPanel.Visible)
                    {
                        pnlCropPanel.Visible = false;
                        gcCrop.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlCropPanel.Visible = true;
                        gcCrop.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    AppSettings.Default.General.CollapseCropControl = !pnlCropPanel.Visible;
                    break;
                case ControlGroups.Convert:
                    if (toggleConvert.IsOn) return;
                    if (pnlConvertPanel.Visible)
                    {
                        pnlConvertPanel.Visible = false;
                        gcConvert.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlConvertPanel.Visible = true;
                        gcConvert.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    AppSettings.Default.General.CollapseConvertControl = !pnlConvertPanel.Visible;
                    break;
                case ControlGroups.Limits:
                    if (toggleDownloadLimits.IsOn) return;
                    if (pnlLimitPanel.Visible)
                    {
                        pnlLimitPanel.Visible = false;
                        gcDownloadLimits.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlLimitPanel.Visible = true;
                        gcDownloadLimits.CustomHeaderButtons[0].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    AppSettings.Default.General.CollapseLimitsControl = !pnlLimitPanel.Visible;
                    break;
            }
            AppSettings.Default.Save();
        }

        public void UpdatePanelStates()
        {
            updateSegmentState();
            updateCropState();
            UpdateConvertState();
            UpdateLimitState();
        }

        private void toggleSegment_Toggled(object sender, EventArgs e)
        {
            updateSegmentState();
        }

        private void updateSegmentState()
        {
            if (toggleSegment.IsOn)
            {
                toggleSegment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
                toggleSegment.Properties.Appearance.BorderColor = Color.LightGreen;
                toggleSegment.BackColor = Color.LightGreen;
                toggleSegment.ForeColor = Color.Black;
            }
            else
            {
                toggleSegment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                toggleSegment.Properties.Appearance.BorderColor = Color.Transparent;
                toggleSegment.BackColor = Color.Transparent;
                toggleSegment.ForeColor = toggleForeColor;
            }
            tsStart.Enabled = toggleSegment.IsOn;
            tsDuration.Enabled = toggleSegment.IsOn;
            lblSegmentDisclaimer.Visible = toggleSegment.IsOn && (currentFormatPair == null || !currentFormatPair.IsValid() || parentMainForm.FormatCount < 1);
            gcSegments.CustomHeaderButtons[0].Properties.Enabled = !toggleSegment.IsOn;
        }

        private void toggleCrop_Toggled(object sender, EventArgs e)
        {
            updateCropState();
        }

        private void updateCropState()
        {
            if (toggleCrop.IsOn)
            {
                toggleCrop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
                toggleCrop.Properties.Appearance.BorderColor = Color.LightGreen;
                toggleCrop.BackColor = Color.LightGreen;
                toggleCrop.ForeColor = Color.Black;
            }
            else
            {
                toggleCrop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                toggleCrop.Properties.Appearance.BorderColor = Color.Transparent;
                toggleCrop.BackColor = Color.Transparent;
                toggleCrop.ForeColor = toggleForeColor;
            }
            txtCropBottom.Enabled = toggleCrop.IsOn;
            txtCropTop.Enabled = toggleCrop.IsOn;
            txtCropLeft.Enabled = toggleCrop.IsOn;
            txtCropRight.Enabled = toggleCrop.IsOn;
            gcCrop.CustomHeaderButtons[0].Properties.Enabled = !toggleCrop.IsOn;
            btnDownloadAudio.Enabled = !toggleCrop.IsOn;
        }

        private void toggleConvert_Click(object sender, EventArgs e)
        {
            this.convertIntended = !this.toggleConvert.IsOn;
            if(this.intendedVideoFormat == VideoFormat.UNSPECIFIED && cbVideoFormat.SelectedIndex >= 0)
            {
                this.intendedVideoFormat = SystemCodecMaps.GetVideoFormatByExtension(cbVideoFormat.SelectedItem.ToString());
            }
            if(this.intendedAudioFormat == AudioFormat.UNSPECIFIED && cbVideoFormat.SelectedIndex >= 0)
            {
                this.intendedAudioFormat = SystemCodecMaps.GetAudioFormatByExtension(cbAudioFormat.SelectedItem.ToString());
            }
        }

        private void toggleConvert_Toggled(object sender, EventArgs e)
        {
            UpdateConvertState();
        }

        public void UpdateConvertState()
        {
            if (toggleConvert.IsOn)
            {
                toggleConvert.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
                toggleConvert.Properties.Appearance.BorderColor = Color.LightGreen;
                toggleConvert.BackColor = Color.LightGreen;
                toggleConvert.ForeColor = Color.Black;
            }
            else
            {
                toggleConvert.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                toggleConvert.Properties.Appearance.BorderColor = Color.Transparent;
                toggleConvert.BackColor = Color.Transparent;
                toggleConvert.ForeColor = toggleForeColor;
            }
            cbVideoFormat.Enabled = toggleConvert.IsOn && !AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            cbAudioFormat.Enabled = toggleConvert.IsOn && !AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            hlblOpenSettings.Visible = toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            lblAlwaysConvert.Visible = toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            gcConvert.CustomHeaderButtons[0].Properties.Enabled = !toggleConvert.IsOn;
        }

        private void toggleDownloadLimits_Toggled(object sender, EventArgs e)
        {
            UpdateLimitState();
        }

        public void UpdateLimitState()
        {
            if (toggleDownloadLimits.IsOn)
            {
                toggleDownloadLimits.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
                toggleDownloadLimits.Properties.Appearance.BorderColor = Color.LightGreen;
                toggleDownloadLimits.BackColor = Color.LightGreen;
                toggleDownloadLimits.ForeColor = Color.Black;
            }
            else
            {
                toggleDownloadLimits.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                toggleDownloadLimits.Properties.Appearance.BorderColor = Color.Transparent;
                toggleDownloadLimits.BackColor = Color.Transparent;
                toggleDownloadLimits.ForeColor = toggleForeColor;
            }
            cbMaxRes.Enabled = toggleDownloadLimits.IsOn;
            txtMaxFilesize.Enabled = toggleDownloadLimits.IsOn;
            gcDownloadLimits.CustomHeaderButtons[0].Properties.Enabled = !toggleDownloadLimits.IsOn;
        }

        private void btnSelectionDL_Click(object sender, EventArgs e)
        {
            if (DownloadSelection_Click != null) DownloadSelection_Click(sender, e);
        }

        private void btnDownloadAudio_Click(object sender, EventArgs e)
        {
            if (DownloadAudio_Click != null) DownloadAudio_Click(sender, e);
        }

        private void btnDownloadBest_Click(object sender, EventArgs e)
        {
            if (DownloadBest_Click != null) DownloadBest_Click(sender, e);
        }

        private void btnCancelProcess_Click(object sender, EventArgs e)
        {
            if(CancelProcess_Click != null) CancelProcess_Click(sender, e);
        }

        private void lblSelectionText_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lblSelectionText.Text))
            {
                lblSelectionText.Padding = new Padding(0, 10, 0, 10);
            }
            else
            {
                lblSelectionText.Padding = new Padding(0);
            }
        }

        private void checkConversionOptions()
        {
            if (cbVideoFormat.SelectedItem != null && !string.IsNullOrEmpty(cbVideoFormat.SelectedItem.ToString()))
            {
                if (this.intendedVideoFormat == VideoFormat.GIF)
                {
                    lblAlwaysConvert.Text = gifWarning;
                    lblAlwaysConvert.Visible = true;
                }
                else
                {
                    lblAlwaysConvert.Text = formatWarning;
                    hlblOpenSettings.Visible = AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
                    lblAlwaysConvert.Visible = AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
                }
            }
            else
            {
                hlblOpenSettings.Visible = false;
                lblAlwaysConvert.Visible = false;
                lblAlwaysConvert.Text = formatWarning;
            }
        }

        private void cbVideoFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat && !disableIntendedVideoUpdate)
            {
                this.intendedVideoFormat = SystemCodecMaps.GetVideoFormatByExtension(cbVideoFormat.SelectedItem.ToString());
            }
            checkConversionOptions();
        }

        private void cbAudioFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat && !disableIntendedAudioUpdate)
            {
                this.intendedAudioFormat = SystemCodecMaps.GetAudioFormatByExtension(cbAudioFormat.SelectedItem.ToString());
            }
            checkConversionOptions();
        }
        
        private void btnOpenDL_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(btnOpenDL.Text) && System.IO.File.Exists(btnOpenDL.Text))
            {
                openFileLocation(btnOpenDL.Text);
            }
        }

        private void openFileLocation(string path)
        {
            string argument = "/select, \"" + path + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }       

        private void btnCancelProcess_MouseMove(object sender, MouseEventArgs e)
        {
            if (Cancel_MouseMove != null)
                Cancel_MouseMove(sender, e);
        }

        private void btnCancelProcess_MouseLeave(object sender, EventArgs e)
        {
            if (Cancel_MouseLeave != null)
                Cancel_MouseLeave(sender, e);
        }        

        private void hlblOpenSettings_Click(object sender, EventArgs e)
        {
            parentMainForm.OpenSettingsDialog("Advanced");
        }

        private void pbListProgress_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (pbListProgress.Tag != null && !string.IsNullOrEmpty(pbListProgress.Tag.ToString()))
            {
                string[] values = pbListProgress.Tag.ToString().Split(':');
                e.DisplayText = $"{values[0]} of {values[1]}";
            }
            else
            {
                e.DisplayText = string.Empty;
            }
        }

        private void cbMaxRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (inInit) return;
            if (!string.IsNullOrEmpty(cbMaxRes.Text))
            {
                Resolution? maxRes = EnumExtensions.ToEnum<Resolution>(cbMaxRes.Text);
                if (maxRes != null)
                {
                    AppSettings.Default.General.MaxResolutionBest = (Resolution)maxRes;
                    AppSettings.Default.Save();
                }
            }
        }

        private void txtMaxFilesize_TextChanged(object sender, EventArgs e)
        {
            if(inInit) return;  
            if (!string.IsNullOrEmpty(txtMaxFilesize.Text))
            {
                bool isint = Int32.TryParse(txtMaxFilesize.Text, out int res);
                if (isint)
                {
                    AppSettings.Default.General.MaxFilesizeBest = res;
                    AppSettings.Default.Save();
                }
            }
        }

        private void tsStart_EditValueChanged(object sender, EventArgs e)
        {
            if(AppSettings.Default.Layout.SegmentControlMode == SegmentControlMode.EndTime && tsDuration.TimeSpan <= tsStart.TimeSpan)
            {
                tsDuration.TimeSpan = tsStart.TimeSpan.Add(TimeSpan.FromSeconds(1));
            }
        }

        private void lblDuration_TextChanged(object sender, EventArgs e)
        {
            if(lblDuration.Text == "End" && tsDuration.TimeSpan <= tsStart.TimeSpan)
                tsDuration.TimeSpan = tsStart.TimeSpan.Add(TimeSpan.FromSeconds(1));
            else
                tsDuration.TimeSpan = TimeSpan.FromSeconds(1);
        }

        private void tsDuration_EditValueChanged(object sender, EventArgs e)
        {
            if(AppSettings.Default.Layout.SegmentControlMode == SegmentControlMode.EndTime && tsDuration.TimeSpan <= tsStart.TimeSpan)
            {
                tsDuration.TimeSpan = tsStart.TimeSpan.Add(TimeSpan.FromSeconds(1));
            }
        }
    }

    public enum ControlGroups
    {
        Segment = 0,
        Crop = 1,
        Convert = 2,
        Limits = 3
    }
}
