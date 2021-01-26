using System;
using Joker.AppInterface;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Joker.UserInterface
{
	[ContentProperty(nameof(Source))]
	internal class ImageAssetExtension : IMarkupExtension
	{
		public string Source { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if(Source == null)
				return null;
			return ImageSource.FromResource(Source, App.Assembly);
		}
	}
}
