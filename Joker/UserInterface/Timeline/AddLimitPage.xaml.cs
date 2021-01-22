using System;
using Joker.AppInterface;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Unclosable view shown only when opening the app after a limit has expired, forcing the user to set a new one.
	/// </summary>
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
					return TextAssetReader.Get("AddNewLimit_Success.txt");
				else
					return TextAssetReader.Get("AddNewLimit_Failure.txt");
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
		/// Button event handler that relays input validation, inserts the new limit into the database, schedules the
		/// corresponding notifications and gets the user to the main page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void OnContinueButton(object sender, EventArgs eventArgs)
		{
			try
			{
				Indicator.IsRunning = true;
				DependencyService.Get<IPlatformNotifier>().CancelLimitExpired();

				Limit limit = new(AmountEntry.Text, DurationEntry.Text);
				Database.Insert(limit);
				AppSettings.LimitExpiredTime = limit.Time + limit.Duration;
				DependencyService.Get<IPlatformNotifier>().ScheduleLimitExpired(AppSettings.LimitExpiredTime);

				App.SetMainPageToDefault();
			}
			catch(ArgumentException error)
			{
				Indicator.IsRunning = false;
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}
	}
}
