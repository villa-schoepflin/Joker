using System;
using System.Collections.Generic;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.BusinessLogic;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// The main page's right tab. A view that lists all recorded contacts and allows the user
	/// to edit and call them.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ContactPage : ContentPage
	{
		/// <summary>
		/// The title of this page, provided here to be accessible from anywhere.
		/// </summary>
		public const string Headline = "Kontakte";

		/// <summary>
		/// Info text for the user concerning the functionality of this page.
		/// </summary>
		public const string Info = "Auf dieser Seite findest Du einige nützliche Telefonnummern, die Du bei "
			+ "Bedarf sofort anrufen kannst. Tippe dazu einfach auf das Telefonsymbol. Du kannst auch online nach "
			+ "Beratungsstellen in Deiner Nähe suchen, indem Du auf den Link im obersten Feld tippst.\n\n"
			+ "Kontakte kannst Du über das \"+\" in der unteren rechten Ecke hinzufügen. Wenn Du einen Kontakt "
			+ "bearbeiten möchtest, tippe einfach auf den jeweiligen Kontakt.";

		/// <summary>
		/// An array of contacts in a view model that can be immediately called by the user.
		/// </summary>
		public ContactViewModel[] Contacts
		{
			get
			{
				var contacts = new List<Contact>(Database.AllContacts());
				contacts.Insert(0, Contact.Bzga);
				return Array.ConvertAll(contacts.ToArray(), contact => new ContactViewModel(this, contact));
			}
		}

		/// <summary>
		/// Initializes XAML elements and binds the view data.
		/// </summary>
		public ContactPage()
		{
			InitializeComponent();
			BindingContext = this;
		}

		/// <summary>
		/// Refreshes the contacts data binding in this view.
		/// </summary>
		public void RefreshContacts()
		{
			OnPropertyChanged(nameof(Contacts));
		}

		/// <summary>
		/// Label event handler that opens the link where the user can search for counseling centers.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnOnlineSearchLink(object sender, EventArgs e)
		{
			await Browser.OpenAsync("https://www.check-dein-spiel.de/hilfe/hilfe-vor-ort",
				BrowserLaunchMode.SystemPreferred);
		}

		/// <summary>
		/// Button event handler that navigates the user to a view where they can add a new contact to the database.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnSubmitButton(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new AddContactPage());
		}
	}
}