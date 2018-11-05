using System;
using System.Windows.Input;

using Xamarin.Forms;

using Joker.AppInterface;
using Joker.BusinessLogic;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// View model containing properties derived from a picture for the purpose of presentation.
	/// </summary>
	public class PictureFeedViewModel : ViewModel<Page, Picture>
	{
		/// <summary>
		/// The picture from which the values of bindable properties are derived.
		/// </summary>
		protected override Picture Model
		{
			get => model;
			set
			{
				model = value;
				OnPropertyChanged(nameof(PresentedImage));
				OnPropertyChanged(nameof(LikeButtonBackgroundColor));
				OnPropertyChanged(nameof(LikeButtonImage));
				OnPropertyChanged(nameof(LikeButtonText));
			}
		}
		private Picture model;

		/// <summary>
		/// Creates a Xamarin.Forms image from the embedded resource file path.
		/// </summary>
		public ImageSource PresentedImage => ImageSource.FromResource(Model.FilePath);

		/// <summary>
		/// Determines the image to display on the Like button based on the current Liked status.
		/// </summary>
		public ImageSource LikeButtonImage => ImageSource.FromFile(Model.Liked ? "ui_heart.png" : "ui_heartoutline.png");

		/// <summary>
		/// Determines the background color of the Like button based on the current Liked status.
		/// </summary>
		public Color LikeButtonBackgroundColor => App.Color(Model.Liked ? "Bgr5" : "Bgr3");

		/// <summary>
		/// Determines the text of the Like button based on the current Liked status.
		/// </summary>
		public string LikeButtonText => Model.Liked ? "Gefällt mir nicht mehr" : "Gefällt mir";

		/// <summary>
		/// Re-binds the view to a randomly selected picture from the database, preferring the liked
		/// pictures with a ratio of 3:1.
		/// </summary>
		public ICommand SetNextPicture => new Command(() =>
		{
			var pics = Database.AllPictures();
			foreach(var pic in Database.LikedPictures())
			{
				pics.Add(pic);
				pics.Add(pic);
			}
			Picture nextPic;
			do
				nextPic = pics[new Random().Next(0, pics.Count)];
			while(nextPic.FilePath == Model.FilePath);
			Model = Database.GetPictureByFileName(nextPic.FilePath);
		});

		/// <summary>
		/// Updates the picture with the changed Liked status in the database and notifies the change of property.
		/// </summary>
		public ICommand ToggleLikedStatus => new Command(() =>
		{
			Database.ToggleLikedStatus(Model);
			OnPropertyChanged(nameof(LikeButtonBackgroundColor));
			OnPropertyChanged(nameof(LikeButtonImage));
			OnPropertyChanged(nameof(LikeButtonText));
		});

		/// <summary>
		/// Copies the current picture's image file to the user's personal phone storage.
		/// </summary>
		public ICommand SavePictureToGallery => new Command(async () =>
		{
			if(await DependencyService.Get<IPlatformFileSaver>().SaveToGallery(Model.FilePath))
				await View.DisplayAlert("In der Galerie gespeichert.", null, "Ok");
			else
				await View.DisplayAlert("Die App ist nicht berechtigt auf den Speicher zuzugreifen.", null, "Ok");
		});

		/// <summary>
		/// Constructs a picture feed view model for the given view.
		/// </summary>
		/// <param name="view">The view for this view model.</param>
		/// <param name="model">The model for this view model.</param>
		public PictureFeedViewModel(Page view, Picture model) : base(view, model) { }
	}
}