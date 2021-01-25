using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Page where the user answers their security questions to access the app in case they forgot their password.
	/// </summary>
	public partial class SecurityQuestionPage : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements.
		/// </summary>
		public SecurityQuestionPage()
		{
			InitializeComponent();

			SecurityQuestion1.Text = UserSettings.FirstSecurityAttribute.Item1;
			SecurityQuestion2.Text = UserSettings.SecondSecurityAttribute.Item1;
		}

		/// <summary>
		/// Text change event handler that checks whether the answer to the first security question is correct. If it's
		/// correct, the user gets navigated to the main page.
		/// </summary>
		/// <param name="sender">Reference to the event's source object.</param>
		/// <param name="eventArgs">Contains event data.</param>
		private void CheckSecurityAnswer(object sender, TextChangedEventArgs eventArgs)
		{
			string correctAnswer;
			if(sender == FirstSecurityAnswerEntry)
				correctAnswer = UserSettings.FirstSecurityAttribute.Item2;
			else
				correctAnswer = UserSettings.SecondSecurityAttribute.Item2;

			if(eventArgs.NewTextValue != correctAnswer)
				return;

			Indicator.IsRunning = true;
			((Entry)sender).Unfocus();
			JokerApp.RequestMainPage();
		}
	}
}
