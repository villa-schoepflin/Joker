using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user defines the data for a new gamble and can insert it into the database.
	/// </summary>
	public partial class GambleCreator : ContentPage
	{
		private readonly Action Refresh;

		private bool userChangedDateOrTime = false;

		internal GambleCreator(Action refresh)
		{
			InitializeComponent();
			Refresh = refresh;
			TimePicker.Time = DateTime.Now.TimeOfDay;
		}

		private void UpdateLengthCounter(object sender, TextChangedEventArgs eventArgs)
		{
			int remaining = Gamble.MaxDescriptionLength - Description.Text.Length;
			LengthCounter.Text = string.Format(Text.CharsRemaining, remaining);
		}

		private void OnDateSelected(object sender, DateChangedEventArgs eventArgs)
		{
			userChangedDateOrTime = true;
		}

		private void OnTimeSelected(object sender, PropertyChangedEventArgs eventArgs)
		{
			if(eventArgs.PropertyName == TimePicker.TimeProperty.PropertyName)
				userChangedDateOrTime = true;
		}

		private void OnTimeResetButton(object sender, EventArgs eventArgs)
		{
			var now = DateTime.Now;
			DatePicker.Date = now.Date;
			TimePicker.Time = now.TimeOfDay;
			userChangedDateOrTime = false;
		}

		private async void OnSubmitButton(object sender, EventArgs eventArgs)
		{
			if(IsExecuting)
				return;

			IsExecuting = true;
			await CreateGamble();
			IsExecuting = false;
		}
		private bool IsExecuting = false;

		private async Task CreateGamble()
		{
			try
			{
				Gamble gamble;
				var type = GambleTypes.GetGambleType(GambleTypePicker.SelectedItem.ToString());
				if(userChangedDateOrTime)
				{
					var time = DatePicker.Date + TimePicker.Time;
					gamble = new(time, Amount.Text, type, Description.Text);
				}
				else
					gamble = new(Amount.Text, type, Description.Text);

				Database.Insert(gamble);
				_ = await Navigation.PopAsync();
				Refresh.Invoke();
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, Text.Ok);
			}
		}
	}
}
