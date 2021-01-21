using System;
using System.Linq;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Microcharts;
using SkiaSharp;
using Xamarin.Forms;

using Entry = Microcharts.Entry;

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
		/// Returns the chart associated with how the limit was depleted over time.
		/// </summary>
		public LineChart HistoryChart => new LineChart()
		{
			Entries = CalculateChartEntries(SKColors.White, SKColors.Red),
			PointMode = PointMode.None,
			LineMode = LineMode.Straight,
			BackgroundColor = SKColors.Transparent
		};

		/* If you're getting errors here because Entry was changed to ChartEntry, then you changed the Microcharts
		 * version to something later than 0.7.1. Before going with the newer version, you might need to explicitly set
		 * the Label and ValueLabel properties to empty strings. That will fix a crash in SkiaSharp, but now the dates
		 * probably won't be drawn in the chart. So unless the dates show up correctly in the chart, you need to stick
		 * with Microcharts 0.7.1. */
		private Entry[] CalculateChartEntries(SKColor goodColor, SKColor badColor)
		{
			var nextLimit = Database.NextLimitAfter(Limit);
			var gambles = Database.AllGamblesWithinLimit(Limit);

			TimeSpan span;
			if(nextLimit == null)
				span = Limit.Duration;
			else
				span = nextLimit.Time - Limit.Time;
			var entries = new Entry[(int)span.TotalHours];

			int[] indices = new int[gambles.Length + 2];
			indices[0] = 0;
			indices[^1] = entries.Length - 1;

			entries[0] = new Entry((float)Limit.Amount) { Color = goodColor };
			float lastValue = (float)Database.CalcBalance(Limit);
			entries[^1] = new Entry(lastValue) { Color = lastValue < 0 ? badColor : goodColor };

			// Enters the remaining limit values after each gamble at appropriate places in the graph.
			for(int i = 0; i < gambles.Length; i++)
			{
				long gambleTimeOffset = (gambles[i].Time - Limit.Time).Ticks;
				indices[i + 1] = (int)(gambleTimeOffset / (float)span.Ticks * (entries.Length - 1));
				float value = (float)Database.CalcRemainingLimit(gambles[i]);
				entries[indices[i + 1]] = new Entry(value) { Color = value < 0 ? badColor : goodColor };
			}

			// Enters all remaining points in the graph by linearly interpolating values.
			indices = indices.Distinct().ToArray();
			for(int i = 0, n = -1; i < entries.Length; i++)
			{
				if(entries[i] == null)
				{
					int last = indices[n];
					int next = indices[n + 1];
					float lastEntry = entries[last].Value;
					float value = lastEntry - (lastEntry - entries[next].Value) / (next - last) * (i - last);
					entries[i] = new Entry(value) { Color = value < 0 ? badColor : goodColor };
				}
				else
					n++;

				// Adds the captions for the points which correspond roughly to the beginning of new days.
				int hour = Limit.Time.ToLocalTime().Hour + (Limit.Time.Minute >= 30 ? 1 : 0);
				int not12am = hour != 0 ? 1 : 0;
				if(i % ((int)span.TotalDays / 5 * 24) == 24 * not12am - hour)
				{
					var date = Limit.Time + TimeSpan.FromDays(not12am + i / 24);
					entries[i].ValueLabel = date.ToLocalTime().ToString("d");
				}
			}
			return entries;
		}

		/// <summary>
		/// Constructs the view model for a limit.
		/// </summary>
		/// <param name="view">The page for this view model.</param>
		/// <param name="model">The limit around which to construct the view model.</param>
		public LimitViewModel(Page view, Limit model) : base(view, model) { }
	}
}
