using System;
using Joker.BusinessLogic;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user must set their first limit.
	/// </summary>
	public partial class LimitTourPage : ContentPage
	{
		internal static Limit FirstLimit;

		internal LimitTourPage()
		{
			InitializeComponent();
		}

		private async void OnContinueButton(object sender, EventArgs eventArgs)
		{
			try
			{
				FirstLimit = new(LimitEntry.Text, Limit.InitialLimitDuration.TotalDays.ToString());
				await Navigation.PushAsync(new Finish());
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}
	}
}
