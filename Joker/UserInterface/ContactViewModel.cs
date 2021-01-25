using System;
using System.Threading.Tasks;
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
	public sealed class ContactViewModel : ViewModel<Page, Contact>
	{
		private readonly Action Refresh;

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
		public string EditButtonText => EditingEnabled ? Text.Save : Text.Edit;

		/// <summary>
		/// The color used to paint the background of the icon frame.
		/// </summary>
		public Color IconBackgroundColor => Model.MarkedAsExpert ? Styles.Primary2 : Styles.Primary1;

		/// <summary>
		/// The icon used to differentiate expert contacts from regular contacts.
		/// </summary>
		public ImageSource TypeIcon => Model.MarkedAsExpert ? Icons.Expert : Icons.Contact;

		/// <summary>
		/// Navigates the user to a detailed view of the view model's contact.
		/// </summary>
		public ICommand OpenInspector => new Command(async () =>
		{
			if(View.Navigation.HasPage<ContactInspector>())
				return;

			ContactInspector inspector = new(this);
			await View.Navigation.PushAsync(inspector);
		});

		/// <summary>
		/// Sets the contact name in the view model from clipboard content.
		/// </summary>
		public ICommand InsertNameFromClipboard => new Command(async () =>
		{
			string clipboard = await Clipboard.GetTextAsync();
			if(string.IsNullOrEmpty(clipboard))
				return;

			if(clipboard.Length > Contact.MaxNameLength)
				clipboard = clipboard.Substring(0, Contact.MaxNameLength);
			ContactName = clipboard;
		});

		/// <summary>
		/// Sets the phone number in the view model from clipboard content.
		/// </summary>
		public ICommand InsertPhoneNumberFromClipboard => new Command(async () =>
		{
			string clipboard = await Clipboard.GetTextAsync();
			if(string.IsNullOrEmpty(clipboard))
				return;

			if(clipboard.Length > Contact.MaxPhoneNumberLength)
				clipboard = clipboard.Substring(0, Contact.MaxPhoneNumberLength);
			PhoneNumber = clipboard;
		});

		/// <summary>
		/// Opens the platform's telephone app with the corresponding contact's phone number.
		/// </summary>
		public ICommand CallContact => new Command(() => PhoneDialer.Open(Model.PhoneNumber));

		/// <summary>
		/// Toggles the editing status for the contact detail page, saving the changes on deactivating editing.
		/// </summary>
		public ICommand ToggleEditingStatus => new Command(async () =>
		{
			try
			{
				if(Model == Contact.Bzga)
				{
					await View.DisplayAlert(null, Text.ContactNotDeletable, Text.Ok);
					return;
				}
				if(EditingEnabled)
				{
					Database.Update(Model);
					Refresh.Invoke();
				}
				EditingEnabled ^= true;
			}
			catch(ArgumentException error)
			{
				await View.DisplayAlert(null, error.Message, Text.Ok);
			}
		});

		/// <summary>
		/// Attempts to delete the contact in the database.
		/// </summary>
		public ICommand Delete => new Command(async () =>
		{
			if(IsExecuting)
				return;

			IsExecuting = true;
			await DeleteContact();
			IsExecuting = false;
		});
		private bool IsExecuting = false;

		/// <summary>
		/// Constructs a contact view model for the given view.
		/// </summary>
		/// <param name="view">The view for this view model.</param>
		/// <param name="model">The model for this view model.</param>
		/// <param name="refresh">Callback for refreshing the contact feed.</param>
		public ContactViewModel(Page view, Contact model, Action refresh) : base(view, model)
		{
			Refresh = refresh;
		}

		private async Task DeleteContact()
		{
			if(Model == Contact.Bzga)
			{
				await View.DisplayAlert(null, Text.ContactNotDeletable, Text.Ok);
				return;
			}
			if(await View.DisplayAlert(null, Text.ContactAboutToBeDeleted, Text.Yes, Text.No))
			{
				Database.Delete(Model);
				_ = await View.Navigation.PopAsync();
				Refresh.Invoke();
			}
		}
	}
}
