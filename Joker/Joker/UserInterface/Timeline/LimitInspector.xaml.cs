using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Microcharts;
using SkiaSharp;

using Joker.BusinessLogic;

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
			var viewModel = new LimitViewModel(this, limit);
			BindingContext = viewModel;

			ChartView.Chart = new LineChart()
			{
				Entries = Array.ConvertAll(viewModel.GetHistory(10), value => new Microcharts.Entry(value)
				{
					Color = value < 0 ? SKColors.Red : SKColors.White
				}),
				PointMode = PointMode.None,
				LineMode = LineMode.Straight,
				BackgroundColor = SKColors.Transparent
			};
		}
	}
}