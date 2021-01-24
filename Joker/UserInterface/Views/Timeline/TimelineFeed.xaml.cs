using System;
using Joker.AppInterface;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// The main page's middle tab. A list of gambling-related spendings and the limits that the user sets for themself.
	/// </summary>
	public partial class TimelineFeed : ContentPage
	{
		/// <summary>
		/// The title of this page, provided here to be accessible from anywhere.
		/// </summary>
		public const string Headline = "Chronik";

		/// <summary>
		/// Generates the feedback concerning the user's current limit.
		/// </summary>
		public string CurrentLimitFeedback
		{
			get
			{
				if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
				{
					if(Database.NoGambleAfterMostRecentLimit())
						return TextAssetReader.Get("Feedback_Current_New.txt");
					else
						return TextAssetReader.Get("Feedback_Current_Success.txt");
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
		/// The data to be displayed, wrapped in view models based on the Limit and Gamble tables of the database.
		/// </summary>
		public TimelineRecordViewModel[] Records
		{
			get
			{
				var records = Database.AllGamblesAndLimits();
				return Array.ConvertAll(records, tr => new TimelineRecordViewModel(this, tr));
			}
		}

		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public TimelineFeed()
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

		/// <summary>
		/// Refreshes the list view's data bindings and blinks the limit feedback box if the limit has been crossed.
		/// </summary>
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

		/// <summary>
		/// Button event handler that toggles the visibility of the limit feedback text.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private void OnToggleFeedbackButton(object sender, EventArgs eventArgs)
		{
			FeedbackText.IsVisible ^= true;
			OnPropertyChanged(nameof(FeedbackTogglerIcon));
		}

		/// <summary>
		/// Button event handler that navigates the user to a view where they can add a new gamble to the database.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void OnSubmitButton(object sender, EventArgs eventArgs)
		{
			if(Navigation.HasPage<AddGamblePage>())
				return;

			AddGamblePage page = new(() =>
			{
				RefreshRecords();
				RefreshFeedback();
			});
			await Navigation.PushAsync(page);
		}
	}
}
