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
		internal const string Headline = "Bilder";
		internal const int InitialPictureAmount = 10;

		internal PictureFeed()
		{
			InitializeComponent();
			BindingContext = new PictureFeedViewModel(this, Database.MostRecentPicture());
		}

		internal void RefreshPresentedPicture()
		{
			CanvasView.InvalidateSurface();
		}
	}
}
