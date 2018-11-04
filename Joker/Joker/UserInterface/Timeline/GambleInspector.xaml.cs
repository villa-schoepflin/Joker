using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.BusinessLogic;

namespace Joker.UserInterface
{
	/// <summary>
	/// A view containing the database-supplied details and properties of a selected gamble.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GambleInspector : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements and provides the data to be bound in the view.
		/// </summary>
		/// <param name="gamble">Gamble whose details should be exposed.</param>
		public GambleInspector(Gamble gamble)
		{
			InitializeComponent();
			BindingContext = new GambleViewModel(this, gamble);
		}
	}
}