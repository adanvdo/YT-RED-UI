using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using YT_RED.Settings;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using YT_RED.Logging;
using YT_RED.Classes;
using DevExpress.XtraEditors;

namespace YT_RED.Controls
{
    public partial class ControlPanel : DevExpress.XtraEditors.XtraUserControl
    {
        private string formatWarning = "YT-RED is currently set to Always Convert to your\nPreferred Video and Audio Format.\nThis can be changed in Advanced Settings";

        [Browsable(false)]
        public DownloadLog TargetLog
        {
            get
            {
                if (selectedHistoryIndex >= 0)
                {
                    var log = gvHistory.GetRow(selectedHistoryIndex) as DownloadLog;
                    return log;
                }
                return null;
            }
        }

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
                    cbAudioFormat.Properties.Items.AddRange(audioFormats);
                    cbVideoFormat.Enabled = false;
                    cbAudioFormat.Enabled = toggleConvert.IsOn;
                    cbAudioFormat.SelectedItem = null;

                }
                else
                {
                    cbVideoFormat.Properties.Items.Clear();
                    cbVideoFormat.Properties.Items.AddRange(videoFormats);
                    cbAudioFormat.Properties.Items.Clear();
                    cbAudioFormat.Enabled = false;
                    cbVideoFormat.Enabled = toggleConvert.IsOn;
                    cbVideoFormat.SelectedItem = null;
                }
            }
            else
            {
                cbVideoFormat.Properties.Items.Clear();
                cbVideoFormat.Properties.Items.AddRange(videoFormats);
                cbAudioFormat.Properties.Items.Clear();
                cbAudioFormat.Properties.Items.AddRange(audioFormats);
                cbVideoFormat.Enabled = toggleConvert.IsOn;
                cbAudioFormat.Enabled = toggleConvert.IsOn;
                cbVideoFormat.SelectedItem = null;
                cbAudioFormat.SelectedItem = null;
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
                    cbVideoFormat.SelectedIndex = cbVideoFormat.Properties.Items.IndexOf(value.ToString());
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
                    cbAudioFormat.SelectedIndex = cbAudioFormat.Properties.Items.IndexOf(value.ToString());
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
                if(toggleDownloadLimits.IsOn && !string.IsNullOrEmpty(txtMasFilesize.Text))
                {
                    int max = 0;
                    string val = txtMasFilesize.Text.Split(' ')[0];
                    bool pi = int.TryParse(val, out int maxSize);
                    if(pi)
                        max = maxSize;
                    return max;
                }
                return 0;
            }
            set
            {
                txtMasFilesize.Text = value.ToString();
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

        private MainForm parentMainForm = null;

        public MainForm ParentMainForm { set { parentMainForm = value; } }

        /// <summary>
        /// Control Constructor
        /// </summary>
        public ControlPanel()
        {
            InitializeComponent();
            toggleForeColor = toggleSegment.ForeColor;
            InitControls();
        }

        public void InitControls()
        {
            videoFormats = new List<string>();
            List<string> vFormats = new List<string>() { "" };
            vFormats.AddRange(Enum.GetNames(typeof(VideoFormat)).Cast<string>());
            videoFormats.AddRange(vFormats.Where(f => f != "UNSPECIFIED"));
            cbVideoFormat.Properties.Items.AddRange(videoFormats);
            cbVideoFormat.SelectedIndex = 0;
            audioFormats = new List<string>();
            List<string> aFormats = new List<string>() { "" };
            aFormats.AddRange(Enum.GetNames(typeof(AudioFormat)).Cast<string>());
            audioFormats.AddRange(aFormats.Where(f => f != "UNSPECIFIED"));
            cbAudioFormat.Properties.Items.AddRange(audioFormats);
            cbAudioFormat.SelectedIndex = 0;
            cbMaxRes.Properties.Items.AddRange(Utils.VideoUtil.ResolutionList);
            cbMaxRes.SelectedItem = AppSettings.Default.General.MaxResolutionBest;
            txtMasFilesize.Text = AppSettings.Default.General.MaxFilesizeBest.ToString();
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
            DisableToggle(true, true, true, forFormatList);
            EnableToggle(true, true, true, false, !forFormatList);
            cbVideoFormat.SelectedIndex = 0;
            cbAudioFormat.SelectedIndex = 0;
            MaxResolution = AppSettings.Default.General.MaxResolutionBest;
            MaxFilesize = AppSettings.Default.General.MaxFilesizeBest;
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
            
            hlblOpenSettings.Visible = !toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            lblAlwaysConvert.Visible = !toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
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
                gcDownloadLimits.Visible = false;
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
            }
            if (enableLimits)
            {
                gcDownloadLimits.Visible = true;
                toggleDownloadLimits.Enabled = true;
                if(turnOn) toggleDownloadLimits.IsOn = true;
            }
        }

        private void gcSegments_Click(object sender, EventArgs e)
        {
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
        }

        private void gcCrop_Click(object sender, EventArgs e)
        {
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
        }

        private void gcConvert_Click(object sender, EventArgs e)
        {
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
        }

        private void gcDownloadLimits_Click(object sender, EventArgs e)
        {
            if(toggleDownloadLimits.IsOn) return;
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
            cbVideoFormat.Enabled = toggleConvert.IsOn;
            cbAudioFormat.Enabled = toggleConvert.IsOn;
            hlblOpenSettings.Visible = !toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            lblAlwaysConvert.Visible = !toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
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
            txtMasFilesize.Enabled = toggleDownloadLimits.IsOn;
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
            if (cbVideoFormat.SelectedItem != null && cbAudioFormat.SelectedItem != null 
                && !string.IsNullOrEmpty(cbVideoFormat.SelectedItem.ToString()) && !string.IsNullOrEmpty(cbAudioFormat.SelectedItem.ToString()))
            {
                lblAlwaysConvert.Text = "Audio Format will be automatically determined\nwhen downloading \"Best [audio+video]\"";
                hlblOpenSettings.Visible = true;
                lblAlwaysConvert.Visible = true;
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
            checkConversionOptions();
        }

        private void cbAudioFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkConversionOptions();
        }

        private void gvHistory_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Logging.DownloadLog row = gvHistory.GetRow(gvHistory.FocusedRowHandle) as Logging.DownloadLog;
                if (row != null && row.FileExists)
                {
                    openFileLocation(row.DownloadLocation);
                }
            }
            catch (Exception ex)
            {
                Logging.ExceptionHandler.LogException(ex);
            }
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

        private void historyTooltip_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            GridHitInfo hitInfo = gvHistory.CalcHitInfo(e.ControlMousePosition);
            if (hitInfo != null && hitInfo.InRowCell)
            {
                object o;
                if (hitInfo.Column.FieldName == "FileExists")
                {
                    bool fileExits = Convert.ToBoolean(gvHistory.GetRowCellValue(hitInfo.RowHandle, "FileExists"));
                    o = $"{hitInfo.HitTest}{hitInfo.RowHandle}";
                    e.Info = new DevExpress.Utils.ToolTipControlInfo(o, fileExits ? "File Exists" : "File Not Found");
                }
                if(hitInfo.Column.FieldName == "PostProcessed")
                {
                    o = $"{hitInfo.HitTest}{hitInfo.RowHandle}";
                    var row = gvHistory.GetRow(hitInfo.RowHandle) as DownloadLog;
                    string details = $"URL: {row.Url}\nFormat: {row.Format}\n";

                    if (row.PostProcessed)
                    {
                        if (row.Start != null && row.Duration != null)
                        {
                            details += $"Segment Start: {(TimeSpan)row.Start} - Duration: {(TimeSpan)row.Duration}\n";
                        }
                        if (row.Crops != null && row.Crops.Length > 0)
                        {
                            details += $"Crop: Top ({row.Crops[0]}) Bottom ({row.Crops[1]}) Left ({row.Crops[2]}) Right ({row.Crops[3]})\n";
                        }
                        if (row.VideoConversionFormat != null)
                        {
                            details += $"Convert Video: {row.VideoConversionFormat}\n";
                        }
                        if (row.AudioConversionFormat != null)
                        {
                            details += $"Convert Audio: {row.AudioConversionFormat}\n";
                        }
                    }
                    e.Info = new DevExpress.Utils.ToolTipControlInfo(o, details);
                }
            }
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

        private int selectedHistoryIndex = -1;
        private void gcHistory_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                GridHitInfo hitInfo = gvHistory.CalcHitInfo(e.Location);
                if (hitInfo != null && hitInfo.InDataRow) {
                    selectedHistoryIndex = hitInfo.RowHandle;
                    historyPopup.ShowPopup(Control.MousePosition);
                }
            }
        }

        private void historyPopup_CloseUp(object sender, EventArgs e)
        {
        }

        private void bbiReDownload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(ReDownload_Click != null) ReDownload_Click(sender, e);
        }

        private void bbiNewDownload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(NewDownload_Click != null) NewDownload_Click(sender, e);
        }

        public void PopulateHistoryColumns()
        {
            gvHistory.Columns.Clear();
            gvHistory.PopulateColumns();
            gvHistory.Columns["FileExists"].VisibleIndex = 0;
            gvHistory.Columns["DownloadType"].VisibleIndex = 1;
            gvHistory.Columns["DownloadLocation"].VisibleIndex = 2;
            gvHistory.Columns["PostProcessed"].VisibleIndex = 3;
            gvHistory.Columns["PostProcessed"].MaxWidth = 25;
            gvHistory.Columns["PostProcessed"].ColumnEdit = repPostProcessed;
            gvHistory.Columns["PostProcessed"].OptionsColumn.ShowCaption = false;
            gvHistory.Columns["FileExists"].ColumnEdit = repFileExists;
            gvHistory.Columns["FileExists"].FieldName = "FileExists";
            gvHistory.Columns["FileExists"].OptionsColumn.ShowCaption = false;
            gvHistory.Columns["FileExists"].MinWidth = 5;
            gvHistory.Columns["FileExists"].MaxWidth = 25;
            gvHistory.Columns["FileExists"].Width = 10;
            gvHistory.Columns["FileExists"].ToolTip = "File Exists?";
            gvHistory.Columns["DownloadType"].Width = 10;
            gvHistory.Columns["DownloadType"].MaxWidth = 75;
            gvHistory.Columns["DownloadType"].Caption = "Type";
            gvHistory.Columns["Url"].Visible = false;
            gvHistory.Columns["TimeLogged"].Visible = false;
            gvHistory.Columns["Type"].Visible = false;
            gvHistory.Columns["Downloaded"].Visible = false;
            gvHistory.Columns["Format"].Visible = false;
            gvHistory.Columns["FormatPair"].Visible = false;
            gvHistory.Columns["Start"].Visible = false;
            gvHistory.Columns["Duration"].Visible = false;
            gvHistory.Columns["Crops"].Visible = false;
            gvHistory.Columns["VideoConversionFormat"].Visible = false;
            gvHistory.Columns["AudioConversionFormat"].Visible = false;
            gvHistory.RefreshData();
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

        
    }
}
