using System;
using Joker.AppInterface;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// The main page's middle tab. A list of gambling-related spendings and the limits that the user sets for themself.
	/// </summary>
	public partial class TimelineFeed : ContentPage
	{
		internal const string Headline = "Chronik";

		/// <summary>
		/// Generates the feedback concerning the user's current limit.
		/// </summary>
		public string CurrentLimitFeedback
		{
			get
			{
				if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
				{
					if(Database.HasGambleAfterMostRecentLimit())
						return TextAssetReader.Get("Feedback_Current_Success.txt");
					else
						return TextAssetReader.Get("Feedback_Current_New.txt");
				}
				else
					return TextAssetReader.Get("Feedback_Current_Failure.txt");
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
				{
					if(Database.CalcPreviousLimitBalance() >= 0)
						return "\n" + TextAssetReader.Get("Feedback_Previous_Success.txt");
					else
						return "\n" + TextAssetReader.Get("Feedback_Previous_Failure.txt");
				}
				else
				{
					PreviousLimitFeedbackLabel.IsVisible = false;
					return "";
				}
			}
		}

		/// <summary>
		/// Determines the icon shown depending on whether the feedback text is visible.
		/// </summary>
		public ImageSource FeedbackTogglerIcon => FeedbackText.IsVisible ? Icons.Remove : Icons.Show;

		/// <summary>
		/// The data to be displayed, wrapped in view models based on whether they are gambles or limits.
		/// </summary>
		public TimelineRecordViewModel[] Records => Array.ConvertAll(Database.AllGamblesAndLimits(), record =>
		{
			TimelineRecordViewModel viewModel;
			if(record is Gamble gamble)
				viewModel = new GambleViewModel(this, gamble);
			else
				viewModel = new LimitViewModel(this, (Limit)record);
			return viewModel;
		});

		internal TimelineFeed()
		{
			InitializeComponent();
			BindingContext = this;
		}

		/// <summary>
		/// Refreshes the limit feedback text in this page.
		/// </summary> 
		public void RefreshFeedback()
		{
			OnPropertyChanged(nameof(CurrentLimitFeedback));
		}

		private void RefreshRecords()
		{
			OnPropertyChanged(nameof(Records));
			if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
				return;

			FeedbackText.IsVisible = true;
			OnPropertyChanged(nameof(FeedbackTogglerIcon));

			const uint blinkCount = 3;
			const double blinkFrequency = 0.25;
			int cycle = 0;
			Device.StartTimer(TimeSpan.FromSeconds(blinkFrequency), () =>
			{
				Header.BackgroundColor = cycle % 2 == 0 ? Styles.Bgr5 : Styles.Bgr3;
				return ++cycle < blinkCount * 2;
			});
		}

		private void OnToggleFeedbackButton(object sender, EventArgs eventArgs)
		{
			FeedbackText.IsVisible ^= true;
			OnPropertyChanged(nameof(FeedbackTogglerIcon));
		}

		private async void OnSubmitButton(object sender, EventArgs eventArgs)
		{
			if(Navigation.HasPage<GambleCreator>())
				return;

			GambleCreator creator = new(() =>
			{
				RefreshRecords();
				RefreshFeedback();
			});
			await Navigation.PushAsync(creator);
		}
	}
}
