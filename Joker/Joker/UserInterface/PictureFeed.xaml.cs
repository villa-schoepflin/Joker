using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// The main page's left tab. A view where the user can shuffle through the different
	/// motivating or informing pictures in the database.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PictureFeed : ContentPage
	{
		/// <summary>
		/// The title of this page, provided here to be accessible from anywhere.
		/// </summary>
		public const string Headline = "Bilder";

		/// <summary>
		/// Initializes XAML elements and sets the binding context.
		/// </summary>
		public PictureFeed()
		{
			InitializeComponent();
			BindingContext = new PictureFeedViewModel(this, Database.MostRecentPicture());
			Picture.Aspect = UserSettings.PreferredAspect;
		}

		/// <summary>
		/// Refreshes the aspect ratio of the picture to whatever the user set in the settings.
		/// </summary>
		public void RefreshPreferredAspect()
		{
			Picture.Aspect = UserSettings.PreferredAspect;
		}
	}
}