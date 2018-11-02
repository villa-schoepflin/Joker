using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.ApplicationLayer;
using Joker.BusinessLogic;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user defines the data for a new contact and can insert it into the database.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddContactPage : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public AddContactPage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Button event handler that opens the contact-picking dialog to the user and enters the
		/// results into the entries in the view.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnSearchDeviceContactsButton(object sender, EventArgs e)
		{
			var contact = await DependencyService.Get<IPlatformContactPicker>().PickContact();
			if(contact != null)
			{
				NameEntry.Text = contact.Name;
				PhoneNumberEntry.Text = contact.PhoneNumber;
			}
		}

		/// <summary>
		/// Button event handler that relays input validation, inserts the contact into the database,
		/// performs necessary refresh actions and navigates the user back to the main page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnSubmitButton(object sender, EventArgs e)
		{
			try
			{
				Database.Insert(new Contact(NameEntry.Text, PhoneNumberEntry.Text, ExpertMarker.IsToggled));
				await Navigation.PopAsync();
				App.CurrentContactPage.RefreshContacts();
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
			}
		}
	}
}