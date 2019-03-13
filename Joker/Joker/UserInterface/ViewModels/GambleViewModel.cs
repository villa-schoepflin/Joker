using System.Windows.Input;

using Xamarin.Forms;

using Joker.BusinessLogic;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// View model for a gamble in the gamble inspector. Uses many properties of the timeline record view model.
	/// </summary>
	public class GambleViewModel : TimelineRecordViewModel
	{
		/// <summary>
		/// Wrapper in order to treat the model as a gamble because of inheritance from TimelineRecordViewModel.
		/// </summary>
		private Gamble OwnModel
		{
			get => (Gamble)Model;
			set => Model = value;
		}

		/// <summary>
		/// The type of this gamble as the proper translation.
		/// </summary>
		public string GambleType => GambleTypes.GetName(OwnModel.Type);

		/// <summary>
		/// The description of this gamble as supplied from a model object.
		/// </summary>
		public string Description
		{
			get => OwnModel.Description;
			set => OwnModel.Description = value;
		}

		/// <summary>
		/// Indicates whether the description of the gamble can currently be edited by the user.
		/// </summary>
		public bool DescriptionEditable
		{
			get => _descriptionEditable;
			set
			{
				_descriptionEditable = value;
				OnPropertyChanged(nameof(DescriptionEditable));
				OnPropertyChanged(nameof(EditButtonText));
				OnPropertyChanged(nameof(EditButtonIcon));
			}
		}
		private bool _descriptionEditable;

		/// <summary>
		/// Determines the text of the button that toggles the editing status.
		/// </summary>
		public string EditButtonText => DescriptionEditable ? "Speichern" : "Bearbeiten";

		/// <summary>
		/// Determines the icon shown in the button that toggles the editing status.
		/// </summary>
		public ImageSource EditButtonIcon => DescriptionEditable ? ImageSource.FromFile("ui_save.png") : null;

		/// <summary>
		/// Toggles the editability of the gamble's description, saving the changes when deactivating editing.
		/// </summary>
		public ICommand ToggleDescriptionEditing => new Command(() =>
		{
			if(DescriptionEditable)
			{
				Database.Update(OwnModel);
				App.CurrentTimelineFeed.RefreshRecords();
			}
			DescriptionEditable ^= true;
		});

		/// <summary>
		/// Constructs the view model for a gamble.
		/// </summary>
		/// <param name="view">The page for this view model.</param>
		/// <param name="model">The gamble around which to construct the view model.</param>
		public GambleViewModel(Page view, Gamble model) : base(view, model) { }
	}
}