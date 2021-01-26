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
namespace Joker.AppInterface
{
	/// <summary>
	/// Main class for the shared code application.
	/// </summary>
	public class App : Application
	{
		/// <summary>
		/// For reflecting on embedded assets.
		/// </summary>
		public static readonly Assembly Assembly = typeof(App).Assembly;

		internal static readonly CultureInfo Locale = new("de-DE");

		/// <summary>
		/// Entry point for the shared code application.
		/// </summary>
		/// <param name="baseDirectory">The directory for files concerning the app directly.</param>
		public App(string baseDirectory)
		{
			AppSettings.DatabaseFilePath = Path.Combine(baseDirectory, "database.sqlite");
		}

		/// <summary>
		/// Directs the user to the regular main page if the most recent limit hasn't expired yet, otherwise directs
		/// them to the page where they can add a new limit.
		/// </summary>
		internal static void RequestMainPage()
		{
			if(DateTime.UtcNow < AppSettings.LimitExpiredTime)
				Current.MainPage = new NavigationPage(new MainPage());
			else
				Current.MainPage = new LimitCreator();
		}

		/// <summary>
		/// Entry point from the user's perspective.
		/// </summary>
		protected override void OnStart()
		{
			VersionTracking.Track();

			if(AppSettings.WelcomeTourCompleted)
				StartOnTourCompleted();
			else
				StartWithWelcomeTour();
		}

		/// <summary>
		/// Entry point if the welcome tour has been completed.
		/// </summary>
		private void StartOnTourCompleted()
		{
			CheckForDeletedPictures();
			CheckForNewPicture();

			if(AppSettings.UserPasswordIsSet)
				MainPage = new NavigationPage(new PasswordPage());
			else
				RequestMainPage();
		}

		/// <summary>
		/// Deletes a picture from the database if it was removed in an update.
		/// </summary>
		private void CheckForDeletedPictures()
		{
			string[] picFiles = Assembly.GetManifestResourceNames();
			foreach(var pic in Database.AllPictures())
				if(!picFiles.Contains(Folders.PictureAssets + pic.FilePath))
					Database.Delete(pic);
		}

		/// <summary>
		/// Inserts a new picture into the database and schedules the next notification for a new picture if necessary.
		/// </summary>
		private void CheckForNewPicture()
		{
			if(DateTime.UtcNow < AppSettings.NewPictureTime)
				return;

			bool picsNotDepleted = Database.InsertPictureFromRandomAsset();
			if(picsNotDepleted)
			{
				AppSettings.NewPictureTime = DateTime.UtcNow + UserSettings.NewPictureInterval;
				var notifier = DependencyService.Get<IPlatformNotifier>();
				notifier.ScheduleNewPicture(AppSettings.NewPictureTime);
			}
		}

		/// <summary>
		/// Entry point if the app launches for the first time after installation.
		/// </summary>
		private void StartWithWelcomeTour()
		{
			MainPage = new NavigationPage(new Welcome())
			{
				BarBackgroundColor = Styles.Primary1,
				BarTextColor = Styles.TextContrast
			};
		}
	}
}
