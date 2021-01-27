using Android.App;
using Android.Content.PM;
using Android.OS;
using Joker.AppInterface;
using Xamarin.Forms.Platform.Android;

using Environment = System.Environment;
using Platform = Xamarin.Essentials.Platform;

namespace Joker.Android
{
	[Activity(Icon = "@mipmap/icon",
			  Theme = "@style/mainTheme",
			  ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	internal class MainActivity : FormsAppCompatActivity
	{
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			LoadApplication(new App(baseDirectory));
		}
	}
}
