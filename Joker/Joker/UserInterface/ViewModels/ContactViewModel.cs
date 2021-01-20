using System;
using System.Windows.Input;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Essentials;
using Xamarin.Forms;

using Contact = Joker.BusinessLogic.Contact;

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
		public string ContactName
		{
			get => Model.Name;
			set
			{
				Model.Name = value;
				OnPropertyChanged(nameof(ContactName));
			}
		}

		/// <summary>
		/// The phone number derived from the contact model.
		/// </summary>
		public string PhoneNumber
		{
			get => Model.PhoneNumber;
			set
			{
				Model.PhoneNumber = value;
				OnPropertyChanged(nameof(PhoneNumber));
			}
		}

		/// <summary>
		/// The status whether a contact is marked as professional, derived from the contact model.
		/// </summary>
		public bool MarkedAsExpert
		{
			get => Model.MarkedAsExpert;
			set => Model.MarkedAsExpert = value;
		}

		/// <summary>
		/// Indicates whether editing of the contact model is currently enabled.
		/// </summary>
		public bool EditingEnabled
		{
			get => _editingEnabled;
			set
			{
				_editingEnabled = value;
				OnPropertyChanged(nameof(EditingEnabled));
				OnPropertyChanged(nameof(EditButtonText));
			}
		}
		private bool _editingEnabled;

		/// <summary>
		/// Determines the text of the button that toggles the editing status.
		/// </summary>
		public string EditButtonText => EditingEnabled ? "Änderungen speichern" : "Kontakt bearbeiten";

		/// <summary>
		/// The color used to paint the background of the icon frame.
		/// </summary>
		public Color IconBackgroundColor => App.Color(Model.MarkedAsExpert ? "Primary2" : "Primary1");

		/// <summary>
		/// The icon used to differentiate expert contacts from regular contacts.
		/// </summary>
		public ImageSource TypeIcon => ImageSource.FromFile(Model.MarkedAsExpert ? "ui_expert.png" : "ui_contact.png");

		/// <summary>
		/// Navigates the user to a detailed view of the view model's contact.
		/// </summary>
		public ICommand OpenDetailPage => new Command(async () =>
		{
			var contactInspector = new ContactInspector(Model);
			await View.Navigation.PushAsync(contactInspector);
		});

		/// <summary>
		/// Inserts text from the clipboard to the property specified by the parameter.
		/// </summary>
		public ICommand InsertFromClipboard => new Command<string>(async propertyName =>
		{
			string text = await Clipboard.GetTextAsync();

			if(!string.IsNullOrEmpty(text))
			{
				switch(propertyName)
				{
					case nameof(ContactName):
						if(text.Length > Contact.MaxNameLength)
							text = text.Substring(0, Contact.MaxNameLength);
						ContactName = text;
						break;
					case nameof(PhoneNumber):
						if(text.Length > Contact.MaxPhoneNumberLength)
							text = text.Substring(0, Contact.MaxPhoneNumberLength);
						PhoneNumber = text;
						break;
					default:
						throw new NotImplementedException();
				}
			}
		});

		/// <summary>
		/// Opens the platform's telephone app with the corresponding contact's phone number.
		/// </summary>
		public ICommand CallContact => new Command(() => PhoneDialer.Open(Model.PhoneNumber));

		/// <summary>
		/// Toggles the editing status for the contact detail page, saving the changes on
		/// deactivating editing.
		/// </summary>
		public ICommand ToggleEditingStatus => new Command(async () =>
		{
			try
			{
				if(Model == Contact.Bzga)
				{
					await View.DisplayAlert(null, Alerts.ContactNotDeletable, Alerts.Ok);
					return;
				}
				if(EditingEnabled)
				{
					Database.Update(Model);
					App.CurrentContactPage.RefreshContacts();
				}
				EditingEnabled ^= true;
			}
			catch(ArgumentException error)
			{
				await View.DisplayAlert(null, error.Message, Alerts.Ok);
			}
		});

		/// <summary>
		/// The command by which the model contact will be deleted in the database.
		/// </summary>
		public ICommand DeleteContact => new Command(async () =>
		{
			if(Model == Contact.Bzga)
			{
				await View.DisplayAlert(null, Alerts.ContactNotDeletable, Alerts.Ok);
				return;
			}
			if(await View.DisplayAlert(null, Alerts.ContactAboutToBeDeleted, Alerts.Yes, Alerts.No))
			{
				Database.Delete(Model);
				await View.Navigation.PopAsync();
				App.CurrentContactPage.RefreshContacts();
			}
		});

		/// <summary>
		/// Constructs a contact view model for the given view.
		/// </summary>
		/// <param name="view">The view for this view model.</param>
		/// <param name="model">The model for this view model.</param>
		public ContactViewModel(Page view, Contact model) : base(view, model) { }
	}
}
