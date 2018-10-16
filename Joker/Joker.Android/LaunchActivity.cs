using Xamarin.Forms;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace Joker.Droid
{
	/// <summary>
	/// Main entry point of the Android app for the main launching process.
	/// </summary>
	[Activity(NoHistory = true,
			  MainLauncher = true,
			  Theme = "@style/Splash",
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

			Forms.Init(this, savedInstanceState);
			StartActivity(new Intent(this, typeof(MainActivity)));
		}
	}
}