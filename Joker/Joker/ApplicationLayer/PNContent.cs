using Joker.DataAccess;

namespace Joker.ApplicationLayer
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
			public static string LimitExpired => "Zeit für ein neues Limit!";

			/// <summary>
			/// Title of the notification indicating that a new picture is available to see.
			/// </summary>
			public static string NewPicture => "Es gibt ein neues Bild für Dich!";

			/// <summary>
			/// Title of the notification reminding the user to always record his acts of gambling.
			/// </summary>
			public static string GambleReminder => $"{UserSettings.UserName}, hast Du in letzter Zeit gespielt?";

			/// <summary>
			/// Title of the notification reminding the user about their limit's current state.
			/// </summary>
			public static string LimitReminder => $"Denk an Dein Limit, {UserSettings.UserName}!";
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
					string s = $"Hey {UserSettings.UserName}, ";
					if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
						s += "Du hast Dein Limit dieses Mal eingehalten. Sehr gut!";
					else
						s += "leider hast Du Dein Limit diesmal überschritten.";
					return s + "\n\nÖffne die App und gib ein neues Limit ein.";
				}
			}

			/// <summary>
			/// Main text of the notification indicating that a new picture is available to see.
			/// </summary>
			public static string NewPicture => $"{UserSettings.UserName}, schau Dir Dein neues Bild direkt in der "
				+ "App an. Es ist das erste Bild, was Dir angezeigt wird.";

			/// <summary>
			/// Main text of the notification reminding the user to always record his acts of gambling.
			/// </summary>
			public static string GambleReminder => "Vergiss nicht Deine Ausgaben einzutragen, " +
				"damit Du Dein Limit im Blick behältst.";

			/// <summary>
			/// Main text of the notification reminding the user about their limit's current state.
			/// </summary>
			public static string LimitReminder
			{
				get
				{
					if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
						return "Im Moment sind noch " + Database.CalcBalance(Database.MostRecentLimit())
							.ToString("C", App.Locale) + " von deinem Limit übrig. Immer weiter so!";
					else
						return "Dieses Mal konntest Du es leider nicht einhalten. Aber gib nicht auf! Nächstes Mal "
							+ "wird es besser laufen.";
				}
			}
		}
	}
}