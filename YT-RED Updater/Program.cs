using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace YTR_Updater
{
    internal static class Program
    {
        public static bool devRun = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string skin = "WXI";
            string palette = "Darkness";
            string appBase = string.Empty;
            string package = string.Empty;
            bool includeUpdater = false;

            bool dummyRun = args.Length < 1;

            foreach (string s in args)
            {
                if (s == "-dev")
                    devRun = true;
                if (s.StartsWith("-skin"))
                    skin = s.Remove(0, 6);
                if (s.StartsWith("-pal"))
                    palette = s.Remove(0, 5);
                if(s.StartsWith("-dir"))
                    appBase = s.Remove(0, 5);
                if (s.StartsWith("-pkg"))
                    package = s.Remove(0, 5);
                if(s.StartsWith("-updater"))
                    includeUpdater = true;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(dummyRun ? new UpdaterForm() : new UpdaterForm(appBase, package, skin, palette, includeUpdater));
        }
    }
}
