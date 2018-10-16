﻿using System;
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
		internal static string DBFilePath
		{
			get => Preferences.Get("DBFilePath", null);
			set => Preferences.Set("DBFilePath", value);
		}

		/// <summary>
		/// Gets or sets whether the user welcome tour was completed.
		/// </summary>
		public static bool WelcomeTourCompleted
		{
			get => Preferences.Get("WelcomeTourCompleted", false);
			set => Preferences.Set("WelcomeTourCompleted", value);
		}

		/// <summary>
		/// Gets or sets the time when the current active limit expires and a new one must be set.
		/// </summary>
		public static DateTime LimitExpiredTime
		{
			get => Preferences.Get("LimitExpiredTime", DateTime.MinValue);
			set => Preferences.Set("LimitExpiredTime", value);
		}

		/// <summary>
		/// Gets or sets the time when the availability of a new picture should be notified to the user.
		/// </summary>
		public static DateTime NewPictureTime
		{
			get => Preferences.Get("NewPictureTime", DateTime.MinValue);
			set => Preferences.Set("NewPictureTime", value);
		}
	}
}