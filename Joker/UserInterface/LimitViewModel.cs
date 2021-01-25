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
	public sealed class LimitViewModel : TimelineRecordViewModel
	{
		/// <summary>
		/// The currently remaining balance of the limit.
		/// </summary>
		public string Balance => Database.CalcBalance(Limit).ToString("C", JokerApp.Locale);

		/// <summary>
		/// The duration in days.
		/// </summary>
		public string DurationInDays => string.Format(Text.TimeInDays, Limit.Duration.TotalDays);

		/// <summary>
		/// A text indicating the state of the limit.
		/// </summary>
		public string LimitState => Database.CalcBalance(Limit) < 0 ? Text.LimitCrossed : Text.LimitKept;

		/// <summary>
		/// The background color marking the state of the limit.
		/// </summary>
		public Color StateBackground => Database.CalcBalance(Limit) < 0 ? Styles.LimitCrossedBgr : Styles.LimitKeptBgr;

		/// <summary>
		/// The color for the text that indicates the limit state.
		/// </summary>
		public Color StateTextColor => Database.CalcBalance(Limit) < 0 ? Styles.LimitCrossedText : Styles.LimitKeptText;

		/// <summary>
		/// Returns the chart associated with how the limit was depleted over time.
		/// </summary>
		public LineChart HistoryChart => new()
		{
			Entries = CalculateChartEntries(),
			PointMode = PointMode.None,
			LineMode = LineMode.Straight,
			BackgroundColor = SKColors.Transparent
		};

		/// <summary>
		/// Wrapper in order to treat the model as a limit because of inheritance from TimelineRecordViewModel.
		/// </summary>
		private Limit Limit
		{
			get => (Limit)Model;
			set => Model = value;
		}

		/// <summary>
		/// Constructs the view model for a limit.
		/// </summary>
		/// <param name="view">The page for this view model.</param>
		/// <param name="model">The limit around which to construct the view model.</param>
		public LimitViewModel(Page view, Limit model) : base(view, model)
		{
			TypeIcon = Icons.Limit;
			CellBackground = Styles.Primary1;
			IconBackground = Styles.Primary1;
			CellTextColor = Styles.TextContrast;
			RemainingLimit = null;
		}

		/* If you're getting errors after here because Entry was changed to ChartEntry, then you changed the Microcharts
		 * version to something later than 0.7.1. Before going with the newer version, you might need to explicitly set
		 * the Label and ValueLabel properties to empty strings. That will fix a crash in SkiaSharp, but now the dates
		 * probably won't be drawn in the chart. So unless the dates show up correctly in the chart, you need to stick
		 * with Microcharts 0.7.1. */
		private Entry[] CalculateChartEntries()
		{
			var span = GetLimitDurationOrTimeToNextLimit();
			var entries = AllocateEntriesBasedOn(span);

			/* The gambles here are the data points that need to be interpolated. The indices are indices into the
			 * entries array and represent where each actual gamble is located in it.*/
			var gambles = Database.AllGamblesWithinLimit(Limit);
			int[] indices = PrepareIndices(entries, gambles);
			SetGambleIndicesAndGambleEntries(span, entries, gambles, indices);
			indices = indices.Distinct().ToArray();

			for(int i = 0, gamblesPassed = -1; i < entries.Length; i++)
			{
				if(entries[i] == null)
					LinearlyInterpolateEntry(entries, indices, i, gamblesPassed);
				else
					gamblesPassed++;

				SetCaptionIfEntryAlignsWithNewDay(span, entries, i);
			}
			return entries;
		}

		private static Entry GetColoredEntry(float value)
		{
			return new(value) { Color = value < 0 ? SKColors.Red : SKColors.White };
		}

		private TimeSpan GetLimitDurationOrTimeToNextLimit()
		{
			var nextLimit = Database.NextLimitAfter(Limit);
			if(nextLimit == null)
				return Limit.Duration;
			else
				return nextLimit.Time - Limit.Time;
		}

		private Entry[] AllocateEntriesBasedOn(TimeSpan span)
		{
			var entries = new Entry[(int)span.TotalHours];
			entries[0] = GetColoredEntry((float)Limit.Amount);

			float finalValue = (float)Database.CalcBalance(Limit);
			entries[^1] = GetColoredEntry(finalValue);

			return entries;
		}

		private static int[] PrepareIndices(Entry[] entries, Gamble[] gambles)
		{
			int[] indices = new int[gambles.Length + 2];
			indices[0] = 0;
			indices[^1] = entries.Length - 1;
			return indices;
		}

		private void SetGambleIndicesAndGambleEntries(TimeSpan span, Entry[] entries, Gamble[] gambles, int[] indices)
		{
			for(int i = 0; i < gambles.Length; i++)
			{
				float ticksFromLimitToGamble = (gambles[i].Time - Limit.Time).Ticks;
				indices[i + 1] = (int)(ticksFromLimitToGamble / span.Ticks * (entries.Length - 1));
				float value = (float)Database.CalcRemainingLimit(gambles[i]);
				entries[indices[i + 1]] = GetColoredEntry(value);
			}
		}

		private static void LinearlyInterpolateEntry(Entry[] entries, int[] indices, int current, int gamblesPassed)
		{
			int last = indices[gamblesPassed];
			int next = indices[gamblesPassed + 1];
			float lastEntry = entries[last].Value;
			float entryDiff = lastEntry - entries[next].Value;
			float value = lastEntry - entryDiff / (next - last) * (current - last);
			entries[current] = GetColoredEntry(value);
		}

		private void SetCaptionIfEntryAlignsWithNewDay(TimeSpan span, Entry[] entries, int current)
		{
			int roundUp = Limit.Time.Minute >= 30 ? 1 : 0;
			int hour = Limit.Time.ToLocalTime().Hour + roundUp;
			int not12am = hour != 0 ? 1 : 0;
			if(current % ((int)span.TotalDays / 5 * 24) == 24 * not12am - hour)
			{
				var date = Limit.Time + TimeSpan.FromDays(not12am + current / 24);
				entries[current].ValueLabel = date.ToLocalTime().ToString("d");
			}
		}
	}
}
