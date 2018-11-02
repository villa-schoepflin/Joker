﻿using System;
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
		internal static string UserName
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
		/// <param name="str">User input from an entry to be parsed.</param>
		/// <exception cref="ArgumentException">Thrown if the argument couldn't be parsed or isn't within the
		/// allowed TimeSpan bounds.</exception>
		internal static void SetNewPictureInterval(string str)
		{
			if(str.Contains("T"))
				str = str.Remove(str.IndexOf('T'));
			if(!uint.TryParse(str, out uint result))
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
		/// <param name="str">User input from an entry to be parsed.</param>
		internal static void SetGambleReminderInterval(string str)
		{
			GambleReminderInterval = ParseReminderInterval(str);
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
		/// <param name="str">User input from an entry to be parsed.</param>
		internal static void SetLimitReminderInterval(string str)
		{
			LimitReminderInterval = ParseReminderInterval(str);
		}

		/// <summary>
		/// Parses a user input string from the settings page according the bounds of reminder notifications.
		/// </summary>
		/// <param name="str">String to be parsed.</param>
		/// <returns>The parsed time span.</returns>
		/// <exception cref="ArgumentException">Thrown if the argument couldn't be parsed or isn't within the
		/// allowed TimeSpan bounds.</exception>
		private static TimeSpan ParseReminderInterval(string str)
		{
			if(str.Contains("S"))
				str = str.Remove(str.IndexOf('S'));
			if(!uint.TryParse(str, out uint result))
				throw new ArgumentException("Das ist keine gültige Zahl.");

			var interval = TimeSpan.FromHours(result);
			if(interval < MinReminderInterval || interval > MaxReminderInterval)
				throw new ArgumentException("Die Zeit zwischen den Erinnerungen sollte zwischen " +
					$"{MinReminderInterval.TotalHours} und {MaxReminderInterval.TotalHours} Stunden liegen.");

			return interval;
		}
	}
}