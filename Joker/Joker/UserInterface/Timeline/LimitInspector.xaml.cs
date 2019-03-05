using System;

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

			var entries = Array.ConvertAll(Database.AllGamblesWithinLimit(limit),
				gamble => new Microcharts.Entry((float)Database.CalcRemainingLimit(gamble))
				{
					Color = SKColors.White
				});

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