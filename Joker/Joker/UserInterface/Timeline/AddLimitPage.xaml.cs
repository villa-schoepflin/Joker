using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.AppInterface;
using Joker.BusinessLogic;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// Inescapable view presented only when opening up the app after a limit has expired,
	/// forcing the user to set a new one.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddLimitPage : ContentPage
	{
		/// <summary>
		/// Provides a feedback text concerning whether the previous limit was exceeded.
		/// </summary>
		public string LimitInfo => UserSettings.UserName + ", " +
			(Database.CalcBalance(Database.MostRecentLimit()) >= 0
				? "Du konntest Dein letztes Limit einhalten. Du kannst stolz auf Dich sein! Jetzt ist es "
					+ "erstmal Zeit Dein nächstes Limit festzulegen, am besten ein bisschen niedriger."
				: "leider konntest Du Dein letztes Limit nicht einhalten. Aber das ist nicht so schlimm. "
					+ "Überlege Dir gut wie Du Dein nächstes Limit setzt. Diesmal schaffst Du es sicher!")
			+ $"\n\nDein letztes Limit war {Database.MostRecentLimit().Amount.ToString("C", App.Locale)}.";

		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public AddLimitPage()
		{
			InitializeComponent();
			BindingContext = this;
		}

		/// <summary>
		/// Button event handler that relays input validation, inserts the new limit into the database,
		/// schedules the corresponding notifications and directs the user to the main page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnContinueButton(object sender, EventArgs e)
		{
			try
			{
				DependencyService.Get<IPlatformNotifier>().CancelLimitExpired();

				var limit = new Limit(AmountEntry.Text, DurationEntry.Text);
				Database.Insert(limit);
				AppSettings.LimitExpiredTime = limit.Time + limit.Duration;
				DependencyService.Get<IPlatformNotifier>().ScheduleLimitExpired(AppSettings.LimitExpiredTime);

				App.SetMainPageToDefault();
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
			}
		}
	}
}