using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace YT_RED.Utils
{
    public static class HtmlUtil
    {
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
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            return result;
        }
    }
}
