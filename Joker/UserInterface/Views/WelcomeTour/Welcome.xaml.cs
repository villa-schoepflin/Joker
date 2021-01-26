using System;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// View presented after opening the app for the first time.
	/// </summary>
	public partial class Welcome : ContentPage
	{
		internal Welcome()
		{
			InitializeComponent();
		}

		private async void OnContinueButton(object sender, EventArgs eventArgs)
		{
			await Navigation.PushAsync(new NameTourPage());
		}
	}
}
