using System.Windows.Input;
using Joker.AppInterface;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// View containing information about the creator and publisher of the app.
	/// </summary>
	public partial class Impressum : ContentPage
	{
		/// <summary>
		/// Contains the information of the app version.
		/// </summary>
		public string Version => TextAssetReader.Get("Impressum_Version.txt");

		/// <summary>
		/// Contains contact information about the publisher.
		/// </summary>
		public string ContactInfo => TextAssetReader.Get("Impressum_ContactInfo.txt");

		/// <summary>
		/// Contains the privacy policy.
		/// </summary>
		public string PrivacyPolicy => TextAssetReader.Get("Impressum_PrivacyPolicy.txt");

		/// <summary>
		/// Event command that opens the platform-specific email app.
		/// </summary>
		public ICommand OpenEmail => new Command<string>(async recipient =>
		{
			EmailMessage email = new("", "", recipient);
			await Email.ComposeAsync(email);
		});

		/// <summary>
		/// Event command that opens the platform-specific browser.
		/// </summary>
		public ICommand OpenWebsite
			=> new Command<string>(async url => await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred));

		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public Impressum()
		{
			InitializeComponent();
			BindingContext = this;
		}
	}
}
