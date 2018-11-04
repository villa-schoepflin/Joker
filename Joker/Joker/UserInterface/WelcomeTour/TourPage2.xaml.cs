using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.ApplicationLayer;
using Joker.BusinessLogic;

namespace Joker.UserInterface
{
	/// <summary>
	/// View where the user can select their personal contact.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TourPage2 : ContentPage
	{
		/// <summary>
		/// The contact that can be selected here and will be inserted upon completing the welcome tour.
		/// </summary>
		public static Contact FirstContact;

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
			FirstContact = await DependencyService.Get<IPlatformContactPicker>().PickContact();
			ContactEntry.Text = FirstContact.Name;
		}

		/// <summary>
		/// Button event handler that navigates the user to the next tour page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private async void OnContinueButton(object sender, EventArgs e)
		{
			if((object)FirstContact != null && FirstContact == Contact.Bzga)
				await DisplayAlert(null, "Diese Nummer ist bereits in der App verzeichnet.", "Ok");
			else
				await Navigation.PushAsync(new TourPage3());
		}
	}
}