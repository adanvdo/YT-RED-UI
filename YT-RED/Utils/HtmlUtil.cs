﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YT_RED.Logging;

namespace YT_RED.Utils
{
    public static class HtmlUtil
    {
        /// <summary>
        /// Checks if the passed url is a reddit or youtube url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static DownloadType CheckUrl(string url)
        {
            if (url.StartsWith(@"https://www.youtube.com") || url.StartsWith("https://youtu.be") || url.StartsWith(@"https://youtube.com"))
                return DownloadType.YouTube;
            if (url.StartsWith(@"https://reddit.com") || url.StartsWith(@"https://www.reddit.com"))
                return DownloadType.Reddit;
            return DownloadType.Unknown;
        }

        /// <summary>
        /// Searches the Video Post Html for known elements that contain the M3U8 url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<List<Classes.StreamLink>> GetVideoFromRedditPage(string url)
        {
            List<Classes.StreamLink> result = new List<Classes.StreamLink>();

            try
            {
                HtmlWeb web = new HtmlWeb();
                var htmlDoc = await web.LoadFromWebAsync(url);

                // First, we look for the ld+json script that would include the video embed url
                var firstXpath = "//script[@type='application/ld+json']";                
                var check = htmlDoc.DocumentNode
                    .SelectSingleNode(firstXpath);
                if (check != null)
                {
                    // If we find the script containing the embed url, we will clean and parse it
                    if (check.ChildNodes != null && check.ChildNodes[0] != null && check.ChildNodes[0].InnerText != null)
                    {
                        string clean = check.ChildNodes[0].InnerText.Replace("@", "");
                        Classes.RedditEmbedObject embed = JsonConvert.DeserializeObject<Classes.RedditEmbedObject>(clean);
                        Uri extract = new Uri(embed.EmbedURL);
                        string divId = $"video-{extract.Segments.Last()}";
                        var dataPath = $"//div[@id='{divId}']";
                        string embedUrl = embed.EmbedURL;

                        // After parsing the embed Url, we scrape it for the DASH url of the video
                        HtmlWeb embedWeb = new HtmlWeb();
                        var embedDoc = await embedWeb.LoadFromWebAsync(embedUrl);
                        var embedCheck = embedDoc.DocumentNode
                            .SelectSingleNode(dataPath);
                        if (embedCheck != null)
                        {
                            result.Add(new Classes.StreamLink(Classes.StreamPlaylistType.DASH, embedCheck.Attributes["data-mpd-url"].Value.Replace(@"&f=sd", "")));
                            result.Add(new Classes.StreamLink(Classes.StreamPlaylistType.HLS, embedCheck.Attributes["data-hls-url"].Value.Replace(@"&f=sd", "")));
                        }
                    }
                }
                
                // if we didnt find the hls url, we will try the second known element
                if(result.Count < 1)
                {
                    var navigator = (HtmlAgilityPack.HtmlNodeNavigator)htmlDoc.CreateNavigator();
                    var secondXpath = "//video/source/@src";
                    var check2 = navigator.SelectSingleNode(secondXpath);

                    // if we found the hls url here, we will use it. otherwise we will check the last known element for the url
                    if (check2 != null)
                        result.Add(new Classes.StreamLink(Classes.StreamPlaylistType.HLS, check2.Value));
                    else
                    {
                        var lastXpath = "//meta[@property='og:video']/@content";
                        var check3 = navigator.SelectSingleNode(lastXpath);
                        if (check3 != null)
                            result.Add(new Classes.StreamLink(Classes.StreamPlaylistType.HLS, check3.Value));
                    }
                }
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }

            return result;
        }

        public static async Task<Tuple<int, string>> GetBestRedditDashVideo(string id)
        {
            int[] resolutions = new int[]
            {
                2160,
                1080,
                720,
                480,
                240
            };
            Tuple<int, string> bestUrl = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    foreach (int res in resolutions)
                    {
                        var response = await client.GetAsync(redditVideoUrl(id, res));
                        if (response != null)
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                bestUrl = Tuple.Create(res, redditVideoUrl(id, res));
                                break;
                            }
                        }
                    }
                }
                if (bestUrl != null) return bestUrl;
            }
            catch(Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }
            return null;
        }        

        public static async Task<bool> MediaExists(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex);
            }

            return false;
        }

        private static string redditVideoUrl(string id, int resolution)
        {
            return string.Format($@"{YT_RED.Settings.AppSettings.Default.General.RedditMediaURLPrefix}{id}{YT_RED.Settings.AppSettings.Default.General.RedditDefaultVideoSuffix}", resolution);
        }
    }
}
