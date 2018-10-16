using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.ApplicationLayer;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user can select their personal contact.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TourPage2 : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public TourPage2()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Entry event handler that opens the contact-picking dialog to the user,
		/// relays data to this view and saves the selected contact.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void PickContact(object sender, EventArgs e)
		{
			ContactEntry.Unfocus();
			UserSettings.PersonalContact = await DependencyService.Get<IPlatformContactPicker>().PickContact();
			ContactEntry.Text = UserSettings.PersonalContact.Name;
		}

		/// <summary>
		/// Button event handler that navigates the user to the next tour page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnContinueButton(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new TourPage3());
		}
	}
}