using System.ComponentModel;

namespace Joker.UserInterface
{
	/// <summary>
	/// An abstract base class for all view models with required interfaces and constraints.
	/// </summary>
	/// <typeparam name="TView">Type of the view where the model's properties are presented.</typeparam>
	/// <typeparam name="TModel">Type of the model object whose properties are accessed for presentation.</typeparam>
	public abstract class ViewModel<TView, TModel> : INotifyPropertyChanged
	{
		/// <summary>
		/// The object to which changes in the view model are communicated.
		/// </summary>
		protected TView View { get; set; }

		/// <summary>
		/// The object from which view model properties are derived.
		/// </summary>
		protected virtual TModel Model { get; set; }

		/// <summary>
		/// Event for when a property has changed in the view model.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Constructs a view model with the given view and model references.
		/// </summary>
		/// <param name="view">The view for this view model.</param>
		/// <param name="model">The model for this view model.</param>
		protected ViewModel(TView view, TModel model)
		{
			View = view;
			Model = model;
		}

		/// <summary>
		/// Invokes subscribers of the PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">Name of the property that has changed.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
