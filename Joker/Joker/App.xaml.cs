using System;
using System.Globalization;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.ApplicationLayer;
using Joker.DataAccess;
using Joker.UserInterface;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Joker
{
	/// <summary>
	/// Main class for the shared code application.
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// Entry point for the shared code application.
		/// </summary>
		/// <param name="baseDirectory">The directory for files concerning the app directly.</param>
		public App(string baseDirectory)
		{
			InitializeComponent();
			AppSettings.DBFilePath = Path.Combine(baseDirectory, "database.sqlite");
		}

		/// <summary>
		/// Entry point from the user's perspective.
		/// </summary>
		protected override void OnStart()
		{
			if(AppSettings.WelcomeTourCompleted)
			{
				/* If the app was opened when there is a new picture to display, insert it and re-schedule the time 
				 * and notification for when to insert the next one. */
				if(DateTime.UtcNow >= AppSettings.NewPictureTime && Database.InsertPictureFromRandomResource())
				{
					AppSettings.NewPictureTime = DateTime.UtcNow + UserSettings.NewPictureInterval;
					DependencyService.Get<IPlatformNotifier>().ScheduleNewPicture(AppSettings.NewPictureTime);
				}

				/* If the most recent limit has not expired yet, direct the user to the regular main page, otherwise
				 * direct them to the page where they can add a new limit and then return to the usual main page. */
				if(DateTime.UtcNow < AppSettings.LimitExpiredTime)
					SetMainPageToDefault();
				else
					MainPage = new AddLimitPage();
			}
			else
				MainPage = new NavigationPage(new Welcome())
				{
					BarBackgroundColor = Color("Primary1"),
					BarTextColor = Color("TextContrast")
				};
		}

		/// <summary>
		/// Directs the user to the main view of the app, clearing the navigation stack.
		/// </summary>
		internal static void SetMainPageToDefault()
		{
			Current.MainPage = new NavigationPage(new MainPage());
		}

		/// <summary>
		/// References the current main page as an instance of the app-specific MainPage class.
		/// </summary>
		internal static MainPage CurrentMainPage => (MainPage)((NavigationPage)Current.MainPage).RootPage;

		/// <summary>
		/// References the current main page's timeline feed page.
		/// </summary>
		internal static TimelineFeed CurrentTimelineFeed => (TimelineFeed)CurrentMainPage.Children[1];

		/// <summary>
		/// References the current main page's contact page.
		/// </summary>
		internal static ContactPage CurrentContactPage => (ContactPage)CurrentMainPage.Children[2];

		/// <summary>
		/// The locale or culture setting to use for this app.
		/// </summary>
		internal static readonly CultureInfo Locale = new CultureInfo("de-DE");

		/// <summary>
		/// Wrapper function to return a color from the application resource dictionary.
		/// </summary>
		/// <param name="key">The key for the dictionary resource.</param>
		/// <returns>A Xamarin.Forms color from the dictionary.</returns>
		public static Color Color(string key)
		{
			return (Color)Current.Resources[key];
		}
	}
}