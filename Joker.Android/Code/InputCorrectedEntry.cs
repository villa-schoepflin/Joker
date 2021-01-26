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
	internal class InputCorrectedEntry : EntryRenderer
	{
		public InputCorrectedEntry(Context context) : base(context) { }

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> eventArgs)
		{
			base.OnElementChanged(eventArgs);

			if(DeviceInfo.Manufacturer == "samsung" && Control != null
				&& !Control.InputType.HasFlag(InputTypes.ClassText))
			{
				Control.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagDecimal;
			}
		}
	}
}
