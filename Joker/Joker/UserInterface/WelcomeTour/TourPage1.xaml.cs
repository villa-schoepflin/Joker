using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user should enter their name.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TourPage1 : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public TourPage1()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Button event handler that relays user input validation and navigates the user to
		/// the next tour page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnContinueButton(object sender, EventArgs e)
		{
			try
			{
				UserSettings.UserName = NameEntry.Text;
				await Navigation.PushAsync(new TourPage2());
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
			}
		}
	}
}