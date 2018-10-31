using System;
using System.Collections.ObjectModel;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.BusinessLogic;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// The main page's right tab. A view that lists several useful contacts as well as giving
	/// feedback on whether the user was successful in keeping his last limits.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SupportPage : ContentPage
	{
		/// <summary>
		/// The title of this page, provided here to be accessible from anywhere.
		/// </summary>
		public const string Headline = "Unterstützung";

		/// <summary>
		/// Info text for the user concerning the functionality of this page.
		/// </summary>
		public const string Info = "Auf dieser Seite findest Du einige nützliche Telefonnummern, die Du bei "
			+ "Bedarf sofort anrufen kannst. Außerdem gibt es hier ein paar Tipps und Infos zum Einhalten deines "
			+ "gesetzten Limits.\n\n"
			+ "Wenn Du Deinen persönlich ausgewählten Kontakt ändern möchtest, kannst Du das in den Einstellungen "
			+ "tun. Tippe dazu auf das Zahnradsymbol.";

		/// <summary>
		/// Checks if there is more than one limit in the database, otherwise you couldn't give
		/// the user feedback on a previous limit.
		/// </summary>
		public bool MoreThanOneLimit => Database.CountLimits() > 1;

		/// <summary>
		/// Generates the feedback concerning the user's current limit.
		/// </summary>
		public string CurrentLimitFeedback => $"Hey {UserSettings.UserName},\n\n" +
			(Database.CalcBalance(Database.MostRecentLimit()) >= 0
				? (Database.NoGambleAfterMostRecentLimit()
					? "Du hast seit Deinem letzten Limit nicht gespielt. Das ist gut, aber vergiss nicht all "
						+ "Deine Ausgaben für Glücksspiel in die Chronik einzutragen, damit Du Dein Limit im "
						+ "Auge behältst."
					: "bisher hast Du Dein aktuelles Limit eingehalten. Im Moment sind noch "
						+ Database.CalcBalance(Database.MostRecentLimit()).ToString("C", App.Locale)
						+ " von Deinem Limit übrig. Immer weiter so!\n\n"
						+ "Vergiss nicht Deine Ausgaben für Glücksspiel in die Chronik einzutragen, damit Du "
						+ "immer weißt, was der aktuelle Stand Deines Limits ist.")
				: "diesmal konntest Du Dein Limit leider nicht einhalten. Am besten Du rufst einen Experten "
					+ "oder deinen persönlichen Kontakt an. Du kannst Dir auch ein paar motivierende Bilder "
					+ "anschauen.\n\n"
					+ "Wichtig ist aber auch, dass Du Dein Limit weiter im Blick behältst, auch wenn Du es "
					+ "diesmal überschritten hast. Vergiss also nicht, jede Ausgabe in Deine Chronik einzutragen.");

		/// <summary>
		/// Generates the feedback concerning the user's previous limit.
		/// </summary>
		public string PreviousLimitFeedback => MoreThanOneLimit
			? (Database.CalcPreviousLimitBalance() >= 0
				? "Dein letztes Limit konntest Du einhalten. Ausgezeichnet! Du kannst stolz "
					+ "auf Dich sein. Denk daran, dass jedes Limit ein bisschen niedriger sein sollte als "
					+ "das vorherige. So gewinnst Du Stück für Stück mehr Kontrolle über das Glücksspiel."
				: "Dein letztes Limit konntest Du leider nicht einhalten. Schade, dass es nicht geklappt "
					+ "hat. Woran es wohl gelegen hat?\n\n"
					+ "Am besten, Du konzentrierst Dich jetzt erstmal darauf, dass Du Dein aktuelles Limit "
					+ "nicht überschreitest. Glaub an Dich! Dieses Mal schaffst Du es sicher.")
			: "";

		/// <summary>
		/// A list of contacts the user can call immediately.
		/// </summary>
		public ObservableCollection<Contact> Contacts
		{
			get
			{
				var list = new ObservableCollection<Contact> { Contact.Bzga };
				if(UserSettings.PersonalContact != null)
					list.Add(UserSettings.PersonalContact);
				return list;
			}
		}

		/// <summary>
		/// Initializes XAML elements and binds the view data.
		/// </summary>
		public SupportPage()
		{
			InitializeComponent();
			BindingContext = this;
		}

		/// <summary>
		/// Performs a flashing animation on the frame containing the limit feedback.
		/// </summary>
		public void FlashLimitFeedback()
		{
			int cycle = 0;
			Device.StartTimer(TimeSpan.FromMilliseconds(250), () =>
			{
				Frame.BackgroundColor = App.Color(cycle % 2 == 0 ? "Bgr5" : "Bgr3");
				return ++cycle < 6;
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
		/// Refreshes the contacts data binding in this view.
		/// </summary>
		public void RefreshContacts()
		{
			OnPropertyChanged(nameof(Contacts));
		}

		/// <summary>
		/// Button event handler that opens the platform's telephone app with the corresponding item's phone number.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void OnPhoneButton(object sender, EventArgs e)
		{
			PhoneDialer.Open(((Contact)((Button)sender).BindingContext).PhoneNumber);
		}
	}
}