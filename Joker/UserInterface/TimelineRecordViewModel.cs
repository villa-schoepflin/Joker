using System.Windows.Input;
using Joker.AppInterface;
using Joker.BusinessLogic;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Represents a single timeline record for display in timeline-related pages.
	/// </summary>
	public abstract class TimelineRecordViewModel : ViewModel<Page, TimelineRecord>
	{
		/// <summary>
		/// The icon to be displayed in the timeline based on what type of record it is.
		/// </summary>
		public abstract ImageSource TypeIcon { get; }

		/// <summary>
		/// The primary tinting color used in the timeline.
		/// </summary>
		public abstract Color CellBackground { get; }

		/// <summary>
		/// The secondary tinting color used in the timeline.
		/// </summary>
		public abstract Color IconBackground { get; }

		/// <summary>
		/// The primary text color used in the timeline.
		/// </summary>
		public abstract Color CellTextColor { get; }

		/// <summary>
		/// The text to be displayed in the remaining limit column in the timeline.
		/// </summary>
		public abstract string RemainingLimit { get; }

		/// <summary>
		/// Navigates the user to a detailed view of the selected limit or gamble item.
		/// </summary>
		public abstract ICommand OpenInspector { get; }

		/// <summary>
		/// Converts the record's time property to the system's time zone in the 24-hour format.
		/// </summary>
		public string LocalizedTime => Model.Time.ToLocalTime().ToString("dd.MM.yyyy, HH:mm");

		/// <summary>
		/// Converts the amount to a Euro monetary value.
		/// </summary>
		public string AmountInEuro => Model.Amount.ToString("C", App.Locale);

		/// <summary>
		/// Constructs a timeline record view model.
		/// </summary>
		/// <param name="view">The view for this view model.</param>
		/// <param name="model">The model for this view model.</param>
		protected TimelineRecordViewModel(Page view, TimelineRecord model) : base(view, model) { }
	}
}
