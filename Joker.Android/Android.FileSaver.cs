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
	/// <summary>
	/// Contains Android-specific file IO functionality.
	/// </summary>
	public class FileSaver : IPlatformFileSaver
	{
		/// <summary>
		/// Associated with the permission request to save files to local storage.
		/// </summary>
		internal const int RequestCode = 1;

		/// <summary>
		/// The callback value for the asynchronous task through which the storage access permission
		/// is requested.
		/// </summary>
		private static TaskCompletionSource<bool> Callback;

		/// <summary>
		/// The picture whose image file should be saved.
		/// </summary>
		private static string FilePath;

		/// <summary>
		/// Requests permission to access phone storage, then begins the saving process.
		/// </summary>
		/// <param name="filePath">File path to the image asset.</param>
		/// <returns>Indicates whether storage access permission was granted.</returns>
		public Task<bool> SaveToGallery(string filePath)
		{
			FilePath = filePath;
			Callback = new();

			string storagePermission = Manifest.Permission.WriteExternalStorage;
			if(Application.Context.CheckSelfPermission(storagePermission) == Permission.Granted)
				Finish(Permission.Granted);
			else
				MainActivity.Instance.RequestPermissions(new[] { storagePermission }, RequestCode);

			return Callback.Task;
		}

		/// <summary>
		/// Saves the image file to the Android gallery in a "Joker" directory.
		/// </summary>
		/// <param name="grantResult">Result of the permission request to the user.</param>
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
			using(var stream = typeof(App).Assembly.GetManifestResourceStream(assetPath))
			{
				fileData = new byte[stream.Length];
				_ = stream.Read(fileData, 0, (int)stream.Length);
			}

			string file = Path.Combine(targetDir, FilePath);
			File.WriteAllBytes(file, fileData);
			MediaScannerConnection.ScanFile(Application.Context, new[] { file }, null, null);
		}
	}
}
