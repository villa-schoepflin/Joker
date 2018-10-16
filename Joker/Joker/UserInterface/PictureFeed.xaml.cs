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
		/// Info text for the user concerning the functionality of this page.
		/// </summary>
		public const string Info = "Auf dieser Seite kannst Du Dich durch verschiedene motivierende Bilder klicken, "
			+ "indem Du jedesmal doppelt auf das aktuell angezeigte Bild tippst.\n\n"
			+ "Wenn Du ein Bild mit \"Gefällt mir\" markierst, wird es häufiger erscheinen. Wenn Du auf den "
			+ "Speicherknopf drückst, wird das aktuelle Bild in Deiner Galerie-App abgelegt.\n\n"
			+ "Du wirst benachrichtigt, sobald die App ein neues Bild für Dich hat. Das neueste Bild wird stets "
			+ "zuerst angezeigt. Neue Bilder bekommst Du in regelmäßigen Abständen.";

		/// <summary>
		/// Initializes XAML elements sets binding context.
		/// </summary>
		public PictureFeed()
		{
			InitializeComponent();
			BindingContext = new PictureFeedViewModel(this, Database.MostRecentPicture());
		}
	}
}