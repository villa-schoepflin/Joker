using System;
using Joker.AppInterface;
using Joker.BusinessLogic;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Main view of the app, usually presented after launching is finished.
	/// </summary>
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
		/// <param name="eventArgs">Contains event data.</param>
		private void OnTabChanged(object sender, EventArgs eventArgs)
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
		/// <param name="eventArgs">Contains event data.</param>
		private async void OnInfoButton(object sender, EventArgs eventArgs)
		{
			if(CurrentPage is PictureFeed)
				await DisplayAlert(PictureFeed.Headline, TextAssetReader.Get("Info_PictureFeed.txt"), Text.Ok);
			else if(CurrentPage is TimelineFeed)
				await DisplayAlert(TimelineFeed.Headline, TextAssetReader.Get("Info_TimelineFeed.txt"), Text.Ok);
			else if(CurrentPage is ContactPage)
				await DisplayAlert(ContactPage.Headline, TextAssetReader.Get("Info_ContactPage.txt"), Text.Ok);
		}

		/// <summary>
		/// Toolbar item event handler that navigates the user to the settings page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void OnSettingsButton(object sender, EventArgs eventArgs)
		{
			SettingsPage settings = new(TimelineFeed.RefreshFeedback);
			await Navigation.PushAsync(settings);
		}
	}
}
