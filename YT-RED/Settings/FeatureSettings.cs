using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading.Tasks;

namespace YT_RED_UI.Settings
{
	public abstract class FeatureSettings
	{
		[Browsable(false)]
		[JsonIgnore]
		public abstract AppFeature Feature { get; }

		public FeatureSettings()
		{

		}

		public virtual async Task<string> ValidateSettings()
		{
			return null;
		}

	}
}
