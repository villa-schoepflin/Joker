using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// A view containing the database-supplied details and properties of a selected contact.
	/// </summary>
	public partial class ContactInspector : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements and provides the data to be bound in the view.
		/// </summary>
		/// <param name="contactViewModel">View model through which the contact details will be exposed.</param>
		public ContactInspector(ContactViewModel contactViewModel)
		{
			InitializeComponent();
			BindingContext = contactViewModel;
		}
	}
}
