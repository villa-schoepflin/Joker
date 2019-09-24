using System.IO;
using System.Text.RegularExpressions;

using Xamarin.Essentials;
using Xamarin.Forms;

using Joker.DataAccess;

namespace Joker.AppInterface
{
	/// <summary>
	/// Utility class that handles interaction with embedded resource files.
	/// </summary>
	public static class FileResourceReader
	{
		/// <summary>
		/// The placeholder of the user name for a text resource file.
		/// </summary>
		private const string UserName = "%USERNAME%";

		/// <summary>
		/// The placeholder of the current's limit remaining balance for a text resource file.
		/// </summary>
		private const string RemainingLimit = "%REMAININGLIMIT%";

		/// <summary>
		/// The placeholder of the value of the most recent limit's amount for a text resource file.
		/// </summary>
		private const string LastLimitAmount = "%LASTLIMITAMOUNT%";

		/// <summary>
		/// The placeholder for the mobile OS on which the app runs.
		/// </summary>
		private const string Version = "%VERSION%";

		/// <summary>
		/// Extracts the plaintext from a specified file resource and fills placeholders with the appropriate text.
		/// </summary>
		/// <param name="fileName">The file to look for.</param>
		/// <returns>The plaintext content of the file as a UTF-8 string.</returns>
		public static string Get(string fileName)
		{
			using(var stream = typeof(App).Assembly.GetManifestResourceStream($"Joker.Resources.Text.{fileName}"))
			using(var fileReader = new StreamReader(stream))
			{
				string pattern = $"({UserName}|{RemainingLimit}|{LastLimitAmount}|{Version})";
				return Regex.Replace(fileReader.ReadToEnd(), pattern, new MatchEvaluator(Replace));
			}
		}

		/// <summary>
		/// Replaces matches for the placeholder keywords with their appropriate values.
		/// </summary>
		/// <param name="match">Match of a placeholder.</param>
		/// <returns>Text with which to replace the placeholders.</returns>
		private static string Replace(Match match)
		{
			switch(match.Value)
			{
				case UserName:
					return UserSettings.UserName;
				case RemainingLimit:
					return Database.CalcBalance(Database.MostRecentLimit()).ToString("C", App.Locale);
				case LastLimitAmount:
					return Database.MostRecentLimit().Amount.ToString("C", App.Locale);
				case Version:
					return $"{Device.RuntimePlatform} Version {VersionTracking.CurrentVersion}";
			}
			return match.Value;
		}
	}
}