using System;

namespace Joker.AppInterface
{
	/// <summary>
	/// Necessary so that the linker doesn't strip out properties that are read only in XAML.
	/// </summary>
	public sealed class PreserveAttribute : Attribute
	{
		/// <summary>
		/// Ensures that all members of this type are preserved.
		/// </summary>
		public bool AllMembers;

		/// <summary>
		/// Flags the method as a method to preserve during linking if the container class is pulled in.
		/// </summary>
		public bool Conditional;
	}
}
