using System;
using System.Threading.Tasks;
using Joker.AppInterface;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Unclosable view shown only when opening the app after a limit has expired, forcing the user to set a new one.
	/// </summary>
	public partial class LimitCreator : ContentPage
	{
		/// <summary>
		/// Provides a feedback text concerning whether the previous limit was crossed.
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

		internal LimitCreator()
		{
			InitializeComponent();
			BindingContext = this;
		}

		private async void OnContinueButton(object sender, EventArgs eventArgs)
		{
			if(IsExecuting)
				return;

			IsExecuting = true;
			await CreateLimit();
			IsExecuting = false;
		}
		private bool IsExecuting = false;

		private async Task CreateLimit()
		{
			try
			{
				Indicator.IsRunning = true;
				var notifier = DependencyService.Get<IPlatformNotifier>();
				notifier.CancelLimitExpired();

				Limit limit = new(AmountEntry.Text, DurationEntry.Text);
				Database.Insert(limit);

				AppSettings.LimitExpiredTime = limit.Time + limit.Duration;
				notifier.ScheduleLimitExpired(AppSettings.LimitExpiredTime);

				App.RequestMainPage();
			}
			catch(ArgumentException error)
			{
				Indicator.IsRunning = false;
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}
	}
}
