using Android.App;
using Android.Content.PM;
using Android.OS;
using Joker.AppInterface;
using Xamarin.Forms.Platform.Android;

using Environment = System.Environment;

namespace Joker.Android
{
	[Activity(Icon = "@mipmap/icon",
			  Theme = "@style/MainTheme",
			  ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	internal class MainActivity : FormsAppCompatActivity
	{
		internal static MainActivity Instance;

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			if(requestCode == FileSaver.PermissionRequestCode)
				FileSaver.Finish(grantResults[0]);
		}

		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);
			Instance = this;

			string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			LoadApplication(new App(baseDirectory));
		}
	}
}
