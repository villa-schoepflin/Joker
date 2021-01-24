using System.Linq;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Contains extension methods related to navigation.
	/// </summary>
	internal static class Navigation
	{
		/// <summary>
		/// Checks if the navigation stack contains an instance of a page type.
		/// </summary>
		/// <typeparam name="T">The type of the page to look for.</typeparam>
		/// <param name="navigation">The navigation to check.</param>
		/// <returns>True if the navigation stack contains a page of this type, false otherwise.</returns>
		public static bool HasPage<T>(this INavigation navigation)
		{
			return navigation.NavigationStack.Any(page => page.GetType() == typeof(T));
		}
	}
}
