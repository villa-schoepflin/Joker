using System;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// View presented after opening the app for the first time.
	/// </summary>
	public partial class Welcome : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public Welcome()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Button event handler that navigates the user to the next tour page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnContinueButton(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new NameTourPage());
		}
	}
}
