using System;
using SQLite;

namespace Joker.BusinessLogic
{
	/// <summary>
	/// Base class for gambles and limits, as they are recordings of gambling or self-imposed limits by the user that
	/// can be ordered chronologically.
	/// </summary>
	public abstract class TimelineRecord
	{
		/// <summary>
		/// The minimum amount for a gamble or a limit.
		/// </summary>
		public const decimal MinAmount = 0.01m;

		/// <summary>
		/// The maximum amount for a gamble or a limit.
		/// </summary>
		public const decimal MaxAmount = 500;

		/// <summary>
		/// The time at which the gamble occurred or the limit has been set.
		/// </summary>
		[PrimaryKey, Column(TimeColumnName)] public DateTime Time { get; set; }

		/// <summary>
		/// The amount relevant to this gamble or limit.
		/// </summary>
		[Column(AmountColumnName)] public decimal Amount { get; set; }

		/// <summary>
		/// Constructor called for limits as well as gambles that don't have a user-specified time.
		/// </summary>
		/// <param name="amount">The amount as put in by the user.</param>
		protected TimelineRecord(string amount)
		{
			Time = DateTime.UtcNow;
			Amount = Parse(amount);
		}

		/// <summary>
		/// This constructor is necessary for the subclasses to be instantiated by SQLite in database queries.
		/// </summary>
		protected TimelineRecord() { }

		/// <summary>
		/// A parsing function to get a decimal monetary value from user input.
		/// </summary>
		/// <param name="amount">The amount to be parsed as put in by the user.</param>
		/// <returns>A monetary value in decimal form.</returns>
		/// <exception cref="ArgumentException">Thrown if the amount parameter couldn't be parsed or isn't within the
		/// allowed bounds.</exception>
		private static decimal Parse(string amount)
		{
			amount = amount.Replace('.', ',');

			// Checks if the parsed result has more than two digits after the comma
			if(decimal.TryParse(amount, out decimal result) && result * 100 == Math.Floor(result * 100))
			{
				if(result is >= MinAmount and <= MaxAmount)
					return result;
				throw new ArgumentException(string.Format(Text.MonetaryValueBounds, MinAmount, MaxAmount));
			}
			throw new ArgumentException(Text.MonetaryValueInvalid);
		}

		#region Identifiers for the database schema (DO NOT CHANGE!)
		private const string TimeColumnName = "Time";
		private const string AmountColumnName = "Amount";
		#endregion
	}
}
