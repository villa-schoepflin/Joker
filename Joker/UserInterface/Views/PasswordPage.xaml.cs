using System;
using System.Text.RegularExpressions;
using Joker.AppInterface;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// A page that can be set to appear upon starting the app, prompting the user for his password.
	/// </summary>
	public partial class PasswordPage : ContentPage
	{
		internal PasswordPage()
		{
			InitializeComponent();
			if(Regex.IsMatch(UserSettings.UserPassword, @"[0-9]+"))
				PasswordEntry.Keyboard = Keyboard.Numeric;
		}

		private void CheckPassword(object sender, TextChangedEventArgs eventArgs)
		{
			if(eventArgs.NewTextValue != UserSettings.UserPassword)
				return;

			Indicator.IsRunning = true;
			PasswordEntry.Unfocus();
			App.RequestMainPage();
		}

		private void TogglePasswordObfuscation(object sender, EventArgs eventArgs)
		{
			PasswordEntry.IsPassword ^= true;
		}

		private async void ShowSecurityQuestions(object sender, EventArgs eventArgs)
		{
			if(Navigation.HasPage<SecurityQuestionPage>())
				return;

			await Navigation.PushAsync(new SecurityQuestionPage());
		}
	}
}
