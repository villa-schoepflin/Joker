using System;
using System.Text.RegularExpressions;
using Joker.BusinessLogic;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Joker.DataAccess
{
	/// <summary>
	/// A wrapper class that simplifies access to persistent preferences set by the user directly.
	/// </summary>
	public static class UserSettings
	{
		/// <summary>
		/// The maximum string length of the user name.
		/// </summary>
		public const int MaxNameLength = 20;

		/// <summary>
		/// The maximum string length of the user password.
		/// </summary>
		public const int MaxPasswordLength = 10;

		/// <summary>
		/// The maximum string length for either a security question or answer.
		/// </summary>
		public const int MaxSecurityAttributeLength = 80;

		/// <summary>
		/// The minimum allowed time span between two new pictures, which is 2 days.
		/// </summary>
		internal static readonly TimeSpan MinNewPictureInterval = TimeSpan.FromDays(2);

		/// <summary>
		/// The maximum allowed time span between two new pictures, which is 28 days.
		/// </summary>
		internal static readonly TimeSpan MaxNewPictureInterval = TimeSpan.FromDays(28);

		/// <summary>
		/// The minimum allowed time span between two reminders, which is 2 hours.
		/// </summary>
		internal static readonly TimeSpan MinReminderInterval = TimeSpan.FromHours(2);

		/// <summary>
		/// The maximum allowed time span between two reminders, which is 200 hours.
		/// </summary>
		internal static readonly TimeSpan MaxReminderInterval = TimeSpan.FromHours(200);

		/// <summary>
		/// The name used to address the user in personalized texts or push notifications.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the user tries to set this property to a text that is longer
		/// than the maximum allowed length or is a null or empty string.</exception>
		public static string UserName
		{
			get => Preferences.Get(UserNameKey, null);
			set
			{
				value = value?.Trim();
				if(string.IsNullOrEmpty(value))
					throw new ArgumentException(Text.InputEmpty);
				if(value.Length > MaxNameLength)
					throw new ArgumentException(string.Format(Text.InputTooLong, MaxNameLength));

				Preferences.Set(UserNameKey, value);
			}
		}

		/// <summary>
		/// The optional password with which the user can protect access to the app's content.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the user tries to set this property to a text that is longer
		/// than the maximum allowed length or is a null or empty string.</exception>
		public static string UserPassword
		{
			get => Preferences.Get(UserPasswordKey, null);
			set
			{
				if(value.Length > MaxPasswordLength)
					throw new ArgumentException(string.Format(Text.PasswordTooLong, MaxPasswordLength));

				if(value.Contains(" "))
					throw new ArgumentException(Text.PasswordContainsSpaces);

				if(!Regex.IsMatch(value, @"[a-zA-Z0-9]*"))
					throw new ArgumentException(Text.PasswordContainsSpecialChars);

				Preferences.Set(UserPasswordKey, value);
			}
		}

		/// <summary>
		/// Holds the text of the first security question and its answer.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the user tries to set this property to a text that is longer
		/// than the maximum allowed length or is a null or empty string.</exception>
		public static (string, string) FirstSecurityAttribute
		{
			get => (Preferences.Get(FirstSecurityQuestionKey, null), Preferences.Get(FirstSecurityAnswerKey, null));
			set
			{
				CheckSecurityAttributePair(ref value);
				Preferences.Set(FirstSecurityQuestionKey, value.Item1);
				Preferences.Set(FirstSecurityAnswerKey, value.Item2);
			}
		}

		/// <summary>
		/// Holds the text of the second security question and its answer.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the user tries to set this property to a text that is longer
		/// than the maximum allowed length or is a null or empty string.</exception>
		public static (string, string) SecondSecurityAttribute
		{
			get => (Preferences.Get(SecondSecurityQuestionKey, null), Preferences.Get(SecondSecurityAnswerKey, null));
			set
			{
				CheckSecurityAttributePair(ref value);
				Preferences.Set(SecondSecurityQuestionKey, value.Item1);
				Preferences.Set(SecondSecurityAnswerKey, value.Item2);
			}
		}

		/// <summary>
		/// The time span between two new pictures. The default value is 2 days.
		/// </summary>
		internal static TimeSpan NewPictureInterval
		{
			get => JsonConvert.DeserializeObject<TimeSpan>(Preferences.Get(NewPictureIntervalKey,
				JsonConvert.SerializeObject(TimeSpan.FromDays(2))));
			private set => Preferences.Set(NewPictureIntervalKey, JsonConvert.SerializeObject(value));
		}

		/// <summary>
		/// The time span between two notifications, that remind the user that they should always record their
		/// gambling-related spendings in the app.
		/// </summary>
		public static TimeSpan GambleReminderInterval
		{
			get => JsonConvert.DeserializeObject<TimeSpan>(Preferences.Get(GambleReminderIntervalKey,
				JsonConvert.SerializeObject(TimeSpan.FromHours(8))));
			private set => Preferences.Set(GambleReminderIntervalKey, JsonConvert.SerializeObject(value));
		}

		/// <summary>
		/// The time span between two push notifications, that remind the user about the current state of their limit.
		/// </summary>
		public static TimeSpan LimitReminderInterval
		{
			get => JsonConvert.DeserializeObject<TimeSpan>(Preferences.Get(LimitReminderIntervalKey,
				JsonConvert.SerializeObject(TimeSpan.FromHours(12))));
			private set => Preferences.Set(LimitReminderIntervalKey, JsonConvert.SerializeObject(value));
		}

		/// <summary>
		/// Sets the time span between two new pictures from a user-supplied text string.
		/// </summary>
		/// <param name="input">User input from an entry to be parsed.</param>
		/// <exception cref="ArgumentException">Thrown if the argument couldn't be parsed or isn't within the allowed
		/// TimeSpan bounds.</exception>
		internal static void SetNewPictureInterval(string input)
		{
			if(!uint.TryParse(input, out uint result))
				throw new ArgumentException(Text.NumberInvalid);

			var interval = TimeSpan.Zero;
			bool overflowed = false;
			try
			{
				interval = TimeSpan.FromDays(result);
			}
			catch(OverflowException)
			{
				overflowed = true;
			}

			if(interval < MinNewPictureInterval || interval > MaxNewPictureInterval || overflowed)
			{
				double min = MinNewPictureInterval.TotalDays;
				double max = MaxNewPictureInterval.TotalDays;
				throw new ArgumentException(string.Format(Text.PictureIntervalBounds, min, max));
			}
			NewPictureInterval = interval;
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
		/// Sets the time span between two limit reminders from a user-supplied text string.
		/// </summary>
		/// <param name="input">User input from an entry to be parsed.</param>
		internal static void SetLimitReminderInterval(string input)
		{
			LimitReminderInterval = ParseReminderInterval(input);
		}

		private static void CheckSecurityAttributePair(ref (string, string) pair)
		{
			pair.Item1 = pair.Item1?.Trim();
			pair.Item2 = pair.Item2?.Trim();
			if(pair.Item1.Length > MaxSecurityAttributeLength || pair.Item2.Length > MaxSecurityAttributeLength)
				throw new ArgumentException(string.Format(Text.InputTooLong, MaxSecurityAttributeLength));
		}

		/// <summary>
		/// Parses a user input string from the settings page according the bounds of reminder notifications.
		/// </summary>
		/// <param name="input">String to be parsed.</param>
		/// <returns>The parsed time span.</returns>
		/// <exception cref="ArgumentException">Thrown if the argument couldn't be parsed or isn't within the allowed
		/// TimeSpan bounds.</exception>
		private static TimeSpan ParseReminderInterval(string input)
		{
			if(!uint.TryParse(input, out uint result))
				throw new ArgumentException(Text.NumberInvalid);

			var interval = TimeSpan.FromHours(result);
			if(interval < MinReminderInterval || interval > MaxReminderInterval)
			{
				double min = MinReminderInterval.TotalHours;
				double max = MaxReminderInterval.TotalHours;
				throw new ArgumentException(string.Format(Text.ReminderIntervalBounds, min, max));
			}
			return interval;
		}

		#region Identifier keys for the settings (DO NOT CHANGE!)
		private const string UserNameKey = "UserName";
		private const string UserPasswordKey = "UserPassword";
		private const string FirstSecurityQuestionKey = "FirstSecurityQuestion";
		private const string FirstSecurityAnswerKey = "FirstSecurityAnswer";
		private const string SecondSecurityQuestionKey = "SecondSecurityQuestion";
		private const string SecondSecurityAnswerKey = "SecondSecurityAnswer";
		private const string NewPictureIntervalKey = "NewPictureInterval";
		private const string GambleReminderIntervalKey = "GambleReminderInterval";
		private const string LimitReminderIntervalKey = "LimitReminderInterval";
		#endregion
	}
}
