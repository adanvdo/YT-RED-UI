using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using YT_RED.Utils;

namespace YT_RED.Controls
{
    public partial class YTRMessageBox : DevExpress.XtraEditors.XtraForm
    {
        private bool logsUploaded = false;

        private MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;
        public MessageBoxButtons MessageBoxButtons
        {
            get { return messageBoxButtons; }
            set
            {
                messageBoxButtons = value;
                if (value == MessageBoxButtons.OK)
                {
                    btnOk.Visible = true;
                }
                else if (value == MessageBoxButtons.YesNo)
                {
                    btnNo.Visible = true;
                    btnYes.Visible = true;
                }
                else if (value == MessageBoxButtons.YesNoCancel)
                {
                    btnCancel.Visible = true;
                    btnNo.Visible = true;
                    btnYes.Visible = true;
                }
                else if (value == MessageBoxButtons.OKCancel)
                {
                    btnCancel.Visible = true;
                    btnOk.Visible = true;
                }
                else if(value == MessageBoxButtons.RetryCancel)
                {
                    btnCancel.Visible = true;
                    btnRetry.Visible = true;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("MessageBoxButtons", $"MessageBoxButtons \"{value.ToString()}\" not supported");
                }
            }
        }

        private MessageBoxIcon messageBoxIcon = MessageBoxIcon.Information;
        public MessageBoxIcon MessageBoxIcon
        {
            get { return messageBoxIcon; }
            set 
            { 
                messageBoxIcon = value;
                if (value == MessageBoxIcon.Information)
                    this.IconOptions.Icon = SystemIcons.Information;
                else if (value == MessageBoxIcon.Warning)
                    this.IconOptions.Icon = SystemIcons.Warning;
                else if (value == MessageBoxIcon.Error)
                    this.IconOptions.Icon = SystemIcons.Error;
                else if (value == MessageBoxIcon.Question)
                    this.IconOptions.Icon = SystemIcons.Question;
                else if (value == MessageBoxIcon.Hand)
                    this.IconOptions.Icon = SystemIcons.Hand;
                else if (value == MessageBoxIcon.Stop)
                    this.IconOptions.Icon = SystemIcons.Hand;
                else if (value == MessageBoxIcon.Asterisk)
                    this.IconOptions.Icon = SystemIcons.Asterisk;
            } 
        }

        public string Caption 
        { 
            get { return this.Text; } 
            set { this.Text = value; }
        }

        public string Message 
        {
            get { return memoMessage.Text; }
            set { memoMessage.Text = value; }
        } 

        public static YTRMessageBox ErrorMessageBox(Exception ex)
        {
            YTRMessageBox box = new YTRMessageBox(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            box.btnUpload.Visible = true;
            return box;
        }

        public YTRMessageBox()
        {
            InitializeComponent();
        }

        public YTRMessageBox(string message) : this(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information) { }
        public YTRMessageBox(string message, string caption) : this(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information) { }
        public YTRMessageBox(string message, string caption, MessageBoxButtons boxButtons) : this(message, caption, boxButtons, MessageBoxIcon.Information) { }

        public YTRMessageBox(string message, string caption, MessageBoxButtons boxButtons, MessageBoxIcon boxIcon)
        {
            InitializeComponent();
            Message = message;
            Caption = caption;
            MessageBoxButtons = boxButtons; 
            MessageBoxIcon = boxIcon;  
        }
        

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            if (this.logsUploaded)
            {
                this.lblIndicator.Text = "Logs Already Uploaded";
                this.lblIndicator.Visible = true;
            }

            this.uploadMarquee.Text = "Sending Logs...";
            this.uploadMarquee.Show();
            bool uploaded = await HttpUtil.UploadErrorLogs(DateTime.Today);
            if(uploaded)
            {
                this.logsUploaded = true;
                this.uploadMarquee.Hide();
                this.uploadMarquee.Text = "";
                this.lblIndicator.Text = "Upload Successful";
                this.lblIndicator.Visible = true;
            }
        }

        private void memoMessage_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            int setHeight = memoMessage.CalcAutoHeight();
            memoMessage.Height = setHeight;
        }

        private void memoMessage_EditValueChanged(object sender, EventArgs e)
        {
            memoMessage.Text = Regex.Replace(memoMessage.Text, "(?<!\r)\n", "\r\n");
        }
    }
}