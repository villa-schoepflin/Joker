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
		/// Generates the feedback concerning the user's current limit.
		/// </summary>
		public string CurrentLimitFeedback
		{
			get
			{
				string s = $"Hey {UserSettings.UserName},\n\n";
				if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
					if(Database.NoGambleAfterMostRecentLimit())
						s += "Du hast seit Deinem letzten Limit nicht gespielt. Das ist gut, aber vergiss nicht all "
							+ "Deine Ausgaben für Glücksspiel in die Chronik einzutragen, damit Du Dein Limit im "
							+ "Auge behältst.";
					else
						s += "bisher hast Du Dein aktuelles Limit eingehalten. Im Moment sind noch "
							+ Database.CalcBalance(Database.MostRecentLimit()).ToString("C", App.Locale)
							+ " von Deinem Limit übrig. Immer weiter so!\n\n"
							+ "Vergiss nicht Deine Ausgaben für Glücksspiel in die Chronik einzutragen, damit Du "
							+ "immer weißt, was der aktuelle Stand Deines Limits ist.";
				else
					s += "diesmal konntest Du Dein Limit leider nicht einhalten. Am besten Du rufst einen Experten "
						+ "oder deinen persönlichen Kontakt an. Du kannst Dir aber auch einige motivierende Bilder "
						+ "anschauen.\n\n"
						+ "Wichtig ist aber auch, dass Du Dein Limit weiter im Blick behältst, auch wenn Du es "
						+ "diesmal überschritten hast. Vergiss also nicht, jede Ausgabe in Deine Chronik einzutragen.";
				return s;
			}
		}

		/// <summary>
		/// Generates the feedback concerning the user's previous limit.
		/// </summary>
		public string PreviousLimitFeedback
		{
			get
			{
				if(Database.CountLimits() > 1)
					if(Database.CalcPreviousLimitBalance() >= 0)
						return "Dein letztes Limit konntest Du einhalten. Ausgezeichnet! Du kannst stolz "
							+ "auf Dich sein. Denk daran, dass jedes Limit ein bisschen niedriger sein sollte als "
							+ "das vorherige. So gewinnst Du Stück für Stück mehr Kontrolle über das Glücksspiel.";
					else
						return "Dein letztes Limit konntest Du leider nicht einhalten. Schade, dass es nicht "
							+ "geklappt hat. Woran es wohl gelegen hat?\n\n"
							+ "Am besten, Du konzentrierst Dich jetzt erstmal darauf, dass Du Dein aktuelles Limit "
							+ "nicht überschreitest. Glaub an Dich! Dieses Mal schaffst Du es sicher.";
				else
					return "";
			}
		}

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
		/// Performs a flashing animation on the frame containing the limit feedback.
		/// </summary>
		public void FlashLimitFeedback()
		{
			if(RecordView.Header == null)
				RecordView.Header = FeedbackHeader;

			int cycle = 0;
			Device.StartTimer(TimeSpan.FromMilliseconds(250), () =>
			{
				Frame.BackgroundColor = App.Color(cycle % 2 == 0 ? "Bgr5" : "Bgr3");
				return ++cycle < 4;
			});
		}

		/// <summary>
		/// Refreshes the info text in this page.
		/// </summary> 
		public void RefreshInfo()
		{
			OnPropertyChanged(nameof(CurrentLimitFeedback));
		}

		/// <summary>
		/// Refreshes the list view's data bindings from a database-supplied collection.
		/// </summary>
		public void RefreshRecords()
		{
			OnPropertyChanged(nameof(Records));
		}

		/// <summary>
		/// Button event handler that removes the limit feedback frame.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void OnCloseFeedbackButton(object sender, EventArgs e)
		{
			RecordView.Header = null;
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