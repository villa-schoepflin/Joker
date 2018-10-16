using System;
using SQLite;

namespace Joker.BusinessLogic
{
	/// <summary>
	/// Determines how much the user is allowed to spend for gambling in the given duration.
	/// </summary>
	[Table("Limit")]
	public sealed class Limit : TimelineRecord
	{
		/// <summary>
		/// The minimum allowed duration of a limit.
		/// </summary>
		public static readonly TimeSpan MinLimitDuration = TimeSpan.FromDays(5);

		/// <summary>
		/// The maximum allowed duration of a limit.
		/// </summary>
		public static readonly TimeSpan MaxLimitDuration = TimeSpan.FromDays(30);

		/// <summary>
		/// The time span after which this limit should have been replaced with a new one.
		/// </summary>
		[Column("Duration")] public TimeSpan Duration { get; set; }

		/// <summary>
		/// The standard constructor that should be used every time a new limit must be set.
		/// </summary>
		/// <param name="amount">The amount as put in by the user.</param>
		/// <param name="durationInDays">The duration in days as put in by the user.</param>
		/// <exception cref="ArgumentException">Thrown if the duration parameter couldn't be parsed
		/// or the parsed duration isn't within the allowed bounds.</exception>
		public Limit(string amount, string durationInDays) : base(amount)
		{
			durationInDays = durationInDays.Replace("Tage", "");
			if(!uint.TryParse(durationInDays, out uint result))
				throw new ArgumentException("Das ist keine erkennbare Dauer.");
			var duration = TimeSpan.FromDays(result);
			if(duration < MinLimitDuration || duration > MaxLimitDuration)
				throw new ArgumentException($"Die Dauer eines Limits sollte zwischen {MinLimitDuration.Days} " +
					$"und {MaxLimitDuration.Days} Tagen liegen.");
			Duration = duration;
		}

		/// <summary>
		/// This constructor only exists for SQLite to be able to return collections of
		/// Limits from the database. It should never be used to instantiate a Limit directly within the app.
		/// </summary>
		public Limit() : base() { }

		/// <summary>
		/// Contains the database-relevant properties of this limit.
		/// </summary>
		/// <returns>A one-line string that represents this limit.</returns>
		public override string ToString()
		{
			return $"Time: {Time}  |  Amount: {Amount}  |  Duration: {Duration}";
		}
	}
}