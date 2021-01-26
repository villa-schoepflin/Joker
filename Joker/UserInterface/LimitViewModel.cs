using System;
using System.Linq;
using System.Windows.Input;
using Joker.AppInterface;
using Joker.BusinessLogic;
using Joker.DataAccess;
using Microcharts;
using SkiaSharp;
using Xamarin.Forms;

using Entry = Microcharts.Entry;

namespace Joker.UserInterface
{
	internal sealed class LimitViewModel : TimelineRecordViewModel
	{
		public override ImageSource TypeIcon => Icons.Limit;
		public override Color CellBackground => Styles.Primary1;
		public override Color IconBackground => Styles.Primary1;
		public override Color CellTextColor => Styles.TextContrast;
		public override string RemainingLimit => null;

		public override ICommand OpenInspector => new Command(async () =>
		{
			if(View.Navigation.HasPage<LimitInspector>())
				return;

			LimitInspector inspector = new(Limit);
			await View.Navigation.PushAsync(inspector);
		});

		public string Balance => Database.CalcBalance(Limit).ToString("C", App.Locale);
		public string DurationInDays => string.Format(Text.TimeInDays, Limit.Duration.TotalDays);
		public string LimitState => Database.CalcBalance(Limit) < 0 ? Text.LimitCrossed : Text.LimitKept;
		public Color StateBackground => Database.CalcBalance(Limit) < 0 ? Styles.LimitCrossedBgr : Styles.LimitKeptBgr;
		public Color StateTextColor => Database.CalcBalance(Limit) < 0 ? Styles.LimitCrossedText : Styles.LimitKeptText;

		public LineChart HistoryChart => new()
		{
			Entries = CalculateChartEntries(),
			PointMode = PointMode.None,
			LineMode = LineMode.Straight,
			BackgroundColor = SKColors.Transparent
		};

		private Limit Limit
		{
			get => (Limit)Model;
			set => Model = value;
		}

		internal LimitViewModel(Page view, Limit model) : base(view, model) { }

		/* If you're getting errors after here because Entry was changed to ChartEntry, then you changed the Microcharts
		 * version to something later than 0.7.1. Before going with the newer version, you might need to explicitly set
		 * the Label and ValueLabel properties to empty strings. That will fix a crash in SkiaSharp, but now the dates
		 * probably won't be drawn in the chart. So unless the dates show up correctly in the chart, you need to stick
		 * with Microcharts 0.7.1. */
		private Entry[] CalculateChartEntries()
		{
			var span = GetLimitDurationOrTimeToNextLimit();
			var entries = PrepareEntriesBasedOn(span);

			/* The gambles here are the data points that need to be interpolated. The indices are indices into the
			 * entries array and represent where each actual gamble is located in it. */
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
				SetLabelIfEntryAlignsWithDay(span, entries, i);
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

		private Entry[] PrepareEntriesBasedOn(TimeSpan span)
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
				float gamblePositionInSpan = ticksFromLimitToGamble / span.Ticks;
				indices[i + 1] = (int)(gamblePositionInSpan * (entries.Length - 1));
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

		private void SetLabelIfEntryAlignsWithDay(TimeSpan span, Entry[] entries, int current)
		{
			int roundUp = Limit.Time.Minute >= 30 ? 1 : 0;
			int limitHour = Limit.Time.ToLocalTime().Hour + roundUp;
			int zeroIf12am = limitHour == 0 ? 0 : 1;

			const int daySkippingThreshold = 5;
			const int hoursInDay = 24;

			int hoursBetweenLabels = (int)span.TotalDays / daySkippingThreshold * hoursInDay;
			int hoursTillNewDayFromLimit = hoursInDay * zeroIf12am - limitHour;
			if(current % hoursBetweenLabels == hoursTillNewDayFromLimit)
			{
				int offsetInDays = current / hoursInDay + zeroIf12am;
				var date = Limit.Time + TimeSpan.FromDays(offsetInDays);
				entries[current].ValueLabel = date.ToLocalTime().ToString("d");
			}
		}
	}
}
