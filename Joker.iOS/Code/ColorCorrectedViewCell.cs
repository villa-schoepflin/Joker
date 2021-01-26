using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewCell), typeof(Joker.iOS.ColorCorrectedViewCell))]
namespace Joker.iOS
{
	/// <summary>
	/// Custom renderer to correct the ugly selection colors shown after tapping on a view cell.
	/// </summary>
	internal class ColorCorrectedViewCell : ViewCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			return cell;
		}
	}
}
