using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Joker.Android
{
	/// <summary>
	/// Main entry point of the Android app for the main launching process.
	/// </summary>
	[Activity(NoHistory = true,
			  MainLauncher = true,
			  Theme = "@style/Splash",
			  Icon = "@mipmap/ic_launcher",
			  ScreenOrientation = ScreenOrientation.Portrait)]
	public class LaunchActivity : Activity
	{
		/// <summary>
		/// Presents a splash screen to the user and performs the main launching process.
		/// </summary>
		/// <param name="savedInstanceState">Parameters supplied for this activity.</param>
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Platform.Init(this, savedInstanceState);
			Forms.Init(this, savedInstanceState);
			StartActivity(new Intent(this, typeof(MainActivity)));
		}
	}
}
