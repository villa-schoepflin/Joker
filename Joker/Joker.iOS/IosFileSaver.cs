using System.Threading.Tasks;

using Foundation;
using Photos;
using UIKit;

using Joker.AppInterface;

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
		/// <param name="filePath">File path to the image resource.</param>
		public Task<bool> SaveToGallery(string filePath)
		{
			var callback = new TaskCompletionSource<bool>();

			PHPhotoLibrary.RequestAuthorization(grant
				=> callback.SetResult(grant == PHAuthorizationStatus.Authorized));

			if(PHPhotoLibrary.AuthorizationStatus == PHAuthorizationStatus.Authorized)
			{
				var stream = typeof(App).Assembly.GetManifestResourceStream($"Joker.Resources.PictureFeed.{filePath}");
				new UIImage(NSData.FromStream(stream)).SaveToPhotosAlbum(null);
			}

			return callback.Task;
		}
	}
}