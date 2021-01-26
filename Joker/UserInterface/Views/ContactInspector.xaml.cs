using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// A view containing the database-supplied details and properties of a selected contact.
	/// </summary>
	public partial class ContactInspector : ContentPage
	{
		internal ContactInspector(ContactViewModel contactViewModel)
		{
			InitializeComponent();
			BindingContext = contactViewModel;
		}
	}
}
