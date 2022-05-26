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
        public static InitialFunction initialFunction = InitialFunction.None;

        private static List<string> functions = new List<string>()
        {
            "lf",
            "listformats",
            "dlb",
            "downloadbest"
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
                runForm = new MainForm(initialFunction, initialYTLink, Classes.MediaSource.YouTube);
            else if (!string.IsNullOrEmpty(initialRedLink))
                runForm = new MainForm(initialFunction, initialRedLink, Classes.MediaSource.Reddit);
            else
                runForm = new MainForm();

            var controller = new ApplicationController(runForm);            
            controller.Run(Environment.GetCommandLineArgs());
        }
    }

    public enum InitialFunction
    {
        ListFormats = 0,
        DownloadBest = 1,
        None = 2
    }
}
