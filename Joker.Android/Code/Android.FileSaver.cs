using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Media;
using Joker.AppInterface;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.Android.FileSaver))]
namespace Joker.Android
{
	internal class FileSaver : IPlatformFileSaver
	{
		public async Task<bool> RequestSaveToGallery(string filePath)
		{
			var result = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
			if(result == PermissionStatus.Granted)
				return SaveToGallery(filePath);

			result = await Permissions.RequestAsync<Permissions.StorageWrite>();
			if(result == PermissionStatus.Granted)
				return SaveToGallery(filePath);

			return false;
		}

		private bool SaveToGallery(string filePath)
		{
			var mediaDirs = Application.Context.GetExternalMediaDirs();
			string mediaDir = mediaDirs[0].AbsolutePath;
			string targetDir = Path.Combine(mediaDir, AppInfo.Name);

			if(!Directory.Exists(targetDir))
				_ = Directory.CreateDirectory(targetDir);

			string assetPath = Folders.PictureAssets + filePath;
			using var stream = App.Assembly.GetManifestResourceStream(assetPath);
			byte[] fileData = new byte[stream.Length];
			_ = stream.Read(fileData, 0, (int)stream.Length);

			string file = Path.Combine(targetDir, filePath);
			File.WriteAllBytes(file, fileData);
			MediaScannerConnection.ScanFile(Application.Context, new[] { file }, null, null);
			return true;
		}
	}
}
