using System;
using Joker.AppInterface;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user can change user-specific settings.
	/// </summary>
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

			SecurityQuestion1.Text = UserSettings.FirstSecurityAttribute.Item1;
			SecurityAnswer1.Text = UserSettings.FirstSecurityAttribute.Item2;
			SecurityQuestion2.Text = UserSettings.SecondSecurityAttribute.Item1;
			SecurityAnswer2.Text = UserSettings.SecondSecurityAttribute.Item2;

			NewPictureEntry.Text = UserSettings.NewPictureInterval.TotalDays.ToString();
			GambleReminderEntry.Text = UserSettings.GambleReminderInterval.TotalHours.ToString();
			LimitReminderEntry.Text = UserSettings.LimitReminderInterval.TotalHours.ToString();
		}

		/// <summary>
		/// Toolbar item event handler that shows the user a message concerning this page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnInfoButton(object sender, EventArgs e)
		{
			await DisplayAlert(Headline, TextAssetReader.Get("Info_SettingsPage.txt"), Alerts.Ok);
		}

		/// <summary>
		/// Button event handler that relays input validation for the user name setting and saves it
		/// if applicable.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SaveUserName(object sender, EventArgs e)
		{
			try
			{
				UserSettings.UserName = UserNameEntry.Text;

				string msg = string.Format(Alerts.NameSaved, UserSettings.UserName);
				await DisplayAlert(Alerts.TitleOnSaved, msg, Alerts.Ok);

				App.CurrentTimelineFeed.RefreshInfo();
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Alerts.Ok);
			}
		}

		/// <summary>
		/// Button event handler that toggles whether the password and security answers should be
		/// hidden.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void ToggleObfuscation(object sender, EventArgs e)
		{
			if(sender == FirstAnswerObfuscator)
				SecurityAnswer1.IsPassword ^= true;
			else if(sender == SecondAnswerObfuscator)
				SecurityAnswer2.IsPassword ^= true;
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
			if(AppSettings.UserPasswordIsSet)
				if(await DisplayAlert(null, Alerts.PasswordAboutToBeDeleted, Alerts.Yes, Alerts.No))
				{
					UserSettings.UserPassword = "";
					AppSettings.UserPasswordIsSet = false;

					UserSettings.FirstSecurityAttribute = ("", "");
					UserSettings.SecondSecurityAttribute = ("", "");
					AppSettings.FirstSecurityQuestionIsSet = false;
					AppSettings.SecondSecurityQuestionIsSet = false;

					UserPasswordEntry.Text = "";
					SecurityQuestion1.Text = UserSettings.FirstSecurityAttribute.Item1;
					SecurityAnswer1.Text = UserSettings.FirstSecurityAttribute.Item2;
					SecurityQuestion2.Text = UserSettings.SecondSecurityAttribute.Item1;
					SecurityAnswer2.Text = UserSettings.SecondSecurityAttribute.Item2;

					await DisplayAlert(Alerts.TitleOnPasswordDeleted, Alerts.PasswordDeleted, Alerts.Ok);
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
					throw new ArgumentException(Alerts.SecurityAttributesNotSaved);

				if(string.IsNullOrWhiteSpace(UserPasswordEntry.Text))
					throw new ArgumentException(Alerts.InputEmpty);

				UserSettings.UserPassword = UserPasswordEntry.Text;
				AppSettings.UserPasswordIsSet = true;
				await DisplayAlert(Alerts.TitleOnPasswordSaved, Alerts.PasswordSaved, Alerts.Ok);
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Alerts.Ok);
				UserPasswordEntry.Text = UserSettings.UserPassword;
			}
		}

		/// <summary>
		/// Button event handler that opens an input dialog for selecting one of several predefined
		/// security questions.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void ShowSecurityQuestionProposals(object sender, EventArgs e)
		{
			string proposalsAsset = TextAssetReader.Get("SecurityQuestions.txt");
			string[] proposals = proposalsAsset.Replace("\r", "").Split('\n');
			string question = await DisplayActionSheet(Alerts.TitleOnSecurityProposals, Alerts.Cancel, null, proposals);

			if(!string.IsNullOrWhiteSpace(question) && question != Alerts.Cancel)
			{
				if(sender == FirstQuestionMenuButton)
					SecurityQuestion1.Text = question;
				else
					SecurityQuestion2.Text = question;
			}
		}

		/// <summary>
		/// Saves the text of the security question and its answer based on which button was pressed and relays possible
		/// input errors to the user.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SaveSecurityAttribute(object sender, EventArgs e)
		{
			try
			{
				if(sender == FirstSecurityAttributeSaver)
				{
					if(string.IsNullOrEmpty(SecurityQuestion1.Text) || string.IsNullOrEmpty(SecurityAnswer1.Text))
						throw new ArgumentException(Alerts.InputEmpty);
					UserSettings.FirstSecurityAttribute = (SecurityQuestion1.Text, SecurityAnswer1.Text);
					AppSettings.FirstSecurityQuestionIsSet = true;
				}
				else
				{
					if(string.IsNullOrEmpty(SecurityQuestion2.Text) || string.IsNullOrEmpty(SecurityAnswer2.Text))
						throw new ArgumentException(Alerts.InputEmpty);
					UserSettings.SecondSecurityAttribute = (SecurityQuestion2.Text, SecurityAnswer2.Text);
					AppSettings.SecondSecurityQuestionIsSet = true;
				}
				await DisplayAlert(Alerts.TitleOnSaved, Alerts.SecurityAttributeSaved, Alerts.Ok);
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Alerts.Ok);
			}
		}

		/// <summary>
		/// Button event handler that relays input validation for saving the interval between the
		/// insertion of new pictures into the database and saves the setting if applicable.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SaveNewPictureInterval(object sender, EventArgs e)
		{
			try
			{
				UserSettings.SetNewPictureInterval(NewPictureEntry.Text);
				await DisplayAlert(Alerts.TitleOnSaved, Alerts.PictureIntervalSaved, Alerts.Ok);
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Alerts.Ok);
				NewPictureEntry.Text = UserSettings.NewPictureInterval.TotalDays.ToString();
			}
		}

		/// <summary>
		/// Button event handler that relays input validation for saving the interval between
		/// gambling reminders and saves the setting if applicable.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void SaveGambleReminderInterval(object sender, EventArgs e)
		{
			try
			{
				UserSettings.SetGambleReminderInterval(GambleReminderEntry.Text);
				DependencyService.Get<IPlatformNotifier>().ScheduleGambleReminder(UserSettings.GambleReminderInterval);

				double hours = UserSettings.GambleReminderInterval.TotalHours;
				string msg = string.Format(Alerts.ReminderIntervalSaved, hours);
				await DisplayAlert(Alerts.TitleOnSaved, msg, Alerts.Ok);
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Alerts.Ok);
				GambleReminderEntry.Text = UserSettings.GambleReminderInterval.TotalHours.ToString();
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

				double hours = UserSettings.LimitReminderInterval.TotalHours;
				string msg = string.Format(Alerts.ReminderIntervalSaved, hours);
				await DisplayAlert(Alerts.TitleOnSaved, msg, Alerts.Ok);
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Alerts.Ok);
				LimitReminderEntry.Text = UserSettings.LimitReminderInterval.TotalHours.ToString();
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
