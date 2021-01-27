using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Joker.Android
{
	[Activity(MainLauncher = true,
			  NoHistory = true,
			  Icon = "@mipmap/icon",
			  Theme = "@style/splash",
			  ScreenOrientation = ScreenOrientation.Portrait)]
	internal class LaunchActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Platform.Init(this, savedInstanceState);
			Forms.Init(this, savedInstanceState);

			Intent launch = new(this, typeof(MainActivity));
			StartActivity(launch);
		}
	}
}
