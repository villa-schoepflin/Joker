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

		private readonly Action RefreshFeedback;

		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		/// <param name="refreshFeedback">Callback for refreshing the timeline feedback text.</param>
		public SettingsPage(Action refreshFeedback)
		{
			InitializeComponent();
			RefreshFeedback = refreshFeedback;

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
		/// <param name="eventArgs">Contains event data.</param>
		private async void OnInfoButton(object sender, EventArgs eventArgs)
		{
			await DisplayAlert(Headline, TextAssetReader.Get("Info_SettingsPage.txt"), Text.Ok);
		}

		/// <summary>
		/// Button event handler that relays input validation for the user name setting and saves it if possible.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void SaveUserName(object sender, EventArgs eventArgs)
		{
			try
			{
				UserSettings.UserName = UserNameEntry.Text;

				string msg = string.Format(Text.NameSaved, UserSettings.UserName);
				await DisplayAlert(Text.TitleOnSaved, msg, Text.Ok);

				RefreshFeedback.Invoke();
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}

		/// <summary>
		/// Button event handler that toggles whether the password and security answers should be hidden.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private void ToggleObfuscation(object sender, EventArgs eventArgs)
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
		/// <param name="eventArgs">Contains event data.</param>
		private async void RemovePassword(object sender, EventArgs eventArgs)
		{
			if(!AppSettings.UserPasswordIsSet)
				return;

			if(await DisplayAlert(null, Text.PasswordAboutToBeDeleted, Text.Yes, Text.No))
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

				await DisplayAlert(Text.TitleOnPasswordDeleted, Text.PasswordDeleted, Text.Ok);
			}
		}

		/// <summary>
		/// Button event handler that saves the text in the password entry as password if valid.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void SavePassword(object sender, EventArgs eventArgs)
		{
			try
			{
				if(!AppSettings.FirstSecurityQuestionIsSet || !AppSettings.SecondSecurityQuestionIsSet)
					throw new ArgumentException(Text.SecurityAttributesNotSaved);

				if(string.IsNullOrWhiteSpace(UserPasswordEntry.Text))
					throw new ArgumentException(Text.InputEmpty);

				UserSettings.UserPassword = UserPasswordEntry.Text;
				AppSettings.UserPasswordIsSet = true;
				await DisplayAlert(Text.TitleOnPasswordSaved, Text.PasswordSaved, Text.Ok);
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}

		/// <summary>
		/// Button event handler that opens an input dialog for selecting one of several security question proposals.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void ShowSecurityQuestionProposals(object sender, EventArgs eventArgs)
		{
			string proposalsAsset = TextAssetReader.Get("SecurityQuestions.txt");
			string[] proposals = proposalsAsset.Replace("\r", "").Split('\n');
			string question = await DisplayActionSheet(Text.TitleOnSecurityProposals, Text.Cancel, null, proposals);

			if(string.IsNullOrWhiteSpace(question) || question == Text.Cancel)
				return;

			if(sender == FirstQuestionMenuButton)
				SecurityQuestion1.Text = question;
			else
				SecurityQuestion2.Text = question;
		}

		/// <summary>
		/// Saves the text of the security question and its answer based on which button was pressed and relays possible
		/// input errors to the user.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void SaveSecurityAttribute(object sender, EventArgs eventArgs)
		{
			try
			{
				if(sender == FirstSecurityAttributeSaver)
				{
					if(string.IsNullOrEmpty(SecurityQuestion1.Text) || string.IsNullOrEmpty(SecurityAnswer1.Text))
						throw new ArgumentException(Text.InputEmpty);
					UserSettings.FirstSecurityAttribute = (SecurityQuestion1.Text, SecurityAnswer1.Text);
					AppSettings.FirstSecurityQuestionIsSet = true;
				}
				else
				{
					if(string.IsNullOrEmpty(SecurityQuestion2.Text) || string.IsNullOrEmpty(SecurityAnswer2.Text))
						throw new ArgumentException(Text.InputEmpty);
					UserSettings.SecondSecurityAttribute = (SecurityQuestion2.Text, SecurityAnswer2.Text);
					AppSettings.SecondSecurityQuestionIsSet = true;
				}
				await DisplayAlert(Text.TitleOnSaved, Text.SecurityAttributeSaved, Text.Ok);
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}

		/// <summary>
		/// Button event handler that relays input validation for saving the interval between the insertion of new
		/// pictures into the database and saves the setting if possible.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void SaveNewPictureInterval(object sender, EventArgs eventArgs)
		{
			try
			{
				UserSettings.SetNewPictureInterval(NewPictureEntry.Text);
				await DisplayAlert(Text.TitleOnSaved, Text.PictureIntervalSaved, Text.Ok);
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
				NewPictureEntry.Text = UserSettings.NewPictureInterval.TotalDays.ToString();
			}
		}

		/// <summary>
		/// Button event handler that relays input validation for saving the interval between gambling reminders and
		/// saves the setting if possible.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void SaveGambleReminderInterval(object sender, EventArgs eventArgs)
		{
			try
			{
				UserSettings.SetGambleReminderInterval(GambleReminderEntry.Text);
				var notifier = DependencyService.Get<IPlatformNotifier>();
				notifier.ScheduleGambleReminder(UserSettings.GambleReminderInterval);

				double hours = UserSettings.GambleReminderInterval.TotalHours;
				string msg = string.Format(Text.ReminderIntervalSaved, hours);
				await DisplayAlert(Text.TitleOnSaved, msg, Text.Ok);
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
				GambleReminderEntry.Text = UserSettings.GambleReminderInterval.TotalHours.ToString();
			}
		}

		/// <summary>
		/// Button event handler that relays input validation for saving the interval between limit reminders and saves
		/// the setting if possible.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void SaveLimitReminderInterval(object sender, EventArgs eventArgs)
		{
			try
			{
				UserSettings.SetLimitReminderInterval(LimitReminderEntry.Text);
				var notifier = DependencyService.Get<IPlatformNotifier>();
				notifier.ScheduleLimitReminder(UserSettings.LimitReminderInterval);

				double hours = UserSettings.LimitReminderInterval.TotalHours;
				string msg = string.Format(Text.ReminderIntervalSaved, hours);
				await DisplayAlert(Text.TitleOnSaved, msg, Text.Ok);
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
				LimitReminderEntry.Text = UserSettings.LimitReminderInterval.TotalHours.ToString();
			}
		}

		/// <summary>
		/// Button event handler that navigates the user to the impressum.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void OnImpressumButton(object sender, EventArgs eventArgs)
		{
			if(Navigation.HasPage<Impressum>())
				return;

			await Navigation.PushAsync(new Impressum());
		}
	}
}
