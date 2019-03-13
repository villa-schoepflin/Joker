
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Microcharts;
using SkiaSharp;

using Joker.BusinessLogic;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// A view containing the database-supplied details and properties of a selected limit.
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LimitInspector : ContentPage
	{
		/// <summary>
		/// Initializes XAML elements and provides the data to be bound in the view.
		/// </summary>
		/// <param name="limit">Limit whose details should be displayed.</param>
		public LimitInspector(Limit limit)
		{
			InitializeComponent();
			BindingContext = new LimitViewModel(this, limit);

			var entries = new Microcharts.Entry[(int)limit.Duration.TotalDays * 50];

			foreach(var gamble in Database.AllGamblesWithinLimit(limit))
			{
				int index = (int)((gamble.Time - limit.Time).TotalMilliseconds
					/ limit.Duration.TotalMilliseconds * entries.Length);
				float value = (float)Database.CalcRemainingLimit(gamble);
				entries[index] = new Microcharts.Entry(value)
				{
					Color = value < 0 ? SKColors.Red : SKColors.White
				};
			}

			int distance = 0, position = 0;
			float nextValue = -999, difference = 0;
			for(int i = 0; i < entries.Length; i++)
			{
				if(entries[i] != null)
				{
					distance = 1;
					position = 1;
					int j = i + 1;
					for(; j < entries.Length && entries[j] == null; j++)
						distance++;
					nextValue = entries[j >= entries.Length ? i : j].Value;
					difference = nextValue - entries[i].Value;
				}
				else
				{
					float value = i - 1 < 0 ? nextValue : position / distance * difference;
					entries[i] = new Microcharts.Entry(value)
					{
						Color = value < 0 ? SKColors.Red : SKColors.White
					};
					position++;
				}
			}

			ChartView.Chart = new LineChart()
			{
				Entries = entries,
				PointMode = PointMode.None,
				LineMode = LineMode.Straight,
				BackgroundColor = SKColors.Transparent
			};
		}
	}
}