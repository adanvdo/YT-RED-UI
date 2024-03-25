using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using YTR.Classes;
using YTR.Logging;
using YTR.Settings;
using System.Drawing;
using DevExpress.Xpo.DB.Helpers;

namespace YTR.Utils
{
    public static class HttpUtil
    {
        #region General

        public static async Task<HttpResponseMessage> SendGetRequest(string url, string userAgent = "")
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    UriBuilder baseUri = new UriBuilder(url);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseUri.Uri);
                    if (!string.IsNullOrEmpty(userAgent))
                        client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    HttpResponseMessage response = await client.SendAsync(request);
                    return response;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        #endregion

        #region API
        private static string serverUrl = Program.DevRun ? @"http://localhost:3000/api" : @"https://www.jamgalactic.com/api";
        private static async Task<HttpWebResponse> postErrorLogs(DateTime date)
        {
            try
            {
                string tidyLogs = string.Empty;
                string logFile = Path.Combine(AppSettings.Default.General.ErrorLogPath, $"ErrorLogs_{date.ToShortDateString()}.txt");
                if (File.Exists(logFile))
                {
                    string rawLogs = File.ReadAllText(logFile);
                    tidyLogs = rawLogs.Replace(@"\", @"\\").Replace(System.Environment.NewLine, @"\n").Replace("\n", @"\n").Trim();
                    if (tidyLogs.StartsWith("\n"))
                    {
                        tidyLogs = tidyLogs.Remove(0, 1);
                    }
                    if (tidyLogs.StartsWith("\\n"))
                    {
                        tidyLogs = tidyLogs.Remove(0, 2);
                    }                    
                }

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($"{serverUrl}/ytred");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
                {
                    LogPostRequest request = new LogPostRequest(ReportingUtil.GetMac(), DateTime.Now, tidyLogs);
                    string json = JsonConvert.SerializeObject(request);

                    streamWriter.Write(json);
                }

                HttpWebResponse httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

                return httpResponse;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        private static async Task<HttpWebResponse> postErrorLogs(string logs)
        {
            if (string.IsNullOrEmpty(logs))
                throw new ArgumentNullException("logs");

            try
            {
                string tidyLogs = logs.Replace(@"\", @"\\").Replace(System.Environment.NewLine, @"\n").Replace("\n", @"\n").Trim();
                if (tidyLogs.StartsWith("\n"))
                {
                    tidyLogs = tidyLogs.Remove(0, 1);
                }
                if (tidyLogs.StartsWith("\\n"))
                {
                    tidyLogs = tidyLogs.Remove(0, 2);
                }
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($"{serverUrl}/ytred");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
                {
                    LogPostRequest request = new LogPostRequest(ReportingUtil.GetMac(), DateTime.Now, tidyLogs);
                    string json = JsonConvert.SerializeObject(request);

                    streamWriter.Write(json);
                }

                HttpWebResponse httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

                return httpResponse;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }
        public static async Task<bool> UploadErrorLogs(DateTime date)
        {
            try
            {
                HttpWebResponse postResponse = await postErrorLogs(date);
                if (postResponse.StatusCode == System.Net.HttpStatusCode.Created)
                    return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return false;
        }

        public static async Task<bool> UploadErrorLogs(int max)
        {
            try
            {
                DirectoryInfo logDir = new DirectoryInfo(AppSettings.Default.General.ErrorLogPath);
                FileInfo[] files = await Task.Run(() => logDir.GetFiles());
                for(int i = 0; i < files.Length && i < max; i++)
                {
                    FileInfo file = files[i];
                    string logs = string.Empty;
                    if (File.Exists(file.FullName))
                    {
                        logs = await Task.Run(() => File.ReadAllText(file.FullName)); 
                        HttpWebResponse postResponse = await postErrorLogs(logs);
                        if (postResponse.StatusCode != HttpStatusCode.Created)
                            return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return false;
        }

        public static async Task<HttpResponseMessage> Get(string endpoint, string query)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    UriBuilder baseUri = new UriBuilder($"{serverUrl}/{endpoint}");
                    baseUri.Query = query;
                    HttpResponseMessage response = await client.GetAsync(baseUri.ToString());
                    return response;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }

        #endregion

        #region WEB

        public static async Task<byte[]> GetImageAsByteArrayAsync(string url, bool useAbsoluteUri = true)
        {
            try
            {
                Uri uri = new Uri(url);
                string useUrl = useAbsoluteUri ? $"https://{uri.Host}{uri.AbsolutePath}" : uri.ToString();
                using(HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(useUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsByteArrayAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }
        #endregion
    }
}
