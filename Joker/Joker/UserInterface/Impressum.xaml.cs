using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Joker.UserInterface
{
	/// <summary>
	/// View containing information about the creator and publisher of the app.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Impressum : ContentPage
	{
		/// <summary>
		/// Contains the information of the app version.
		/// </summary>
		public string VersionText => "Joker – Behalte die Kontrolle!\n"
			+ "\n"
			+ "Joker." + (Device.RuntimePlatform == Device.iOS ? "iOS" : "Android") + " Version 1.0.9\n"
			+ "© 2018 Villa Schöpflin gGmbH\n"
			+ "Alle Rechte vorbehalten.";

		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public Impressum()
		{
			InitializeComponent();
			BindingContext = this;
		}

		/// <summary>
		/// Event command that opens the platform-specific email app.
		/// </summary>
		public ICommand OpenEmail => new Command<string>(async recipient =>
		{
			await Email.ComposeAsync(new EmailMessage { To = new List<string> { recipient } });
		});

		/// <summary>
		/// Event command that opens the platform-specific browser.
		/// </summary>
		public ICommand OpenWebsite => new Command<string>(async url =>
		{
			await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
		});
	}
}