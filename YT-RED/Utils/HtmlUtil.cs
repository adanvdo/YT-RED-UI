using HtmlAgilityPack;
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
            if (url.StartsWith(@"https://twitter.com") || url.StartsWith(@"https://www.twitter.com") || url.StartsWith(@"https://m.twitter.com") || url.StartsWith(@"http://mobile.twitter.com"))
                return DownloadType.Twitter;
            if (url.StartsWith(@"https://www.twitch.tv") || url.StartsWith(@"https://twitch.tv") || url.StartsWith(@"https://go.twitch.tv") || url.StartsWith(@"https://m.twitch.tv"))
                return DownloadType.Twitch;
            if (url.StartsWith(@"https://vimeo.com") || url.StartsWith(@"https://www.vimeo.com") || url.StartsWith(@"https://player.vimeo.com") || url.StartsWith(@"https://vimeopro.com"))
                return DownloadType.Vimeo;
            if (url.StartsWith(@"https://instagram.com") || url.StartsWith(@"https://www.instagram.com"))
                return DownloadType.Instagram;
            return DownloadType.Unknown;
        }       
    }
}
