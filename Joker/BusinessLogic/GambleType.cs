using System.Collections.Generic;
using System.Linq;

namespace Joker.BusinessLogic
{
	/// <summary>
	/// Contains the different types a gamble can be classified as, usually the kind of place where the money was spent.
	/// </summary>
	public enum GambleType
	{
		/// <summary>
		/// The default type.
		/// </summary>
		Other = 0,
		/// <summary>
		/// Indicates a lottery, such as LOTTO 6aus49 or Eurojackpot.
		/// </summary>
		Lottery = 1,
		/// <summary>
		/// Indicates a sports betting institution, such as ODDSET or Tipico.
		/// </summary>
		SportsBet = 2,
		/// <summary>
		/// Indicates a casino or "gambling house" of any kind.
		/// </summary>
		Casino = 3,
		/// <summary>
		/// Indicates a gamble on a slot machine of some kind.
		/// </summary>
		SlotMachine = 4
	}

	/// <summary>
	/// Encapsulates logic concerning the GambleType enumeration.
	/// </summary>
	public static class GambleTypes
	{
		/// <summary>
		/// Holds the translated German terms for each type of gamble.
		/// </summary>
		private static readonly Dictionary<GambleType, string> TypeNames = new()
		{
			[GambleType.Other] = "Sonstige",
			[GambleType.Lottery] = "Lotterie",
			[GambleType.SportsBet] = "Sportwette",
			[GambleType.Casino] = "Casino",
			[GambleType.SlotMachine] = "Geldspielautomat"
		};

		/// <summary>
		/// Returns the translated German terms of the gamble types in order.
		/// </summary>
		public static string[] Names => TypeNames.Values.ToArray();

		/// <summary>
		/// Gets the translated German term for the supplied gamble type.
		/// </summary>
		/// <param name="type">The gamble type constant whose translation should be given.</param>
		/// <returns>The translated term as a string.</returns>
		public static string GetName(GambleType type)
		{
			return TypeNames[type];
		}

		/// <summary>
		/// Finds the first key in the dictionary whose name equals the parameter.
		/// </summary>
		/// <param name="name">Translated name of the gamble type for which the type should be returned.</param>
		/// <returns>The gamble type belonging to the supplied translated name.</returns>
		public static GambleType GetGambleType(string name)
		{
			return TypeNames.First(pair => pair.Value == name).Key;
		}
	}
}
