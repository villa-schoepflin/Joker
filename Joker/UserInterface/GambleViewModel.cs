using System.Windows.Input;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// View model for a gamble in the gamble inspector. Uses many properties of the timeline record view model.
	/// </summary>
	public sealed class GambleViewModel : TimelineRecordViewModel
	{
		/// <summary>
		/// Wrapper in order to treat the model as a gamble because of inheritance from TimelineRecordViewModel.
		/// </summary>
		private Gamble Gamble
		{
			get => (Gamble)Model;
			set => Model = value;
		}

		/// <summary>
		/// The description of this gamble as supplied from a model object.
		/// </summary>
		public string Description { get; set; }

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
				OnPropertyChanged(nameof(EditButtonBgrColor));
				OnPropertyChanged(nameof(EditButtonTextColor));
			}
		}
		private bool _descriptionEditable;

		/// <summary>
		/// The type of this gamble as the proper translation.
		/// </summary>
		public string GambleType => GambleTypes.GetName(Gamble.Type);

		/// <summary>
		/// Determines the text of the button that toggles the editing status.
		/// </summary>
		public string EditButtonText => DescriptionEditable ? Text.Save : Text.Edit;

		/// <summary>
		/// Determines the background color of the button that toggles the editing status.
		/// </summary>
		public Color EditButtonBgrColor => DescriptionEditable ? Styles.Primary1 : Styles.Bgr4;

		/// <summary>
		/// Determines the text color of the button that toggles the editing status.
		/// </summary>
		public Color EditButtonTextColor => DescriptionEditable ? Styles.TextContrast : Styles.Text1;

		/// <summary>
		/// Toggles the editability of the gamble's description, saving the changes when deactivating editing.
		/// </summary>
		public ICommand ToggleDescriptionEditing => new Command(() =>
		{
			if(DescriptionEditable)
			{
				Gamble.Description = Description;
				Database.Update(Gamble);
			}
			DescriptionEditable ^= true;
		});

		/// <summary>
		/// Constructs the view model for a gamble.
		/// </summary>
		/// <param name="view">The page for this view model.</param>
		/// <param name="model">The gamble around which to construct the view model.</param>
		public GambleViewModel(Page view, Gamble model) : base(view, model)
		{
			Description = model.Description;
		}
	}
}
