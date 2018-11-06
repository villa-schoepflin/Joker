using Joker.DataAccess;

namespace Joker.AppInterface
{
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

	/// <summary>
	/// Container class for constructing the text shown in the different kinds of push notifications (PN).
	/// </summary>
	public static class PNContent
	{
		/// <summary>
		/// Struct holding the titles of each kind of notification.
		/// </summary>
		public struct Title
		{
			/// <summary>
			/// Title of the notification indicating that the current has limit has expired.
			/// </summary>
			public static string LimitExpired => FileResourceReader.Get("Notification_Title_LimitExpired.txt");

			/// <summary>
			/// Title of the notification indicating that a new picture is available to see.
			/// </summary>
			public static string NewPicture => FileResourceReader.Get("Notification_Title_NewPicture.txt");

			/// <summary>
			/// Title of the notification reminding the user to always record his acts of gambling.
			/// </summary>
			public static string GambleReminder => FileResourceReader.Get("Notification_Title_GambleReminder.txt");

			/// <summary>
			/// Title of the notification reminding the user about their limit's current state.
			/// </summary>
			public static string LimitReminder => FileResourceReader.Get("Notification_Title_LimitReminder.txt");
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
						return FileResourceReader.Get("Notification_Body_LimitExpired_Success.txt");
					else
						return FileResourceReader.Get("Notification_Body_LimitExpired_Failure.txt");
				}
			}

			/// <summary>
			/// Main text of the notification indicating that a new picture is available to see.
			/// </summary>
			public static string NewPicture => FileResourceReader.Get("Notification_Body_NewPicture.txt");

			/// <summary>
			/// Main text of the notification reminding the user to always record his acts of gambling.
			/// </summary>
			public static string GambleReminder => FileResourceReader.Get("Notification_Body_GambleReminder.txt");

			/// <summary>
			/// Main text of the notification reminding the user about their limit's current state.
			/// </summary>
			public static string LimitReminder
			{
				get
				{
					if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
						return FileResourceReader.Get("Notification_Body_LimitReminder_Success.txt");
					else
						return FileResourceReader.Get("Notification_Body_LimitReminder_Failure.txt");
				}
			}
		}
	}
}