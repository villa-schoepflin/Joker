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
		internal Finish()
		{
			InitializeComponent();
		}

		private void OnContinueButton(object sender, EventArgs eventArgs)
		{
			Indicator.IsRunning = true;
			Database.CreateTables();

			if(ContactTourPage.FirstContact is object)
				Database.Insert(ContactTourPage.FirstContact);

			/* Inserts the first limit, sets the persistent time setting for when it expires and schedules the
			 * corresponding push notification. */
			Database.Insert(LimitTourPage.FirstLimit);
			AppSettings.LimitExpiredTime = LimitTourPage.FirstLimit.Time + LimitTourPage.FirstLimit.Duration;

			var notifier = DependencyService.Get<IPlatformNotifier>();
			notifier.ScheduleLimitExpired(AppSettings.LimitExpiredTime);

			/* Inserts the first pictures into the database for the picture feed, sets the time setting for when a new
			 * picture should be inserted and alerted to the user and schedules the corresponding notification. */
			for(int i = 0; i < PictureFeed.InitialPictureAmount; i++)
				_ = Database.InsertPictureFromRandomAsset();
			AppSettings.NewPictureTime = DateTime.UtcNow + UserSettings.NewPictureInterval;
			notifier.ScheduleNewPicture(AppSettings.NewPictureTime);

			notifier.ScheduleGambleReminder(UserSettings.GambleReminderInterval);
			notifier.ScheduleLimitReminder(UserSettings.LimitReminderInterval);

			AppSettings.WelcomeTourCompleted = true;
			App.RequestMainPage();
		}
	}
}
