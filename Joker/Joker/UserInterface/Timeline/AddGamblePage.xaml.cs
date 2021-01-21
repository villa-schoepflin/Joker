using System;
using System.ComponentModel;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user defines the data for a new gamble and can insert it into the database.
	/// </summary>
	public partial class AddGamblePage : ContentPage
	{
		private bool userChangedDateOrTime = false;

		/// <summary>
		/// Initializes XAML elements and sets the necessary view elements to their default values.
		/// </summary>
		public AddGamblePage()
		{
			InitializeComponent();

			GambleTypePicker.ItemsSource = GambleTypes.Names();
			GambleTypePicker.SelectedIndex = 0;

			TimePicker.Time = DateTime.Now.TimeOfDay;
		}

		/// <summary>
		/// Editor event handler that indicates how many characters can still be inserted into the editor.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private void UpdateLengthCounter(object sender, TextChangedEventArgs eventArgs)
		{
			int remaining = Gamble.MaxDescriptionLength - Description.Text.Length;
			LengthCounter.Text = string.Format(Text.CharsRemaining, remaining);
		}

		/// <summary>
		/// DatePicker event handler that indicates that the user set the date of the gamble.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private void OnDateSelected(object sender, DateChangedEventArgs eventArgs)
		{
			userChangedDateOrTime = true;
		}

		/// <summary>
		/// TimePicker event handler that indicates that the user set the time of the gamble.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private void OnTimeSelected(object sender, PropertyChangedEventArgs eventArgs)
		{
			if(eventArgs.PropertyName == TimePicker.TimeProperty.PropertyName)
				userChangedDateOrTime = true;
		}

		/// <summary>
		/// Button event handler that resets the date and time of the gamble to the present date and time.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private void OnTimeResetButton(object sender, EventArgs eventArgs)
		{
			var now = DateTime.Now;
			DatePicker.Date = now.Date;
			TimePicker.Time = now.TimeOfDay;
			userChangedDateOrTime = false;
		}

		/// <summary>
		/// Button event handler that relays input validation, inserts the gamble into the database, performs necessary
		/// refresh actions and navigates the user back to the main page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private async void OnSubmitButton(object sender, EventArgs eventArgs)
		{
			try
			{
				var type = GambleTypes.GetGambleType(GambleTypePicker.SelectedItem.ToString());
				if(userChangedDateOrTime)
				{
					var time = DatePicker.Date + TimePicker.Time;
					Database.Insert(new Gamble(time, Amount.Text, type, Description.Text));
				}
				else
					Database.Insert(new Gamble(Amount.Text, type, Description.Text));

				App.CurrentTimelineFeed.RefreshRecords();
				App.CurrentTimelineFeed.RefreshInfo();
				await Navigation.PopAsync();

				if(Database.CalcBalance(Database.MostRecentLimit()) < 0)
					App.CurrentTimelineFeed.BlinkLimitFeedback();
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}
	}
}
