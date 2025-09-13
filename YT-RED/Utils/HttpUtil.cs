using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YTR.Classes;
using YTR.Logging;
using YTR.Settings;

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

        /// <summary>
        /// Gets a MemoryStream from a given URL, handling direct image links and HTML pages containing images.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<MemoryStream> GetStreamFromUrl(string url)
        {

            using var client = new HttpClient();

            // Add headers to mimic a real browser
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            client.DefaultRequestHeaders.Add("Accept",
                "image/webp,image/apng,image/*,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

            try
            {
                var response = await client.GetAsync(url);
                if(response.StatusCode != HttpStatusCode.OK) { return null; }

                var contentType = response.Content.Headers.ContentType?.MediaType;

                if (contentType?.StartsWith("image/") == true)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return new MemoryStream(bytes);
                }

                // If we get HTML, try to extract the real image URL
                if (contentType?.Contains("text/html") == true)
                {
                    var html = await response.Content.ReadAsStringAsync();
                    var match = Regex.Match(html, @"<img[^>]+src=[""']([^""']+)[""']",
                        RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        var imageUrl = match.Groups[1].Value;
                        var imageResponse = await client.GetAsync(imageUrl);
                        var bytes = await imageResponse.Content.ReadAsByteArrayAsync();
                        return new MemoryStream(bytes);
                    }
                }
            }
            catch (HttpRequestException ex)
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

        #region Images

        public static async Task<byte[]> GetImageAsByteArrayAsync(string url, bool useAbsoluteUri = true)
        {
            try
            {
                Uri uri = new Uri(url);
                string useUrl = useAbsoluteUri ? $"https://{uri.Host}{uri.AbsolutePath}" : uri.ToString();
                using (HttpClient client = new HttpClient())
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

        public static async Task<(byte[] imageBytes, string contentType)> DownloadImageSimpleAsync(string url)
        {
            using var client = new HttpClient();

            // Add headers to mimic a real browser
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            client.DefaultRequestHeaders.Add("Accept",
                "image/webp,image/apng,image/*,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

            // Try the original URL first
            try
            {
                var response = await client.GetAsync(url);
                var contentType = response.Content.Headers.ContentType?.MediaType;

                if (contentType?.StartsWith("image/") == true)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return (bytes, contentType);
                }

                // If we get HTML, try to extract the real image URL
                if (contentType?.Contains("text/html") == true)
                {
                    var html = await response.Content.ReadAsStringAsync();
                    var match = Regex.Match(html, @"<img[^>]+src=[""']([^""']+)[""']",
                        RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        var imageUrl = match.Groups[1].Value;
                        var imageResponse = await client.GetAsync(imageUrl);
                        var bytes = await imageResponse.Content.ReadAsByteArrayAsync();
                        return (bytes, imageResponse.Content.Headers.ContentType?.MediaType);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ExceptionHandler.LogException(ex);
            }

            throw new InvalidOperationException("Could not download image from Reddit URL");
        }
        

        #endregion
    }
}
