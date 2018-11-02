using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;

using Joker.BusinessLogic;

namespace Joker.UserInterface
{
	/// <summary>
	/// Represents a single contact for display in contact-related pages.
	/// </summary>
	public class ContactViewModel : ViewModel<Page, Contact>
	{
		/// <summary>
		/// The name derived from the contact model.
		/// </summary>
		public string ContactName => Model.Name;

		/// <summary>
		/// The phone number derived from the contact model.
		/// </summary>
		public string PhoneNumber => Model.PhoneNumber;

		/// <summary>
		/// Navigates the user to a detailed view of the view model's contact.
		/// </summary>
		public ICommand OpenDetailPage => new Command(async () =>
		{
			await View.Navigation.PushAsync(new ContactInspector(Model));
		});

		/// <summary>
		/// Opens the platform's telephone app with the corresponding contact's phone number.
		/// </summary>
		public ICommand CallContact => new Command(() =>
		{
			PhoneDialer.Open(Model.PhoneNumber);
		});

		/// <summary>
		/// Constructs a contact view model for the given view.
		/// </summary>
		/// <param name="view">The view for this view model.</param>
		/// <param name="model">The model for this view model.</param>
		public ContactViewModel(Page view, Contact model) : base(view, model) { }
	}
}