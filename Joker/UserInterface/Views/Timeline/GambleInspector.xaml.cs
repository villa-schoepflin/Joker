using Joker.BusinessLogic;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// A view containing the database-supplied details and properties of a selected gamble.
	/// </summary>
	public partial class GambleInspector : ContentPage
	{
		internal GambleInspector(Gamble gamble)
		{
			InitializeComponent();
			BindingContext = new GambleViewModel(this, gamble);
		}
	}
}
