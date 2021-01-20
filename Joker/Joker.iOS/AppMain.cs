﻿using UIKit;

namespace Joker.iOS
{
	/// <summary>
	/// Main class of the iOS app.
	/// </summary>
	public static class AppMain
	{
		/// <summary>
		/// Main entry point of the application.
		/// </summary>
		/// <param name="args">Parameters given from the command line for the app launch.</param>
		public static void Main(string[] args)
		{
			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}
