using System.Collections.Generic;

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

		public float[] GetHistory(int granularity)
		{
			float[] limitHistory = new float[(int)OwnModel.Duration.TotalDays * granularity];
			limitHistory[0] = (float)OwnModel.Amount;
			for(int i = 1; i < limitHistory.Length; ++i)
				limitHistory[i] = float.NaN;

			var gambles = Database.AllGamblesWithinLimit(OwnModel);
			var indices = new List<int>(gambles.Length + 1) { 0 };

			foreach(var gamble in gambles)
			{
				int index = (int)((gamble.Time - OwnModel.Time).TotalMilliseconds /
					OwnModel.Duration.TotalMilliseconds * limitHistory.Length);
				limitHistory[index] = (float)Database.CalcRemainingLimit(gamble);
				if(index > 0)
					indices.Add(index);
			}

			int gamblesPassed = 0;
			int distance = indices.Count > 1 ? indices[1] : -1;
			int position = 1;
			int minuendIndex = 0;
			float difference = indices.Count > 1 ? limitHistory[0] - limitHistory[indices[1]] : 0;
			for(int i = 1; i < limitHistory.Length; ++i)
			{
				if(float.IsNaN(limitHistory[i]))
					limitHistory[i] = limitHistory[minuendIndex] - position++ / distance * difference;
				else
				{
					gamblesPassed++;
					position = 1;
					if(gamblesPassed < gambles.Length)
					{
						distance = indices[gamblesPassed + 1] - minuendIndex;
						minuendIndex = indices[gamblesPassed];
						difference = limitHistory[indices[gamblesPassed + 1]] - limitHistory[minuendIndex];
					}
					else
					{
						distance = -1;
						difference = 0;
					}
				}
			}

			//int distance = 1, position = 1;
			//float nextValue = (float)limit.Amount, difference = 0;
			//for(int i = 0; i < entries.Length; i++)
			//{
			//	if(entries[i] == null)
			//	{
			//		float value = i - 1 < 0 ? nextValue : position / distance * difference;
			//		entries[i] = new Microcharts.Entry(value)
			//		{
			//			Color = value < 0 ? SKColors.Red : SKColors.White
			//		};
			//		position++;
			//	}
			//	else
			//	{
			//		distance = 1;
			//		position = 1;
			//		int j = i + 1;
			//		for(; j < entries.Length && entries[j] == null; j++)
			//			distance++;
			//		nextValue = entries[j >= entries.Length ? i : j].Value;
			//		difference = nextValue - entries[i].Value;
			//	}
			//}

			return limitHistory;
		}
	}
}