using System.Threading.Tasks;

namespace Joker.AppInterface
{
	/// <summary>
	/// A custom API for the shared code that abstracts away the platform-specific
	/// file IO services of the target systems.
	/// </summary>
	public interface IPlatformFileSaver
	{
		/// <summary>
		/// Saves a picture's file to the user's personal storage and indicates whether
		/// the process was successful.
		/// </summary>
		/// <param name="filePath">File path to the image resource.</param>
		/// <returns>True if the file could be saved, otherwise false.</returns>
		Task<bool> SaveToGallery(string filePath);
	}
}