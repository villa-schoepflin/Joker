using Joker.UserInterface;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(Joker.iOS.ColorCorrectedTabbedPage))]
namespace Joker.iOS
{
	/// <summary>
	/// Custom renderer to correct the false colors otherwise shown in the tab bar.
	/// </summary>
	internal class ColorCorrectedTabbedPage : TabbedRenderer
	{
		public ColorCorrectedTabbedPage()
		{
			TabBar.UnselectedItemTintColor = Styles.Primary2.ToUIColor();
			TabBar.Translucent = false;
		}
	}
}
