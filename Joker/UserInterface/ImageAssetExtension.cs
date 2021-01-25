using System;
using Joker.AppInterface;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Joker.UserInterface
{
	/// <summary>
	/// Necessary for images to be embedded in XAML.
	/// </summary>
	[ContentProperty(nameof(Source))]
	public class ImageAssetExtension : IMarkupExtension
	{
		/// <summary>
		/// The source path for the embedded resource image.
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Converts the source path to a Xamarin.Forms image source.
		/// </summary>
		/// <param name="serviceProvider">Provides a service object for this extension.</param>
		/// <returns>A Xamarin.Forms image source as a generic object.</returns>
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if(Source == null)
				return null;
			return ImageSource.FromResource(Source, App.Assembly);
		}
	}
}
