using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using YT_RED.Settings;

namespace YT_RED.Controls
{
    public partial class ControlPanel : DevExpress.XtraEditors.XtraUserControl
    {
        private string formatWarning = "YT-RED is currently set to Always Convert to your\nPreferred Video and Audio Format.\nThis can be changed in Advanced Settings";

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

        private Classes.YTDLFormatData currentFormat = null;
        public Classes.YTDLFormatData CurrentFormat 
        { 
            get { return currentFormat; }
            set
            {
                currentFormat = value;
                lblSelectionText.Text = currentFormat == null ? "" : currentFormat.Format;
                if (currentFormat != null)
                {
                    switch (currentFormat.Type)
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
        }

        public bool EmbedThumbnail
        {
            get
            {
                if (currentFormat == null || currentFormat.Type != Classes.StreamType.Audio)
                    return false;

                if (cbAudioFormat.SelectedItem.ToString().ToLower() != "opus")
                    return true;

                if (currentFormat.AudioCodec == "opus" || currentFormat.Extension == "webm")
                    return false;

                return true;
            }
        }
        
        private void formatChanged()
        {
            if (currentFormat != null)
            {
                if (currentFormat.VideoCodec == "none")
                {                   
                    cbVideoFormat.Properties.Items.Clear();
                    cbAudioFormat.Properties.Items.Clear();
                    cbAudioFormat.Properties.Items.AddRange(audioFormats);
                    cbVideoFormat.Enabled = false;
                    cbAudioFormat.Enabled = toggleConvert.IsOn;
                }
                else
                {
                    cbVideoFormat.Properties.Items.Clear();
                    cbVideoFormat.Properties.Items.AddRange(videoFormats);
                    cbAudioFormat.Properties.Items.Clear();
                    cbAudioFormat.Enabled = false;
                    cbVideoFormat.Enabled = toggleConvert.IsOn;
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

        public Settings.VideoFormat? ConvertVideoFormat
        {
            get
            {
                if (toggleConvert.IsOn && !string.IsNullOrEmpty(cbVideoFormat.SelectedItem.ToString()))
                {
                    Settings.VideoFormat vf = VideoFormat.UNSPECIFIED;
                    bool pf = Enum.TryParse(cbVideoFormat.SelectedItem.ToString(), out VideoFormat pvf);
                    if (pf)
                        vf = pvf;
                    if(vf != Settings.VideoFormat.UNSPECIFIED)
                        return vf;
                }
                return null;
            }
        }

        public Settings.AudioFormat? ConvertAudioFormat
        {
            get
            {
                if (toggleConvert.IsOn && !string.IsNullOrEmpty(cbAudioFormat.SelectedItem.ToString()))
                {
                    Settings.AudioFormat af = AudioFormat.UNSPECIFIED;
                    bool pf = Enum.TryParse(cbAudioFormat.SelectedItem.ToString(), out AudioFormat paf);
                    if (pf)
                        af = paf;
                    return af;
                }
                return null;
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

        public void ShowProgress()
        {
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
        }

        public void HideDownloadLocation()
        {
            btnOpenDL.Visible = false;
            btnOpenDL.Text = string.Empty;
        }

        [Browsable(true)]
        public event EventHandler DownloadSelection_Click;

        [Browsable(true)]
        public event EventHandler DownloadAudio_Click;

        [Browsable(true)]
        public event EventHandler DownloadBest_Click;

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
        }

        private List<string> videoFormats;
        private List<string> audioFormats;

        public void ResetControls()
        {
            toggleSegment.IsOn = false;
            tsStart.TimeSpan = TimeSpan.Zero;
            tsDuration.TimeSpan = TimeSpan.FromSeconds(1);
            toggleCrop.IsOn = false;
            txtCropBottom.Text = String.Empty;
            txtCropTop.Text = String.Empty;
            txtCropLeft.Text = String.Empty;
            txtCropRight.Text = String.Empty;
            toggleConvert.IsOn = false;
            cbVideoFormat.SelectedIndex = 0;
            cbAudioFormat.SelectedIndex = 0;
            HideDownloadLocation();
        }

        public void DisableToggle(bool disableSegment = false, bool disableCrop = false, bool disableConvert = false)
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
        }

        public void EnableToggle(bool enableSegment = false, bool enableCrop = false, bool enableConvert = false)
        {
            if (enableSegment)
            {
                toggleSegment.Enabled = true;
            }
            if (enableCrop)
            {
                toggleCrop.Enabled = true;
            }
            if (enableConvert)
            {
                toggleConvert.Enabled = true;
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

        public void UpdatePanelStates()
        {
            updateSegmentState();
            updateCropState();
            UpdateConvertState();
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
            lblSegmentDisclaimer.Visible = toggleSegment.IsOn && (currentFormat == null || parentMainForm.FormatCount < 1);
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
            lblAlwaysConvert.Visible = !toggleConvert.IsOn && AppSettings.Default.Advanced.AlwaysConvertToPreferredFormat;
            gcConvert.CustomHeaderButtons[0].Properties.Enabled = !toggleConvert.IsOn;
        }

        private void btnSelectionDL_Click(object sender, EventArgs e)
        {
            if (DownloadSelection_Click != null) DownloadSelection_Click(sender, e);
        }

        private void btnDownloadAudio_Click(object sender, EventArgs e)
        {
            if(DownloadAudio_Click != null) DownloadAudio_Click(sender, e);
        }

        private void btnDownloadBest_Click(object sender, EventArgs e)
        {
            if(DownloadBest_Click != null) DownloadBest_Click(sender, e);
        }

        private void lblSelectionText_TextChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(lblSelectionText.Text))
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
            if(!string.IsNullOrEmpty(cbVideoFormat.SelectedItem.ToString()) && !string.IsNullOrEmpty(cbAudioFormat.SelectedItem.ToString()))
            {
                lblAlwaysConvert.Text = "Audio Format will be automatically determined\nwhen downloading \"Best [audio+video]\"";
                lblAlwaysConvert.Visible = true;
            }
            else
            {
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
                if(row != null && row.FileExists)
                {
                    if (AppSettings.Default.General.AutomaticallyOpenDownloadLocation)
                    {
                        string argument = "/select, \"" + row.DownloadLocation + "\"";

                        System.Diagnostics.Process.Start("explorer.exe", argument);
                    }
                }
            }
            catch(Exception ex)
            {
                Logging.ExceptionHandler.LogException(ex);
            }
        }
    }
}
