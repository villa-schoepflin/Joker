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
		/// <summary>
		/// The first limit to be inserted into the database on finishing the welcome tour.
		/// </summary>
		public static Limit FirstLimit;

		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public LimitTourPage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Button event handler that navigates the user to the final tour page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void OnContinueButton(object sender, EventArgs eventArgs)
		{
			try
			{
				FirstLimit = new Limit(LimitEntry.Text, Limit.InitialLimitDuration.TotalDays.ToString());
				await Navigation.PushAsync(new Finish());
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}
	}
}
