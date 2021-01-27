using System.Threading.Tasks;

namespace Joker.AppInterface
{
	/// <summary>
	/// A custom API for the shared code that abstracts away the platform-specific file IO services.
	/// </summary>
	public interface IPlatformFileSaver
	{
		/// <summary>
		/// Requests permission to access device storage and then saves the file to the user's gallery storage.
		/// </summary>
		/// <param name="filePath">Path to the file that should be saved.</param>
		/// <returns>True if the permission was granted, otherwise false.</returns>
		Task<bool> RequestSaveToGallery(string filePath);
	}
}
