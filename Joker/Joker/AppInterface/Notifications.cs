using Joker.DataAccess;

namespace Joker.AppInterface
{
	/// <summary>
	/// Container class for constructing the text shown in different kinds of push notifications.
	/// </summary>
	public static class Notifications
	{
		/// <summary>
		/// Struct holding the titles of each kind of notification.
		/// </summary>
		public struct Title
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
		/// Struct holding the main texts of each kind of notification.
		/// </summary>
		public struct Body
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
	/// The different kinds of push notifications (PN) sent by the application.
	/// Also used to supply IDs to each notification and related properties.
	/// </summary>
	public enum PNType
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
