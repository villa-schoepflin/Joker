using System.Windows.Input;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Represents a single timeline record for display in timeline-related pages.
	/// </summary>
	public class TimelineRecordViewModel : ViewModel<Page, TimelineRecord>
	{
		/// <summary>
		/// Converts the record's time property to the system's time zone in the 24-hour format.
		/// </summary>
		public string LocalizedTime => Model.Time.ToLocalTime().ToString("dd.MM.yyyy, HH:mm");

		/// <summary>
		/// Converts the amount to a Euro monetary value.
		/// </summary>
		public string AmountInEuro => Model.Amount.ToString("C", App.Locale);

		/// <summary>
		/// The text to be displayed in the remaining limit column in the timeline.
		/// </summary>
		public string RemainingLimit { get; private set; }

		/// <summary>
		/// The icon to be displayed in the timeline based on what type of record it is.
		/// </summary>
		public ImageSource TypeIcon { get; private set; }

		/// <summary>
		/// The primary tinting color used in the timeline.
		/// </summary>
		public Color CellBgrColor { get; private set; }

		/// <summary>
		/// The secondary tinting color used in the timeline.
		/// </summary>
		public Color IconBgrColor { get; private set; }

		/// <summary>
		/// The primary text color used in the timeline.
		/// </summary>
		public Color CellTextColor { get; private set; }

		/// <summary>
		/// Navigates the user to a detailed view of the selected limit or gamble item.
		/// </summary>

		public ICommand OpenDetailPage => new Command(async () =>
		{
			if(Model is Gamble gamble)
				await View.Navigation.PushAsync(new GambleInspector(gamble));
			else
				await View.Navigation.PushAsync(new LimitInspector((Limit)Model));
		});

		/// <summary>
		/// Constructs a row view model based on whether the corresponding timeline record is a
		/// Gamble or a Limit.
		/// </summary>
		/// <param name="view">The view for this view model.</param>
		/// <param name="model">The model for this view model.</param>
		public TimelineRecordViewModel(Page view, TimelineRecord model) : base(view, model)
		{
			if(Model is Gamble gamble)
			{
				RemainingLimit = Database.CalcRemainingLimit(gamble).ToString("C", App.Locale);
				TypeIcon = ImageSource.FromFile($"tl_{gamble.Type.ToString().ToLower()}.png");
				CellBgrColor = App.Color("Bgr3");
				IconBgrColor = App.Color("Bgr4");
				CellTextColor = App.Color("Text1");
			}
			else
			{
				RemainingLimit = null;
				TypeIcon = ImageSource.FromFile("tl_limit.png");
				CellBgrColor = App.Color("Primary1");
				IconBgrColor = App.Color("Primary1");
				CellTextColor = App.Color("TextContrast");
			}
		}
	}
}
