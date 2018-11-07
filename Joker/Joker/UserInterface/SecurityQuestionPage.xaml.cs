using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// Page where the user answers their security questions to access the app in case they forgot their password.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SecurityQuestionPage : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public SecurityQuestionPage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Text change event handler that checks whether the answer to the first security question is correct.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void CheckFirstSecurityQuestion(object sender, TextChangedEventArgs e)
		{
			if(e.NewTextValue == UserSettings.FirstSecurityAnswer)
				OnSecurityConfirmed((Entry)sender);
		}

		/// <summary>
		/// Text change event handler that checks whether the answer to the first security question is correct.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="e">Contains event data.</param>
		private void CheckSecondSecurityQuestion(object sender, TextChangedEventArgs e)
		{
			if(e.NewTextValue == UserSettings.SecondSecurityAnswer)
				OnSecurityConfirmed((Entry)sender);
		}

		/// <summary>
		/// Navigates the user to the appropriate main page, closing the password dialog pages.
		/// </summary>
		/// <param name="sender">The entry where the user has correctly answered a security question.</param>
		private async void OnSecurityConfirmed(Entry sender)
		{
			Indicator.IsRunning = true;
			sender.Unfocus();
			await Task.Delay(300);
			App.SetMainPageToDefault();
		}
	}
}