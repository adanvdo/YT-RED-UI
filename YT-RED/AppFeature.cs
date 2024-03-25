using System.ComponentModel;
namespace YTR
{
	public enum AppFeature
	{
		[Description("General")]
		General = 0,
		[Description("Advanced")]
		Advanced = 1,
		[Description("About")]
		About = 2,
        [Description("Layout")]
		Layout = 3
	}
}
