using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Joker.UserInterface
{
	/// <summary>
	/// Main view of the app, usually presented after launching is finished.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage
	{
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public MainPage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Changes the navigation page's title based on which tab is currently displayed.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void OnTabChanged(object sender, EventArgs e)
		{
			if(CurrentPage is PictureFeed)
				Title = PictureFeed.Headline;
			else if(CurrentPage is TimelineFeed)
				Title = TimelineFeed.Headline;
			else if(CurrentPage is ContactPage)
				Title = ContactPage.Headline;
		}

		/// <summary>
		/// Toolbar item event handler that shows the user a message concerning the current tab's functionality.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnInfoButton(object sender, EventArgs e)
		{
			if(CurrentPage is PictureFeed)
				await DisplayAlert(PictureFeed.Headline, PictureFeed.Info, "Ok");
			else if(CurrentPage is TimelineFeed)
				await DisplayAlert(TimelineFeed.Headline, TimelineFeed.Info, "Ok");
			else if(CurrentPage is ContactPage)
				await DisplayAlert(ContactPage.Headline, ContactPage.Info, "Ok");
		}

		/// <summary>
		/// Toolbar item event handler that navigates the user to the settings page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnSettingsButton(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new SettingsPage());
		}
	}
}