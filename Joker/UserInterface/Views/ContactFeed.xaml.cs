using System;
using Joker.DataAccess;
using Xamarin.Essentials;
using Xamarin.Forms;

using Contact = Joker.BusinessLogic.Contact;

namespace Joker.UserInterface
{
	/// <summary>
	/// The main page's right tab. A view that lists all recorded contacts and allows the user to edit and call them.
	/// </summary>
	public partial class ContactFeed : ContentPage
	{
		internal const string Headline = "Kontakte";

		/// <summary>
		/// An array of contacts in a view model that can be immediately called by the user.
		/// </summary>
		public ContactViewModel[] Contacts
		{
			get
			{
				var dbContacts = Database.AllContacts();
				var allContacts = new Contact[dbContacts.Length + 1];
				Array.ConstrainedCopy(dbContacts, 0, allContacts, 1, dbContacts.Length);
				allContacts[0] = Contact.Bzga;
				return Array.ConvertAll(allContacts, contact => new ContactViewModel(this, contact, Refresh));
			}
		}

		internal ContactFeed()
		{
			InitializeComponent();
			BindingContext = this;
		}

		private void Refresh()
		{
			OnPropertyChanged(nameof(Contacts));
		}

		private async void OnSearchCounselingCentersOnline(object sender, EventArgs eventArgs)
		{
			const string url = "https://www.check-dein-spiel.de/hilfe/hilfe-vor-ort";
			await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
		}

		private async void OnSubmitButton(object sender, EventArgs eventArgs)
		{
			if(Navigation.HasPage<ContactCreator>())
				return;

			ContactCreator page = new(Refresh);
			await Navigation.PushAsync(page);
		}
	}
}
