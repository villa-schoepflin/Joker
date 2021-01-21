using System;
using SQLite;

namespace Joker.BusinessLogic
{
	/// <summary>
	/// Determines how much the user is allowed to spend for gambling in the given duration.
	/// </summary>
	[Table(LimitTableName)]
	public sealed class Limit : TimelineRecord
	{
		/// <summary>
		/// The duration applied to the first limit set in the welcome tour.
		/// </summary>
		public static readonly TimeSpan InitialLimitDuration = TimeSpan.FromDays(7);

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
		[Column(DurationColumnName)] public TimeSpan Duration { get; set; }

		/// <summary>
		/// The standard constructor that should be used every time a new limit must be set.
		/// </summary>
		/// <param name="amount">The amount as put in by the user.</param>
		/// <param name="durationInDays">The duration in days as put in by the user.</param>
		/// <exception cref="ArgumentException">Thrown if the duration parameter couldn't be parsed or the parsed
		/// duration isn't within the allowed bounds.</exception>
		public Limit(string amount, string durationInDays) : base(amount)
		{
			if(!uint.TryParse(durationInDays, out uint result))
				throw new ArgumentException(Text.TimeSpanInvalid);

			var duration = TimeSpan.FromDays(result);
			if(duration < MinLimitDuration || duration > MaxLimitDuration)
			{
				string msg = string.Format(Text.LimitDurationBounds, MinLimitDuration.Days, MaxLimitDuration.Days);
				throw new ArgumentException(msg);
			}
			Duration = duration;
		}

		/// <summary>
		/// This constructor only exists for SQLite to be able to return collections of limits from the database. It
		/// should never be used to instantiate a limit directly within the app.
		/// </summary>
		public Limit() : base() { }

		/// <summary>
		/// Contains the database-relevant properties of this limit.
		/// </summary>
		/// <returns>A one-line string that represents this limit.</returns>
		public override string ToString()
		{
			return $"Time: {Time} | Amount: {Amount} | Duration: {Duration}";
		}

		#region Identifiers for the database schema (DO NOT CHANGE!)
		private const string LimitTableName = "Limit";
		private const string DurationColumnName = "Duration";
		#endregion
	}
}
