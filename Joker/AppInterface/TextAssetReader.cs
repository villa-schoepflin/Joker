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
		/// Regex pattern for detecting uses of placeholder keywords in a text asset.
		/// </summary>
		private static readonly Regex Pattern = new($"({UserName}|{RemainingLimit}|{LastLimitAmount}|{Version})");

		/// <summary>
		/// Extracts the plaintext from a specified text asset and fills placeholders with the appropriate text.
		/// </summary>
		/// <param name="fileName">The file to look for.</param>
		/// <returns>The plaintext content of the file as a UTF-8 string.</returns>
		public static string Get(string fileName)
		{
			string assetPath = Folders.TextAssets + fileName;
			using var stream = JokerApp.Assembly.GetManifestResourceStream(assetPath);
			using StreamReader fileReader = new(stream);
			return Pattern.Replace(fileReader.ReadToEnd(), match => match.Value switch
			{
				UserName => UserSettings.UserName,
				RemainingLimit => Database.CalcBalance(Database.MostRecentLimit()).ToString("C", JokerApp.Locale),
				LastLimitAmount => Database.MostRecentLimit().Amount.ToString("C", JokerApp.Locale),
				Version => $"{Device.RuntimePlatform} Version {VersionTracking.CurrentVersion}",
				_ => match.Value
			});
		}

		#region Replacement constants for text assets (DO NOT CHANGE!)
		private const string UserName = "%USERNAME%";
		private const string RemainingLimit = "%REMAININGLIMIT%";
		private const string LastLimitAmount = "%LASTLIMITAMOUNT%";
		private const string Version = "%VERSION%";
		#endregion
	}
}
