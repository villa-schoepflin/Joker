using System;
using System.Text.RegularExpressions;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// A page that can be set to appear upon starting the app, prompting the user for his password.
	/// </summary>
	public partial class PasswordPage : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public PasswordPage()
		{
			InitializeComponent();
			if(Regex.IsMatch(UserSettings.UserPassword, @"[0-9]+"))
				PasswordEntry.Keyboard = Keyboard.Numeric;
		}

		/// <summary>
		/// Text change event handler that checks whether the password is correct.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private void CheckPassword(object sender, TextChangedEventArgs eventArgs)
		{
			if(eventArgs.NewTextValue == UserSettings.UserPassword)
			{
				Indicator.IsRunning = true;
				PasswordEntry.Unfocus();
				App.SetMainPageToDefault();
			}
		}

		/// <summary>
		/// Button event handler that toggles whether the password should be hidden.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private void TogglePasswordObfuscation(object sender, EventArgs eventArgs)
		{
			PasswordEntry.IsPassword ^= true;
		}

		/// <summary>
		/// Label event handler that navigates the user to the page where they can answer their security questions.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void ShowSecurityQuestions(object sender, EventArgs eventArgs)
		{
			await Navigation.PushAsync(new SecurityQuestionPage());
		}
	}
}
