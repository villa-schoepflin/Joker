using System.Linq;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Contains extension methods for navigation.
	/// </summary>
	internal static class Navigation
	{
		internal static bool HasPage<T>(this INavigation navigation)
		{
			return navigation.NavigationStack.Any(page => page is T);
		}
	}
}
