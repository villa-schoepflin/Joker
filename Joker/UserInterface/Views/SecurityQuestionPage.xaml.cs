using Joker.AppInterface;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Page where the user answers their security questions to access the app in case they forgot their password.
	/// </summary>
	public partial class SecurityQuestionPage : ContentPage
	{
		internal SecurityQuestionPage()
		{
			InitializeComponent();

			SecurityQuestion1.Text = UserSettings.FirstSecurityAttribute.Item1;
			SecurityQuestion2.Text = UserSettings.SecondSecurityAttribute.Item1;
		}

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
			App.RequestMainPage();
		}
	}
}
