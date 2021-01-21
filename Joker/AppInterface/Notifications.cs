using Joker.DataAccess;

namespace Joker.AppInterface
{
	/// <summary>
	/// Container class for constructing the text shown in different kinds of push notifications.
	/// </summary>
	public static class Notifications
	{
		/// <summary>
		/// Holds the names of each notification channel.
		/// </summary>
		public static class Channel
		{
			/// <summary>
			/// Notification channel for when the current limit expires.
			/// </summary>
			public const string LimitExpired = "Limit-Benachrichtigungen";

			/// <summary>
			/// Notification channel for when a new picture is available.
			/// </summary>
			public const string NewPicture = "Bilderbenachrichtigungen";

			/// <summary>
			/// Notification channel for reminders about recording gambling-related spendings.
			/// </summary>
			public const string GambleReminder = "Spieleinsatz-Erinnerungen";

			/// <summary>
			/// Notification channel for reminders about the current limit state.
			/// </summary>
			public const string LimitReminder = "Limit-Erinnerungen";
		}

		/// <summary>
		/// Holds the titles of each kind of notification.
		/// </summary>
		public static class Title
		{
			/// <summary>
			/// Title of the notification indicating that the current has limit has expired.
			/// </summary>
			public static string LimitExpired => TextAssetReader.Get("Notification_Title_LimitExpired.txt");

			/// <summary>
			/// Title of the notification indicating that a new picture is available to see.
			/// </summary>
			public static string NewPicture => TextAssetReader.Get("Notification_Title_NewPicture.txt");

			/// <summary>
			/// Title of the notification reminding the user to always record his acts of gambling.
			/// </summary>
			public static string GambleReminder => TextAssetReader.Get("Notification_Title_GambleReminder.txt");

			/// <summary>
			/// Title of the notification reminding the user about their limit's current state.
			/// </summary>
			public static string LimitReminder => TextAssetReader.Get("Notification_Title_LimitReminder.txt");
		}

		/// <summary>
		/// Holds the main texts of each kind of notification.
		/// </summary>
		public static class Body
		{
			/// <summary>
			/// Main text of the notification indicating that the current has limit has expired.
			/// </summary>
			public static string LimitExpired
			{
				get
				{
					if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
						return TextAssetReader.Get("Notification_Body_LimitExpired_Success.txt");
					else
						return TextAssetReader.Get("Notification_Body_LimitExpired_Failure.txt");
				}
			}

			/// <summary>
			/// Main text of the notification indicating that a new picture is available to see.
			/// </summary>
			public static string NewPicture => TextAssetReader.Get("Notification_Body_NewPicture.txt");

			/// <summary>
			/// Main text of the notification reminding the user to record his acts of gambling.
			/// </summary>
			public static string GambleReminder => TextAssetReader.Get("Notification_Body_GambleReminder.txt");

			/// <summary>
			/// Main text of the notification reminding the user about their limit's current state.
			/// </summary>
			public static string LimitReminder
			{
				get
				{
					if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
						return TextAssetReader.Get("Notification_Body_LimitReminder_Success.txt");
					else
						return TextAssetReader.Get("Notification_Body_LimitReminder_Failure.txt");
				}
			}
		}
	}

	/// <summary>
	/// The different kinds of notifications sent by the application. Also used to supply IDs to each notification and
	/// related properties.
	/// </summary>
	public enum NotificationType
	{
		/// <summary>
		/// Notifies the user that his limit has expired and a new one must be set.
		/// </summary>
		LimitExpired = 0,
		/// <summary>
		/// Notifies the user that a new motivating picture has been added to the database.
		/// </summary>
		NewPicture = 1,
		/// <summary>
		/// Notifies the user to always record his spendings for gambling.
		/// </summary>
		GambleReminder = 2,
		/// <summary>
		/// Notifies the user about the current state of their limit.
		/// </summary>
		LimitReminder = 3
	}
}
