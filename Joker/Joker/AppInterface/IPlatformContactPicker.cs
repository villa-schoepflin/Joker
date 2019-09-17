using System.Threading.Tasks;

using Joker.BusinessLogic;

namespace Joker.AppInterface
{
	/// <summary>
	/// A custom API for the shared code that abstracts away the platform-specific
	/// contact and address book services of the target systems.
	/// </summary>
	public interface IPlatformContactPicker
	{
		/// <summary>
		/// Picks a single contact from the contact list through a platform-supplied view.
		/// </summary>
		/// <returns>A contact with first and last name and phone number. Returns null if
		/// the picking process was cancelled or the contact has no phone number recorded.</returns>
		Task<Contact> PickContact();
	}
}