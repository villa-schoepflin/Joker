using System;
using SQLite;

namespace Joker.BusinessLogic
{
	/// <summary>
	/// Represents a motivating or informing picture in the database and in the app view.
	/// </summary>
	[Table(PictureTableName)]
	public sealed class Picture
	{
		/// <summary>
		/// The file path of the image asset associated with the picture, used as a primary key.
		/// </summary>
		[PrimaryKey, Column(FilePathColumnName)] public string FilePath { get; set; }

		/// <summary>
		/// The time this picture was added to the database.
		/// </summary>
		[Column(TimeAddedColumnName)] public DateTime TimeAdded { get; set; }

		/// <summary>
		/// The status indicating whether this picture is preferred by the user, which makes the picture appear more
		/// often by the randomized refresh in the picture feed.
		/// </summary>
		[Column(LikedColumnName)] public bool Liked { get; set; }

		/// <summary>
		/// The standard constructor for this class. Creates a picture based on the given file path and sets its
		/// properties to their default value.
		/// </summary>
		/// <param name="filePath">The file path of the embedded image resource.</param>
		public Picture(string filePath)
		{
			string[] parts = filePath.Split('.');
			FilePath = $"{parts[^2]}.{parts[^1]}";
			TimeAdded = DateTime.UtcNow;
			Liked = false;
		}

		/// <summary>
		/// This constructor only exists for SQLite to be able to return collections of pictures from the database. It
		/// should never be used to instantiate a picture directly within the app.
		/// </summary>
		public Picture()
		{ }

		#region Identifiers for the database schema (DO NOT CHANGE!)
		private const string PictureTableName = "Picture";
		private const string FilePathColumnName = "FilePath";
		private const string TimeAddedColumnName = "TimeAdded";
		private const string LikedColumnName = "Liked";
		#endregion
	}
}
