using System;
using System.Threading.Tasks;
using Joker.BusinessLogic;
using Xamarin.Essentials;
using Xamarin.Forms;

using Contact = Joker.BusinessLogic.Contact;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user can select their personal contact.
	/// </summary>
	public partial class ContactTourPage : ContentPage
	{
		/// <summary>
		/// The contact that can be selected here and will be inserted upon completing the welcome
		/// tour.
		/// </summary>
		public static Contact FirstContact;

		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public ContactTourPage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Entry event handler that opens the contact-picking dialog to the user,
		/// relays data to this view and saves the selected contact.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void PickContact(object sender, EventArgs e)
		{
			ContactEntry.Unfocus();
			try
			{
				var contact = await Contacts.PickContactAsync();

				if(contact.Phones.Count == 0)
					await DisplayAlert(null, Alerts.ContactWithoutPhoneNumber, Alerts.Ok);
				else
				{
					FirstContact = new Contact(contact.DisplayName, contact.Phones[0].PhoneNumber, false);
					ContactEntry.Text = FirstContact.Name;
				}
			}
			catch(PermissionException)
			{
				await DisplayAlert(null, Alerts.ContactPermissionDenied, Alerts.Ok);
			}
			catch(TaskCanceledException)
			{
				// Happens when the contact selection screen is returned from without picking a contact. Can be ignored.
			}
		}

		/// <summary>
		/// Button event handler that navigates the user to the next tour page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnContinueButton(object sender, EventArgs e)
		{
			if(FirstContact is object && FirstContact == Contact.Bzga)
				await DisplayAlert(null, Alerts.ContactAlreadyExists, Alerts.Ok);
			else
				await Navigation.PushAsync(new LimitTourPage());
		}
	}
}
