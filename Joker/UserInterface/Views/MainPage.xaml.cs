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
		internal MainPage()
		{
			InitializeComponent();
		}

		private void OnTabChanged(object sender, EventArgs eventArgs)
		{
			if(CurrentPage is PictureFeed)
				Title = PictureFeed.Headline;
			else if(CurrentPage is TimelineFeed)
				Title = TimelineFeed.Headline;
			else if(CurrentPage is ContactFeed)
				Title = ContactFeed.Headline;
		}

		private async void OnInfoButton(object sender, EventArgs eventArgs)
		{
			if(CurrentPage is PictureFeed)
				await DisplayAlert(PictureFeed.Headline, TextAssetReader.Get("Info_PictureFeed.txt"), Text.Ok);
			else if(CurrentPage is TimelineFeed)
				await DisplayAlert(TimelineFeed.Headline, TextAssetReader.Get("Info_TimelineFeed.txt"), Text.Ok);
			else if(CurrentPage is ContactFeed)
				await DisplayAlert(ContactFeed.Headline, TextAssetReader.Get("Info_ContactPage.txt"), Text.Ok);
		}

		private async void OnSettingsButton(object sender, EventArgs eventArgs)
		{
			if(Navigation.HasPage<SettingsPage>())
				return;

			SettingsPage settings = new(TimelineFeed.RefreshFeedback);
			await Navigation.PushAsync(settings);
		}
	}
}
