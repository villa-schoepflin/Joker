using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Joker.BusinessLogic;

namespace Joker.UserInterface
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ContactInspector : ContentPage
	{
		public ContactInspector(Contact contact)
		{
			InitializeComponent();
		}
	}
}