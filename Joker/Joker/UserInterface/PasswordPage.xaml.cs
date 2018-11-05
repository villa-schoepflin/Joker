using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.DataAccess;

namespace Joker.UserInterface
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PasswordPage : ContentPage
	{
		public PasswordPage()
		{
			InitializeComponent();
		}

		private void OnPasswordEntryTextChanged(object sender, EventArgs e)
		{
			if(PasswordEntry.Text == UserSettings.UserPassword)
			{
				Device.StartTimer;

				/* If the most recent limit hasn't expired yet, direct the user to the regular main page, otherwise
				 * direct them to the page where they can add a new limit, then return to the regular main page. */
				if(DateTime.UtcNow < AppSettings.LimitExpiredTime)
					App.SetMainPageToDefault();
				else
					Application.Current.MainPage = new AddLimitPage();
			}
		}

		private void OnForgotPasswordLabel(object sender, EventArgs e)
		{
			;
		}
	}
}