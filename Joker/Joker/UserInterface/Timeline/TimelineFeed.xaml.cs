using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// The main page's middle tab. A listing of all gambling-related spendings and
	/// the limits that the user sets for themself.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TimelineFeed : ContentPage
	{
		/// <summary>
		/// The title of this page, provided here to be accessible from anywhere.
		/// </summary>
		public const string Headline = "Chronik";

		/// <summary>
		/// Info text for the user concerning the functionality of this page.
		/// </summary>
		public const string Info = "Auf dieser Seite findest Du einen Überblick über Deine Limits (blau markiert) "
			+ "und Glücksspielausgaben (grau markiert), die Du in der App festgehalten hast.\n\n"
			+ "Wenn Du eine neue Ausgabe hinzufügen möchtest, tippe auf das \"+\" in der unteren rechten Ecke. Das "
			+ "Hinzufügen eines neuen Limits ist erst möglich, wenn das aktuell bestehende Limit abgelaufen ist.\n\n"
			+ "Details zu jedem Limit und jeder Ausgabe findest Du, wenn Du den dazugehörigen Eintrag antippst.";

		/// <summary>
		/// The data to be displayed, wrapped in view models based on the Limit and Gamble tables of the database.
		/// </summary>
		public TimelineRecordViewModel[] Records
			=> Array.ConvertAll(Database.AllGamblesAndLimits(), tr => new TimelineRecordViewModel(this, tr));

		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public TimelineFeed()
		{
			InitializeComponent();
			BindingContext = this;
		}

		/// <summary>
		/// Refreshes the list view's data bindings from a database-supplied collection.
		/// </summary>
		public void Refresh()
		{
			OnPropertyChanged(nameof(Records));
		}

		/// <summary>
		/// Button event handler that navigates the user to a view where they can add a new gamble to the database.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnSubmitButton(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new AddGamblePage());
		}
	}
}