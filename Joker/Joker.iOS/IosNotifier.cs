using System;

using Foundation;
using UserNotifications;

using Joker.AppInterface;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.iOS.IosNotifier))]
namespace Joker.iOS
{
	/// <summary>
	/// Contains iOS-specific notification functionality.
	/// </summary>
	public class IosNotifier : IPlatformNotifier
	{
		/// <summary>
		/// iOS-specific implementation of an API method that schedules a notification
		/// indicating the current limit has expired and a new one should be set.
		/// </summary>
		/// <param name="timeSetting">The time at which the notification should appear.</param>
		public void ScheduleLimitExpired(DateTime timeSetting)
		{
			UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert,
				(approved, e) => { });

			var content = new UNMutableNotificationContent
			{
				Title = PNContent.Title.LimitExpired,
				Subtitle = "",
				Body = PNContent.Body.LimitExpired,
				Badge = 0,
			};

			var limitExpiredTime = timeSetting.ToLocalTime();
			var time = new NSDateComponents
			{
				Year = limitExpiredTime.Year,
				Month = limitExpiredTime.Month,
				Day = limitExpiredTime.Day,
				Hour = limitExpiredTime.Hour,
				Minute = limitExpiredTime.Minute,
				Second = limitExpiredTime.Second
			};
			var trigger = UNCalendarNotificationTrigger.CreateTrigger(time, false);

			var req = UNNotificationRequest.FromIdentifier(PNType.LimitExpired.ToString(), content, trigger);
			UNUserNotificationCenter.Current.AddNotificationRequest(req, null);
		}

		/// <summary>
		/// iOS-specific implementation of an API method that removes the notification that
		/// indicates that the current limit has expired.
		/// </summary>
		public void CancelLimitExpired()
		{
			UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new[] { PNType.LimitExpired.ToString() });
		}

		/// <summary>
		/// iOS-specific implementation of an API method that schedules a notification
		/// indicating that a new picture is available to see.
		/// </summary>
		/// <param name="timeSetting">The time at which the notification should appear.</param>
		public void ScheduleNewPicture(DateTime timeSetting)
		{
			var content = new UNMutableNotificationContent
			{
				Title = PNContent.Title.NewPicture,
				Subtitle = "",
				Body = PNContent.Body.NewPicture,
				Badge = 0,
			};

			var newPictureTime = timeSetting.ToLocalTime();
			var time = new NSDateComponents
			{
				Year = newPictureTime.Year,
				Month = newPictureTime.Month,
				Day = newPictureTime.Day,
				Hour = newPictureTime.Hour,
				Minute = newPictureTime.Minute,
				Second = newPictureTime.Second
			};
			var trigger = UNCalendarNotificationTrigger.CreateTrigger(time, false);

			var req = UNNotificationRequest.FromIdentifier(PNType.NewPicture.ToString(), content, trigger);
			UNUserNotificationCenter.Current.AddNotificationRequest(req, null);
		}

		/// <summary>
		/// iOS-specific implementation of an API method that schedules a notification
		/// reminding the user to always record acts of gambling within the app.
		/// </summary>
		/// <param name="interval">The interval for the times the notification should appear.</param>
		public void ScheduleGambleReminder(TimeSpan interval)
		{
			var content = new UNMutableNotificationContent
			{
				Title = PNContent.Title.GambleReminder,
				Subtitle = "",
				Body = PNContent.Body.GambleReminder,
				Badge = 0,
			};

			var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(interval.TotalSeconds, true);
			var req = UNNotificationRequest.FromIdentifier(PNType.GambleReminder.ToString(), content, trigger);
			UNUserNotificationCenter.Current.AddNotificationRequest(req, null);
		}

		/// <summary>
		/// iOS-specific implementation of an API method that schedules a notification
		/// reminding the user about the state of their current limit.
		/// </summary> 
		/// <param name="interval">The interval for the times the notification should appear.</param>
		public void ScheduleLimitReminder(TimeSpan interval)
		{
			var content = new UNMutableNotificationContent
			{
				Title = PNContent.Title.LimitReminder,
				Subtitle = "",
				Body = PNContent.Body.LimitReminder,
				Badge = 0,
			};

			var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(interval.TotalSeconds, true);
			var req = UNNotificationRequest.FromIdentifier(PNType.LimitReminder.ToString(), content, trigger);
			UNUserNotificationCenter.Current.AddNotificationRequest(req, null);
		}
	}
}