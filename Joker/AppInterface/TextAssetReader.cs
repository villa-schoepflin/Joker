using System.IO;
using System.Text.RegularExpressions;
using Joker.DataAccess;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Joker.AppInterface
{
	internal static class TextAssetReader
	{
		private static readonly Regex Pattern = new($"({UserName}|{RemainingLimit}|{LastLimitAmount}|{Version})");

		internal static string Get(string fileName)
		{
			string assetPath = Folders.TextAssets + fileName;
			using var stream = App.Assembly.GetManifestResourceStream(assetPath);
			using StreamReader fileReader = new(stream);
			return Pattern.Replace(fileReader.ReadToEnd(), match => match.Value switch
			{
				UserName => UserSettings.UserName,
				RemainingLimit => Database.CalcBalance(Database.MostRecentLimit()).ToString("C", App.Locale),
				LastLimitAmount => Database.MostRecentLimit().Amount.ToString("C", App.Locale),
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
