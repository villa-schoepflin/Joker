using System;
using System.Threading.Tasks;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Essentials;
using Xamarin.Forms;

using Contact = Joker.BusinessLogic.Contact;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user defines the data for a new contact and can insert it into the database.
	/// </summary>
	public partial class ContactCreator : ContentPage
	{
		private readonly Action Refresh;

		internal ContactCreator(Action refresh)
		{
			InitializeComponent();
			Refresh = refresh;
		}

		private async void OnNameClipboardButton(object sender, EventArgs eventArgs)
		{
			string clipboard = await Clipboard.GetTextAsync();
			if(string.IsNullOrEmpty(clipboard))
				return;

			if(clipboard.Length > Contact.MaxNameLength)
				clipboard = clipboard.Substring(0, Contact.MaxNameLength);
			NameEntry.Text = clipboard;
		}

		private async void OnPhoneNumberClipboardButton(object sender, EventArgs eventArgs)
		{
			string clipboard = await Clipboard.GetTextAsync();
			if(string.IsNullOrEmpty(clipboard))
				return;

			if(clipboard.Length > Contact.MaxPhoneNumberLength)
				clipboard = clipboard.Substring(0, Contact.MaxPhoneNumberLength);
			PhoneNumberEntry.Text = clipboard;
		}

		private async void OnSearchDeviceContactsButton(object sender, EventArgs eventArgs)
		{
			try
			{
				var contact = await Contacts.PickContactAsync();
				if(contact.Phones.Count == 0)
				{
					await DisplayAlert(null, Text.ContactWithoutPhoneNumber, Text.Ok);
					return;
				}
				NameEntry.Text = contact.DisplayName;
				PhoneNumberEntry.Text = contact.Phones[0].PhoneNumber;
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

		private async void OnSubmitButton(object sender, EventArgs eventArgs)
		{
			if(IsExecuting)
				return;

			IsExecuting = true;
			await CreateContact();
			IsExecuting = false;
		}
		private bool IsExecuting = false;

		private async Task CreateContact()
		{
			try
			{
				Contact contact = new(NameEntry.Text, PhoneNumberEntry.Text, ExpertMarker.IsToggled);
				Database.Insert(contact);
				_ = await Navigation.PopAsync();
				Refresh.Invoke();
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}
	}
}
