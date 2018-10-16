using System;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.BusinessLogic;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user defines the data for a new gamble and can insert it into the database.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddGamblePage : ContentPage
	{
		private bool userChangedDateTime = false;

		/// <summary>
		/// Initializes XAML element and sets the necessary view elements to their default values.
		/// </summary>
		public AddGamblePage()
		{
			InitializeComponent();

			GambleTypePicker.ItemsSource = GambleTypes.Names();
			GambleTypePicker.SelectedIndex = 0;

			TimePicker.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
		}

		/// <summary>
		/// Editor event handler that indicates how many characters can still be inserted into the editor.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void UpdateLengthCounter(object sender, TextChangedEventArgs e)
		{
			LengthCounter.Text = $"noch {Gamble.MaxDescriptionLength - Description.Text.Length} Zeichen";
		}

		/// <summary>
		/// DatePicker event handler that indicates that the user set the date of the gamble.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void DateSelected(object sender, DateChangedEventArgs e)
		{
			userChangedDateTime = true;
		}

		/// <summary>
		/// TimePicker event handler that indicates that the user set the time of the gamble.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void TimeSelected(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == TimePicker.TimeProperty.PropertyName)
				userChangedDateTime = true;
		}

		/// <summary>
		/// Button event handler that resets the date and time of the gamble to the present date and time.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void OnTimeResetButton(object sender, EventArgs e)
		{
			DatePicker.Date = DateTime.Today;
			TimePicker.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
			userChangedDateTime = false;
		}

		/// <summary>
		/// Button event handler that relays input validation, inserts the gamble into the database,
		/// performs necessary refresh actions and navigates the user back to the main page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnSubmitButton(object sender, EventArgs e)
		{
			try
			{
				var type = GambleTypes.GetGambleType(GambleTypePicker.SelectedItem.ToString());
				if(userChangedDateTime)
					Database.Insert(new Gamble(DatePicker.Date + TimePicker.Time, Amount.Text, type, Description.Text));
				else
					Database.Insert(new Gamble(Amount.Text, type, Description.Text));

				App.CurrentTimelineFeed.Refresh();
				App.CurrentSupportPage.RefreshInfo();
				await Navigation.PopAsync();

				if(Database.CalcBalance(Database.MostRecentLimit()) < 0)
				{
					App.CurrentMainPage.CurrentPage = App.CurrentSupportPage;
					App.CurrentSupportPage.FlashLimitFeedback();
				}
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
			}
		}
	}
}