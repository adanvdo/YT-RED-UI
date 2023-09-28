using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YTR.Classes
{
    public class RedditEmbedObject
    {
        [JsonProperty("context")]
        public string Context { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("embedUrl")]
        public string EmbedURL { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }
        [JsonProperty("uploadDate")]
        public DateTime UploadDate { get; set; }
    }
}
