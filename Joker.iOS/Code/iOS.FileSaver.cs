using System.Threading.Tasks;
using Foundation;
using Joker.AppInterface;
using Photos;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.iOS.FileSaver))]
namespace Joker.iOS
{
	internal class FileSaver : IPlatformFileSaver
	{
		public Task<bool> SaveToGallery(string filePath)
		{
			TaskCompletionSource<bool> callback = new();

			void handler(PHAuthorizationStatus status)
			{
				callback.SetResult(status == PHAuthorizationStatus.Authorized);
			}
			PHPhotoLibrary.RequestAuthorization(handler);

			if(PHPhotoLibrary.AuthorizationStatus == PHAuthorizationStatus.Authorized)
			{
				string assetPath = Folders.PictureAssets + filePath;
				var stream = App.Assembly.GetManifestResourceStream(assetPath);
				new UIImage(NSData.FromStream(stream)).SaveToPhotosAlbum(null);
			}
			return callback.Task;
		}
	}
}
