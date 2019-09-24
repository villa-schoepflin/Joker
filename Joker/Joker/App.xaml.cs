using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.AppInterface;
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
			AppSettings.DatabaseFilePath = Path.Combine(baseDirectory, "database.sqlite");
		}

		/// <summary>
		/// Entry point from the user's perspective.
		/// </summary>
		protected override void OnStart()
		{
			VersionTracking.Track();
			if(AppSettings.WelcomeTourCompleted)
			{
				// Deletes a picture from the database if it was removed in an update.
				string[] picFiles = typeof(App).Assembly.GetManifestResourceNames();
				foreach(var pic in Database.AllPictures())
					if(!picFiles.Contains($"Joker.Resources.PictureFeed.{pic.FilePath}"))
						Database.Delete(pic);

				/* If the app was opened when there is a new picture to display, insert it and re-schedule the time 
				 * and notification for when to insert the next one. */
				if(DateTime.UtcNow >= AppSettings.NewPictureTime && Database.InsertPictureFromRandomResource())
				{
					AppSettings.NewPictureTime = DateTime.UtcNow + UserSettings.NewPictureInterval;
					DependencyService.Get<IPlatformNotifier>().ScheduleNewPicture(AppSettings.NewPictureTime);
				}

				if(AppSettings.UserPasswordIsSet)
					MainPage = new NavigationPage(new PasswordPage());
				else
					SetMainPageToDefault();
			}
			else
				MainPage = new NavigationPage(new Welcome())
				{
					BarBackgroundColor = Color("Primary1"),
					BarTextColor = Color("TextContrast")
				};
		}

		/// <summary>
		/// Directs the user to the regular main page if the most recent limit hasn't expired yet, otherwise
		/// directs them to the page where they can add a new limit.
		/// </summary>
		internal static void SetMainPageToDefault()
		{
			if(DateTime.UtcNow < AppSettings.LimitExpiredTime)
				Current.MainPage = new NavigationPage(new MainPage());
			else
				Current.MainPage = new AddLimitPage();
		}

		/// <summary>
		/// The locale or culture setting to use for this app.
		/// </summary>
		internal static readonly CultureInfo Locale = new CultureInfo("de-DE");

		/// <summary>
		/// References the current main page's timeline feed page.
		/// </summary>
		internal static PictureFeed CurrentPictureFeed => (PictureFeed)CurrentMainPageFromStack.Children[0];

		/// <summary>
		/// References the current main page's timeline feed page.
		/// </summary>
		internal static TimelineFeed CurrentTimelineFeed => (TimelineFeed)CurrentMainPageFromStack.Children[1];

		/// <summary>
		/// References the current main page's contact page.
		/// </summary>
		internal static ContactPage CurrentContactPage => (ContactPage)CurrentMainPageFromStack.Children[2];

		/// <summary>
		/// References the current main page as an instance of the app-specific MainPage class.
		/// </summary>
		private static MainPage CurrentMainPageFromStack => (MainPage)((NavigationPage)Current.MainPage).RootPage;

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