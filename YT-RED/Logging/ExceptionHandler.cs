using System;
using System.IO;
using System.Windows.Forms;
using YT_RED.Settings;

namespace YT_RED.Logging
{
    public static class ExceptionHandler
    {
        public static bool LogException(Exception exception, bool showDialog = true)
        {
            string formattedException = $@"
{DateTime.Now.ToString()} ----------------------------

{exception.Message}

{exception.StackTrace}
";
            try
            {
                if (!Directory.Exists(AppSettings.Default.General.ErrorLogPath))
                {
                    Directory.CreateDirectory(AppSettings.Default.General.ErrorLogPath);
                }
                string logFile = Path.Combine(AppSettings.Default.General.ErrorLogPath, $"ErrorLogs_{DateTime.Today.Month}{DateTime.Today.Day}{DateTime.Today.Year}.txt");

                if (!File.Exists(logFile))
                {
                    File.WriteAllText(logFile, formattedException);
                }
                else
                {
                    File.AppendAllText(logFile, formattedException);
                }

                if (showDialog)
                {
                    using (YT_RED.Controls.YTRErrorMessageBox errorBox = YT_RED.Controls.YTRErrorMessageBox.ErrorMessageBox(exception))
                    {
                        errorBox.ShowDialog();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logging.ExceptionHandler.LogException(ex);
            }
            return false;            
        }
    }
}
