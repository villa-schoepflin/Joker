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
		public string LimitInfo
		{
			get
			{
				if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
					return FileResourceReader.Get("AddNewLimit_Success.txt");
				else
					return FileResourceReader.Get("AddNewLimit_Failure.txt");
			}
		}

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
				Indicator.IsRunning = true;
				DependencyService.Get<IPlatformNotifier>().CancelLimitExpired();

				var limit = new Limit(AmountEntry.Text, DurationEntry.Text);
				Database.Insert(limit);
				AppSettings.LimitExpiredTime = limit.Time + limit.Duration;
				DependencyService.Get<IPlatformNotifier>().ScheduleLimitExpired(AppSettings.LimitExpiredTime);

				Application.Current.MainPage = new NavigationPage(new MainPage());
			}
			catch(ArgumentException error)
			{
				Indicator.IsRunning = false;
				await DisplayAlert(null, error.Message, "Ok");
			}
		}
	}
}