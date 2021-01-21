using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// The main page's left tab. A view where the user can shuffle through the different motivating or informing
	/// pictures in the database.
	/// </summary>
	public partial class PictureFeed : ContentPage
	{
		/// <summary>
		/// The title of this page, provided here to be accessible from anywhere.
		/// </summary>
		public const string Headline = "Bilder";

		/// <summary>
		/// The number of pictures initially available after the app was installed.
		/// </summary>
		public const int InitialPictureAmount = 10;

		/// <summary>
		/// Initializes XAML elements and sets the binding context.
		/// </summary>
		public PictureFeed()
		{
			InitializeComponent();
			BindingContext = new PictureFeedViewModel(this, Database.MostRecentPicture());
		}

		/// <summary>
		/// Draws the next image to be presented in the picture feed.
		/// </summary>
		public void RefreshPresentedPicture()
		{
			CanvasView.InvalidateSurface();
		}
	}
}
