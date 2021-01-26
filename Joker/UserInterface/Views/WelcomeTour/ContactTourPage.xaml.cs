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
		internal static Contact FirstContact;

		internal ContactTourPage()
		{
			InitializeComponent();
		}

		private async void PickContact(object sender, EventArgs eventArgs)
		{
			ContactEntry.Unfocus();
			try
			{
				var contact = await Contacts.PickContactAsync();

				if(contact.Phones.Count == 0)
					await DisplayAlert(null, Text.ContactWithoutPhoneNumber, Text.Ok);
				else
				{
					FirstContact = new(contact.DisplayName, contact.Phones[0].PhoneNumber, false);
					ContactEntry.Text = FirstContact.Name;
				}
			}
			catch(PermissionException)
			{
				await DisplayAlert(null, Text.ContactPermissionDenied, Text.Ok);
			}
			catch(TaskCanceledException)
			{
				// Happens when the contact selection screen is returned from without picking a contact. Can be ignored.
			}
		}

		private async void OnContinueButton(object sender, EventArgs eventArgs)
		{
			if(FirstContact is object && FirstContact == Contact.Bzga)
				await DisplayAlert(null, Text.ContactAlreadyExists, Text.Ok);
			else
				await Navigation.PushAsync(new LimitTourPage());
		}
	}
}
