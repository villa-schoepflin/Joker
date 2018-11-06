using System.IO;
using Joker.DataAccess;

namespace Joker.AppInterface
{
	/// <summary>
	/// Utility class that handles interaction with embedded resource files.
	/// </summary>
	public static class FileResourceReader
	{
		/// <summary>
		/// The placeholder for when the user name should be inserted into the content of a file resource.
		/// </summary>
		public const string UserNamePlaceholder = "$USERNAME$";

		/// <summary>
		/// The placeholder for when the current's limit remaining balance should be inserted into the content
		/// of a file resource.
		/// </summary>
		public const string RemainingLimitPlaceholder = "$REMAININGLIMIT$";

		/// <summary>
		/// Extracts the plaintext from a specified file resource and fills placeholders with the appropriate text.
		/// </summary>
		/// <param name="fileName">The file to look for.</param>
		/// <returns>The plaintext content of the file as a UTF-8 string.</returns>
		public static string Get(string fileName)
		{
			string content;

			using(var stream = typeof(App).Assembly.GetManifestResourceStream($"Joker.Resources.Text.{fileName}"))
			using(var fileReader = new StreamReader(stream))
				content = fileReader.ReadToEnd();

			content = content.Replace(UserNamePlaceholder, UserSettings.UserName);
			content = content.Replace(RemainingLimitPlaceholder, Database.CalcBalance(Database.MostRecentLimit())
				.ToString("C", App.Locale));

			return content;
		}
	}
}