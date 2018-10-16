using Xamarin.Forms;
using Joker.BusinessLogic;

namespace Joker.UserInterface
{
	/// <summary>
	/// View model for a gamble in the gamble inspector. Uses many properties of the timeline record view model.
	/// </summary>
	public class GambleViewModel : TimelineRecordViewModel
	{
		/// <summary>
		/// The type of this gamble localized.
		/// </summary>
		public string GambleType => GambleTypes.GetName(((Gamble)Model).Type);

		/// <summary>
		/// The description of this gamble as supplied from a model object.
		/// </summary>
		public string DescriptionText => string.IsNullOrEmpty(((Gamble)Model).Description)
			? "Keine Beschreibung vorhanden."
			: ((Gamble)Model).Description;

		/// <summary>
		/// The color of the description text.
		/// </summary>
		public Color DescriptionColor => string.IsNullOrEmpty(((Gamble)Model).Description)
			? App.Color("Text2")
			: CellText;

		/// <summary>
		/// Constructs the view model for a gamble.
		/// </summary>
		/// <param name="view">The page for this view model.</param>
		/// <param name="model">The gamble around which to construct the view model.</param>
		public GambleViewModel(Page view, Gamble model) : base(view, model) { }
	}
}