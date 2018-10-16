using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.ApplicationLayer;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user can change user-specific settings.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public SettingsPage()
		{
			InitializeComponent();
			UserNameEntry.Text = UserSettings.UserName;
			ContactEntry.Text = UserSettings.PersonalContact?.Name;
			NewPictureEntry.Text = $"{UserSettings.NewPictureInterval.TotalDays} Tage";
			GambleReminderEntry.Text = $"{UserSettings.GambleReminderInterval.TotalHours} Stunden";
			LimitReminderEntry.Text = $"{UserSettings.LimitReminderInterval.TotalHours} Stunden";
		}

		/// <summary>
		/// Button event handler that relays input validation for the user name setting and saves it if applicable.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SaveUserName(object sender, EventArgs e)
		{
			try
			{
				UserSettings.UserName = UserNameEntry.Text;
				await DisplayAlert("Name gespeichert",
					$"Die App spricht Dich ab sofort mit {UserSettings.UserName} an.", "Ok");
				App.CurrentSupportPage.RefreshInfo();
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
			}
		}

		/// <summary>
		/// Entry event handler that opens the contact-picking dialog to the user, relays data to this view
		/// and saves the selected contact.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void PickContact(object sender, EventArgs e)
		{
			ContactEntry.Unfocus();
			var contact = await DependencyService.Get<IPlatformContactPicker>().PickContact();
			if(contact != null)
			{
				UserSettings.PersonalContact = contact;
				ContactEntry.Text = contact.Name;
				App.CurrentSupportPage.RefreshContacts();
			}
		}

		/// <summary>
		/// Button event handler that clears the optional personal contact from the settings.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void RemoveContact(object sender, EventArgs e)
		{
			UserSettings.PersonalContact = null;
			ContactEntry.Text = null;
			App.CurrentSupportPage.RefreshContacts();
		}

		/// <summary>
		/// Button event handler that relays input validation for saving the interval between the insertion
		/// of new pictures into the database and saves the setting if applicable.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SaveNewPictureInterval(object sender, EventArgs e)
		{
			try
			{
				UserSettings.SetNewPictureInterval(NewPictureEntry.Text);
				await DisplayAlert("Gespeichert",
					"Die Einstellung wird nach dem nächsten Bild berücksichtigt.", "Ok");
			}
			catch(ArgumentException error)
			{
				NewPictureEntry.Text = $"{UserSettings.NewPictureInterval.TotalDays} Tage";
				await DisplayAlert(null, error.Message, "Ok");
			}
		}

		/// <summary>
		/// Button event handler that relays input validation for saving the interval between gambling 
		/// reminders and saves the setting if applicable.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SaveGambleReminderInterval(object sender, EventArgs e)
		{
			try
			{
				UserSettings.SetGambleReminderInterval(GambleReminderEntry.Text);
				DependencyService.Get<IPlatformNotifier>().ScheduleGambleReminder(UserSettings.GambleReminderInterval);
				await DisplayAlert("Gespeichert",
					$"Die nächste Erinnerung kommt in circa {UserSettings.GambleReminderInterval.TotalHours} Stunden.",
					"Ok");
			}
			catch(ArgumentException error)
			{
				GambleReminderEntry.Text = $"{UserSettings.GambleReminderInterval.TotalHours} Stunden";
				await DisplayAlert(null, error.Message, "Ok");
			}
		}

		/// <summary>
		/// Button event handler that relays input validation for saving the interval between limit
		/// reminders and saves the setting if applicable.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SaveLimitReminderInterval(object sender, EventArgs e)
		{
			try
			{
				UserSettings.SetLimitReminderInterval(LimitReminderEntry.Text);
				DependencyService.Get<IPlatformNotifier>().ScheduleLimitReminder(UserSettings.LimitReminderInterval);
				await DisplayAlert("Gespeichert",
					$"Die nächste Erinnerung kommt in circa {UserSettings.LimitReminderInterval.TotalHours} Stunden.",
					"Ok");
			}
			catch(ArgumentException error)
			{
				LimitReminderEntry.Text = $"{UserSettings.LimitReminderInterval.TotalHours} Stunden";
				await DisplayAlert(null, error.Message, "Ok");
			}
		}

		/// <summary>
		/// Button event handler that navigates the user to the impressum.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnImpressumButton(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Impressum());
		}
	}
}