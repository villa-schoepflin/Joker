using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.BusinessLogic;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user must set their first limit.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TourPage3 : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public TourPage3()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Button event handler that navigates the user to the final tour page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnContinueButton(object sender, EventArgs e)
		{
			try
			{
				await Navigation.PushAsync(new Finish(new Limit(LimitEntry.Text, "7")));
			}
			catch(ArgumentException error)
			{
				await DisplayAlert(null, error.Message, "Ok");
			}
		}
	}
}