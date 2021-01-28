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
	internal sealed class PictureFeedViewModel : ViewModel<PictureFeed, Picture>
	{
		public ImageSource LikeButtonImage => Model.Liked ? Icons.Heart : Icons.HeartSketch;
		public Color LikeButtonBackgroundColor => Model.Liked ? Styles.Bgr5 : Styles.Bgr3;
		public string LikeButtonText => Model.Liked ? Text.Unlike : Text.Like;

		public ICommand SetNextPicture => new Command(() =>
		{
			var pics = Database.AllPictures();
			foreach(var pic in Database.LikedPictures())
				pics.AddRange(new[] { pic, pic }); // A liked picture will be shown 3x as often by adding it twice.

			Random random = new();
			Picture nextPic;
			do
				nextPic = pics[random.Next(0, pics.Count)];
			while(nextPic.FilePath == Model.FilePath);

			Model = nextPic;
			View.RefreshPresentedPicture();
		});

		public ICommand DrawImage => new Command<SKPaintSurfaceEventArgs>(eventArgs =>
		{
			string assetPath = Folders.PictureAssets + Model.FilePath;
			using var stream = App.Assembly.GetManifestResourceStream(assetPath);
			var bitmap = SKBitmap.Decode(stream);

			SKRect computeRect(float scale)
			{
				float width = scale * bitmap.Width;
				float height = scale * bitmap.Height;
				float left = 0.5f * (eventArgs.Info.Width - width);
				float top = 0.5f * (eventArgs.Info.Height - height);
				return new(left, top, left + width, top + height);
			}

			float widthRatio = (float)eventArgs.Info.Width / bitmap.Width;
			float heightRatio = (float)eventArgs.Info.Height / bitmap.Height;
			float scale = Math.Max(widthRatio, heightRatio) * 1.2f; // Overscale to avoid background bleed when blurring

			var canvas = eventArgs.Surface.Canvas;
			canvas.DrawBitmap(bitmap, computeRect(scale), Blur);

			scale = Math.Min(widthRatio, heightRatio);
			canvas.DrawBitmap(bitmap, computeRect(scale));
		});
		private static readonly SKPaint Blur = new() { ImageFilter = SKImageFilter.CreateBlur(50, 50) };

		public ICommand ToggleLikedStatus => new Command(() =>
		{
			Database.ToggleLikedStatus(Model);
			OnPropertyChanged(nameof(LikeButtonBackgroundColor));
			OnPropertyChanged(nameof(LikeButtonImage));
			OnPropertyChanged(nameof(LikeButtonText));
		});

		public ICommand SavePictureToGallery => new Command(async () =>
		{
			var fileSaver = DependencyService.Get<IPlatformFileSaver>();
			string result;
			if(await fileSaver.RequestSaveToGallery(Model.FilePath))
				result = Text.SavedToGallery;
			else
				result = Text.StoragePermissionDenied;
			await View.DisplayAlert(result, null, Text.Ok);
		});

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

		internal PictureFeedViewModel(PictureFeed view, Picture model) : base(view, model)
		{ }
	}
}
