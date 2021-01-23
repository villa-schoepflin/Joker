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
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public NameTourPage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Button event handler that relays user input validation and navigates the user to the next tour page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
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
