using System.Windows.Input;
using Joker.AppInterface;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	internal sealed class GambleViewModel : TimelineRecordViewModel
	{
		public override ImageSource TypeIcon => Gamble.Type switch
		{
			GambleType.Other => Icons.GambleOther,
			GambleType.Lottery => Icons.GambleLottery,
			GambleType.SportsBet => Icons.GambleSportsBet,
			GambleType.Casino => Icons.GambleCasino,
			GambleType.SlotMachine => Icons.GambleSlotMachine,
			_ => null
		};

		public override Color CellBackground => Styles.Bgr3;
		public override Color IconBackground => Styles.Bgr4;
		public override Color CellTextColor => Styles.Text1;
		public override string RemainingLimit => Database.CalcRemainingLimit(Gamble).ToString("C", App.Locale);

		public override ICommand OpenInspector => new Command(async () =>
		{
			if(View.Navigation.HasPage<GambleInspector>())
				return;

			GambleInspector inspector = new(Gamble);
			await View.Navigation.PushAsync(inspector);
		});

		public string Description { get; set; }

		public bool Editable
		{
			get => _editable;
			set
			{
				_editable = value;
				OnPropertyChanged(nameof(Editable));
				OnPropertyChanged(nameof(EditButtonText));
				OnPropertyChanged(nameof(EditButtonBgrColor));
				OnPropertyChanged(nameof(EditButtonTextColor));
			}
		}
		private bool _editable;

		public bool IsOnlineGamble
		{
			get => Gamble.IsOnlineGamble;
			set => Gamble.IsOnlineGamble = value;
		}

		public string GambleTypeText => GambleTypes.GetName(Gamble.Type);
		public string EditButtonText => Editable ? Text.Save : Text.Edit;
		public Color EditButtonBgrColor => Editable ? Styles.Primary1 : Styles.Bgr4;
		public Color EditButtonTextColor => Editable ? Styles.TextContrast : Styles.Text1;

		public ICommand ToggleDescriptionEditing => new Command(() =>
		{
			if(Editable)
			{
				Gamble.Description = Description;
				Database.Update(Gamble);
			}
			Editable ^= true;
		});

		private Gamble Gamble
		{
			get => (Gamble)Model;
			set => Model = value;
		}

		internal GambleViewModel(Page view, Gamble model) : base(view, model)
		{
			Description = model.Description;
		}
	}
}
