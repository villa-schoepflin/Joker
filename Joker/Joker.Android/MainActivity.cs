using System;

using Xamarin.Forms.Platform.Android;

using Android.App;
using Android.Content;
using Android.Content.PM;

namespace Joker.Droid
{
	/// <summary>
	/// The entry point for the Android app after preliminary launching has finished behind the splash screen.
	/// </summary>
	[Activity(Theme = "@style/MainTheme",
			  ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	{
		/// <summary>
		/// Provides a reference to the main application context for result-based activities.
		/// </summary>
		internal static MainActivity Instance;

		/// <summary>
		/// Creates the main activity and provides the entry point for the shared code.
		/// </summary>
		/// <param name="bundle">Parameters supplied for this activity.</param>
		protected override void OnCreate(Android.OS.Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);
			Instance = this;

			LoadApplication(new App(Environment.GetFolderPath(Environment.SpecialFolder.Personal)));
		}

		/// <summary>
		/// Handles the return of results of result-based activities started during runtime.
		/// </summary>
		/// <param name="requestCode">Request code of a result-based activity.</param>
		/// <param name="resultCode">Code of the result supplied by Android.</param>
		/// <param name="intent">Callback data contained by the result-based activity.</param>
		/// <exception cref="ArgumentException">Thrown if the request code does not correspond to
		/// an intended activity.</exception>
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
		{
			base.OnActivityResult(requestCode, resultCode, intent);
			switch(requestCode)
			{
				case AndroidContactPicker.RequestCode:
					AndroidContactPicker.Finish(intent);
					break;
				default:
					throw new ArgumentException("Request code does not correspond to an intended activity.");
			}
		}

		/// <summary>
		/// Handles the return of permission requests to the user during runtime.
		/// </summary>
		/// <param name="requestCode">Request code of the permission request.</param>
		/// <param name="permissions">Requested permissions.</param>
		/// <param name="grantResults">Results of the request as determined by the user.</param>
		/// <exception cref="ArgumentException">Thrown if the request code does not correspond to
		/// an intended activity.</exception>
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			switch(requestCode)
			{
				case AndroidFileSaver.RequestCode:
					AndroidFileSaver.Finish(grantResults[0]);
					break;
				default:
					throw new ArgumentException("Request code does not correspond to an intended activity.");
			}
		}
	}
}