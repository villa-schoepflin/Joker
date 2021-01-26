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
		internal const int MaxNameLength = 20;
		internal const int MaxPasswordLength = 10;
		internal const int MaxSecurityAttributeLength = 80;

		internal static readonly TimeSpan MinNewPictureInterval = TimeSpan.FromDays(2);
		internal static readonly TimeSpan MaxNewPictureInterval = TimeSpan.FromDays(28);
		internal static readonly TimeSpan MinReminderInterval = TimeSpan.FromHours(2);
		internal static readonly TimeSpan MaxReminderInterval = TimeSpan.FromHours(200);

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

		internal static string UserName
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

		internal static string UserPassword
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

		internal static (string, string) FirstSecurityAttribute
		{
			get => (Preferences.Get(FirstSecurityQuestionKey, null), Preferences.Get(FirstSecurityAnswerKey, null));
			set
			{
				CheckSecurityAttributePair(ref value);
				Preferences.Set(FirstSecurityQuestionKey, value.Item1);
				Preferences.Set(FirstSecurityAnswerKey, value.Item2);
			}
		}

		internal static (string, string) SecondSecurityAttribute
		{
			get => (Preferences.Get(SecondSecurityQuestionKey, null), Preferences.Get(SecondSecurityAnswerKey, null));
			set
			{
				CheckSecurityAttributePair(ref value);
				Preferences.Set(SecondSecurityQuestionKey, value.Item1);
				Preferences.Set(SecondSecurityAnswerKey, value.Item2);
			}
		}

		internal static TimeSpan NewPictureInterval
		{
			get => JsonConvert.DeserializeObject<TimeSpan>(Preferences.Get(NewPictureIntervalKey,
				JsonConvert.SerializeObject(TimeSpan.FromDays(2))));
			private set => Preferences.Set(NewPictureIntervalKey, JsonConvert.SerializeObject(value));
		}

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

		internal static void SetGambleReminderInterval(string input)
		{
			GambleReminderInterval = ParseReminderInterval(input);
		}

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
