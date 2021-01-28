using System;
using Foundation;
using Joker.AppInterface;
using UserNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.iOS.Notifier))]
namespace Joker.iOS
{
	internal class Notifier : IPlatformNotifier
	{
		public Notifier()
		{
			var notifier = UNUserNotificationCenter.Current;
			Action<bool, NSError> callback = (granted, error) => { };
			notifier.RequestAuthorization(UNAuthorizationOptions.Alert, callback);
		}

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

		public void CancelLimitExpired()
		{
			string[] id = new[] { NotificationType.LimitExpired.ToString() };
			UNUserNotificationCenter.Current.RemoveDeliveredNotifications(id);
		}

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
