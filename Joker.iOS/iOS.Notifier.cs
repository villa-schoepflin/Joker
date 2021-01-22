using System;
using Foundation;
using Joker.AppInterface;
using UserNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.iOS.Notifier))]
namespace Joker.iOS
{
	/// <summary>
	/// Contains iOS-specific notification functionality.
	/// </summary>
	public class Notifier : IPlatformNotifier
	{
		/// <summary>
		/// Requests permission from the user to send notifications.
		/// </summary>
		public Notifier()
		{
			var notifier = UNUserNotificationCenter.Current;
			notifier.RequestAuthorization(UNAuthorizationOptions.Alert, (granted, error) => { });
		}

		/// <summary>
		/// iOS-specific implementation of an API method that schedules a notification indicating the current limit has
		/// expired and a new one should be set.
		/// </summary>
		/// <param name="timeSetting">The time at which the notification should appear.</param>
		public void ScheduleLimitExpired(DateTime timeSetting)
		{
			string id = NotificationType.LimitExpired.ToString();
			var content = new UNMutableNotificationContent
			{
				Title = Notifications.Title.LimitExpired,
				Subtitle = "",
				Body = Notifications.Body.LimitExpired,
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

			var req = UNNotificationRequest.FromIdentifier(id, content, trigger);
			UNUserNotificationCenter.Current.AddNotificationRequest(req, null);
		}

		/// <summary>
		/// iOS-specific implementation of an API method that removes the notification that indicates that the current
		/// limit has expired.
		/// </summary>
		public void CancelLimitExpired()
		{
			string[] id = new[] { NotificationType.LimitExpired.ToString() };
			UNUserNotificationCenter.Current.RemoveDeliveredNotifications(id);
		}

		/// <summary>
		/// iOS-specific implementation of an API method that schedules a notification indicating that a new picture is
		/// available to see.
		/// </summary>
		/// <param name="timeSetting">The time at which the notification should appear.</param>
		public void ScheduleNewPicture(DateTime timeSetting)
		{
			string id = NotificationType.NewPicture.ToString();
			var content = new UNMutableNotificationContent
			{
				Title = Notifications.Title.NewPicture,
				Subtitle = "",
				Body = Notifications.Body.NewPicture,
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

			var req = UNNotificationRequest.FromIdentifier(id, content, trigger);
			UNUserNotificationCenter.Current.AddNotificationRequest(req, null);
		}

		/// <summary>
		/// iOS-specific implementation of an API method that schedules a notification reminding the user to always
		/// record acts of gambling within the app.
		/// </summary>
		/// <param name="interval">The interval for the time the notification should appear.</param>
		public void ScheduleGambleReminder(TimeSpan interval)
		{
			string id = NotificationType.GambleReminder.ToString();
			var content = new UNMutableNotificationContent
			{
				Title = Notifications.Title.GambleReminder,
				Subtitle = "",
				Body = Notifications.Body.GambleReminder,
				Badge = 0,
			};
			var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(interval.TotalSeconds, true);
			var req = UNNotificationRequest.FromIdentifier(id, content, trigger);
			UNUserNotificationCenter.Current.AddNotificationRequest(req, null);
		}

		/// <summary>
		/// iOS-specific implementation of an API method that schedules a notification
		/// reminding the user about the state of their current limit.
		/// </summary> 
		/// <param name="interval">The interval for the time the notification should appear.</param>
		public void ScheduleLimitReminder(TimeSpan interval)
		{
			string id = NotificationType.LimitReminder.ToString();
			var content = new UNMutableNotificationContent
			{
				Title = Notifications.Title.LimitReminder,
				Subtitle = "",
				Body = Notifications.Body.LimitReminder,
				Badge = 0,
			};
			var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(interval.TotalSeconds, true);
			var req = UNNotificationRequest.FromIdentifier(id, content, trigger);
			UNUserNotificationCenter.Current.AddNotificationRequest(req, null);
		}
	}
}
