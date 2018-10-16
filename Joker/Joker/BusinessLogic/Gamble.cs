using System;
using SQLite;

namespace Joker.BusinessLogic
{
	/// <summary>
	/// A gamble is an act of spending for gambling purposes, used
	/// to calculate how much of each limit has been depleted.
	/// </summary>
	[Table("Gamble")]
	public sealed class Gamble : TimelineRecord
	{
		/// <summary>
		/// Maximum length of a gamble's description.
		/// </summary>
		public const int MaxDescriptionLength = 500;

		/// <summary>
		/// The type this gamble is classified as.
		/// </summary>
		[Column("Type")] public GambleType Type { get; set; }

		/// <summary>
		/// Optional description given verbatim by the user.
		/// </summary>
		[Column("Description")] public string Description { get; set; }

		/// <summary>
		/// Constructor used when the user doesn't specify a time.
		/// </summary>
		/// <param name="amount">The amount as put in by the user.</param>
		/// <param name="type">The type as selected by the user.</param>
		/// <param name="description">The description as given by the user.</param>
		public Gamble(string amount, GambleType type, string description) : base(amount)
		{
			Type = type;
			Description = description;
		}

		/// <summary>
		/// Constructor used when the user specifies a time.
		/// </summary>
		/// <param name="time">The time as selected by the user.</param>
		/// <param name="amount">The amount as given by the user.</param>
		/// <param name="type">The type as selected by the user.</param>
		/// <param name="description">The description as given by the user.</param>
		public Gamble(DateTime time, string amount, GambleType type, string description) : base(time, amount)
		{
			Type = type;
			Description = description;
		}

		/// <summary>
		/// This constructor only exists for SQLite to be able to return collections of Gambles from the database.
		/// It should never be used to instantiate a Gamble directly within the app.
		/// </summary>
		public Gamble() : base() { }

		/// <summary>
		/// Contains the database-relevant properties of this gamble.
		/// </summary>
		/// <returns>A one-line string that represents this gamble.</returns>
		public override string ToString()
		{
			return $"Time: {Time}  |  Amount: {Amount}  |  Type: {Type}  |  Description: {Description}";
		}
	}
}