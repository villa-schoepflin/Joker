using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.AppInterface;
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
		/// Generates the feedback concerning the user's current limit.
		/// </summary>
		public string CurrentLimitFeedback
		{
			get
			{
				if(Database.CalcBalance(Database.MostRecentLimit()) >= 0)
				{
					if(Database.NoGambleAfterMostRecentLimit())
						return FileResourceReader.Get("Feedback_Current_New.txt");
					else
						return FileResourceReader.Get("Feedback_Current_Success.txt");
				}
				else
					return FileResourceReader.Get("Feedback_Current_Failure.txt");
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
						return FileResourceReader.Get("Feedback_Previous_Success.txt");
					else
						return FileResourceReader.Get("Feedback_Previous_Failure.txt");
				}
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