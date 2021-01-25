using System.Windows.Input;
using Joker.AppInterface;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// View model for a gamble in the gamble inspector.
	/// </summary>
	public sealed class GambleViewModel : TimelineRecordViewModel
	{
		/// <summary>
		/// The icon to be displayed in the timeline for the gamble, depends on the type of the gamble.
		/// </summary>
		public override ImageSource TypeIcon => Gamble.Type switch
		{
			GambleType.Other => Icons.GambleOther,
			GambleType.Lottery => Icons.GambleLottery,
			GambleType.SportsBet => Icons.GambleSportsBet,
			GambleType.Casino => Icons.GambleCasino,
			GambleType.SlotMachine => Icons.GambleSlotMachine,
			_ => null
		};

		/// <summary>
		/// The primary tinting color used in the timeline for gambles.
		/// </summary>
		public override Color CellBackground => Styles.Bgr3;

		/// <summary>
		/// The secondary tinting color used in the timeline for gambles.
		/// </summary>
		public override Color IconBackground => Styles.Bgr4;

		/// <summary>
		/// The primary text color used in the timeline for gambles.
		/// </summary>
		public override Color CellTextColor => Styles.Text1;

		/// <summary>
		/// The remaining value of the associated limit, after this gamble was deducted.
		/// </summary>
		public override string RemainingLimit => Database.CalcRemainingLimit(Gamble).ToString("C", App.Locale);

		/// <summary>
		/// Navigates the user to a detailed view of the selected gamble.
		/// </summary>
		public override ICommand OpenInspector => new Command(async () =>
		{
			if(View.Navigation.HasPage<GambleInspector>())
				return;

			GambleInspector inspector = new(Gamble);
			await View.Navigation.PushAsync(inspector);
		});

		/// <summary>
		/// Differs from the saved description in the database during editing unless saved by the user.
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
		public string GambleTypeText => GambleTypes.GetName(Gamble.Type);

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
		/// Wrapper in order to treat the model as a gamble because of inheritance from TimelineRecordViewModel.
		/// </summary>
		private Gamble Gamble
		{
			get => (Gamble)Model;
			set => Model = value;
		}

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
