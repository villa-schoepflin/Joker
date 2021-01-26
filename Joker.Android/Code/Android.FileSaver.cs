using System.IO;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Media;
using Joker.AppInterface;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.Android.FileSaver))]
namespace Joker.Android
{
	internal class FileSaver : IPlatformFileSaver
	{
		internal const int PermissionRequestCode = 1;

		private static TaskCompletionSource<bool> Callback;
		private static string FilePath;

		internal static void Finish(Permission grantResult)
		{
			Callback.SetResult(grantResult == Permission.Granted);
			if(grantResult != Permission.Granted)
				return;

			string mediaDir = Application.Context.GetExternalMediaDirs()[0].AbsolutePath;
			string targetDir = Path.Combine(mediaDir, Folders.GalleryFolderName);
			using(Java.IO.File folder = new(targetDir))
			{
				if(!folder.Exists())
					_ = folder.Mkdir();
			}

			byte[] fileData;
			string assetPath = Folders.PictureAssets + FilePath;
			using(var stream = App.Assembly.GetManifestResourceStream(assetPath))
			{
				fileData = new byte[stream.Length];
				_ = stream.Read(fileData, 0, (int)stream.Length);
			}

			string file = Path.Combine(targetDir, FilePath);
			File.WriteAllBytes(file, fileData);
			MediaScannerConnection.ScanFile(Application.Context, new[] { file }, null, null);
		}

		public Task<bool> SaveToGallery(string filePath)
		{
			FilePath = filePath;
			Callback = new();

			string storagePermission = Manifest.Permission.WriteExternalStorage;
			if(Application.Context.CheckSelfPermission(storagePermission) == Permission.Granted)
				Finish(Permission.Granted);
			else
				MainActivity.Instance.RequestPermissions(new[] { storagePermission }, PermissionRequestCode);

			return Callback.Task;
		}
	}
}
