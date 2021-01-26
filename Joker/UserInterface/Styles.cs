using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Holds commonly used style values for XAML pages.
	/// </summary>
	public static class Styles
	{
		internal const double CustomSmall = 11;
		internal const double CustomMedium = 18;

		/// <summary>
		/// The primary blue for the Joker design.
		/// </summary>
		public static readonly Color Primary1 = Color.FromHex("#0369a9");

		/// <summary>
		/// A darker blue used with the primary blue for shading.
		/// </summary>
		public static readonly Color Primary2 = Color.FromHex("#024976");

		/// <summary>
		/// Text color for dark backgrounds.
		/// </summary>
		public static readonly Color TextContrast = Color.FromHex("#fff");

		internal static readonly Thickness PagePadding = new(15, 15, 15, 15);
		internal static readonly Thickness StripMargin = new(0, 5, 0, 5);
		internal static readonly Color Bgr1 = Color.FromHex("#fafafa");
		internal static readonly Color Bgr2 = Color.FromHex("#f3f3f3");
		internal static readonly Color Bgr3 = Color.FromHex("#e1e1e1");
		internal static readonly Color Bgr4 = Color.FromHex("#c8c8c8");
		internal static readonly Color Bgr5 = Color.FromHex("#afafaf");
		internal static readonly Color Text1 = Color.FromHex("#000");
		internal static readonly Color Text2 = Color.FromHex("#777");
		internal static readonly Color Link = Color.FromHex("#4e96c2");
		internal static readonly Color LimitCrossedBgr = Color.FromHex("#ffc0cb");
		internal static readonly Color LimitKeptBgr = Color.FromHex("#90ee90");
		internal static readonly Color LimitCrossedText = Color.FromHex("#75585d");
		internal static readonly Color LimitKeptText = Color.FromHex("#406e40");
	}
}
