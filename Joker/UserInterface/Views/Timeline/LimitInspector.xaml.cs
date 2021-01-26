using Joker.BusinessLogic;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// A view containing the database-supplied details and properties of a selected limit.
	/// </summary>
	public partial class LimitInspector : ContentPage
	{
		internal LimitInspector(Limit limit)
		{
			InitializeComponent();
			BindingContext = new LimitViewModel(this, limit);
		}
	}
}
