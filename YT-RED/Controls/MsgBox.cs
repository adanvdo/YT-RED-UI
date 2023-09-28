using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YTR.Controls
{
    public partial class MsgBox : DevExpress.XtraEditors.XtraForm
    {
        private static MsgBox _msgBox = new MsgBox();
        private static string _message = string.Empty;
        private static string _caption = string.Empty;
        private static Buttons _buttons = Buttons.OK;
        private static Icon _icon = YTR.Controls.Icon.Warning;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MessageBeep(uint type);

        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        private bool urlCheck = false;
        private bool flash = false;

        public MsgBox(bool urlCheck = false, bool flash = false)
        {
            InitializeComponent();
            this.urlCheck = urlCheck;
            this.flash = flash;
        }

        public static DialogResult Show(string message, FormStartPosition startPosition = FormStartPosition.CenterScreen, bool beep = false)
        {
            _msgBox = new MsgBox(false, beep);
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            _msgBox.lblCaption.Visible = false;
            _msgBox.initButtons(_buttons);
            _msgBox.initIcon(_icon);
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            if (beep) MessageBeep((uint)beepType.OK);
            _msgBox.ShowDialog();
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, string caption, FormStartPosition startPosition = FormStartPosition.CenterScreen, bool beep = false)
        {
            _msgBox = new MsgBox(false, beep);
            _msgBox.StartPosition = startPosition;
            _msgBox.initButtons(_buttons);
            _msgBox.initIcon(_icon);
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Text = caption;
            _msgBox.lblCaption.Visible = true;
            if (beep) MessageBeep((uint)beepType.OK);
            _msgBox.ShowDialog();
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, Buttons buttons, FormStartPosition startPosition = FormStartPosition.CenterScreen, bool beep = false)
        {
            _msgBox = new MsgBox(false, beep);
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Visible = false;
            _msgBox.initButtons(buttons);
            _msgBox.initIcon(_icon);
            if (beep) MessageBeep((uint)beepType.OK);
            _msgBox.ShowDialog();
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, string caption, Buttons buttons, FormStartPosition startPosition = FormStartPosition.CenterScreen, bool beep = false)
        {            
            _msgBox = new MsgBox(false, beep);
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Text = caption;
            _msgBox.initButtons(buttons);
            _msgBox.initIcon(_icon);
            _msgBox.lblCaption.Visible = true;
            if (buttons == Buttons.OK && beep) MessageBeep((uint)beepType.OK);
            else MessageBeep((uint)beepType.Question);
            _msgBox.ShowDialog();
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, Buttons buttons, Icon icon, FormStartPosition startPosition = FormStartPosition.CenterScreen, bool beep = false)
        {
            _msgBox = new MsgBox(false, beep);
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Visible = false;
            _msgBox.initButtons(buttons);
            _msgBox.initIcon(icon);
            if (beep)
            {
                switch (icon)
                {
                    case YTR.Controls.Icon.Application:
                        MessageBeep((uint)beepType.OK);
                        break;
                    case YTR.Controls.Icon.Error:
                        MessageBeep((uint)beepType.Exclamation);
                        break;
                    case YTR.Controls.Icon.Exclamation:
                        MessageBeep((uint)beepType.Exclamation);
                        break;
                    case YTR.Controls.Icon.Info:
                        MessageBeep((uint)beepType.SimpleBeep);
                        break;
                    case YTR.Controls.Icon.Shield:
                        MessageBeep((uint)beepType.Asterisk);
                        break;
                    default:
                        if (buttons == Buttons.OK && beep) MessageBeep((uint)beepType.OK);
                        else MessageBeep((uint)beepType.Question);
                        break;
                }
            }
            _msgBox.ShowDialog();
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, string caption, Buttons buttons, Icon icon, FormStartPosition startPosition = FormStartPosition.CenterScreen, bool beep = false)
        {
            _msgBox = new MsgBox(false, beep);
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Text = caption; 
            _msgBox.initButtons(buttons);
            _msgBox.initIcon(icon);
            _msgBox.lblCaption.Visible = true;
            if (beep)
            {
                switch (icon)
                {
                    case YTR.Controls.Icon.Application:
                        MessageBeep((uint)beepType.OK);
                        break;
                    case YTR.Controls.Icon.Error:
                        MessageBeep((uint)beepType.Exclamation);
                        break;
                    case YTR.Controls.Icon.Exclamation:
                        MessageBeep((uint)beepType.Exclamation);
                        break;
                    case YTR.Controls.Icon.Info:
                        MessageBeep((uint)beepType.SimpleBeep);
                        break;
                    case YTR.Controls.Icon.Shield:
                        MessageBeep((uint)beepType.Asterisk);
                        break;
                    default:
                        if (buttons == Buttons.OK && beep) MessageBeep((uint)beepType.OK);
                        else MessageBeep((uint)beepType.Question);
                        break;
                }
            }
            _msgBox.ShowDialog();
            return _msgBox.DialogResult;
        }

        public static DialogResult ShowUrlCheckWarning(string message, string caption, Buttons buttons, Icon icon, FormStartPosition startPosition = FormStartPosition.CenterScreen, bool beep = false)
        {
            _msgBox = new MsgBox(true, beep);
            _msgBox.StartPosition = startPosition;
            _msgBox.lblMessage.Text = message;
            int addHeight = heightIncrease(message);
            _msgBox.messagePanel.Height = _msgBox.messagePanel.Height + addHeight;
            _msgBox.lblMessage.Height = _msgBox.lblMessage.Height + addHeight;
            _msgBox.lblCaption.Text = caption;
            _msgBox.initButtons(buttons);
            _msgBox.initIcon(icon);
            _msgBox.lblCaption.Visible = true;
            _msgBox.pnlSuppressPanel.Visible = true;
            if (beep)
            {
                switch (icon)
                {
                    case YTR.Controls.Icon.Application:
                        MessageBeep((uint)beepType.OK);
                        break;
                    case YTR.Controls.Icon.Error:
                        MessageBeep((uint)beepType.Exclamation);
                        break;
                    case YTR.Controls.Icon.Exclamation:
                        MessageBeep((uint)beepType.Exclamation);
                        break;
                    case YTR.Controls.Icon.Info:
                        MessageBeep((uint)beepType.SimpleBeep);
                        break;
                    case YTR.Controls.Icon.Shield:
                        MessageBeep((uint)beepType.Asterisk);
                        break;
                    default:
                        if (buttons == Buttons.OK && beep) MessageBeep((uint)beepType.OK);
                        else MessageBeep((uint)beepType.Question);
                        break;
                }
            }
            _msgBox.ShowDialog();
            return _msgBox.DialogResult;
        }

        public static DialogResult Show(string message, string caption, Buttons buttons, Icon icon, Point location, FormStartPosition startPosition = FormStartPosition.CenterScreen, bool beep = false)
        {
            _msgBox = new MsgBox(false, beep);
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
            if (beep)
            {
                switch (icon)
                {
                    case YTR.Controls.Icon.Application:
                        MessageBeep((uint)beepType.OK);
                        break;
                    case YTR.Controls.Icon.Error:
                        MessageBeep((uint)beepType.Exclamation);
                        break;
                    case YTR.Controls.Icon.Exclamation:
                        MessageBeep((uint)beepType.Exclamation);
                        break;
                    case YTR.Controls.Icon.Info:
                        MessageBeep((uint)beepType.SimpleBeep);
                        break;
                    case YTR.Controls.Icon.Shield:
                        MessageBeep((uint)beepType.Asterisk);
                        break;
                    default:
                        if (buttons == Buttons.OK && beep) MessageBeep((uint)beepType.OK);
                        else MessageBeep((uint)beepType.Question);
                        break;
                }
            }
            _msgBox.ShowDialog();
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
                case YTR.Controls.Icon.Application:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.application;
                    break;
                case YTR.Controls.Icon.Error:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.highimportance;
                    break;
                case YTR.Controls.Icon.Exclamation:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.exclamation;
                    break;
                case YTR.Controls.Icon.Info:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.about;
                    break;
                case YTR.Controls.Icon.Question:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.actions_question;
                    break;
                case YTR.Controls.Icon.Search:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.enablesearch;
                    break;
                case YTR.Controls.Icon.Shield:
                    _msgBox.msgIcon.SvgImage = Properties.Resources.shield;
                    break;
                case YTR.Controls.Icon.Warning:
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

        private void chkSuppress_CheckedChanged(object sender, EventArgs e)
        {
            if (this.urlCheck)
            {
                Settings.AppSettings.Default.General.ShowHostWarning = !this.chkSuppress.Checked;
                Settings.AppSettings.Default.Save();
            }
        }

        private void MsgBox_Shown(object sender, EventArgs e)
        {
            if (this.flash)
            {
                FlashWindow(this.Handle, true);
            }
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

    public enum beepType : uint
    {
        /// <summary>
        /// A simple windows beep
        /// </summary>            
        SimpleBeep = 0xFFFFFFFF,
        /// <summary>
        /// A standard windows OK beep
        /// </summary>
        OK = 0x00,
        /// <summary>
        /// A standard windows Question beep
        /// </summary>
        Question = 0x20,
        /// <summary>
        /// A standard windows Exclamation beep
        /// </summary>
        Exclamation = 0x30,
        /// <summary>
        /// A standard windows Asterisk beep
        /// </summary>
        Asterisk = 0x40,
    }
}