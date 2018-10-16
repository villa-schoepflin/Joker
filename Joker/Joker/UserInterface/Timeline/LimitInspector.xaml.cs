using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.BusinessLogic;

namespace Joker.UserInterface
{
	/// <summary>
	/// A view containing the database-supplied details and properties of a selected limit.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LimitInspector : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements and provides the data to be bound in the view.
		/// </summary>
		/// <param name="limit">Limit whose details should be displayed.</param>
		public LimitInspector(Limit limit)
		{
			InitializeComponent();
			BindingContext = new LimitViewModel(this, limit);
		}
	}
}