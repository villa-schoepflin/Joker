using System;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user should enter their name.
	/// </summary>
	public partial class NameTourPage : ContentPage
	{
		internal NameTourPage()
		{
			InitializeComponent();
		}

		private async void OnContinueButton(object sender, EventArgs eventArgs)
		{
			try
			{
				UserSettings.UserName = NameEntry.Text;
				await Navigation.PushAsync(new ContactTourPage());
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}
	}
}
