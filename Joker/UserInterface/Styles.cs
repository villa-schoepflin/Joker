using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Holds commonly used style values for XAML pages.
	/// </summary>
	public static class Styles
	{
		/// <summary>
		/// Regular padding for pages.
		/// </summary>
		public static readonly Thickness PagePadding = new(15, 15, 15, 15);

		/// <summary>
		/// Padding for the light-gray sections used in several pages.
		/// </summary>
		public static readonly Thickness StripMargin = new(0, 5, 0, 5);

		/// <summary>
		/// Used mainly as background color for most pages.
		/// </summary>
		public static readonly Color Bgr1 = Color.FromHex("#fafafa");

		/// <summary>
		/// Used mainly as background color for small page sections.
		/// </summary>
		public static readonly Color Bgr2 = Color.FromHex("#f3f3f3");

		/// <summary>
		/// Used mainly as background color for most buttons and highlighted sections.
		/// </summary>
		public static readonly Color Bgr3 = Color.FromHex("#e1e1e1");

		/// <summary>
		/// Used mainly as background color for buttons in highlighted sections.
		/// </summary>
		public static readonly Color Bgr4 = Color.FromHex("#c8c8c8");

		/// <summary>
		/// Used mainly as background color for important buttons.
		/// </summary>
		public static readonly Color Bgr5 = Color.FromHex("#afafaf");

		/// <summary>
		/// The primary blue for the Joker design.
		/// </summary>
		public static readonly Color Primary1 = Color.FromHex("#0369a9");

		/// <summary>
		/// A darker blue used with the primary blue for shading.
		/// </summary>
		public static readonly Color Primary2 = Color.FromHex("#024976");

		/// <summary>
		/// Primary text color.
		/// </summary>
		public static readonly Color Text1 = Color.FromHex("#000");

		/// <summary>
		/// Secondary text color for less visible text.
		/// </summary>
		public static readonly Color Text2 = Color.FromHex("#777");

		/// <summary>
		/// Text color for dark backgrounds.
		/// </summary>
		public static readonly Color TextContrast = Color.FromHex("#fff");

		/// <summary>
		/// Text color for hyperlinks.
		/// </summary>
		public static readonly Color Link = Color.FromHex("#4e96c2");

		/// <summary>
		/// Background color associated with crossed limits.
		/// </summary>
		public static readonly Color LimitCrossedBgr = Color.FromHex("#ffc0cb");

		/// <summary>
		/// Background color associated with kept limits.
		/// </summary>
		public static readonly Color LimitKeptBgr = Color.FromHex("#90ee90");

		/// <summary>
		/// Text color associated with crossed limits.
		/// </summary>
		public static readonly Color LimitCrossedText = Color.FromHex("#75585d");

		/// <summary>
		/// Text color associated with kept limits.
		/// </summary>
		public static readonly Color LimitKeptText = Color.FromHex("#406e40");

		/// <summary>
		/// Small text size.
		/// </summary>
		public const double CustomSmall = 11;

		/// <summary>
		/// Medium text size.
		/// </summary>
		public const double CustomMedium = 18;
	}
}
