using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Joker.AppInterface;
using Joker.DataAccess;
using Joker.UserInterface;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: Preserve]
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Joker
{
	/// <summary>
	/// Main class for the shared code application.
	/// </summary>
	public class JokerApp : Application
	{
		/// <summary>
		/// Gets the app assembly for reflecting on embedded assets.
		/// </summary>
		public static readonly Assembly Assembly = typeof(JokerApp).Assembly;

		/// <summary>
		/// The locale or culture setting to use for this app.
		/// </summary>
		internal static readonly CultureInfo Locale = new("de-DE");

		/// <summary>
		/// Entry point for the shared code application.
		/// </summary>
		/// <param name="baseDirectory">The directory for files concerning the app directly.</param>
		public JokerApp(string baseDirectory)
		{
			AppSettings.DatabaseFilePath = Path.Combine(baseDirectory, "database.sqlite");
		}

		/// <summary>
		/// Directs the user to the regular main page if the most recent limit hasn't expired yet, otherwise directs
		/// them to the page where they can add a new limit.
		/// </summary>
		internal static void SetMainPageToDefault()
		{
			if(DateTime.UtcNow < AppSettings.LimitExpiredTime)
				Current.MainPage = new NavigationPage(new MainPage());
			else
				Current.MainPage = new AddLimitPage();
		}

		/// <summary>
		/// Entry point from the user's perspective.
		/// </summary>
		protected override void OnStart()
		{
			VersionTracking.Track();

			if(AppSettings.WelcomeTourCompleted)
				DefaultStart();
			else
				MainPage = new NavigationPage(new Welcome())
				{
					BarBackgroundColor = Styles.Primary1,
					BarTextColor = Styles.TextContrast
				};
		}

		/// <summary>
		/// Entry point if the welcome tour has been completed.
		/// </summary>
		private void DefaultStart()
		{
			// Deletes a picture from the database if it was removed in an update.
			string[] picFiles = Assembly.GetManifestResourceNames();
			foreach(var pic in Database.AllPictures())
				if(!picFiles.Contains(Folders.PictureAssets + pic.FilePath))
					Database.Delete(pic);

			if(DateTime.UtcNow >= AppSettings.NewPictureTime)
			{
				bool picsNotDepleted = Database.InsertPictureFromRandomAsset();
				if(picsNotDepleted)
				{
					AppSettings.NewPictureTime = DateTime.UtcNow + UserSettings.NewPictureInterval;
					var notifier = DependencyService.Get<IPlatformNotifier>();
					notifier.ScheduleNewPicture(AppSettings.NewPictureTime);
				}
			}

			if(AppSettings.UserPasswordIsSet)
				MainPage = new NavigationPage(new PasswordPage());
			else
				SetMainPageToDefault();
		}
	}
}
