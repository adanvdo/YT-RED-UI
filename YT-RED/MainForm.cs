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
using YT_RED.Logging;
using YT_RED.Settings;

namespace YT_RED
{
    public partial class MainForm : DevExpress.XtraBars.TabForm
    {
        public static bool IsLocked = false;
        public MainForm()
        {
            InitializeComponent();
            Init();
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
                GetStreamList(this.txtRedditPost.Text);
            }
        }

        private async void GetStreamList(string playlistUrl)
        {
            this.UseWaitCursor = true;
            this.redditListMarquee.Visible = true;
            try
            {
                List<Classes.StreamLink> streamLinks = await Utils.HtmlUtil.GetVideoFromRedditPage(playlistUrl);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (MainForm.IsLocked)
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

        }
    }
}
