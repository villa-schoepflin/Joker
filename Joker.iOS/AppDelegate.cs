using System;
using System.IO;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Joker.iOS
{
	/// <summary>
	/// Class responsible for launching the user interface of the application, as well as
	/// listening and responding to application events from iOS.
	/// </summary>
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate
	{
		/// <summary>
		/// Invoked when the application has loaded and is ready to run. Instantiates the window,
		/// loads the UI into it and then makes the window visible. iOS will terminate the
		/// application if if hasn't returned from there after 17 seconds.
		/// </summary>
		/// <param name="app">Contains the main processing loop for a MonoTouch application.</param>
		/// <param name="options">A dictionary of launching options.</param>
		/// <returns>Returns whether the launching process was completed.</returns>
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Forms.Init();

			string personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			LoadApplication(new App(Path.Combine(personalFolder, "..", "Library")));
			CorrectNavigationBarColors();

			return base.FinishedLaunching(app, options);
		}

		/// <summary>
		/// Workaround to fix the bug that makes Xamarin.iOS sometimes ignore the colors set in the
		/// shared code.
		/// </summary>
		internal static void CorrectNavigationBarColors()
		{
			UINavigationBar.Appearance.BarTintColor = App.Color("Primary1").ToUIColor();
			UINavigationBar.Appearance.TintColor = App.Color("TextContrast").ToUIColor();
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
			{
				TextColor = App.Color("TextContrast").ToUIColor()
			});
		}
	}
}