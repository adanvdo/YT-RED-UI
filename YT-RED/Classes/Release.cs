using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YT_RED.Classes
{
    public class Release
    {
        [JsonProperty("ReleaseID")]
        public Guid ReleaseID { get; set; }

        [JsonProperty("Channel")]
        public Settings.ReleaseChannel Channel { get; set; }

        [JsonProperty("Major")]
        public int Major { get; set; }

        [JsonProperty("Minor")]
        public int Minor { get; set; }

        [JsonProperty("Build")]
        public int Build { get; set; }

        [JsonProperty("Revision")]
        public int Revision { get; set; }

        [JsonProperty("ReleaseDate")]
        public DateTime ReleaseDate { get; set; }

        [JsonProperty("Active")]
        public bool Active { get; set; }

        [JsonProperty("x86Url")]
        public string x86Url { get; set; }

        [JsonProperty("x64Url")]
        public string x64Url { get; set; }

        [JsonProperty("ReplaceUpdater")]
        public bool ReplaceUpdater { get; set; }

        [JsonProperty("ManualInstallRequired")]
        public bool ManualInstallRequired { get; set; }

        [JsonIgnore]
        public Version Version
        {
            get
            {
                return new Version(Major, Minor, Build, Revision);
            }
        }

        public Release()
        {

        }
    }
}
