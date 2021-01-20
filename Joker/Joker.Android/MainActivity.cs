using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.Platform.Android;

using Environment = System.Environment;

namespace Joker.Android
{
	/// <summary>
	/// The entry point for the Android app after preliminary launching has finished behind the
	/// splash screen.
	/// </summary>
	[Activity(Theme = "@style/MainTheme",
			  Icon = "@mipmap/ic_launcher",
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
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);
			Instance = this;

			string personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			LoadApplication(new App(personalFolder));
		}

		/// <summary>
		/// Handles the return of permission requests to the user during runtime.
		/// </summary>
		/// <param name="requestCode">Request code of the permission request.</param>
		/// <param name="permissions">Requested permissions.</param>
		/// <param name="grantResults">Results of the request as determined by the user.</param>
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			if(requestCode == AndroidFileSaver.RequestCode)
				AndroidFileSaver.Finish(grantResults[0]);
		}
	}
}
