﻿using System;
using System.IO;
using System.Windows.Forms;
using YT_RED.Settings;

namespace YT_RED.Logging
{
    public static class ExceptionHandler
    {
        public static bool LogFFmpegException(Exception exception, bool showDialog = true)
        {
            return LogException(exception, showDialog, true);
        }


        public static bool LogException(Exception exception, string videoUrl, string audioUrl = null, bool ffmpeg = false)
        {
            return LogException(exception, true, ffmpeg, videoUrl, audioUrl);
        }

        public static bool LogException(Exception exception, bool showDialog = true, bool ffmpeg = false, string videoUrl = "", string audioUrl = "")
        {
            string media = "";
            if (!string.IsNullOrEmpty(videoUrl)) media += $"\nVideo URL: {videoUrl}";
            if (!string.IsNullOrEmpty(audioUrl)) media += $"\nAudio URL: {audioUrl}";
            string formattedException = $@"
{DateTime.Now.ToString()} ----------------------------

{media}

{exception.Message}

{exception.StackTrace}
";
            try
            {
                if (!Directory.Exists(AppSettings.Default.General.ErrorLogPath))
                {
                    Directory.CreateDirectory(AppSettings.Default.General.ErrorLogPath);
                }
                string logFile = Path.Combine(AppSettings.Default.General.ErrorLogPath, $"ErrorLogs_{DateTime.Today.ToString("yyyyMMdd")}.txt");

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
                    if (ffmpeg)
                    {
                        using (Controls.YTRErrorMessageBox errorBox = Controls.YTRErrorMessageBox.FFMpegErrorBox(exception))
                        {
                            errorBox.ShowDialog();
                        }
                    }
                    else
                    {
                        using (YT_RED.Controls.YTRErrorMessageBox errorBox = new Controls.YTRErrorMessageBox(exception))
                        {
                            errorBox.ShowDialog();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                using (YT_RED.Controls.YTRErrorMessageBox errorBox = new Controls.YTRErrorMessageBox(exception))
                {
                    errorBox.ShowDialog();
                }
            }
            return false;            
        }
    }
}
