using System;
using Joker.AppInterface;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// The main page's middle tab. A listing of all gambling-related spendings and the limits that
	/// the user sets for themself.
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
		public ImageSource FeedbackTogglerIcon
			=> ImageSource.FromFile(FeedbackHeader.IsVisible ? "ui_remove.png" : "ui_show.png");

		/// <summary>
		/// The data to be displayed, wrapped in view models based on the Limit and Gamble tables of the database.
		/// </summary>
		public TimelineRecordViewModel[] Records
		{
			get
			{
				var timelineRecords = Database.AllGamblesAndLimits();
				return Array.ConvertAll(timelineRecords, tr => new TimelineRecordViewModel(this, tr));
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
		/// Performs a flashing animation on the frame containing the limit feedback.
		/// </summary>
		public void BlinkLimitFeedback()
		{
			FeedbackHeader.IsVisible = true;
			OnPropertyChanged(nameof(FeedbackTogglerIcon));

			const uint colorCount = 2;
			const uint blinkCount = 3;
			const double blinkFrequency = 0.25;

			int cycle = 0;
			Device.StartTimer(TimeSpan.FromSeconds(blinkFrequency), () =>
			{
				string color = (cycle % colorCount) switch
				{
					0 => "Bgr5",
					1 => "Bgr3",
					_ => throw new NotImplementedException(),
				};
				Frame.BackgroundColor = App.Color(color);
				return ++cycle < colorCount * blinkCount;
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
		/// Button event handler that toggles the visibility of the limit feedback text.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void OnToggleFeedbackButton(object sender, EventArgs e)
		{
			FeedbackHeader.IsVisible ^= true;
			OnPropertyChanged(nameof(FeedbackTogglerIcon));
		}

		/// <summary>
		/// Button event handler that navigates the user to a view where they can add a new gamble
		/// to the database.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnSubmitButton(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new AddGamblePage());
		}
	}
}
