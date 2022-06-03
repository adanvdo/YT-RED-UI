﻿using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}