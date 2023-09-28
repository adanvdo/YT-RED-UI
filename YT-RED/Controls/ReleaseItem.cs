using System;
using System.ComponentModel;
using YTR.Classes;

namespace YTR.Controls
{
    public partial class ReleaseItem : DevExpress.XtraEditors.XtraUserControl
    {
        [Browsable(true)]
        public event EventHandler Download64Click;

        [Browsable(true)]
        public event EventHandler Download86Click;

        private Release release = null;

        public Release Release
        {
            get { return release; }
            set
            {
                release = value;
                lblVersionX64.Text = $"{release.Version}-{release.Channel}-x64";
                lblVersionX86.Text = $"{release.Version}-{release.Channel}-x86";
            }
        }

        public ReleaseItem()
        {
            InitializeComponent();
            this.release = null;
        }

        public ReleaseItem(Release release)
        {
            InitializeComponent();
            if (release == null) return;
            this.release = release; 
            lblVersionX64.Text = $"{release.Version}-{release.Channel}-x64";
            lblVersionX86.Text = $"{release.Version}-{release.Channel}-x86";
        }

        private void btnDownload64_Click(object sender, EventArgs e)
        {
            if(Download64Click != null)
                Download64Click(sender, e);
        }

        private void btnDownload86_Click(object sender, EventArgs e)
        {
            if (Download86Click != null)
                Download86Click(sender, e);
        }
    }
}
