using Android.Content;
using Android.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(Joker.Android.InputCorrectedEntry))]
namespace Joker.Android
{
	/// <summary>
	/// Custom renderer to enable use of dots in numeric entries on Samsung devices.
	/// </summary>
	public class InputCorrectedEntry : EntryRenderer
	{
		/// <summary>
		/// Renders the custom entry.
		/// </summary>
		/// <param name="context">Context in which to render the entry.</param>
		public InputCorrectedEntry(Context context) : base(context) { }

		/// <summary>
		/// Changes the entry's keyboard so dots can be used as decimal separators when keyboard is supposed to appear.
		/// </summary>
		/// <param name="e">Event arguments for when a property of the entry has changed.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if(DeviceInfo.Manufacturer == "samsung"
				&& Control != null
				&& !Control.InputType.HasFlag(InputTypes.ClassText))
			{
				Control.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagDecimal;
			}
		}
	}
}
