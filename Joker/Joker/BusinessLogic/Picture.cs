using System;
using SQLite;

namespace Joker.BusinessLogic
{
	/// <summary>
	/// Represents a motivating or informing picture in the database and in the app view.
	/// </summary>
	[Table("Picture")]
	public sealed class Picture
	{
		/// <summary>
		/// The file path of the image resource associated with the picture, used as a primary key.
		/// </summary>
		[PrimaryKey, Column("FilePath")] public string FilePath { get; set; }

		/// <summary>
		/// The time this picture was added to the database.
		/// </summary>
		[Column("TimeAdded")] public DateTime TimeAdded { get; set; }

		/// <summary>
		/// The status indicating whether this picture is preferred by the user, which makes the picture
		/// appear more often by the randomized refresh in the picture feed.
		/// </summary>
		[Column("Liked")] public bool Liked { get; set; }

		/// <summary>
		/// The standard constructor for this class. Creates a picture based on the given file path and sets its
		/// properties to their default value.
		/// </summary>
		/// <param name="filePath">The file path of the embedded image resource.</param>
		public Picture(string filePath)
		{
			FilePath = filePath;
			TimeAdded = DateTime.UtcNow;
			Liked = false;
		}

		/// <summary>
		/// This constructor only exists for SQLite to be able to return collections of Pictures from the database.
		/// It should never be used to instantiate a Picture directly within the app.
		/// </summary>
		public Picture() { }

		/// <summary>
		/// Contains the database-relevant properties of this picture.
		/// </summary>
		/// <returns>A one-line string that represents this picture.</returns>
		public override string ToString()
		{
			return $"File path: {FilePath}  |  Time added: {TimeAdded}  |  Liked: {Liked}";
		}
	}
}