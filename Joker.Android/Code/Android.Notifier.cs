using System;
using Android.App;
using Android.Content;
using Joker.AppInterface;
using Joker.DataAccess;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.Android.Notifier))]
namespace Joker.Android
{
	internal class Notifier : IPlatformNotifier
	{
		private static AlarmManager AlarmService
			=> (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);

		private static NotificationManager NotifService
			=> (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

		public void ScheduleLimitExpired(DateTime timeSetting)
		{
			Intent intent = new(Application.Context, typeof(LimitExpiredReceiver));
			var broadcast = PendingIntent.GetBroadcast(Application.Context, (int)NotificationType.LimitExpired, intent,
				PendingIntentFlags.UpdateCurrent);

			AlarmService.Set(AlarmType.RtcWakeup, GetSystemTime(timeSetting), broadcast);
		}

		public void CancelLimitExpired()
		{
			NotifService.Cancel((int)NotificationType.LimitExpired);
		}

		public void ScheduleNewPicture(DateTime timeSetting)
		{
			Intent intent = new(Application.Context, typeof(NewPictureReceiver));
			var broadcast = PendingIntent.GetBroadcast(Application.Context, (int)NotificationType.NewPicture, intent,
				PendingIntentFlags.UpdateCurrent);

			AlarmService.Set(AlarmType.RtcWakeup, GetSystemTime(timeSetting), broadcast);
		}

		public void ScheduleGambleReminder(TimeSpan interval)
		{
			Intent intent = new(Application.Context, typeof(GambleReminderReceiver));
			var broadcast = PendingIntent.GetBroadcast(Application.Context, (int)NotificationType.GambleReminder,
				intent, PendingIntentFlags.UpdateCurrent);

			long time = GetSystemTime(DateTime.UtcNow + interval);
			AlarmService.SetRepeating(AlarmType.RtcWakeup, time, (long)interval.TotalMilliseconds, broadcast);
		}

		public void ScheduleLimitReminder(TimeSpan interval)
		{
			Intent intent = new(Application.Context, typeof(LimitReminderReceiver));
			var broadcast = PendingIntent.GetBroadcast(Application.Context, (int)NotificationType.LimitReminder, intent,
				PendingIntentFlags.UpdateCurrent);

			long time = GetSystemTime(DateTime.UtcNow + interval);
			AlarmService.SetRepeating(AlarmType.RtcWakeup, time, (long)interval.TotalMilliseconds, broadcast);
		}

		[BroadcastReceiver]
		private class LimitExpiredReceiver : BroadcastReceiver
		{
			public override void OnReceive(Context context, Intent intent)
			{
				string id = NotificationType.LimitExpired.ToString();
				NotificationChannel channel = new(id, Notifications.Channel.LimitExpired, NotificationImportance.Max);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				NotifService.CreateNotificationChannel(channel);

				Intent launchIntent = new(context, typeof(LaunchActivity));
				var launch = PendingIntent.GetActivity(context, 0, launchIntent, 0);
				var notifBuilder = new Notification.Builder(context, id)
					.SetContentTitle(Notifications.Title.LimitExpired)
					.SetContentText(Notifications.Body.LimitExpired)
					.SetSmallIcon(Resource.Drawable.ui_icon)
					.SetStyle(new Notification.BigTextStyle().BigText(Notifications.Body.LimitExpired))
					.SetOngoing(true)
					.SetAutoCancel(false)
					.SetContentIntent(launch);
				NotifService.Notify((int)NotificationType.LimitExpired, notifBuilder.Build());
			}
		}

		[BroadcastReceiver]
		private class NewPictureReceiver : BroadcastReceiver
		{
			public override void OnReceive(Context context, Intent intent)
			{
				string id = NotificationType.NewPicture.ToString();
				NotificationChannel channel = new(id, Notifications.Channel.NewPicture, NotificationImportance.Max);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				NotifService.CreateNotificationChannel(channel);

				Intent launchIntent = new(context, typeof(LaunchActivity));
				var launch = PendingIntent.GetActivity(context, 0, launchIntent, 0);
				var notifBuilder = new Notification.Builder(context, id)
					.SetContentTitle(Notifications.Title.NewPicture)
					.SetContentText(Notifications.Body.NewPicture)
					.SetSmallIcon(Resource.Drawable.ui_icon)
					.SetStyle(new Notification.BigTextStyle().BigText(Notifications.Body.NewPicture))
					.SetAutoCancel(true)
					.SetOngoing(true)
					.SetContentIntent(launch);
				NotifService.Notify((int)NotificationType.NewPicture, notifBuilder.Build());
			}
		}

		[BroadcastReceiver]
		private class GambleReminderReceiver : BroadcastReceiver
		{
			public override void OnReceive(Context context, Intent intent)
			{
				string id = NotificationType.GambleReminder.ToString();
				NotificationChannel channel = new(id, Notifications.Channel.GambleReminder, NotificationImportance.Max);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				NotifService.CreateNotificationChannel(channel);

				Intent launchIntent = new(context, typeof(LaunchActivity));
				var launch = PendingIntent.GetActivity(context, 0, launchIntent, 0);
				var notifBuilder = new Notification.Builder(context, id)
					.SetContentTitle(Notifications.Title.GambleReminder)
					.SetContentText(Notifications.Body.GambleReminder)
					.SetSmallIcon(Resource.Drawable.ui_icon)
					.SetStyle(new Notification.BigTextStyle().BigText(Notifications.Body.GambleReminder))
					.SetAutoCancel(true)
					.SetContentIntent(launch);
				NotifService.Notify((int)NotificationType.GambleReminder, notifBuilder.Build());
			}
		}

		[BroadcastReceiver]
		private class LimitReminderReceiver : BroadcastReceiver
		{
			public override void OnReceive(Context context, Intent intent)
			{
				string id = NotificationType.LimitReminder.ToString();
				NotificationChannel channel = new(id, Notifications.Channel.LimitReminder, NotificationImportance.Max);
				channel.LockscreenVisibility = NotificationVisibility.Public;
				NotifService.CreateNotificationChannel(channel);

				Intent launchIntent = new(context, typeof(LaunchActivity));
				var launch = PendingIntent.GetActivity(context, 0, launchIntent, 0);
				var notifBuilder = new Notification.Builder(context, id)
					.SetContentTitle(Notifications.Title.LimitReminder)
					.SetContentText(Notifications.Body.LimitReminder)
					.SetSmallIcon(Resource.Drawable.ui_icon)
					.SetStyle(new Notification.BigTextStyle().BigText(Notifications.Body.LimitReminder))
					.SetAutoCancel(true)
					.SetContentIntent(launch);
				NotifService.Notify((int)NotificationType.LimitReminder, notifBuilder.Build());
			}
		}

		[BroadcastReceiver]
		[IntentFilter(new[] { Intent.ActionBootCompleted }, Priority = (int)IntentFilterPriority.HighPriority)]
		private class BootReceiver : BroadcastReceiver
		{
			public override void OnReceive(Context context, Intent intent)
			{
				if(!AppSettings.WelcomeTourCompleted)
					return;

				Notifier notifier = new();
				notifier.ScheduleLimitExpired(AppSettings.LimitExpiredTime);
				notifier.ScheduleNewPicture(AppSettings.NewPictureTime);
				notifier.ScheduleGambleReminder(UserSettings.GambleReminderInterval);
				notifier.ScheduleLimitReminder(UserSettings.LimitReminderInterval);
			}
		}

		private static long GetSystemTime(DateTime time)
		{
			return (long)(time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		}
	}
}
