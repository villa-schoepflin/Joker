using Joker.BusinessLogic;
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
		/// <param name="contact">Contact whose details should be exposed.</param>
		public ContactInspector(Contact contact)
		{
			InitializeComponent();
			BindingContext = new ContactViewModel(this, contact.Copy());
		}
	}
}
