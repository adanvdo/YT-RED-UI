using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using YT_RED.Controls;
using YT_RED.Logging;
using YT_RED.Settings;
using Xabe.FFmpeg;
using Newtonsoft.Json;

namespace YT_RED
{
    public partial class MainForm : DevExpress.XtraBars.TabForm
    {
        public bool IsLocked { 
            get 
            { 
                foreach(CustomTabFormPage pg in this.tabFormControl1.Pages)
                {
                    if (pg.IsLocked)
                        return true;
                }
                return false;
            } 
        }
        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        protected override void OnLoad(EventArgs e)
        {
            txtRedditPost.Text = AppSettings.Default.General.RedditSampleUrl;
            base.OnLoad(e);
        }

        private async void Init()
        {
            if (AppSettings.Default.General.EnableDownloadHistory)
            {
                bool loadDownloadHistory = await Historian.LoadDownloadHistory();
                if (loadDownloadHistory)
                {
                    
                }
            }
        }

        private void btnRedditDefault_Click(object sender, EventArgs e)
        {

        }

        private void btnRedditList_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(this.txtRedditPost.Text))
            {
                redditScrape(this.txtRedditPost.Text, true);
            }
        }

        private async void redditScrape(string playlistUrl, bool listFormats = false)
        {
            this.UseWaitCursor = true;
            (this.tabFormControl1.SelectedPage as CustomTabFormPage).IsLocked = true;
            this.redditListMarquee.Show();
            try
            {
                List<Classes.StreamLink> streamLinks = await Utils.HtmlUtil.GetVideoFromRedditPage(playlistUrl);
                if(streamLinks != null && listFormats)
                {
                    gcReddit.Visible = true;
                    List<Classes.RedditStream> dashStreamList = new List<Classes.RedditStream>();
                    List<Classes.RedditStream> hlsStreamList = new List<Classes.RedditStream>();

                    foreach (Classes.StreamLink link in streamLinks)
                    {
                        IMediaInfo m3u8contents = await Utils.VideoUtil.ParseM3U8(link.StreamUrl);
                        List<Classes.RedditStream> l = new List<Classes.RedditStream>();
                        foreach(IStream ist in m3u8contents.Streams) 
                        {
                            string transform = JsonConvert.SerializeObject(ist);
                            Classes.RedditStream rs = JsonConvert.DeserializeObject<Classes.RedditStream>(transform);
                            rs.DeliverStream = ist;
                            l.Add(rs);
                        }
                        foreach(Classes.RedditStream stream in l)
                        {
                            stream.PlaylistType = link.PlaylistType;
                        }
                        if (link.PlaylistType == Classes.StreamPlaylistType.DASH)
                            dashStreamList.AddRange(l);
                        else if (link.PlaylistType == Classes.StreamPlaylistType.HLS)
                            hlsStreamList.AddRange(l);
                    }

                    List<Classes.ResultStream> buildList = new List<Classes.ResultStream>();
                    buildList.AddRange(await Utils.VideoUtil.ConsolidateStreams(dashStreamList));
                    buildList.AddRange(await Utils.VideoUtil.ConsolidateStreams(hlsStreamList));
                    gcReddit.DataSource = buildList;
                    gvReddit.PopulateColumns();
                    gvReddit.Columns["StreamType"].Visible = false;
                    gvReddit.Columns["VideoPath"].Visible = false;
                    gvReddit.Columns["AudioPath"].Visible = false;
                    gvReddit.Columns["VideoStream"].Visible = false;
                    gvReddit.Columns["AudioStream"].Visible = false;
                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            lblSelect.Visible = true;
            this.UseWaitCursor = false;
            (this.tabFormControl1.SelectedPage as CustomTabFormPage).IsLocked = false;
            this.redditListMarquee.Hide();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.IsLocked)
            {
                DialogResult res = MessageBox.Show("A Task is currently in progress. Would you like to cancel the task and exit?", "A Task is Busy", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }

        private void tabFormControl1_PageClosing(object sender, DevExpress.XtraBars.PageClosingEventArgs e)
        {
            CustomTabFormPage page = sender as CustomTabFormPage;
            if(page.IsLocked)
            {
                MessageBox.Show("A Task is currently in progress and cannot be cancelled.\nPlease wait for the operation to complete.", "A Task is Busy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }
        }

        private void gvReddit_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            selectedStream = gvReddit.GetFocusedRow() as Classes.ResultStream;
            if (selectedStream != null)
            {
                string displayText = $"Row {selectedStream.Row} - {selectedStream.StreamType}\n\n".Replace("AudioAndVideo", "Video + Audio");
                if (selectedStream.StreamType == Classes.StreamType.Video) displayText += $"Video: {selectedStream.PlaylistType} {selectedStream.VideoCodec} {selectedStream.Width}x{selectedStream.Height}";
                else if (selectedStream.StreamType.ToString() == "Audio") displayText += $"Audio: {selectedStream.AudioCodec} {selectedStream.AudioChannels}ch {selectedStream.AudioSampleRate}kHz";
                else displayText += $"Video: {selectedStream.PlaylistType} {selectedStream.VideoCodec} {selectedStream.Width}x{selectedStream.Height}\nAudio: {selectedStream.AudioCodec} {selectedStream.AudioChannels}ch {selectedStream.AudioSampleRate}kHz";
                lblSelectionText.Text = displayText;
                lblSelectionText.Refresh();
                simpleButton1.Visible = true;
                simpleButton1.Enabled = true;
            }
            else simpleButton1.Visible = false;
        }

        private Classes.ResultStream selectedStream = null;

        private async void simpleButton1_Click(object sender, EventArgs e)
        {
            if(selectedStream != null)
            {
                IConversion conversion = Utils.VideoUtil.PrepareConversion(selectedStream);
                string destination = conversion.OutputFilePath;
                conversion.OnProgress += Conversion_OnProgress;
                this.pbDownloadProgress.Visible = true;
                await conversion.Start();
                this.pbDownloadProgress.Visible = false;
                this.lblDLLocation.Text = destination;
            }
        }

        private void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
        {
            var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
            safeUpdateDownloadProgress(percent);
        }

        private void safeUpdateDownloadProgress(int percent)
        {
            if (pbDownloadProgress.InvokeRequired)
            {
                Action safeUpdate = delegate { pbDownloadProgress.Position = percent; };
                pbDownloadProgress.Invoke(safeUpdate);
            }
            else
                pbDownloadProgress.Position = percent;
        }

        private void lblDLLocation_Click(object sender, EventArgs e)
        {
            string argument = "/select, \"" + lblDLLocation.Text + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }
}
