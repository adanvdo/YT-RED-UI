using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YT_RED.Classes
{
    public class GithubTag
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("zipball_url")]
        public string ZipballUrl { get; set; }

        [JsonProperty("node_id")]
        public string NodeId { get; set; }
    }
}
