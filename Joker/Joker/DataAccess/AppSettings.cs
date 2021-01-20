using System;
using Xamarin.Essentials;

namespace Joker.DataAccess
{
	/// <summary>
	/// A wrapper class that simplifies access to persistent preferences concerning app operations.
	/// </summary>
	public static class AppSettings
	{
		/// <summary>
		/// Gets or sets the path of the database file.
		/// </summary>
		internal static string DatabaseFilePath
		{
			get => Preferences.Get(DatabaseFilePathKey, null);
			set => Preferences.Set(DatabaseFilePathKey, value);
		}

		/// <summary>
		/// Gets or sets whether the user welcome tour was completed.
		/// </summary>
		public static bool WelcomeTourCompleted
		{
			get => Preferences.Get(WelcomeTourCompletedKey, false);
			set => Preferences.Set(WelcomeTourCompletedKey, value);
		}

		/// <summary>
		/// Gets or sets the condition whether the user has protected the app with a password.
		/// </summary>
		public static bool UserPasswordIsSet
		{
			get => Preferences.Get(UserPasswordIsSetKey, false);
			set => Preferences.Set(UserPasswordIsSetKey, value);
		}

		/// <summary>
		/// Gets or sets the condition whether the user has set their first security question.
		/// </summary>
		public static bool FirstSecurityQuestionIsSet
		{
			get => Preferences.Get(FirstSecurityQuestionIsSetKey, false);
			set => Preferences.Set(FirstSecurityQuestionIsSetKey, value);
		}

		/// <summary>
		/// Gets or sets the condition whether the user has set their second security question.
		/// </summary>
		public static bool SecondSecurityQuestionIsSet
		{
			get => Preferences.Get(SecondSecurityQuestionIsSetKey, false);
			set => Preferences.Set(SecondSecurityQuestionIsSetKey, value);
		}

		/// <summary>
		/// Gets or sets the time when the current active limit expires and a new one must be set.
		/// </summary>
		public static DateTime LimitExpiredTime
		{
			get => Preferences.Get(LimitExpiredTimeKey, DateTime.MinValue);
			set => Preferences.Set(LimitExpiredTimeKey, value);
		}

		/// <summary>
		/// Gets or sets the time when the availability of a new picture should be notified.
		/// </summary>
		public static DateTime NewPictureTime
		{
			get => Preferences.Get(NewPictureTimeKey, DateTime.MinValue);
			set => Preferences.Set(NewPictureTimeKey, value);
		}

		#region Identifier keys for the settings (DO NOT CHANGE!)
		private const string DatabaseFilePathKey = "DatabaseFilePath";
		private const string WelcomeTourCompletedKey = "WelcomeTourCompleted";
		private const string UserPasswordIsSetKey = "UserPasswordIsSet";
		private const string FirstSecurityQuestionIsSetKey = "FirstSecurityQuestionIsSet";
		private const string SecondSecurityQuestionIsSetKey = "SecondSecurityQuestionIsSet";
		private const string LimitExpiredTimeKey = "LimitExpiredTime";
		private const string NewPictureTimeKey = "NewPictureTime";
		#endregion
	}
}
