using System;
using System.Text.RegularExpressions;

using Xamarin.Essentials;

using Newtonsoft.Json;

namespace Joker.DataAccess
{
	/// <summary>
	/// A wrapper class that simplifies access to persistent preferences concerning the user directly.
	/// </summary>
	public static class UserSettings
	{
		/// <summary>
		/// The maximum string length of the user name.
		/// </summary>
		public const int MaxNameLength = 20;

		/// <summary>
		/// The name used to address the user in personalized texts or push notifications.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the user tries to set this to an empty string or exceeds
		/// the maximum allowed length.</exception>
		public static string UserName
		{
			get => Preferences.Get("UserName", null);
			set
			{
				if(value.Length > MaxNameLength)
					throw new ArgumentException($"Der Name darf nicht länger als {MaxNameLength} Zeichen sein.");
				value = value.Trim();
				if(string.IsNullOrEmpty(value))
					throw new ArgumentException("Der angegebene Name ist leer.");
				Preferences.Set("UserName", value);
			}
		}

		/// <summary>
		/// The maximum string length of the user password.
		/// </summary>
		public const int MaxPasswordLength = 10;

		/// <summary>
		/// The optional password with which the user can protect access to the app's UI.
		/// </summary>
		public static string UserPassword
		{
			get => Preferences.Get("UserPassword", null);
			set
			{
				if(value.Length > MaxPasswordLength)
					throw new ArgumentException($"Das Passwort darf nicht länger als {MaxPasswordLength} Zeichen sein.");
				if(value.Contains(" "))
					throw new ArgumentException("Das Passwort darf keine Leerzeichen enthalten.");
				if(!Regex.IsMatch(value, @"[a-zA-Z0-9]*"))
					throw new ArgumentException("Das Passwort darf nur Groß-/Kleinbuchstaben und Zahlen enthalten.");
				Preferences.Set("UserPassword", value);
			}
		}

		public static string FirstSecurityQuestion { get; set; }

		public static string FirstSecurityAnswer { get; set; }

		public static string SecondSecurityQuestion { get; set; }

		public static string SecondSecurityAnswer { get; set; }

		/// <summary>
		/// The minimum allowed time span between two new pictures, which is 2 days.
		/// </summary>
		internal static readonly TimeSpan MinNewPictureInterval = TimeSpan.FromDays(2);

		/// <summary>
		/// The maximum allowed time span between two new pictures, which is 28 days.
		/// </summary>
		internal static readonly TimeSpan MaxNewPictureInterval = TimeSpan.FromDays(28);

		/// <summary>
		/// The time span between two new pictures. The default value is 2 days.
		/// </summary>
		internal static TimeSpan NewPictureInterval
		{
			get => JsonConvert.DeserializeObject<TimeSpan>(Preferences.Get("NewPictureInterval",
				JsonConvert.SerializeObject(TimeSpan.FromDays(2))));
			private set => Preferences.Set("NewPictureInterval", JsonConvert.SerializeObject(value));
		}

		/// <summary>
		/// Sets the time span between two new pictures from a user-supplied text string.
		/// </summary>
		/// <param name="input">User input from an entry to be parsed.</param>
		/// <exception cref="ArgumentException">Thrown if the argument couldn't be parsed or isn't within the
		/// allowed TimeSpan bounds.</exception>
		internal static void SetNewPictureInterval(string input)
		{
			if(input.Contains("T"))
				input = input.Remove(input.IndexOf('T'));
			if(!uint.TryParse(input, out uint result))
				throw new ArgumentException("Das ist keine gültige Zahl.");

			var interval = TimeSpan.FromDays(result);
			if(interval < MinNewPictureInterval || interval > MaxNewPictureInterval)
				throw new ArgumentException($"Die Zeit zwischen neuen Bildern sollte zwischen " +
					$"{MinNewPictureInterval.TotalDays} und {MaxNewPictureInterval.TotalDays} Tagen liegen.");

			NewPictureInterval = interval;
		}

		/// <summary>
		/// The minimum allowed time span between two reminders, which is 2 hours.
		/// </summary>
		internal static readonly TimeSpan MinReminderInterval = TimeSpan.FromHours(2);

		/// <summary>
		/// The maximum allowed time span between two reminders, which is 200 hours.
		/// </summary>
		internal static readonly TimeSpan MaxReminderInterval = TimeSpan.FromHours(200);

		/// <summary>
		/// The time span between two push notifications, that remind the user that he should always
		/// record his acts of gambling in the app.
		/// </summary>
		public static TimeSpan GambleReminderInterval
		{
			get => JsonConvert.DeserializeObject<TimeSpan>(Preferences.Get("GambleReminderInterval",
				JsonConvert.SerializeObject(TimeSpan.FromHours(8))));
			private set => Preferences.Set("GambleReminderInterval", JsonConvert.SerializeObject(value));
		}

		/// <summary>
		/// Sets the time span between two gambling reminders from a user-supplied text string.
		/// </summary>
		/// <param name="input">User input from an entry to be parsed.</param>
		internal static void SetGambleReminderInterval(string input)
		{
			GambleReminderInterval = ParseReminderInterval(input);
		}

		/// <summary>
		/// The time span between two push notifications, that remind the user about the current state of their limit.
		/// </summary>
		public static TimeSpan LimitReminderInterval
		{
			get => JsonConvert.DeserializeObject<TimeSpan>(Preferences.Get("LimitReminderInterval",
				JsonConvert.SerializeObject(TimeSpan.FromHours(12))));
			private set => Preferences.Set("LimitReminderInterval", JsonConvert.SerializeObject(value));
		}

		/// <summary>
		/// Sets the time span between two limit reminders from a user-supplied text string.
		/// </summary>
		/// <param name="input">User input from an entry to be parsed.</param>
		internal static void SetLimitReminderInterval(string input)
		{
			LimitReminderInterval = ParseReminderInterval(input);
		}

		/// <summary>
		/// Parses a user input string from the settings page according the bounds of reminder notifications.
		/// </summary>
		/// <param name="input">String to be parsed.</param>
		/// <returns>The parsed time span.</returns>
		/// <exception cref="ArgumentException">Thrown if the argument couldn't be parsed or isn't within the
		/// allowed TimeSpan bounds.</exception>
		private static TimeSpan ParseReminderInterval(string input)
		{
			if(input.Contains("S"))
				input = input.Remove(input.IndexOf('S'));
			if(!uint.TryParse(input, out uint result))
				throw new ArgumentException("Das ist keine gültige Zahl.");

			var interval = TimeSpan.FromHours(result);
			if(interval < MinReminderInterval || interval > MaxReminderInterval)
				throw new ArgumentException("Die Zeit zwischen den Erinnerungen muss zwischen " +
					$"{MinReminderInterval.TotalHours} und {MaxReminderInterval.TotalHours} Stunden liegen.");

			return interval;
		}
	}
}