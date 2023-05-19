using System;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Forms;

namespace YT_RED
{
    public class ApplicationController : WindowsFormsApplicationBase
    {
        private MainForm mainForm;
        public ApplicationController(MainForm form)
        {
            //We keep a reference to main form 
            //To run and also use it when we need to bring to front
            mainForm = form;
            this.IsSingleInstance = Program.DevRun ? false : true;
            this.StartupNextInstance += this_StartupNextInstance;
        }

        void this_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
        {
            //Here we bring application to front
            if (!Program.DevRun)
            {
                e.BringToForeground = true;
                mainForm.ShowInTaskbar = true;
                mainForm.WindowState = FormWindowState.Minimized;
                mainForm.Show();
                mainForm.WindowState = FormWindowState.Normal;
            }
        }

        protected override void OnCreateMainForm()
        {
            this.MainForm = mainForm;
        }
    }
}
