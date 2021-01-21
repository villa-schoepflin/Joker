using System;
using Joker.AppInterface;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Final view of the welcome tour that also contains essential setup logic.
	/// </summary>
	public partial class Finish : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements and receives the first limit for further processing.
		/// </summary>
		public Finish()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Button event handler that finalizes the setup for the app and navigates the user to the main page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private void OnContinueButton(object sender, EventArgs eventArgs)
		{
			Indicator.IsRunning = true;
			Database.Initialize();

			// Inserts the optional first contact
			if(ContactTourPage.FirstContact is object)
				Database.Insert(ContactTourPage.FirstContact);

			/* Inserts the first limit, sets the persistent time setting for when it expires and
			 * schedules the corresponding push notification. */
			Database.Insert(LimitTourPage.FirstLimit);
			AppSettings.LimitExpiredTime = LimitTourPage.FirstLimit.Time + LimitTourPage.FirstLimit.Duration;
			DependencyService.Get<IPlatformNotifier>().ScheduleLimitExpired(AppSettings.LimitExpiredTime);

			/* Inserts the first pictures into the database for the picture feed, sets the
			 * persistent time setting for when a new picture should be inserted and alerted to the
			 * user and schedules the corresponding push notification. */
			for(int i = 0; i < PictureFeed.InitialPictureAmount; i++)
				Database.InsertPictureFromRandomAsset();
			AppSettings.NewPictureTime = DateTime.UtcNow + UserSettings.NewPictureInterval;
			DependencyService.Get<IPlatformNotifier>().ScheduleNewPicture(AppSettings.NewPictureTime);

			// Schedules reminder notifications.
			DependencyService.Get<IPlatformNotifier>().ScheduleGambleReminder(UserSettings.GambleReminderInterval);
			DependencyService.Get<IPlatformNotifier>().ScheduleLimitReminder(UserSettings.LimitReminderInterval);

			// Completes the welcome tour and redirects the user to the main page.
			AppSettings.WelcomeTourCompleted = true;
			App.SetMainPageToDefault();
		}
	}
}
