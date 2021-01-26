using System;
using System.Windows.Input;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Helper class for converting SKCanvasView's paint surface events to commands.
	/// </summary>
	internal sealed class PaintSurfaceBehavior : Behavior<SKCanvasView>
	{
		internal static readonly BindableProperty CommandProperty
			= BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(PaintSurfaceBehavior), null);

		public ICommand Command
		{
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		protected override void OnAttachedTo(SKCanvasView bindable)
		{
			base.OnAttachedTo(bindable);

			bindable.BindingContextChanged += OnBindingContextChanged;
			bindable.PaintSurface += OnPaintSurface;
		}

		protected override void OnDetachingFrom(SKCanvasView bindable)
		{
			base.OnDetachingFrom(bindable);

			bindable.BindingContextChanged -= OnBindingContextChanged;
			bindable.PaintSurface -= OnPaintSurface;
		}

		private void OnBindingContextChanged(object sender, EventArgs eventArgs)
		{
			BindingContext = ((BindableObject)sender).BindingContext;
		}

		private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs eventArgs)
		{
			if(Command?.CanExecute(eventArgs) == true)
				Command.Execute(eventArgs);
		}
	}
}
