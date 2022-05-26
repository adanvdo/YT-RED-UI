using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YT_RED.Controls
{
    public partial class MsgBox : DevExpress.XtraEditors.XtraForm
    {
        private static MsgBox _msgBox = new MsgBox();
        private static string _message = string.Empty;
        private static string _caption = string.Empty;
        private static Buttons _buttons = Buttons.OK;
        private static Icon _icon = YT_RED.Controls.Icon.Warning;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MessageBeep(uint type);

        public MsgBox()
        {
            InitializeComponent();
        }

        public static DialogResult Show(string message, FormStartPosition startPosition = FormStartPosition.CenterScreen)
        {
            _msgBox = new MsgBox();
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            _msgBox.lblCaption.Visible = false;
            _msgBox.initButtons(_buttons);
            _msgBox.initIcon(_icon);
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.ShowDialog();
            MessageBeep(0);
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, string caption, FormStartPosition startPosition = FormStartPosition.CenterScreen)
        {
            _msgBox = new MsgBox();
            _msgBox.StartPosition = startPosition;
            _msgBox.initButtons(_buttons);
            _msgBox.initIcon(_icon);
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Text = caption;
            _msgBox.lblCaption.Visible = true;
            _msgBox.ShowDialog();
            MessageBeep(0);
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, Buttons buttons, FormStartPosition startPosition = FormStartPosition.CenterScreen)
        {
            _msgBox = new MsgBox();
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Visible = false;
            _msgBox.initButtons(buttons);
            _msgBox.initIcon(_icon);
            _msgBox.ShowDialog();
            MessageBeep(0);
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, string caption, Buttons buttons, FormStartPosition startPosition = FormStartPosition.CenterScreen)
        {            
            _msgBox = new MsgBox();
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Text = caption;
            _msgBox.initButtons(buttons);
            _msgBox.initIcon(_icon);
            _msgBox.lblCaption.Visible = true;
            _msgBox.ShowDialog();
            MessageBeep(0);
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, Buttons buttons, Icon icon, FormStartPosition startPosition = FormStartPosition.CenterScreen)
        {
            _msgBox = new MsgBox();
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Visible = false;
            _msgBox.initButtons(buttons);
            _msgBox.initIcon(icon);
            _msgBox.ShowDialog();
            MessageBeep(0);
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, string caption, Buttons buttons, Icon icon, FormStartPosition startPosition = FormStartPosition.CenterScreen)
        {
            _msgBox = new MsgBox();
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Text = caption; 
            _msgBox.initButtons(buttons);
            _msgBox.initIcon(icon);
            _msgBox.lblCaption.Visible = true;
            _msgBox.ShowDialog();
            MessageBeep(0);
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, string caption, Buttons buttons, Icon icon, Point location, FormStartPosition startPosition = FormStartPosition.CenterScreen)
        {
            _msgBox = new MsgBox();
            _msgBox.StartPosition = startPosition;
            _msgBox.StartPosition = FormStartPosition.Manual;
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.Location = new Point(location.X, location.Y - _msgBox.Height);
            _msgBox.lblCaption.Text = caption;
            _msgBox.initButtons(buttons);
            _msgBox.initIcon(icon);
            _msgBox.lblCaption.Visible = true;
            _msgBox.ShowDialog();
            MessageBeep(0);
            return _msgBox.DialogResult;
        }        
            
        private void initButtons(Buttons buttons)
        {
            switch (buttons)
            {

                case Buttons.AbortRetryIgnore:
                    _msgBox.btnAbort.Visible = true;
                    _msgBox.btnRetry.Visible = true;
                    _msgBox.btnIgnore.Visible = true;
                    break;
                case Buttons.OK:
                    _msgBox.btnOk.Visible = true;
                    break;
                case Buttons.OKCancel:
                    _msgBox.btnOk.Visible = true;
                    _msgBox.btnCancel.Visible = true;
                    break;
                case Buttons.RetryCancel:
                    _msgBox.btnRetry.Visible = true;
                    _msgBox.btnCancel.Visible = true;
                    break;
                case Buttons.YesNo:
                    _msgBox.btnYes.Visible = true;
                    _msgBox.btnNo.Visible = true;
                    break;
                case Buttons.YesNoCancel:
                    _msgBox.btnYes.Visible = true;
                    _msgBox.btnNo.Visible = true;
                    _msgBox.btnCancel.Visible = true;
                    break;
                default:
                    _msgBox.btnOk.Visible = true;
                    break;
            }
        }

        private void initIcon(Icon icon)
        {
            switch (icon)
            {
                case YT_RED.Controls.Icon.Application:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.application;
                    break;
                case YT_RED.Controls.Icon.Error:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.highimportance;
                    break;
                case YT_RED.Controls.Icon.Exclamation:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.exclamation;
                    break;
                case YT_RED.Controls.Icon.Info:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.about;
                    break;
                case YT_RED.Controls.Icon.Question:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.actions_question;
                    break;
                case YT_RED.Controls.Icon.Search:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.enablesearch;
                    break;
                case YT_RED.Controls.Icon.Shield:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.shield;
                    break;
                case YT_RED.Controls.Icon.Warning:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.bo_attention;
                    break;
                default:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.bo_attention;
                    break;
            }
        }

        private static int heightIncrease(string message)
        {
            Graphics g = _msgBox.CreateGraphics();
            int width = 287;
            int height = 72;

            SizeF size = g.MeasureString(message, new Font("Tahoma", (float)10), width);

            if(size.Height > height)
            {
                return (int)size.Height;
            }
            return 0;
        }

        private void btnClick(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.SimpleButton btn = (SimpleButton)sender;
            switch(btn.Text)
            {
                case "Abort":
                    _msgBox.DialogResult = DialogResult.Abort;
                    break;
                case "Retry":
                    _msgBox.DialogResult = DialogResult.Retry;
                    break;
                case "Ignore":
                    _msgBox.DialogResult = DialogResult.Ignore;
                    break;
                case "OK":
                    _msgBox.DialogResult = DialogResult.OK;
                    break;
                case "Cancel":
                    _msgBox.DialogResult = DialogResult.Cancel;
                    break;
                case "Yes":
                    _msgBox.DialogResult = DialogResult.Yes;
                    break;
                case "No":
                    _msgBox.DialogResult = DialogResult.No;
                    break;
            }

            _msgBox.Dispose();
        }
    }

    public enum Buttons
    {
        AbortRetryIgnore = 1,
        OK = 2,
        OKCancel = 3,
        RetryCancel = 4,
        YesNo = 5,
        YesNoCancel = 6
    }

    public enum Icon
    {
        Application = 1,
        Exclamation = 2,
        Error = 3,
        Warning = 4,
        Info = 5,
        Question = 6,
        Shield = 7,
        Search = 8
    }

    public enum AnimateStyle
    {
        SlideDown = 1,
        FadeIn = 2,
        ZoomIn = 3
    }
}