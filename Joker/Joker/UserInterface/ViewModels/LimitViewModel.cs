using System;
using System.Linq;

using Xamarin.Forms;

using SkiaSharp;
using Microcharts;

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
		private Limit Limit
		{
			get => (Limit)Model;
			set => Model = value;
		}

		/// <summary>
		/// The currently remaining balance of the limit.
		/// </summary>
		public string Balance => Database.CalcBalance(Limit).ToString("C", App.Locale);

		/// <summary>
		/// The duration in days.
		/// </summary>
		public string DurationInDays => $"{Limit.Duration.TotalDays} Tage";

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
		/// Returns the Microcharts chart associated with how the limit was depleted over time.
		/// </summary>
		public LineChart Chart
		{
			get
			{
				var nextLimit = Database.NextLimitAfter(Limit);

				TimeSpan span;
				if(nextLimit == null)
					span = Limit.Duration;
				else
					span = nextLimit.Time - Limit.Time;

				var data = new Microcharts.Entry[(int)span.TotalHours];
				var gambles = Database.AllGamblesWithinLimit(Limit);

				int[] indices = new int[gambles.Length + 2];
				indices[0] = 0;
				indices[indices.Length - 1] = data.Length - 1;

				data[0] = new Microcharts.Entry((float)Limit.Amount) { Color = SKColors.White };

				float lastValue = (float)Database.CalcBalance(Limit);
				data[data.Length - 1] = new Microcharts.Entry(lastValue)
				{
					Color = lastValue < 0 ? SKColors.Red : SKColors.White
				};

				// Enters the remaining limit values after each gamble at appropriate places in the graph.
				for(int i = 0; i < gambles.Length; i++)
				{
					indices[i + 1] = (int)((gambles[i].Time - Limit.Time).Ticks / (float)span.Ticks * (data.Length - 1));
					decimal value = Database.CalcRemainingLimit(gambles[i]);
					data[indices[i + 1]] = new Microcharts.Entry((float)value)
					{
						Color = value < 0 ? SKColors.Red : SKColors.White
					};
				}

				// Enters all remaining points in the graph by interpolating values.
				indices = indices.Distinct().ToArray();
				for(int i = 0, n = -1; i < data.Length; i++)
				{
					if(data[i] == null)
					{
						int last = indices[n];
						int next = indices[n + 1];
						float value = data[last].Value - (data[last].Value - data[next].Value) / (next - last) * (i - last);
						data[i] = new Microcharts.Entry(value) { Color = value < 0 ? SKColors.Red : SKColors.White };
					}
					else
						n++;
					// Adds the captions for the points which correspond roughly to the beginning of new days.
					int hour = Limit.Time.ToLocalTime().Hour + (Limit.Time.Minute < 30 ? 0 : 1);
					int not12am = hour == 0 ? 0 : 1;
					if(i % ((int)span.TotalDays / 5 * 24) == 24 * not12am - hour)
						data[i].ValueLabel = (Limit.Time + TimeSpan.FromDays(not12am + i / 24)).ToLocalTime().ToString("d");
				}

				return new LineChart()
				{
					Entries = data,
					PointMode = PointMode.None,
					LineMode = LineMode.Straight,
					BackgroundColor = SKColors.Transparent
				};
			}
		}

		/// <summary>
		/// Constructs the view model for a limit.
		/// </summary>
		/// <param name="view">The page for this view model.</param>
		/// <param name="model">The limit around which to construct the view model.</param>
		public LimitViewModel(Page view, Limit model) : base(view, model) { }
	}
}