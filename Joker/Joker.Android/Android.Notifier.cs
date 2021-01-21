using System;
using Android.App;
using Android.Content;
using Joker.AppInterface;
using Joker.DataAccess;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.Android.Notifier))]
namespace Joker.Android
{
	/// <summary>
	/// Contains Android-specific notification functionality.
	/// </summary>
	public class Notifier : IPlatformNotifier
	{
		private static AlarmManager AlarmService
			=> (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);

		private static NotificationManager NotifService
			=> (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

		/// <summary>
		/// Android-specific implementation of an API method that schedules a notification indicating the current limit
		/// has expired and a new one should be set.
		/// </summary>
		/// <param name="timeSetting">The time at which the notification should appear.</param>
		public void ScheduleLimitExpired(DateTime timeSetting)
		{
			var intent = new Intent(Application.Context, typeof(LimitExpiredReceiver));
			var broadcast = PendingIntent.GetBroadcast(Application.Context, (int)NotificationType.LimitExpired, intent,
				PendingIntentFlags.UpdateCurrent);

			AlarmService.Set(AlarmType.RtcWakeup, GetSystemTime(timeSetting), broadcast);
		}

		/// <summary>
		/// Android-specific implementation of an API method that removes the notification that indicates that the
		/// current limit has expired.
		/// </summary>
		public void CancelLimitExpired()
		{
			NotifService.Cancel((int)NotificationType.LimitExpired);
		}

		/// <summary>
		/// Handles the notification broadcast received from the Android OS at the scheduled time.
		/// </summary>
		[BroadcastReceiver]
		internal class LimitExpiredReceiver : BroadcastReceiver
		{
			/// <summary>
			/// Receives the broadcast from Android and pushes an ongoing notification indicating that the current limit
			/// has expired. Creates the respective notification channel if necessary.
			/// </summary>
			/// <param name="context">The context through which the broadcast is processed.</param>
			/// <param name="intent">Not used here.</param>
			public override void OnReceive(Context context, Intent intent)
			{
				string channelId = NotificationType.LimitExpired.ToString();
				var channel = new NotificationChannel(channelId, Notifications.Channel.LimitExpired,
					NotificationImportance.Max);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				NotifService.CreateNotificationChannel(channel);

				intent = new Intent(context, typeof(LaunchActivity));
				var launch = PendingIntent.GetActivity(context, 0, intent, 0);
				var notifBuilder = new Notification.Builder(context, channelId)
					.SetContentTitle(Notifications.Title.LimitExpired)
					.SetContentText(Notifications.Body.LimitExpired)
					.SetSmallIcon(Resource.Drawable.ui_icon)
					.SetStyle(new Notification.BigTextStyle()
					.BigText(Notifications.Body.LimitExpired))
					.SetOngoing(true)
					.SetAutoCancel(false)
					.SetContentIntent(launch);
				NotifService.Notify((int)NotificationType.LimitExpired, notifBuilder.Build());
			}
		}

		/// <summary>
		/// Android-specific implementation of an API method that schedules a notification indicating that a new picture
		/// is available to see.
		/// </summary>
		/// <param name="timeSetting">The time at which the notification should appear.</param>
		public void ScheduleNewPicture(DateTime timeSetting)
		{
			var intent = new Intent(Application.Context, typeof(NewPictureReceiver));
			var broadcast = PendingIntent.GetBroadcast(Application.Context, (int)NotificationType.NewPicture, intent,
				PendingIntentFlags.UpdateCurrent);

			AlarmService.Set(AlarmType.RtcWakeup, GetSystemTime(timeSetting), broadcast);
		}

		/// <summary>
		/// Handles the notification broadcast received from the Android OS at the scheduled time.
		/// </summary>
		[BroadcastReceiver]
		internal class NewPictureReceiver : BroadcastReceiver
		{
			/// <summary>
			/// Receives the broadcast from Android and pushes an ongoing notification indicating that a new pictures is
			/// available to see. Creates the respective notification channel if necessary. The notification is removed
			/// when it is opened.
			/// </summary>
			/// <param name="context">The context through which the broadcast is processed.</param>
			/// <param name="intent">Not used here.</param>
			public override void OnReceive(Context context, Intent intent)
			{
				string channelId = NotificationType.NewPicture.ToString();
				var channel = new NotificationChannel(channelId, Notifications.Channel.NewPicture,
					NotificationImportance.High);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				NotifService.CreateNotificationChannel(channel);

				intent = new Intent(context, typeof(LaunchActivity));
				var launch = PendingIntent.GetActivity(context, 0, intent, 0);
				var notifBuilder = new Notification.Builder(context, channelId)
					.SetContentTitle(Notifications.Title.NewPicture)
					.SetContentText(Notifications.Body.NewPicture)
					.SetSmallIcon(Resource.Drawable.ui_icon)
					.SetStyle(new Notification.BigTextStyle()
					.BigText(Notifications.Body.NewPicture))
					.SetAutoCancel(true)
					.SetOngoing(true)
					.SetContentIntent(launch);
				NotifService.Notify((int)NotificationType.NewPicture, notifBuilder.Build());
			}
		}

		/// <summary>
		/// Android-specific implementation of an API method that schedules a notification reminding the user to always
		/// record acts of gambling within the app.
		/// </summary>
		/// <param name="interval">The interval for the time the notification should appear.</param>
		public void ScheduleGambleReminder(TimeSpan interval)
		{
			var intent = new Intent(Application.Context, typeof(GambleReminderReceiver));
			var broadcast = PendingIntent.GetBroadcast(Application.Context, (int)NotificationType.GambleReminder,
				intent, PendingIntentFlags.UpdateCurrent);

			long time = GetSystemTime(DateTime.UtcNow + interval);
			AlarmService.SetRepeating(AlarmType.RtcWakeup, time, (long)interval.TotalMilliseconds, broadcast);
		}

		/// <summary>
		/// Handles the notification broadcast received from the Android OS at the scheduled time.
		/// </summary>
		[BroadcastReceiver]
		internal class GambleReminderReceiver : BroadcastReceiver
		{
			/// <summary>
			/// Receives the broadcast from Android and pushes a notification reminding the user that their acts of
			/// gambling should be recorded with the app. Creates the respective notification channel if necessary.
			/// </summary>
			/// <param name="context">The context through which the broadcast is processed.</param>
			/// <param name="intent">Not used here.</param>
			public override void OnReceive(Context context, Intent intent)
			{
				string channelId = NotificationType.GambleReminder.ToString();
				var channel = new NotificationChannel(channelId, Notifications.Channel.GambleReminder,
					NotificationImportance.Default);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				NotifService.CreateNotificationChannel(channel);

				intent = new Intent(context, typeof(LaunchActivity));
				var launch = PendingIntent.GetActivity(context, 0, intent, 0);
				var notifBuilder = new Notification.Builder(context, channelId)
					.SetContentTitle(Notifications.Title.GambleReminder)
					.SetContentText(Notifications.Body.GambleReminder)
					.SetSmallIcon(Resource.Drawable.ui_icon)
					.SetStyle(new Notification.BigTextStyle()
					.BigText(Notifications.Body.GambleReminder))
					.SetAutoCancel(true)
					.SetContentIntent(launch);
				NotifService.Notify((int)NotificationType.GambleReminder, notifBuilder.Build());
			}
		}

		/// <summary>
		/// Android-specific implementation of an API method that schedules a notification reminding the user about
		/// their current limit's state.
		/// </summary>
		/// <param name="interval">The interval for the time the notification should appear.</param>
		public void ScheduleLimitReminder(TimeSpan interval)
		{
			var intent = new Intent(Application.Context, typeof(LimitReminderReceiver));
			var broadcast = PendingIntent.GetBroadcast(Application.Context, (int)NotificationType.LimitReminder, intent,
				PendingIntentFlags.UpdateCurrent);

			long time = GetSystemTime(DateTime.UtcNow + interval);
			AlarmService.SetRepeating(AlarmType.RtcWakeup, time, (long)interval.TotalMilliseconds, broadcast);
		}

		/// <summary>
		/// Handles the notification broadcast received from the Android OS at the scheduled time.
		/// </summary>
		[BroadcastReceiver]
		internal class LimitReminderReceiver : BroadcastReceiver
		{
			/// <summary>
			/// Receives the broadcast from Android and pushes a notification reminding the user about their current
			/// limit's state. Creates the respective notification channel if necessary.
			/// </summary>
			/// <param name="context">The context through which the broadcast is processed.</param>
			/// <param name="intent">Not used here.</param>
			public override void OnReceive(Context context, Intent intent)
			{
				string channelId = NotificationType.LimitReminder.ToString();
				var channel = new NotificationChannel(channelId, Notifications.Channel.LimitReminder,
					NotificationImportance.Default);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				NotifService.CreateNotificationChannel(channel);

				intent = new Intent(context, typeof(LaunchActivity));
				var launch = PendingIntent.GetActivity(context, 0, intent, 0);
				var notifBuilder = new Notification.Builder(context, channelId)
					.SetContentTitle(Notifications.Title.LimitReminder)
					.SetContentText(Notifications.Body.LimitReminder)
					.SetSmallIcon(Resource.Drawable.ui_icon)
					.SetStyle(new Notification.BigTextStyle()
					.BigText(Notifications.Body.LimitReminder))
					.SetAutoCancel(true)
					.SetContentIntent(launch);
				NotifService.Notify((int)NotificationType.LimitReminder, notifBuilder.Build());
			}
		}

		/// <summary>
		/// Handles the broadcast received from the Android OS when the phone has been turned on.
		/// </summary>
		[BroadcastReceiver]
		[IntentFilter(new[] { Intent.ActionBootCompleted }, Priority = (int)IntentFilterPriority.HighPriority)]
		internal class BootReceiver : BroadcastReceiver
		{
			/// <summary>
			/// Re-schedules all notifications after the phone has been turned on.
			/// </summary>
			/// <param name="context">Not used here.</param>
			/// <param name="intent">Not used here.</param>
			public override void OnReceive(Context context, Intent intent)
			{
				if(AppSettings.WelcomeTourCompleted)
				{
					var notifier = new Notifier();
					notifier.ScheduleLimitExpired(AppSettings.LimitExpiredTime);
					notifier.ScheduleNewPicture(AppSettings.NewPictureTime);
					notifier.ScheduleGambleReminder(UserSettings.GambleReminderInterval);
					notifier.ScheduleLimitReminder(UserSettings.LimitReminderInterval);
				}
			}
		}

		/// <summary>
		/// Converts a C# DateTime object into an Android-specific time format as represented by a 64-bit integer.
		/// </summary>
		/// <param name="time">The DateTime object to be converted.</param>
		/// <returns>A 64-bit integer representing a point in time.</returns>
		private static long GetSystemTime(DateTime time)
		{
			return (long)(time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		}
	}
}
