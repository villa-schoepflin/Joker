using System;

using Android.App;
using Android.Content;

using Joker.ApplicationLayer;
using Joker.DataAccess;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.Droid.AndroidNotifier))]
namespace Joker.Droid
{
	/// <summary>
	/// Contains Android-specific notification functionality.
	/// </summary>
	public class AndroidNotifier : IPlatformNotifier
	{
		/// <summary>
		/// Gets the app-specific Android alarm manager to schedule broadcasts and notifications.
		/// </summary>
		private static AlarmManager Scheduler
			=> (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);

		/// <summary>
		/// Gets the app-specific Android notification manager to create push notifications on the phone.
		/// </summary>
		private static NotificationManager Notifier
			=> (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

		/// <summary>
		/// Android-specific implementation of an API method that schedules a notification
		/// indicating the current limit has expired and a new one should be set.
		/// </summary>
		/// <param name="timeSetting">The time at which the notification should appear.</param>
		public void ScheduleLimitExpired(DateTime timeSetting)
		{
			var broadcast = PendingIntent.GetBroadcast(Application.Context,
				(int)PNType.LimitExpired,
				new Intent(Application.Context, typeof(LimitExpiredReceiver)),
				PendingIntentFlags.UpdateCurrent);

			Scheduler.Set(AlarmType.RtcWakeup, GetSystemTime(timeSetting), broadcast);
		}

		/// <summary>
		/// Android-specific implementation of an API method that removes the notification that
		/// indicates that the current limit has expired.
		/// </summary>
		public void CancelLimitExpired()
		{
			Notifier.Cancel((int)PNType.LimitExpired);
		}

		/// <summary>
		/// Handles the notification broadcast received from the Android OS at the scheduled time.
		/// </summary>
		[BroadcastReceiver]
		internal class LimitExpiredReceiver : BroadcastReceiver
		{
			/// <summary>
			/// Receives the broadcast from Android and pushes an ongoing notification indicating that the
			/// current limit has expired. Creates the respective notification channel if necessary.
			/// </summary>
			/// <param name="context">The context through which the broadcast is processed.</param>
			/// <param name="intent">Not used here.</param>
			public override void OnReceive(Context context, Intent intent)
			{
				var channel = new NotificationChannel(PNType.LimitExpired.ToString(),
					"Limit-Benachrichtigungen",
					NotificationImportance.Max);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				Notifier.CreateNotificationChannel(channel);

				var launch = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(LaunchActivity)), 0);
				var notifBuilder = new Notification.Builder(context, PNType.LimitExpired.ToString())
					.SetContentTitle(PNContent.Title.LimitExpired)
					.SetContentText(PNContent.Body.LimitExpired)
					.SetSmallIcon(Resource.Drawable.app_icon)
					.SetStyle(new Notification.BigTextStyle().BigText(PNContent.Body.LimitExpired))
					.SetOngoing(true)
					.SetAutoCancel(false)
					.SetContentIntent(launch);
				Notifier.Notify((int)PNType.LimitExpired, notifBuilder.Build());
			}
		}

		/// <summary>
		/// Android-specific implementation of an API method that schedules a notification
		/// indicating that a new picture is available to see.
		/// </summary>
		/// <param name="timeSetting">The time at which the notification should appear.</param>
		public void ScheduleNewPicture(DateTime timeSetting)
		{
			var broadcast = PendingIntent.GetBroadcast(Application.Context,
				(int)PNType.NewPicture,
				new Intent(Application.Context, typeof(NewPictureReceiver)),
				PendingIntentFlags.UpdateCurrent);

			Scheduler.Set(AlarmType.RtcWakeup, GetSystemTime(timeSetting), broadcast);
		}

		/// <summary>
		/// Handles the notification broadcast received from the Android OS at the scheduled time.
		/// </summary>
		[BroadcastReceiver]
		internal class NewPictureReceiver : BroadcastReceiver
		{
			/// <summary>
			/// Receives the broadcast from Android and pushes an ongoing notification indicating that a
			/// new pictures is available to see. Creates the respective notification channel if necessary.
			/// The notification is removed when it is opened.
			/// </summary>
			/// <param name="context">The context through which the broadcast is processed.</param>
			/// <param name="intent">Not used here.</param>
			public override void OnReceive(Context context, Intent intent)
			{
				var channel = new NotificationChannel(PNType.NewPicture.ToString(),
					"Bilderbenachrichtigungen",
					NotificationImportance.High);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				Notifier.CreateNotificationChannel(channel);

				var launch = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(LaunchActivity)), 0);
				var notifBuilder = new Notification.Builder(context, PNType.NewPicture.ToString())
					.SetContentTitle(PNContent.Title.NewPicture)
					.SetContentText(PNContent.Body.NewPicture)
					.SetSmallIcon(Resource.Drawable.app_icon)
					.SetStyle(new Notification.BigTextStyle().BigText(PNContent.Body.NewPicture))
					.SetAutoCancel(true)
					.SetOngoing(true)
					.SetContentIntent(launch);
				Notifier.Notify((int)PNType.NewPicture, notifBuilder.Build());
			}
		}

		/// <summary>
		/// Android-specific implementation of an API method that schedules a notification
		/// reminding the user to always record acts of gambling within the app.
		/// </summary>
		/// <param name="interval">The interval for the times the notification should appear.</param>
		public void ScheduleGambleReminder(TimeSpan interval)
		{
			var broadcast = PendingIntent.GetBroadcast(Application.Context,
				(int)PNType.GambleReminder,
				new Intent(Application.Context, typeof(GambleReminderReceiver)),
				PendingIntentFlags.UpdateCurrent);

			Scheduler.SetRepeating(AlarmType.RtcWakeup,
				GetSystemTime(DateTime.UtcNow + interval),
				(long)interval.TotalMilliseconds,
				broadcast);
		}

		/// <summary>
		/// Handles the notification broadcast received from the Android OS at the scheduled time.
		/// </summary>
		[BroadcastReceiver]
		internal class GambleReminderReceiver : BroadcastReceiver
		{
			/// <summary>
			/// Receives the broadcast from Android and pushes a notification reminding the user that
			/// their acts of gambling should be recorded with the app. Creates the respective notification
			/// channel if necessary.
			/// </summary>
			/// <param name="context">The context through which the broadcast is processed.</param>
			/// <param name="intent">Not used here.</param>
			public override void OnReceive(Context context, Intent intent)
			{
				var channel = new NotificationChannel(PNType.GambleReminder.ToString(),
					"Spieleinsatz-Erinnerungen",
					NotificationImportance.Default);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				Notifier.CreateNotificationChannel(channel);

				var launch = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(LaunchActivity)), 0);
				var notifBuilder = new Notification.Builder(context, PNType.GambleReminder.ToString())
					.SetContentTitle(PNContent.Title.GambleReminder)
					.SetContentText(PNContent.Body.GambleReminder)
					.SetSmallIcon(Resource.Drawable.app_icon)
					.SetStyle(new Notification.BigTextStyle().BigText(PNContent.Body.GambleReminder))
					.SetAutoCancel(true)
					.SetContentIntent(launch);
				Notifier.Notify((int)PNType.GambleReminder, notifBuilder.Build());
			}
		}

		/// <summary>
		/// Android-specific implementation of an API method that schedules a notification
		/// reminding the user about their current limit's state.
		/// </summary>
		/// <param name="interval">The interval for the times the notification should appear.</param>
		public void ScheduleLimitReminder(TimeSpan interval)
		{
			var broadcast = PendingIntent.GetBroadcast(Application.Context,
				(int)PNType.LimitReminder,
				new Intent(Application.Context, typeof(LimitReminderReceiver)),
				PendingIntentFlags.UpdateCurrent);

			Scheduler.SetRepeating(AlarmType.RtcWakeup,
				GetSystemTime(DateTime.UtcNow + interval),
				(long)interval.TotalMilliseconds,
				broadcast);
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
				var channel = new NotificationChannel(PNType.LimitReminder.ToString(),
					"Limit-Erinnerungen",
					NotificationImportance.Default);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				Notifier.CreateNotificationChannel(channel);

				var launch = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(LaunchActivity)), 0);
				var notifBuilder = new Notification.Builder(context, PNType.LimitReminder.ToString())
					.SetContentTitle(PNContent.Title.LimitReminder)
					.SetContentText(PNContent.Body.LimitReminder)
					.SetSmallIcon(Resource.Drawable.app_icon)
					.SetStyle(new Notification.BigTextStyle().BigText(PNContent.Body.LimitReminder))
					.SetAutoCancel(true)
					.SetContentIntent(launch);
				Notifier.Notify((int)PNType.LimitReminder, notifBuilder.Build());
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
					var notifier = new AndroidNotifier();
					notifier.ScheduleLimitExpired(AppSettings.LimitExpiredTime);
					notifier.ScheduleNewPicture(AppSettings.NewPictureTime);
					notifier.ScheduleGambleReminder(UserSettings.GambleReminderInterval);
					notifier.ScheduleLimitReminder(UserSettings.LimitReminderInterval);
				}
			}
		}

		/// <summary>
		/// Converts a C# DateTime object into an Android-specific time format as represented by
		/// a 64-bit integer.
		/// </summary>
		/// <param name="time">The DateTime object to be converted.</param>
		/// <returns>A 64-bit integer representing a point in time.</returns>
		private static long GetSystemTime(DateTime time)
		{
			return (long)(time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		}
	}
}