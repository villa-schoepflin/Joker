using System;
using System.Windows.Input;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Joker.UserInterface
{
	/// <summary>
	/// Helper class for converting SKCanvasView's paint surface events to commands.
	/// </summary>
	public sealed class PaintSurfaceBehavior : Behavior<SKCanvasView>
	{
		/// <summary>
		/// Bindable property required for the command.
		/// </summary>
		public static readonly BindableProperty CommandProperty
			= BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(PaintSurfaceBehavior), null);

		/// <summary>
		/// Holds the command invoked instead of a paint surface event handler.
		/// </summary>
		public ICommand Command
		{
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		/// <summary>
		/// Invoked immediately after the behavior is attached to a control.
		/// </summary>
		/// <param name="bindable">The bindable canvas view to which the behavior was attached.</param>
		protected override void OnAttachedTo(SKCanvasView bindable)
		{
			base.OnAttachedTo(bindable);

			bindable.BindingContextChanged += OnBindingContextChanged;
			bindable.PaintSurface += OnPaintSurface;
		}

		/// <summary>
		/// Invoked when the behavior is removed from the control.
		/// </summary>
		/// <param name="bindable">The bindable canvas view to which the behavior was attached.</param>
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
