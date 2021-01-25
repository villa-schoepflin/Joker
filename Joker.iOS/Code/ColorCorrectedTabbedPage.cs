using Joker.UserInterface;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(Joker.iOS.ColorCorrectedTabbedPage))]
namespace Joker.iOS
{
	/// <summary>
	/// Custom renderer to correct the false colors otherwise shown in the tab bar.
	/// </summary>
	public class ColorCorrectedTabbedPage : TabbedRenderer
	{
		/// <summary>
		/// Sets the correct color scheme for any tabbed page created in the shared code.
		/// </summary>
		public ColorCorrectedTabbedPage()
		{
			TabBar.UnselectedItemTintColor = Styles.Primary2.ToUIColor();
			TabBar.Translucent = false;
		}
	}
}
