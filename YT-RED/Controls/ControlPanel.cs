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
using DevExpress.Dialogs.Core;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Svg;
using DevExpress.Skins;

namespace YT_RED.Controls
{
    public partial class ControlPanel : DevExpress.XtraEditors.XtraUserControl
    {
        private OptionManager optionManager = new OptionManager();
        public OptionManager OptionManager
        {
            get { return optionManager; }
            set { optionManager = value; }
        }

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
                return SegmentEnabled || CropEnabled || ConversionEnabled || PrependEnabled || ExternalAudioEnabled || ExternalImageEnabled;
            }
        }

        [Browsable(false)]
        public bool SegmentEnabled
        {
            get { return optionManager.Segment.FunctionallyEnabled; }
        }

        [Browsable(false)]
        public bool CropEnabled
        {
            get { return optionManager.Crop.FunctionallyEnabled; }
        }

        [Browsable(false)]
        public bool ConversionEnabled
        {
            get { return optionManager.Convert.FunctionallyEnabled; }
        }

        [Browsable(false)]
        public bool LimitsEnabled
        {
            get { return optionManager.Restrictions.FunctionallyEnabled; }
        }

        [Browsable(false)]
        public bool PrependEnabled
        {
            get { return optionManager.PrependImage.FunctionallyEnabled; }
        }

        [Browsable(false)]
        public bool ExternalAudioEnabled
        {
            get { return optionManager.ExternalAudio.FunctionallyEnabled; }
        }

        [Browsable(false)]
        public bool ExternalImageEnabled
        {
            get { return optionManager.ExternalImage.FunctionallyEnabled; }
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
                    cbVideoFormat.SelectedItem = null;
                    ConvertAudioFormat = AppSettings.Default.Advanced.PreferredAudioFormat;

                }
                else
                {
                    cbVideoFormat.Properties.Items.Clear();
                    cbVideoFormat.Properties.Items.AddRange(videoFormats);
                    cbAudioFormat.Properties.Items.Clear();
                    cbAudioFormat.Enabled = false;
                    cbVideoFormat.Enabled = toggleConvert.IsOn;
                    cbAudioFormat.SelectedItem = null;
                    ConvertVideoFormat = AppSettings.Default.Advanced.PreferredVideoFormat;
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
            }
            cbAudioFormat.Enabled = toggleConvert.IsOn && !AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            cbVideoFormat.Enabled = toggleConvert.IsOn && !AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            hlblOpenSettings.Visible = toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            lblAlwaysConvert.Visible = toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
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

        [Browsable(false)]
        public string PrependImagePath
        {
            get { return txtPrependPath.Text; }
            set { txtPrependPath.Text = value; }
        }

        public string PrependDuration
        {
            get { return txtPrependDuration.Text; }
            set 
            {
                bool parse = Int32.TryParse(value.ToString(), out int dur);
                if (parse && dur < 1) txtPrependDuration.Text = "1";
                else if(value.ToString() != "0") txtPrependDuration.Text = value; 
            }
        }

        [Browsable(false)]
        public MediaDuration? PrependDurationType
        {
            get
            {
                if (togglePrepend.IsOn && cbPrependType.SelectedItem != null && !string.IsNullOrEmpty(cbPrependType.Text))
                {
                    MediaDuration dt = MediaDuration.Frames;
                    bool p = Enum.TryParse(cbPrependType.Text, out MediaDuration md);
                    if (p)
                        dt = md;
                    return dt;
                }
                return null;
            }
            set
            {
                MediaDuration? d = value;
                if (d != null)
                    cbPrependType.SelectedIndex = cbPrependType.Properties.Items.IndexOf(value.ToString());
            }
        }

        [Browsable(false)]
        public string CustomAudioPath
        {
            get { return txtAudioPath.Text; }
            set { txtAudioPath.Text = value; }
        }

        [Browsable(false)]
        public TimeSpan? CustomAudioStart
        {
            get { return tsAudioStart.TimeSpan; }
            set { tsAudioStart.TimeSpan = value == null ? TimeSpan.Zero : (TimeSpan)value; }
        }

        [Browsable(false)]
        public TimeSpan? CustomAudioDuration
        {
            get { return tsAudioDuration.TimeSpan; }
            set { tsAudioDuration.TimeSpan = value == null ? TimeSpan.Zero : (TimeSpan)value;}
        }

        [Browsable(false)]
        public string VideoImagePath
        {
            get { return txtImageVideoPath.Text; }
            set { txtImageVideoPath.Text = value; }
        }

        [Browsable(false)]
        public TargetResolution? VideoTargetResolution
        {
            get
            {
                if(toggleExternalImage.IsOn && cbTargetRes.SelectedItem != null && !string.IsNullOrEmpty(cbTargetRes.SelectedItem.ToString()))
                {
                    TargetResolution? tr = EnumExtensions.GetFromDescription<TargetResolution>(cbTargetRes.Text);
                    return tr;
                }
                return null;
            }

            set
            {
                TargetResolution? tr = value;
                if(tr != null)
                {
                    cbTargetRes.SelectedIndex = cbTargetRes.Properties.Items.IndexOf(value.ToString());
                }
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
        public ResolutionFilter? MaxResolution
        {
            get
            {
                if(toggleDownloadLimits.IsOn && cbMaxRes.SelectedItem != null && !string.IsNullOrEmpty(cbMaxRes.SelectedItem.ToString()))
                {
                    ResolutionFilter r = ResolutionFilter.ANY;
                    bool pr = Enum.TryParse(cbMaxRes.SelectedItem.ToString(), out ResolutionFilter ppr);
                    if (pr)
                        r = ppr;
                    return r;
                }
                return null;
            }
            set
            {
                ResolutionFilter? v = value;
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
                    case Classes.ResolutionFilter.SD:
                        return 480;
                    case Classes.ResolutionFilter.HD720p:
                        return 720;
                    case Classes.ResolutionFilter.HD1080p:
                        return 1080;
                    case Classes.ResolutionFilter.UHD2160p:
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
                    if (!pbProgress.Visible) pbProgress.Visible = true;
                    pbProgress.Position = progress;
                    pbProgress.Refresh();
                    if(progress == 100) pbProgress.Visible = false;
                };
                pbProgress.Invoke(safeUpdate);
            }
            else
            {
                if(!pbProgress.Visible) pbProgress.Visible = true;
                pbProgress.Position = progress;
                pbProgress.Refresh();
                if (progress == 100) pbProgress.Visible = false;
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
            pbProgress.Visible = true;
        }

        public void HideProgress()
        {
            pbProgress.Position = 0;
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
            InitControls();
        }

        public void InitControls(bool invoked = false)
        {            
            videoFormats = new List<string>();
            List<string> vFormats = new List<string>() { "" };
            vFormats.AddRange(Enum.GetNames(typeof(VideoFormat)).Cast<string>());
            videoFormats.AddRange(vFormats.Where(f => f != "UNSPECIFIED"));
            cbVideoFormat.Properties.Items.Clear();
            cbVideoFormat.Properties.Items.AddRange(videoFormats);
            cbVideoFormat.SelectedIndex = 0;
            audioFormats = new List<string>();
            List<string> aFormats = new List<string>() { "" };
            aFormats.AddRange(Enum.GetNames(typeof(AudioFormat)).Cast<string>());
            audioFormats.AddRange(aFormats.Where(f => f != "UNSPECIFIED"));
            cbAudioFormat.Properties.Items.Clear();
            cbAudioFormat.Properties.Items.AddRange(audioFormats);
            cbAudioFormat.SelectedIndex = 0;
            cbMaxRes.Properties.Items.Clear();
            cbMaxRes.Properties.Items.AddRange(Utils.VideoUtil.ResolutionList);
            cbMaxRes.SelectedIndex = 4;
            txtMaxFilesize.Text = "0";

            txtPrependPath.Text = string.Empty;
            txtPrependDuration.Text = "2"; 
            List<string> durationTypes = new List<string>();
            durationTypes.AddRange(Enum.GetNames(typeof(MediaDuration)).Cast<string>());
            cbPrependType.Properties.Items.Clear();
            cbPrependType.Properties.Items.AddRange(durationTypes);
            cbPrependType.SelectedIndex = 0;

            txtAudioPath.Text = string.Empty;
            tsAudioStart.TimeSpan = TimeSpan.Zero;
            tsAudioDuration.TimeSpan = TimeSpan.Zero;

            txtImageVideoPath.Text = string.Empty;
            List<string> targetResolutions = new List<string>();
            var t = EnumExtensions.GetCustomDescriptions(typeof(TargetResolution));
            targetResolutions.AddRange(t);
            cbTargetRes.Properties.Items.Clear();
            cbTargetRes.Properties.Items.AddRange(targetResolutions);
            cbTargetRes.SelectedIndex = cbTargetRes.Properties.Items.Count - 1;
            
            inInit = false;
            if (invoked)
            {
                ResetControls();
            }

            this.controlsUpdated();
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
            ShowHideControlGroup(ControlGroups.Segment, AppSettings.Default.General.SegmentOption.Visible);
            ShowHideControlGroup(ControlGroups.Crop, AppSettings.Default.General.CropOption.Visible);
            ShowHideControlGroup(ControlGroups.Convert, AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat || AppSettings.Default.General.ConvertOption.Visible);
            cbVideoFormat.SelectedIndex = 0;
            cbAudioFormat.SelectedIndex = 0;
            cbVideoFormat.Enabled = true;
            cbAudioFormat.Enabled = true;
            DisableToggle(true, true, true, forFormatList, true, true, true);
            EnableToggle(true, true, true, false, !forFormatList, true, true, true);
            bool check = !forFormatList && AppSettings.Default.General.RestrictionOption.Visible;
            ShowHideControlGroup(ControlGroups.Limits, AppSettings.Default.General.EnforceRestrictions || AppSettings.Default.General.RestrictionOption.Visible);
            ShowHideControlGroup(ControlGroups.Prepend, AppSettings.Default.General.PrependOption.Visible);
            ShowHideControlGroup(ControlGroups.Audio, AppSettings.Default.General.AudioOption.Visible);
            txtAudioPath.Text = string.Empty;
            tsAudioStart.TimeSpan = TimeSpan.Zero;
            tsAudioDuration.TimeSpan = TimeSpan.Zero;
            ShowHideControlGroup(ControlGroups.Image, AppSettings.Default.General.ImageOption.Visible);
            txtImageVideoPath.Text = string.Empty;
            
            if (AppSettings.Default.General.EnforceRestrictions && !forFormatList)
            {
                EnableToggle(false, false, false, true, true);
                MaxResolution = AppSettings.Default.General.MaxResolutionBest;
                MaxFilesize = AppSettings.Default.General.MaxFilesizeBest;
                cbMaxRes.Enabled = false;
                txtMaxFilesize.Enabled = false;
                gcDownloadLimits.SendToBack();
            }

            hlblGenSettings.Visible = AppSettings.Default.General.EnforceRestrictions;
            lblLimitWarning.Visible = AppSettings.Default.General.EnforceRestrictions;

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
                gcConvert.SendToBack();
            }

            hlblOpenSettings.Visible = toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            lblAlwaysConvert.Visible = toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;

            togglePrepend.IsOn = false;
            txtPrependPath.Text = string.Empty;
            txtPrependDuration.Text = "2";
            cbPrependType.SelectedIndex = 0;

            toggleExternalAudio.IsOn = false;
            txtAudioPath.Text = string.Empty;
            tsAudioStart.TimeSpan = TimeSpan.Zero;
            tsAudioDuration.TimeSpan = TimeSpan.Zero;

            toggleExternalImage.IsOn = false;
            txtImageVideoPath.Text = string.Empty;                

            this.controlsUpdated();
        }

        private Color customBackcolor = Color.White;
        private Color customForecoler = Color.Black;

        private void controlsUpdated()
        {         
            if (Controls_Updated != null) Controls_Updated(this, EventArgs.Empty);
        }

        public void DisableToggle(bool disableSegment = false, bool disableCrop = false, bool disableConvert = false, bool disableLimits = false,
            bool disablePrepend = false, bool disableAudio = false, bool disableImage = false)
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
            if(disablePrepend)
            {
                togglePrepend.IsOn = false;
                togglePrepend.Enabled = false;
            }
            if (disableAudio)
            {
                toggleExternalAudio.IsOn = false;
                toggleExternalAudio.Enabled = false;
            }
            if (disableImage)
            {
                toggleExternalImage.IsOn = false;
                toggleExternalImage.Enabled = false;
            }
        }

        public void DisableNonRestrictionToggles()
        {
            DisableToggle(true, true, true, false, true, true, true);
        }

        public void SetSelectionText(string text)
        {
            this.lblSelectionText.Text = text;            
        }

        public void EnableToggle(bool enableSegment = false, bool enableCrop = false, bool enableConvert = false, bool turnOn = false, 
            bool enableLimits = false, bool enablePrepend = false, bool enableAudio = false, bool enableImage = false)
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
                cbMaxRes.Enabled = !AppSettings.Default.General.EnforceRestrictions;
                txtMaxFilesize.Enabled = !AppSettings.Default.General.EnforceRestrictions;
            }
            if(enablePrepend)
            {
                togglePrepend.Enabled = true;
                if(turnOn) togglePrepend.IsOn = true;

            }
            if (enableAudio)
            {
                toggleExternalAudio.Enabled = true;
                if(turnOn) toggleExternalAudio.IsOn = true;

            }
            if (enableImage)
            {
                toggleExternalImage.Enabled = true;
                if(turnOn) toggleExternalImage.IsOn = true;

            }
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
                    if (AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat) gcConvert.SendToBack();
                    gcConvert.Visible = show; 
                    break;
                case ControlGroups.Limits:
                    gcDownloadLimits.Visible = show; break;
                case ControlGroups.Prepend:
                    gcPrependImage.Visible = show; break;
                case ControlGroups.Audio:
                    gcOverrideAudio.Visible = show; break;
                case ControlGroups.Image:
                    gcExternalImage.Visible = show; break;
            }
            this.controlsUpdated();
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
                        gcSegments.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlSegPanel.Visible = true;
                        gcSegments.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
                case ControlGroups.Crop:
                    if (toggleCrop.IsOn) return;
                    if (pnlCropPanel.Visible)
                    {
                        pnlCropPanel.Visible = false;
                        gcCrop.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlCropPanel.Visible = true;
                        gcCrop.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
                case ControlGroups.Convert:
                    if (toggleConvert.IsOn) return;
                    if (pnlConvertPanel.Visible)
                    {
                        pnlConvertPanel.Visible = false;
                        gcConvert.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlConvertPanel.Visible = true;
                        gcConvert.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
                case ControlGroups.Limits:
                    if (toggleDownloadLimits.IsOn) return;
                    if (pnlLimitPanel.Visible)
                    {
                        pnlLimitPanel.Visible = false;
                        gcDownloadLimits.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlLimitPanel.Visible = true;
                        gcDownloadLimits.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
                case ControlGroups.Prepend:
                    if (togglePrepend.IsOn) return;
                    if (pnlPrepend.Visible)
                    {
                        pnlPrepend.Visible = false;
                        gcPrependImage.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlPrepend.Visible = true;
                        gcPrependImage.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
                case ControlGroups.Audio:
                    if (toggleExternalAudio.IsOn) return;
                    if (pnlAudio.Visible)
                    {
                        pnlAudio.Visible = false;
                        gcOverrideAudio.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlAudio.Visible = true;
                        gcOverrideAudio.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
                case ControlGroups.Image:
                    if (toggleExternalImage.IsOn) return;
                    if (pnlImage.Visible)
                    {
                        pnlImage.Visible = false;
                        gcExternalImage.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_add;
                    }
                    else
                    {
                        pnlImage.Visible = true;
                        gcExternalImage.CustomHeaderButtons[1].Properties.ImageOptions.SvgImage = Properties.Resources.actions_remove;
                    }
                    break;
            }
        }

        public void UpdatePanelStates()
        {
            updateSegmentState();
            updateCropState();
            UpdateConvertState();
            UpdateLimitState();
            UpdatePrependState();
            UpdateExternalAudioState();
            UpdateExternalImageState();
            UpdateOptionButtonStates();

            int i = 0;
            foreach(Control ctrl in pnlOptionPanel.Controls)
            {
                if (ctrl.GetType() == typeof(YT_RED.Controls.YTRGroupControl))
                {
                    ctrl.TabIndex = i;
                    i++;
                }
                else ctrl.TabIndex = (pnlOptionPanel.Controls.Count + 1) - i;
            }
            int retainer = pnlOptionPanel.VerticalScroll.Value;
            pnlOptionPanel.VerticalScroll.Value = pnlOptionPanel.VerticalScroll.Maximum;
            bottomScrollValue = pnlOptionPanel.VerticalScroll.Value;
            pnlOptionPanel.VerticalScroll.Value = retainer;

            updateControlScroller();
        }

        private void updateControlScroller()
        {
            DevExpress.Skins.Skin currentSkin = DevExpress.Skins.CommonSkins.GetSkin(this.LookAndFeel.ActiveLookAndFeel);
            var s = currentSkin.SvgPalettes["DefaultSkinPalette"];
            var a = s.Colors.FirstOrDefault(c => c.Name == "Foreground 50");
            if (a != null) customBackcolor = a.Value;
            var b = s.Colors.FirstOrDefault(c => c.Name == "Foreground 100");
            if (b != null) customForecoler = b.Value;

            if (pnlOptionPanel.VerticalScroll.Visible && currentSkin != null)
            {
                Bitmap bmp = new Bitmap(btnScrollOptions.Width, btnScrollOptions.Height);
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    gfx.Clear(customBackcolor);
                }

                btnScrollOptions.BackgroundImage = bmp;
                btnScrollOptions.ForeColor = customForecoler;
            }

            btnScrollOptions.Visible = pnlOptionPanel.VerticalScroll.Visible;
        }

        public void UpdateOptionButtonStates()
        {
            lblOptionsInfo.Visible = !OptionManager.VisibleOption;
            bbiSegment.Enabled = !optionManager.Segment.Visible;
            bbiCrop.Enabled = !optionManager.Crop.Visible;
            bbiConvert.Enabled = !optionManager.Convert.Visible;
            bbiRestrictions.Enabled = !optionManager.Restrictions.Visible;
            bbiPrepend.Enabled = !optionManager.PrependImage.Visible;
            bbiAudio.Enabled = !optionManager.ExternalAudio.Visible;
            bbiImage.Enabled = !optionManager.ExternalImage.Visible;
        }

        private void toggleSegment_Toggled(object sender, EventArgs e)
        {
            updateSegmentState();
        }

        private void updateSegmentState()
        {
            optionManager.Segment.Enabled = toggleSegment.IsOn;
            optionManager.Segment.Visible = gcSegments.Visible;
            optionManager.Save();
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
            gcSegments.CustomHeaderButtons[1].Properties.Enabled = !toggleSegment.IsOn;
        }

        private void toggleCrop_Toggled(object sender, EventArgs e)
        {
            updateCropState();
        }

        private void updateCropState()
        {
            optionManager.Crop.Enabled = toggleCrop.IsOn;
            optionManager.Crop.Visible = gcCrop.Visible;
            optionManager.Save();
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
            gcCrop.CustomHeaderButtons[1].Properties.Enabled = !toggleCrop.IsOn;
            btnDownloadAudio.Enabled = !toggleCrop.IsOn;
        }

        private void toggleConvert_Toggled(object sender, EventArgs e)
        {
            UpdateConvertState();
        }

        public void UpdateConvertState()
        {
            optionManager.Convert.Enabled = toggleConvert.IsOn;
            optionManager.Convert.Visible = gcConvert.Visible;
            optionManager.Save();
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
            gcConvert.CustomHeaderButtons[0].Properties.Enabled = !AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            gcConvert.CustomHeaderButtons[1].Properties.Enabled = !toggleConvert.IsOn;
        }

        private void togglePrepend_Toggled(object sender, EventArgs e)
        {
            UpdatePrependState();
        }

        public void UpdatePrependState()
        {
            optionManager.PrependImage.Enabled = togglePrepend.IsOn;
            optionManager.PrependImage.Visible = gcPrependImage.Visible;
            optionManager.Save();
            if (togglePrepend.IsOn)
            {
                togglePrepend.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
                togglePrepend.Properties.Appearance.BorderColor = Color.LightGreen;
                togglePrepend.BackColor = Color.LightGreen;
                togglePrepend.ForeColor = Color.Black;
            }
            else
            {
                togglePrepend.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                togglePrepend.Properties.Appearance.BorderColor = Color.Transparent;
                togglePrepend.BackColor = Color.Transparent;
                togglePrepend.ForeColor = toggleForeColor;
            }
            txtPrependPath.Enabled = togglePrepend.IsOn;
            txtPrependDuration.Enabled = togglePrepend.IsOn;
            cbPrependType.Enabled = togglePrepend.IsOn;
            btnBrowsePrepend.Enabled = togglePrepend.IsOn;
            gcPrependImage.CustomHeaderButtons[1].Properties.Enabled = !togglePrepend.IsOn;
        }
        private void toggleExternalAudio_Toggled(object sender, EventArgs e)
        {
            UpdateExternalAudioState();
        }        

        public void UpdateExternalAudioState()
        {
            optionManager.ExternalAudio.Enabled = toggleExternalAudio.IsOn;
            optionManager.ExternalAudio.Visible = gcOverrideAudio.Visible;
            optionManager.Save();
            if (toggleExternalAudio.IsOn)
            {
                toggleExternalAudio.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
                toggleExternalAudio.Properties.Appearance.BorderColor = Color.LightGreen;
                toggleExternalAudio.BackColor = Color.LightGreen;
                toggleExternalAudio.ForeColor = Color.Black;
            }
            else
            {
                toggleExternalAudio.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                toggleExternalAudio.Properties.Appearance.BorderColor = Color.Transparent;
                toggleExternalAudio.BackColor = Color.Transparent;
                toggleExternalAudio.ForeColor = toggleForeColor;
            }
            txtAudioPath.Enabled = toggleExternalAudio.IsOn;
            tsAudioStart.Enabled = toggleExternalAudio.IsOn;
            tsAudioDuration.Enabled = toggleExternalAudio.IsOn;
            btnBrowseAudio.Enabled = toggleExternalAudio.IsOn;
            gcOverrideAudio.CustomHeaderButtons[1].Properties.Enabled = !toggleExternalAudio.IsOn;
        }

        private void toggleExternalImage_Toggled(object sender, EventArgs e)
        {
            UpdateExternalImageState();
        }

        public void UpdateExternalImageState()
        {
            optionManager.ExternalImage.Enabled = toggleExternalImage.IsOn;
            optionManager.ExternalImage.Visible = gcExternalImage.Visible;
            optionManager.Save();
            if (toggleExternalImage.IsOn)
            {
                toggleExternalImage.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
                toggleExternalImage.Properties.Appearance.BorderColor = Color.LightGreen;
                toggleExternalImage.BackColor = Color.LightGreen;
                toggleExternalImage.ForeColor = Color.Black;
            }
            else
            {
                toggleExternalImage.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                toggleExternalImage.Properties.Appearance.BorderColor = Color.Transparent;
                toggleExternalImage.BackColor = Color.Transparent;
                toggleExternalImage.ForeColor = toggleForeColor;
            }
            txtImageVideoPath.Enabled = toggleExternalImage.IsOn;
            btnBrowseVidImage.Enabled = toggleExternalImage.IsOn;
            cbTargetRes.Enabled = toggleExternalImage.IsOn;
            gcOverrideAudio.CustomHeaderButtons[1].Properties.Enabled = !toggleExternalImage.IsOn;
            btnDownloadBest.Enabled = !toggleExternalImage.IsOn;
        }

        private void toggleDownloadLimits_Toggled(object sender, EventArgs e)
        {
            UpdateLimitState();
        }

        public void UpdateLimitState()
        {
            optionManager.Restrictions.Enabled = toggleDownloadLimits.IsOn;
            optionManager.Restrictions.Visible = gcDownloadLimits.Visible;
            optionManager.Save();
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
            hlblGenSettings.Visible = toggleDownloadLimits.IsOn && AppSettings.Default.General.EnforceRestrictions;
            lblLimitWarning.Visible = toggleDownloadLimits.IsOn && AppSettings.Default.General.EnforceRestrictions;
            cbMaxRes.Enabled = toggleDownloadLimits.IsOn && !AppSettings.Default.General.EnforceRestrictions;
            txtMaxFilesize.Enabled = toggleDownloadLimits.IsOn && !AppSettings.Default.General.EnforceRestrictions;
            gcDownloadLimits.CustomHeaderButtons[0].Properties.Enabled = !AppSettings.Default.General.EnforceRestrictions;
            gcDownloadLimits.CustomHeaderButtons[1].Properties.Enabled = !toggleDownloadLimits.IsOn;
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
                lblAlwaysConvert.Text = formatWarning;
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
            if (inInit) return;
            if (cbVideoFormat.SelectedItem == null) optionManager.Convert.Parameters["video"] = VideoFormat.UNSPECIFIED;
            else
            {
                bool vfp = Enum.TryParse(cbVideoFormat.SelectedItem.ToString(), out Settings.VideoFormat vf);
                optionManager.Convert.Parameters["video"] = vfp ? vf : VideoFormat.UNSPECIFIED;
            }
        }

        private void cbAudioFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkConversionOptions();
            if(inInit) return;            
            if (cbAudioFormat.SelectedItem == null) optionManager.Convert.Parameters["audio"] = AudioFormat.UNSPECIFIED;
            else
            {
                bool afp = Enum.TryParse(cbAudioFormat.SelectedItem.ToString(), out Settings.AudioFormat af);
                optionManager.Convert.Parameters["audio"] = afp ? af : AudioFormat.UNSPECIFIED;
            }
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
                if(hitInfo.Column.FieldName == "AdditionalSettings")
                {
                    string details = "";
                    o = $"{hitInfo.HitTest}{hitInfo.RowHandle}";
                    var row = gvHistory.GetRow(hitInfo.RowHandle) as DownloadLog;                    
                    if (!string.IsNullOrEmpty(row.PlaylistUrl))
                    {
                        details += $"Playlist: {row.PlaylistTitle}\nPlaylist URL: {row.PlaylistUrl}\n";
                    }
                    
                    details += $"URL: {row.Url}\nFormat: {row.Format}\n";                    

                    if (row.AdditionalSettings)
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
                        if(row.MaxResolution != null)
                        {
                            details += $"Max Resolution: {row.MaxResolution.ToFriendlyString(false, false)}\n";
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
            gvHistory.Columns["AdditionalSettings"].VisibleIndex = 3;
            gvHistory.Columns["AdditionalSettings"].MaxWidth = 25;
            gvHistory.Columns["AdditionalSettings"].ColumnEdit = repPostProcessed;
            gvHistory.Columns["AdditionalSettings"].OptionsColumn.ShowCaption = false;
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
            gvHistory.Columns["InSubFolder"].Visible = false;
            gvHistory.Columns["PlaylistTitle"].Visible = false;
            gvHistory.Columns["PlaylistUrl"].Visible = false;
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
            gvHistory.Columns["MaxResolution"].Visible = false;
            gvHistory.Columns["MaxFileSize"].Visible = false;
            gvHistory.Columns["PrependImagePath"].Visible = false;
            gvHistory.Columns["PrependDuration"].Visible = false;
            gvHistory.Columns["PrependDurationType"].Visible = false;
            gvHistory.Columns["ExternalAudioPath"].Visible = false;
            gvHistory.Columns["AudioStartTime"].Visible = false;
            gvHistory.Columns["ExternalImagePath"].Visible = false;
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

        private void cbMaxRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (inInit) return;
            if (!string.IsNullOrEmpty(cbMaxRes.Text))
            {
                ResolutionFilter? maxRes = EnumExtensions.ToEnum<ResolutionFilter>(cbMaxRes.Text);
                if (maxRes != null)
                {
                    AppSettings.Default.General.MaxResolutionBest = (ResolutionFilter)maxRes;
                    AppSettings.Default.Save();
                }
            }
            optionManager.Restrictions.Parameters["maxres"] = (ResolutionFilter)cbMaxRes.SelectedItem;
            optionManager.Restrictions.Parameters["maxsize"] = txtMaxFilesize.Text;
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

        private void segment_OptionChanged(object sender, EventArgs e)
        {
            if(inInit) return;
            optionManager.Segment.Parameters["start"] = tsStart.TimeSpan;
            optionManager.Segment.Parameters["duration"] = tsDuration.TimeSpan;
        }

        private void crop_OptionChanged(object sender, EventArgs e)
        {
            if (inInit) return;
            optionManager.Crop.Parameters["top"] = txtCropTop.Text;
            optionManager.Crop.Parameters["bottom"] = txtCropBottom.Text;
            optionManager.Crop.Parameters["left"] = txtCropLeft.Text;
            optionManager.Crop.Parameters["right"] = txtCropRight.Text;
        }

        private void txtMaxFilesize_EditValueChanged(object sender, EventArgs e)
        {
            if (inInit) return;
            optionManager.Restrictions.Parameters["maxsize"] = txtMaxFilesize.Text;
        }

        private void prepend_OptionChanged(object sender, EventArgs e)
        {
            if (inInit) return;
            bool parse = Int32.TryParse(txtPrependDuration.Text, out int dur);
            if (parse) 
            {
                if (PrependDurationType == MediaDuration.Frames &&  dur < 2) txtPrependDuration.Text = "2";
                else if(PrependDurationType == MediaDuration.Seconds && dur < 1) txtPrependDuration.Text = "1";
            }
            else txtPrependDuration.Text = PrependDurationType == MediaDuration.Frames ? "2" : "1";
            optionManager.PrependImage.Parameters["path"] = txtPrependPath.Text;
            optionManager.PrependImage.Parameters["duration"] = txtPrependDuration.Text;           
        }

        private void customAudio_OptionChanged(object sender, EventArgs e)
        {
            if (inInit) return;
            optionManager.ExternalAudio.Parameters["path"] = txtAudioPath.Text;
            optionManager.ExternalAudio.Parameters["start"] = tsAudioStart.TimeSpan;
            optionManager.ExternalAudio.Parameters["duration"] = tsAudioDuration.TimeSpan;
        }

        private void txtImageVideoPath_EditValueChanged(object sender, EventArgs e)
        {
            if(inInit) return;
            optionManager.ExternalImage.Parameters["path"] = txtImageVideoPath.Text;
        }
        private void cbTargetRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (inInit) return;
            TargetResolution? res = null;
            if (!string.IsNullOrEmpty(cbTargetRes.Text))
            {
                res = EnumExtensions.GetFromDescription<TargetResolution>(cbTargetRes.Text);
                if (res != null)
                {
                    AppSettings.Default.General.ImageOption.Parameters["resolution"] = (TargetResolution)res;
                    AppSettings.Default.Save();
                }
            }
            optionManager.ExternalImage.Parameters["resolution"] = res;
        }

        private void bbiSegment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowHideControlGroup(ControlGroups.Segment, true);
            gcSegments.BringToFront();
            UpdatePanelStates();
        }

        private void bbiCrop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowHideControlGroup(ControlGroups.Crop, true);
            gcCrop.BringToFront();
            UpdatePanelStates();
        }

        private void bbiConvert_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowHideControlGroup(ControlGroups.Convert, true);
            gcConvert.BringToFront(); 
            UpdatePanelStates();
        }

        private void bbiRestrictions_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowHideControlGroup(ControlGroups.Limits, true);
            gcDownloadLimits.BringToFront();
            UpdatePanelStates();
        }

        private void bbiPrepend_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowHideControlGroup(ControlGroups.Prepend, true);
            gcPrependImage.BringToFront();
            UpdatePanelStates();
        }

        private void bbiAudio_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowHideControlGroup(ControlGroups.Audio, true);
            gcOverrideAudio.BringToFront();
            UpdatePanelStates();
        }

        private void bbiImage_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowHideControlGroup(ControlGroups.Image, true);
            gcExternalImage.BringToFront();
            UpdatePanelStates();
        }

        private void groupControl_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            YTRGroupControl gc = sender as YTRGroupControl;
            if(gc != null)
            {
                if(e.Button.Properties.Tag.ToString() == "rem")
                {
                    ShowHideControlGroup(gc.ControlGroup, false);
                    UpdatePanelStates();
                }    
                else
                {
                    ExpandCollapseControlGroup(gc.ControlGroup);
                }
            }
        }

        private void groupControl_MouseDown(object sender, MouseEventArgs e)
        {
            YTRGroupControl gc = sender as YTRGroupControl;
            if (gc != null)
            {
                ExpandCollapseControlGroup(gc.ControlGroup);
            }
        }

        private void ddbOptionMenu_Click(object sender, EventArgs e)
        {
            ddbOptionMenu.ShowDropDown();
        }

        private void hlblGenSettings_Click(object sender, EventArgs e)
        {
            parentMainForm.OpenSettingsDialog("General");
        }

        private void btnBrowsePrepend_Click(object sender, EventArgs e)
        {
            browseForFile(ControlGroups.Prepend);
        }

        private void btnBrowseAudio_Click(object sender, EventArgs e)
        {
            browseForFile(ControlGroups.Audio);
        }

        private void btnBrowseVidImage_Click(object sender, EventArgs e)
        {
            browseForFile(ControlGroups.Image);
        }

        private void browseForFile(ControlGroups group)
        {
            Cursor.Current = Cursors.WaitCursor;
            string path = string.Empty;
            using (XtraOpenFileDialog ofd = new XtraOpenFileDialog())
            {
                ofd.DefaultViewMode = DevExpress.Dialogs.Core.View.ViewMode.MediumIcon;
                if (group == ControlGroups.Prepend || group == ControlGroups.Image)
                {
                    ofd.Filter = "Image Files (*.jpg, *.png)|*.jpg;*.png";
                    ofd.Title = "Select an Image File";
                }
                else if (group == ControlGroups.Audio)
                {
                    ofd.Filter = "Audio Files (*.wav, *.mp3, *.m4a, *.acc)|*.wav;*.mp3;*.m4a;*.acc";
                    ofd.Title = "Select an Audio File";
                }
                
                ofd.CheckFileExists = true;
                ofd.Multiselect = false;

                DialogResult res = ofd.ShowDialog();
                if(res == DialogResult.OK)
                {
                    path = ofd.FileName;
                }
            }

            switch (group)
            {
                case ControlGroups.Prepend:
                    txtPrependPath.Text = path;
                    break;
                case ControlGroups.Audio:
                    txtAudioPath.Text = path;
                    break;
                case ControlGroups.Image:
                    txtImageVideoPath.Text = path;
                    break;
            }
            Cursor.Current = Cursors.Default;
        }

        private void cbPrependType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (inInit) return;
            if (!string.IsNullOrEmpty(cbPrependType.Text))
            {
                MediaDuration? dur = EnumExtensions.ToEnum<MediaDuration>(cbPrependType.Text);
                if (dur != null)
                {
                    optionManager.PrependImage.Parameters["dtype"] = (object)cbPrependType.SelectedItem;
                }
            }
        }

        private int bottomScrollValue = 0;

        private void btnToBottom_Click(object sender, EventArgs e)
        {
            try
            {
                var oc = pnlOptionPanel.Controls.OfType<Control>().Where(c => c.GetType() == typeof(YT_RED.Controls.YTRGroupControl));
                if (oc.Count() > 0)
                {
                    if (btnScrollOptions.Text == "Go To Bottom")
                    {
                        oc = oc.OrderBy(c => c.TabIndex);
                        oc.FirstOrDefault().Select();
                        btnScrollOptions.Text = "Go To Top";
                        btnScrollOptions.ImageOptions.SvgImage = Properties.Resources.moveup;
                    }
                    else
                    {
                        oc = oc.OrderByDescending(c => c.TabIndex);
                        oc.FirstOrDefault().Select();
                        btnScrollOptions.Text = "Go To Bottom";
                        btnScrollOptions.ImageOptions.SvgImage = Properties.Resources.movedown;
                    }
                }
            }
            catch(Exception ex)
            {
                Logging.ExceptionHandler.LogException(ex);
            }
        }

        private void ControlPanel_Paint(object sender, PaintEventArgs e)
        {            
        }

        private void pnlOptionPanel_Scroll(object sender, XtraScrollEventArgs e)
        {
            if(pnlOptionPanel.VerticalScroll.Value < bottomScrollValue)
            {
                btnScrollOptions.Text = "Go To Bottom";
                btnScrollOptions.ImageOptions.SvgImage = Properties.Resources.movedown;
            }
            else
            {
                btnScrollOptions.Text = "Go To Top";
                btnScrollOptions.ImageOptions.SvgImage = Properties.Resources.moveup;
            }
        }

      
    }

    public enum ControlGroups
    {
        Segment = 0,
        Crop = 1,
        Convert = 2,
        Limits = 3,
        Prepend = 4,
        Audio = 5,
        Image = 6,
        General = 7
    }
}
