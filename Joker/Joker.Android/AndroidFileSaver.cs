using System.IO;
using System.Threading.Tasks;

using Android;
using Android.App;
using Android.Content.PM;
using Android.Media;
using Android.OS;

using Joker.ApplicationLayer;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.Droid.AndroidFileSaver))]
namespace Joker.Droid
{
	/// <summary>
	/// Contains Android-specific file IO functionality.
	/// </summary>
	public class AndroidFileSaver : IPlatformFileSaver
	{
		/// <summary>
		/// Associated with the permission request to save files to local storage.
		/// </summary>
		internal const int RequestCode = 1;

		/// <summary>
		/// The callback value for the asynchronous task through which the storage access permission is requested.
		/// </summary>
		private static TaskCompletionSource<bool> Callback;

		/// <summary>
		/// The picture whose image file should be saved.
		/// </summary>
		private static string FilePath;

		/// <summary>
		/// Requests permission to access phone storage, then begins the saving process.
		/// </summary>
		/// <param name="filePath">File path to the image resource.</param>
		/// <returns>Indicates whether storage access permission was granted.</returns>
		public Task<bool> SaveToGallery(string filePath)
		{
			FilePath = filePath;
			Callback = new TaskCompletionSource<bool>();

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
			if(grantResult == Permission.Denied)
				return;

			string dir = Path.Combine(Environment.ExternalStorageDirectory.Path, "Joker");
			var folder = new Java.IO.File(dir);
			if(!folder.Exists())
				folder.Mkdir();

			byte[] fileData;
			using(var stream = typeof(App).Assembly.GetManifestResourceStream(FilePath))
			{
				fileData = new byte[stream.Length];
				stream.Read(fileData, 0, (int)stream.Length);
			}

			string[] parts = FilePath.Split('.');
			string file = Path.Combine(dir, $"{parts[parts.Length - 2]}.{parts[parts.Length - 1]}");
			File.WriteAllBytes(file, fileData);
			MediaScannerConnection.ScanFile(Application.Context, new[] { file }, null, null);
		}
	}
}