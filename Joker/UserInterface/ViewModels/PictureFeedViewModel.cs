using System;
using System.Windows.Input;
using Joker.AppInterface;
using Joker.BusinessLogic;
using Joker.DataAccess;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

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
			get => _model;
			set
			{
				_model = value;
				OnPropertyChanged(nameof(LikeButtonImage));
				OnPropertyChanged(nameof(LikeButtonBackgroundColor));
				OnPropertyChanged(nameof(LikeButtonText));
			}
		}
		private Picture _model;

		/// <summary>
		/// Determines the image to display on the Like button based on the current Liked status.
		/// </summary>
		public ImageSource LikeButtonImage => ImageSource.FromFile(Model.Liked ? "ui_heart.png" : "ui_heartsketch.png");

		/// <summary>
		/// Determines the background color of the Like button based on the current Liked status.
		/// </summary>
		public Color LikeButtonBackgroundColor => App.Color(Model.Liked ? "Bgr5" : "Bgr3");

		/// <summary>
		/// Determines the text of the Like button based on the current Liked status.
		/// </summary>
		public string LikeButtonText => Model.Liked ? Text.Unlike : Text.Like;

		/// <summary>
		/// Re-binds the view to a randomly selected picture from the database, preferring the liked pictures with a
		/// ratio of 3:1.
		/// </summary>
		public ICommand SetNextPicture => new Command(() =>
		{
			var pics = Database.AllPictures();
			foreach(var pic in Database.LikedPictures())
				pics.AddRange(new[] { pic, pic }); // A liked picture will be shown 3x as often by adding it twice.

			var random = new Random();
			Picture nextPic;
			do
				nextPic = pics[random.Next(0, pics.Count)];
			while(nextPic.FilePath == Model.FilePath);

			Model = nextPic;
			App.CurrentPictureFeed.RefreshPresentedPicture();
		});

		/// <summary>
		/// Loads and draws the bitmap corresponding to the picture that should be presented and draws a blur around it.
		/// </summary>
		public ICommand DrawImage => new Command<SKPaintSurfaceEventArgs>(eventArgs =>
		{
			string assetPath = Folders.PictureAssets + Model.FilePath;
			using var stream = typeof(App).Assembly.GetManifestResourceStream(assetPath);
			var bitmap = SKBitmap.Decode(stream);

			SKRect computeRect(float scale)
			{
				float width = scale * bitmap.Width;
				float height = scale * bitmap.Height;
				float left = 0.5f * (eventArgs.Info.Width - width);
				float top = 0.5f * (eventArgs.Info.Height - height);
				return new SKRect(left, top, left + width, top + height);
			}

			var canvas = eventArgs.Surface.Canvas;
			float widthRatio = (float)eventArgs.Info.Width / bitmap.Width;
			float heightRatio = (float)eventArgs.Info.Height / bitmap.Height;

			float scale = Math.Max(widthRatio, heightRatio) * 1.1f; // Overscale to avoid background bleed when blurring
			canvas.DrawBitmap(bitmap, computeRect(scale), Blur);

			scale = Math.Min(widthRatio, heightRatio);
			canvas.DrawBitmap(bitmap, computeRect(scale));
		});
		private static readonly SKPaint Blur = new SKPaint() { ImageFilter = SKImageFilter.CreateBlur(50, 50) };

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
			string msg;
			if(await DependencyService.Get<IPlatformFileSaver>().SaveToGallery(Model.FilePath))
				msg = Text.SavedToGallery;
			else
				msg = Text.StoragePermissionDenied;

			await View.DisplayAlert(msg, null, Text.Ok);
		});

		/// <summary>
		/// Constructs a picture feed view model for the given view.
		/// </summary>
		/// <param name="view">The view for this view model.</param>
		/// <param name="model">The model for this view model.</param>
		public PictureFeedViewModel(Page view, Picture model) : base(view, model) { }
	}
}