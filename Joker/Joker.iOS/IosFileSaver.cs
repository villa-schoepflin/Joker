using System.Threading.Tasks;
using Foundation;
using Joker.AppInterface;
using Photos;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.iOS.IosFileSaver))]
namespace Joker.iOS
{
	/// <summary>
	/// Contains iOS-specific file IO functionality.
	/// </summary>
	public class IosFileSaver : IPlatformFileSaver
	{
		/// <summary>
		/// Saves the image file contained by the parameter to the iOS photos app,
		/// after requesting permission to do so.
		/// </summary>
		/// <param name="filePath">File path to the image asset.</param>
		public Task<bool> SaveToGallery(string filePath)
		{
			var callback = new TaskCompletionSource<bool>();

			PHPhotoLibrary.RequestAuthorization(grant => callback.SetResult(grant == PHAuthorizationStatus.Authorized));
			if(PHPhotoLibrary.AuthorizationStatus == PHAuthorizationStatus.Authorized)
			{
				string assetPath = Folders.PictureAssets + filePath;
				var stream = typeof(App).Assembly.GetManifestResourceStream(assetPath);
				new UIImage(NSData.FromStream(stream)).SaveToPhotosAlbum(null);
			}

			return callback.Task;
		}
	}
}
