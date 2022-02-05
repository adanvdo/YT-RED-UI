using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Xabe.FFmpeg;

namespace YT_RED
{
    internal static class Program
    {
        public static bool DevRun = false;
        public static bool x64 = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            x64 = IntPtr.Size == 8;
            if(args.Length > 0)
            {
                if(args.Contains("-dev") || args.Contains("dev"))
                    DevRun = true;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FFmpeg.SetExecutablesPath(@".\Resources\App");
            Application.Run(new MainForm());
        }
    }
}
