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
        public static string initialYTLink = string.Empty;
        public static string initialRedLink = string.Empty;
        public static bool newUpdater = false;
        public static bool updated = false;
        public static InitialFunction initialFunction = InitialFunction.None;

        private static List<string> functions = new List<string>()
        {
            "lf",
            "listformats",
            "dlb",
            "downloadbest",
            "uploadtest"
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            
            x64 = IntPtr.Size == 8;           

            if(args.Length > 0)
            {
                try
                {
                    foreach (string s in args)
                    {
                        if (s.StartsWith("-dev") || s == "dev")
                            DevRun = true;
                        if (s.StartsWith("-if"))
                        {
                            DevRun = false;
                            string func = s.Remove(0, 4);
                            if (!functions.Contains(func.ToLower()))
                                throw new ArgumentException($"The Function {func} is not valid");

                            if (func == "lf") func = "ListFormats";
                            else if (func == "dlb") func = "DownloadBest";
                            initialFunction = (InitialFunction)Enum.Parse(typeof(InitialFunction), func);
                        }
                        if (s.StartsWith("-yt"))
                        {
                            DevRun = false;
                            initialYTLink = s.Remove(0, 4);
                        }
                        if (s.StartsWith("-red"))
                        {
                            DevRun = false;
                            initialRedLink = s.Remove(0, 5);
                        }
                        if(s.StartsWith("-uploadtest") || s.StartsWith("-ut"))
                        {
                            initialFunction = InitialFunction.UploadTest;
                        }
                        if (s.StartsWith("-updater"))
                        {
                            newUpdater = true;
                        }
                        if(s.StartsWith("-updated"))
                        {
                            updated = true;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Logging.ExceptionHandler.LogException(ex);
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FFmpeg.SetExecutablesPath(@".\Resources\App");
            MainForm runForm;
            if (!string.IsNullOrEmpty(initialYTLink))
                runForm = new MainForm(initialFunction, initialYTLink, newUpdater);
            else if (!string.IsNullOrEmpty(initialRedLink))
                runForm = new MainForm(initialFunction, initialRedLink, newUpdater);
            else
                runForm = new MainForm(newUpdater);

            try
            {
                var controller = new ApplicationController(runForm);
                string[] argList = Environment.GetCommandLineArgs();
                controller.Run(argList);
            }
            catch(Exception ex)
            {
                bool logged = Logging.ExceptionHandler.LogException(ex);
            }
        }
    }

    public enum InitialFunction
    {
        ListFormats = 0,
        DownloadBest = 1,
        UploadTest = 2,
        None = 3
    }
}
