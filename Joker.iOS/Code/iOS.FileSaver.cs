using System.Threading.Tasks;
using Foundation;
using Joker.AppInterface;
using UIKit;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.iOS.FileSaver))]
namespace Joker.iOS
{
	internal class FileSaver : IPlatformFileSaver
	{
		public async Task<bool> RequestSaveToGallery(string filePath)
		{
			var result = await Permissions.CheckStatusAsync<Permissions.Photos>();
			if(result == PermissionStatus.Granted)
				return SaveToGallery(filePath);

			result = await Permissions.RequestAsync<Permissions.Photos>();
			if(result == PermissionStatus.Granted)
				return SaveToGallery(filePath);

			return false;
		}

		private bool SaveToGallery(string filePath)
		{
			string assetPath = Folders.PictureAssets + filePath;
			var stream = App.Assembly.GetManifestResourceStream(assetPath);
			var data = NSData.FromStream(stream);

			UIImage image = new(data);
			image.SaveToPhotosAlbum(null);
			return true;
		}
	}
}
