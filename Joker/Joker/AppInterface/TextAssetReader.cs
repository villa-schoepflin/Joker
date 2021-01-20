using System.IO;
using System.Text.RegularExpressions;
using Joker.DataAccess;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Joker.AppInterface
{
	/// <summary>
	/// Utility class that handles interaction with embedded text assets.
	/// </summary>
	public static class TextAssetReader
	{
		/// <summary>
		/// The placeholder of the user name for a text asset.
		/// </summary>
		private const string UserName = "%USERNAME%";

		/// <summary>
		/// The placeholder of the current's limit remaining balance for a text asset.
		/// </summary>
		private const string RemainingLimit = "%REMAININGLIMIT%";

		/// <summary>
		/// The placeholder of the value of the most recent limit's amount for a text asset.
		/// </summary>
		private const string LastLimitAmount = "%LASTLIMITAMOUNT%";

		/// <summary>
		/// The placeholder for the mobile OS on which the app runs.
		/// </summary>
		private const string Version = "%VERSION%";

		/// <summary>
		/// Regex pattern for detecting uses of placeholder keywords in a text asset.
		/// </summary>
		private static readonly Regex Pattern = new Regex($"({UserName}|{RemainingLimit}|{LastLimitAmount}|{Version})");

		/// <summary>
		/// Extracts the plaintext from a specified file resource and fills placeholders with the
		/// appropriate text.
		/// </summary>
		/// <param name="fileName">The file to look for.</param>
		/// <returns>The plaintext content of the file as a UTF-8 string.</returns>
		public static string Get(string fileName)
		{
			string textAssetUrl = $"Joker.Assets.Text.{fileName}";
			using var stream = typeof(App).Assembly.GetManifestResourceStream(textAssetUrl);
			using var fileReader = new StreamReader(stream);
			return Pattern.Replace(fileReader.ReadToEnd(), Replace);
		}

		/// <summary>
		/// Replaces matches for the placeholder keywords with their appropriate values.
		/// </summary>
		/// <param name="match">Match of a placeholder.</param>
		/// <returns>Text with which to replace the placeholders.</returns>
		private static string Replace(Match match)
		{
			return match.Value switch
			{
				UserName => UserSettings.UserName,
				RemainingLimit => Database.CalcBalance(Database.MostRecentLimit()).ToString("C", App.Locale),
				LastLimitAmount => Database.MostRecentLimit().Amount.ToString("C", App.Locale),
				Version => $"{Device.RuntimePlatform} Version {VersionTracking.CurrentVersion}",
				_ => match.Value,
			};
		}
	}
}
