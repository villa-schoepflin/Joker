using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using UIKit;

[assembly: ExportRenderer(typeof(ViewCell), typeof(Joker.iOS.ColorCorrectedViewCell))]
namespace Joker.iOS
{
	/// <summary>
	/// Custom renderer to correct the ugly selection colors shown after tapping on a view cell.
	/// </summary>
	public class ColorCorrectedViewCell : ViewCellRenderer
	{
		/// <summary>
		/// Corrects the selection colors of a view cell on iOS.
		/// </summary>
		/// <param name="item">Contains styling information about a Forms cell.</param>
		/// <param name="reusableCell">Contains styling information about a native iOS cell.</param>
		/// <param name="tv">Contains information about the table view containing the cell.</param>
		/// <returns>The customized native iOS cell with the corrected color scheme.</returns>
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			return cell;
		}
	}
}