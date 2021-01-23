using System.Threading.Tasks;
using Foundation;
using Joker.AppInterface;
using Photos;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.iOS.FileSaver))]
namespace Joker.iOS
{
	/// <summary>
	/// Contains iOS-specific file IO functionality.
	/// </summary>
	public class FileSaver : IPlatformFileSaver
	{
		/// <summary>
		/// Saves the image file contained by the parameter to the iOS photos app,
		/// after requesting permission to do so.
		/// </summary>
		/// <param name="filePath">File path to the image asset.</param>
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
				var stream = JokerApp.Assembly.GetManifestResourceStream(assetPath);
				new UIImage(NSData.FromStream(stream)).SaveToPhotosAlbum(null);
			}
			return callback.Task;
		}
	}
}
