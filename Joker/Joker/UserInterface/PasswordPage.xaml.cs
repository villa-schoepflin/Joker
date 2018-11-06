using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// A page that can be set to appear upon starting the app, prompting the user for his password.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PasswordPage : ContentPage
	{
		private bool SwitchedToSecurityQuestions;

		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public PasswordPage()
		{
			InitializeComponent();
			if(Regex.IsMatch(UserSettings.UserPassword, @"[0-9]+"))
				AnswerEntry.Keyboard = Keyboard.Numeric;
		}

		/// <summary>
		/// Text change event handler that checks whether the password is correct.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void CheckPassword(object sender, TextChangedEventArgs e)
		{
			if(AnswerEntry.Text == UserSettings.UserPassword)
				OnSecurityInformationConfirmed();
		}

		private async void OnSecurityInformationConfirmed()
		{
			Indicator.IsRunning = true;
			AnswerEntry.Unfocus();
			await Task.Delay(300);

			/* If the most recent limit hasn't expired yet, direct the user to the regular main page, otherwise
			 * direct them to the page where they can add a new limit, then return to the regular main page. */
			if(DateTime.UtcNow < AppSettings.LimitExpiredTime)
				App.SetMainPageToDefault();
			else
				Application.Current.MainPage = new AddLimitPage();
		}

		/// <summary>
		/// Button event handler that toggles whether the password should be hidden.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void TogglePasswordObfuscation(object sender, EventArgs e)
		{
			AnswerEntry.IsPassword ^= true;
		}

		/// <summary>
		/// Label event handler that allows the user to select one of the two security questions to answer in case
		/// the password was forgotten.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void ShowSecurityQuestions(object sender, EventArgs e)
		{
			SwitchedToSecurityQuestions ^= true;
			SecurityQuestion.Text = await DisplayActionSheet(
				"Wähle die Sicherheitsfrage aus, die Du beantworten möchtest:", "Abbrechen", null,
				UserSettings.FirstSecurityQuestion, UserSettings.SecondSecurityQuestion);

			if(SwitchedToSecurityQuestions)
			{
				AnswerEntry.TextChanged -= CheckPassword;
				AnswerEntry.TextChanged += CheckFirstSecurityQuestion;
				AnswerEntry.Placeholder = "Deine Antwort";
			}
			else
			{
				AnswerEntry.TextChanged += CheckPassword;
				AnswerEntry.Placeholder = "Dein Passwort";
			}
		}
	}
}