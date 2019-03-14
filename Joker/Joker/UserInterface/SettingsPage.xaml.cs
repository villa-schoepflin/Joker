using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.AppInterface;
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
		/// The title of this page, provided here to be accessible from anywhere.
		/// </summary>
		public const string Headline = "Einstellungen";

		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public SettingsPage()
		{
			InitializeComponent();

			AspectPicker.ItemsSource = new[] { "Anpassen", "Dehnen", "Ausfüllen" };
			AspectPicker.SelectedIndex = (int)UserSettings.PreferredAspect;

			NewPictureEntry.Text = $"{UserSettings.NewPictureInterval.TotalDays} Tage";
			GambleReminderEntry.Text = $"{UserSettings.GambleReminderInterval.TotalHours} Stunden";
			LimitReminderEntry.Text = $"{UserSettings.LimitReminderInterval.TotalHours} Stunden";
		}

		/// <summary>
		/// Toolbar item event handler that shows the user a message concerning this page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnInfoButton(object sender, EventArgs e)
		{
			await DisplayAlert(Headline, FileResourceReader.Get("Info_SettingsPage.txt"), "Ok");
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
				await DisplayAlert("Name gespeichert", "Die App spricht Dich ab jetzt mit " + UserSettings.UserName
					+ " an.", "Ok");
				App.CurrentTimelineFeed.RefreshInfo();
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
			}
		}

		/// <summary>
		/// Picker event handler that saves the selected image aspect for the picture feed.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void SavePictureAspect(object sender, EventArgs e)
		{
			UserSettings.PreferredAspect = (Aspect)AspectPicker.SelectedIndex;
			App.CurrentPictureFeed.RefreshPreferredAspect();
		}

		/// <summary>
		/// Button event handler that toggles whether the password and security answers should be hidden.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void ToggleObfuscation(object sender, EventArgs e)
		{
			if(sender == FirstAnswerObfuscator)
				FirstSecurityAnswer.IsPassword ^= true;
			else if(sender == SecondAnswerObfuscator)
				SecondSecurityAnswer.IsPassword ^= true;
			else
				UserPasswordEntry.IsPassword ^= true;
		}

		/// <summary>
		/// Button event handler that removes the password and its text.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void RemovePassword(object sender, EventArgs e)
		{
			if(!AppSettings.UserPasswordIsSet)
				await DisplayAlert(null, "Du müsstest erstmal ein Passwort setzen, um es entfernen zu können.", "Ok");
			else if(await DisplayAlert(null, "Möchtest Du den Passwortschutz entfernen?", "Ja", "Nein"))
			{
				FirstSecurityQuestion.Text = UserSettings.FirstSecurityQuestion = "";
				FirstSecurityAnswer.Text = UserSettings.FirstSecurityAnswer = "";
				AppSettings.FirstSecurityQuestionIsSet = false;

				SecondSecurityQuestion.Text = UserSettings.SecondSecurityQuestion = "";
				SecondSecurityAnswer.Text = UserSettings.SecondSecurityAnswer = "";
				AppSettings.SecondSecurityQuestionIsSet = false;

				UserPasswordEntry.Text = "";
				UserSettings.UserPassword = "";
				AppSettings.UserPasswordIsSet = false;
				await DisplayAlert("Passwort entfernt", "Du brauchst jetzt kein Passwort mehr für die App.", "Ok");
			}
		}

		/// <summary>
		/// Button event handler that saves the text in the password entry as password if valid.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SavePassword(object sender, EventArgs e)
		{
			try
			{
				if(!AppSettings.FirstSecurityQuestionIsSet || !AppSettings.SecondSecurityQuestionIsSet)
					throw new ArgumentException("Bitte speichere zuerst die Sicherheitsfragen und Antworten.");
				if(string.IsNullOrEmpty(UserPasswordEntry.Text))
					throw new ArgumentException("Du kannst kein leeres Passwort setzen.");
				UserSettings.UserPassword = UserPasswordEntry.Text;
				AppSettings.UserPasswordIsSet = true;
				await DisplayAlert("Passwort erstellt", "Ab jetzt wird Dich die App nach dem Passwort fragen.", "Ok");
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
				UserPasswordEntry.Text = UserSettings.UserPassword;
			}
		}

		/// <summary>
		/// Button event handler that opens an input dialog for selecting one of several predefined security questions.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void ShowSecurityQuestionProposals(object sender, EventArgs e)
		{
			string selection = await DisplayActionSheet("Vorschläge für Sicherheitsfragen:", "Abbrechen", null,
				FileResourceReader.Get("SecurityQuestions.txt").Replace("\r", "").Split('\n'));

			if(!string.IsNullOrEmpty(selection) && selection != "Abbrechen")
			{
				if(sender == FirstQuestionMenuButton)
					FirstSecurityQuestion.Text = selection;
				else
					SecondSecurityQuestion.Text = selection;
			}
		}

		/// <summary>
		/// Saves the text of the first security question and its answer and relays possible input errors to the user.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SaveFirstSecurityQuestion(object sender, EventArgs e)
		{
			try
			{
				if(string.IsNullOrEmpty(FirstSecurityQuestion.Text) || string.IsNullOrEmpty(FirstSecurityAnswer.Text))
					throw new ArgumentException("Deine Angabe darf hier nicht leer sein.");
				UserSettings.FirstSecurityQuestion = FirstSecurityQuestion.Text;
				UserSettings.FirstSecurityAnswer = FirstSecurityAnswer.Text;
				AppSettings.FirstSecurityQuestionIsSet = true;
				await DisplayAlert("Gespeichert", "Die erste Sicherheitsfrage und Antwort sind gespeichert.", "Ok");
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
			}
		}

		/// <summary>
		/// Saves the text of the second security question and its answer and relays possible input errors to the user.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SaveSecondSecurityQuestion(object sender, EventArgs e)
		{
			try
			{
				if(string.IsNullOrEmpty(SecondSecurityQuestion.Text) || string.IsNullOrEmpty(SecondSecurityAnswer.Text))
					throw new ArgumentException("Deine Angabe darf hier nicht leer sein.");
				UserSettings.SecondSecurityQuestion = SecondSecurityQuestion.Text;
				UserSettings.SecondSecurityAnswer = SecondSecurityAnswer.Text;
				AppSettings.SecondSecurityQuestionIsSet = true;
				await DisplayAlert("Gespeichert", "Die zweite Sicherheitsfrage und Antwort sind gespeichert.", "Ok");
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
			}
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
				await DisplayAlert("Gespeichert", "Die Einstellung wird nach dem nächsten Bild berücksichtigt.", "Ok");
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
				NewPictureEntry.Text = $"{UserSettings.NewPictureInterval.TotalDays} Tage";
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
				await DisplayAlert("Gespeichert", "Die nächste Erinnerung kommt in circa "
					+ UserSettings.GambleReminderInterval.TotalHours + " Stunden.", "Ok");
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
				GambleReminderEntry.Text = $"{UserSettings.GambleReminderInterval.TotalHours} Stunden";
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
				await DisplayAlert("Gespeichert", "Die nächste Erinnerung kommt in circa "
					+ UserSettings.LimitReminderInterval.TotalHours + " Stunden.", "Ok");
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
				LimitReminderEntry.Text = $"{UserSettings.LimitReminderInterval.TotalHours} Stunden";
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