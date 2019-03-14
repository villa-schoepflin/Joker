using Xamarin.Forms;

using Joker.BusinessLogic;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// View model for a limit in the limit inspector. Uses many properties of the timeline record view model.
	/// </summary>
	public class LimitViewModel : TimelineRecordViewModel
	{
		/// <summary>
		/// Wrapper in order to treat the model as a limit because of inheritance from TimelineRecordViewModel.
		/// </summary>
		private Limit OwnModel
		{
			get => (Limit)Model;
			set => Model = value;
		}

		/// <summary>
		/// The currently remaining balance of the limit.
		/// </summary>
		public string Balance => Database.CalcBalance(OwnModel).ToString("C", App.Locale);

		/// <summary>
		/// The duration in days.
		/// </summary>
		public string DurationInDays => $"{OwnModel.Duration.TotalDays} Tage";

		/// <summary>
		/// A text indicating the state of the limit.
		/// </summary>
		public string LimitState => Balance.StartsWith("-") ? "Limit überschritten" : "Limit eingehalten";

		/// <summary>
		/// The color marking the state of the limit.
		/// </summary>
		public Color LimitStateBackground => Color.FromHex(Balance.StartsWith("-") ? "#ffc0cb" : "#90ee90");

		/// <summary>
		/// The color for the text that indicates the limit state.
		/// </summary>
		public Color LimitStateTextColor => Color.FromHex(Balance.StartsWith("-") ? "#75585d" : "#406e40");

		/// <summary>
		/// Constructs the view model for a limit.
		/// </summary>
		/// <param name="view">The page for this view model.</param>
		/// <param name="model">The limit around which to construct the view model.</param>
		public LimitViewModel(Page view, Limit model) : base(view, model) { }
	}
}