using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace YT_RED.Controls
{
    public partial class InputPanel : DevExpress.XtraEditors.XtraUserControl
    {
        [Browsable(true)]
        public event EventHandler ListFormats_Click;

        [Browsable(true)]
        public event EventHandler ResetList_Click;

        [Browsable(true)]
        public event EventHandler Url_Changed;

        [Browsable(true)]
        public event EventHandler Crab_Click;

        [Browsable(true)]
        public event KeyEventHandler Url_KeyDown;

        [Browsable(true)]
        public event EventHandler ListFormats_MouseMove;        

        [Browsable(true)]
        public event EventHandler ListFormats_MouseLeave;

        public bool ShowCrab
        {
            get { return btnCrab.Visible; }
            set { btnCrab.Visible = value; }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public string URL
        {
            get
            {
                return txtUrl.Text;
            }

            set
            {
                txtUrl.Text = value;
            }
        }

        public InputPanel()
        {
            InitializeComponent();
        }

        private void btnListFormats_Click(object sender, EventArgs e)
        {
            if (ListFormats_Click != null)
                ListFormats_Click(sender, e);
        }

        private void btnListReset_Click(object sender, EventArgs e)
        {
            if(ResetList_Click != null)
                ResetList_Click(sender, e);
        }

        private void txtUrl_EditValueChanged(object sender, EventArgs e)
        {
            if(Url_Changed != null)
                Url_Changed(sender, e);
        }

        private void txtUrl_Click(object sender, EventArgs e)
        {
            txtUrl.SelectAll();
        }

        private void btnCrab_Click(object sender, EventArgs e)
        {
            if(Crab_Click != null)
                Crab_Click(sender, e);
        }

        private void txtUrl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (Url_KeyDown != null)
                Url_KeyDown(sender, e);
        }

        private void btnListFormats_MouseMove(object sender, MouseEventArgs e)
        {
            if(ListFormats_MouseMove != null)
                ListFormats_MouseMove(sender, e);
        }

        private void btnListFormats_MouseLeave(object sender, EventArgs e)
        {
            if (ListFormats_MouseLeave != null)
                ListFormats_MouseLeave(sender, e);
        }

        public void UpdateMarqueeText(string text)
        {
            if (marqeeMain.InvokeRequired)
            {
                Action safeUpdate = delegate
                {
                    marqeeMain.Text = text;
                    marqeeMain.Refresh();
                };
                marqeeMain.Invoke(safeUpdate);
            }
            else
            {
                marqeeMain.Text = text;
                marqeeMain.Refresh();
            }
        }
    }
}
