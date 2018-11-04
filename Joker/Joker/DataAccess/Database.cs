using System;
using System.Collections.Generic;
using System.Linq;

using SQLite;

using Joker.BusinessLogic;

namespace Joker.DataAccess
{
	/// <summary>
	/// A utility class containing all methods that interact with the app's local database.
	/// </summary>
	internal static class Database
	{
		/// <summary>
		/// Ensures all necessary tables for database operations exist before they are performed.
		/// </summary>
		internal static void Initialize()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
			{
				db.CreateTable<Limit>();
				db.CreateTable<Gamble>();
				db.CreateTable<Contact>();
				db.CreateTable<Picture>();
			}
		}

		/// <summary>
		/// Inserts the specified Limit object into the database.
		/// </summary>
		/// <param name="limit">The limit to be inserted.</param>
		/// <exception cref="SQLiteException">Thrown if a limit with the parameter's time property already
		/// exists in the database.</exception>
		internal static void Insert(Limit limit)
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				db.Insert(limit);
		}

		/// <summary>
		/// Safely inserts a Gamble object into the database. Contains input validation.
		/// </summary>
		/// <param name="gamble">The gamble to be inserted.</param>
		/// <exception cref="ArgumentException">Thrown if the parameter's time property is earlier than the
		/// first limit's time.</exception>
		internal static void Insert(Gamble gamble)
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
			{
				/* Prevents the insertion of Gamble objects when there is no limit set before the gamble's
				 * time property as this wouldn't make sense for the app's use cases. */
				if(db.Table<Limit>().Where(l => l.Time < gamble.Time).Count() == 0)
					throw new ArgumentException("Bitte gib keine Ausgabe ein, die zeitlich vor dem ersten Limit liegt.");

				/* If there already exists an entry in the database with the same time as the argument, the
				 * milliseconds of the timestamp will continually be incremented. This is to keep the Time
				 * property (primary key) unique without raising exceptions. */
				while(db.Table<Gamble>().Where(g => g.Time == gamble.Time).Count() > 0)
					gamble.Time = gamble.Time.AddMilliseconds(1);

				db.Insert(gamble);
			}
		}

		/// <summary>
		/// Inserts a contact into the database when there is no other contact with the same phone number.
		/// </summary>
		/// <param name="contact">The contact to be inserted.</param>
		/// <exception cref="ArgumentException">Thrown if a contact with the same phone number already exists
		/// in the database.</exception>
		internal static void Insert(Contact contact)
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
			{
				if(db.Table<Contact>().Any(c => c == contact || contact == Contact.Bzga))
					throw new ArgumentException("Ein Kontakt mit dieser Telefonnummer ist bereits verzeichnet.");
				db.Insert(contact);
			}
		}

		/// <summary>
		/// Selects a random image resource from the PictureFeed folder and inserts it as a Picture object
		/// into the database. Prevents duplicate images.
		/// </summary>
		/// <returns>Returns whether a random picture could be inserted, "true" indicating success.</returns>
		internal static bool InsertPictureFromRandomResource()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
			{
				// All resource paths in the PictureFeed folder are put into an array.
				string[] files = typeof(App).Assembly.GetManifestResourceNames()
					.Where(name => name.Contains("PictureFeed")).ToArray();

				/* If the count of pictures in the Picture table is not less than the number of resource paths,
				 * the method returns false because there would be no more new pictures to add. */
				if(db.Table<Picture>().Count() >= files.Length)
					return false;

				/* If there are still image resources available, select a random resource path that isn't in the
				 * database yet, wrap it in a Picture instance, insert it and return true if a row was added. */
				Picture pic;
				do
					pic = new Picture(files[new Random().Next(0, files.Length)]);
				while(db.Table<Picture>().Where(p => p.FilePath == pic.FilePath).Count() > 0);
				return db.Insert(pic) > 0;
			}
		}

		/// <summary>
		/// Counts the number of limits stored in the database.
		/// </summary>
		/// <returns>Number of rows of the Limit table.</returns>
		internal static int CountLimits()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				return db.Table<Limit>().Count();
		}

		/// <summary>
		/// Joins the Gamble and Limit tables, copying them as an array of timeline records
		/// sorted by their Time property from newest to oldest.
		/// </summary>
		/// <returns>An array of all gambles and limits, unbound to the database.</returns>
		internal static TimelineRecord[] AllGamblesAndLimits()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
			{
				var list = new List<TimelineRecord>();
				list.AddRange(db.Table<Gamble>());
				list.AddRange(db.Table<Limit>());
				return list.OrderByDescending(tr => tr.Time).ToArray();
			}
		}

		/// <summary>
		/// Gets a copy of all contact entries in the database, from newest to oldest.
		/// </summary>
		/// <returns>An array of contacts, unbound to the database.</returns>
		internal static Contact[] AllContacts()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				return db.Table<Contact>().Reverse().ToArray();
		}

		/// <summary>
		/// Gets a copy of all picture entries in the database.
		/// </summary>
		/// <returns>A list of pictures, unbound to the database.</returns>
		internal static List<Picture> AllPictures()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				return db.Table<Picture>().ToList();
		}

		/// <summary>
		/// Gets a copy of all picture entries in the database marked as liked.
		/// </summary>
		/// <returns>An array of pictures marked as liked, unbound to the database.</returns>
		internal static Picture[] LikedPictures()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				return db.Table<Picture>().Where(p => p.Liked).ToArray();
		}

		/// <summary>
		/// Gets a copy of the most recent limit added to the database.
		/// </summary>
		/// <returns>A single Limit instance, unbound to the database.</returns>
		internal static Limit MostRecentLimit()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				return db.Table<Limit>().OrderByDescending(l => l.Time).First();
		}

		/// <summary>
		/// Returns a copy of the most recent picture added to the database.
		/// </summary>
		/// <returns>A single Picture instance, unbound to the database.</returns>
		internal static Picture MostRecentPicture()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				return db.Table<Picture>().OrderByDescending(p => p.TimeAdded).First();
		}

		/// <summary>
		/// Returns a copy of the picture from the database whose file name matches the argument.
		/// </summary>
		/// <param name="filePath">File path by which the picture should be found.</param>
		/// <returns>A single Picture instance, unbound to the database.</returns>
		internal static Picture GetPictureByFileName(string filePath)
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				return db.Table<Picture>().Where(p => p.FilePath == filePath).First();
		}

		/// <summary>
		/// Indicates whether the user has entered a gamble after the most recent limit has been set.
		/// </summary>
		/// <returns>True when there are no gambles after the most recent limit, false otherwise.</returns>
		internal static bool NoGambleAfterMostRecentLimit()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				return !db.Table<Gamble>().Any(g => g.Time > MostRecentLimit().Time);
		}

		/// <summary>
		/// Returns how much of a limit's amount has been consumed by the gambles within its duration.
		/// </summary>
		/// <param name="limit">The limit whose balance should be calculated.</param>
		/// <returns>How much of the limit remains as a decimal number.</returns>
		internal static decimal CalcBalance(Limit limit)
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
			{
				TableQuery<Gamble> query;
				try
				{
					// Finds the next limit after this one, chronologically.
					var nextLimit = db.Table<Limit>().Where(l => l.Time > limit.Time).First();

					// Finds all gambles that lie chronologically between the parameter limit and the limit after it.
					query = db.Table<Gamble>().Where(g => g.Time > limit.Time && g.Time < nextLimit.Time);
				}
				catch(InvalidOperationException)
				{
					/* If the next limit after the argument can't be found (because it doesn't exist yet), only the
					 * gambles directly after the argument are queried, without an upper bound. */
					query = db.Table<Gamble>().Where(g => g.Time > limit.Time);
				}
				// Subtracts the amounts of all gambles captured by the query from the supplied parameter limit.
				return query.Aggregate(limit.Amount, (limitAmount, g) => limitAmount - g.Amount);
			}
		}

		/// <summary>
		/// Returns how much of the second most recent limit's amount has been depleted by the user's gambling records.
		/// </summary>
		/// <returns>The current balance of the limit as a decimal.</returns>
		internal static decimal CalcPreviousLimitBalance()
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				return CalcBalance(db.Table<Limit>().OrderByDescending(l => l.Time).ElementAt(1));
		}

		/// <summary>
		/// Calculates the remaining limit amount chronologically after a particular gamble.
		/// </summary>
		/// <param name="gamble">The gamble whose remaining limit should be calculated.</param>
		/// <returns>The decimal amount of the limit minus all relevant gambles' amounts.</returns>
		internal static decimal CalcRemainingLimit(Gamble gamble)
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
			{
				/* First, the most recent limit before the argument is selected by querying all limits
				 * whose times are earlier than the argument's time, then ordering them descending by time
				 * and selecting the first element. */
				var prevLimit = db.Table<Limit>().Where(l => l.Time < gamble.Time)
					.OrderByDescending(l => l.Time).First();

				/* Then, all gambles whose times are later than the last limit's time up to the argument are selected
				 * from the Gamble table and their amounts are sequentially subtracted from the last limit's amount. */
				return db.Table<Gamble>().Where(g => g.Time > prevLimit.Time && g.Time <= gamble.Time)
					.Aggregate(prevLimit.Amount, (prevLimitAmount, g) => prevLimitAmount - g.Amount);
			}
		}

		/// <summary>
		/// Switches the Liked status of the supplied picture in the database if it exists.
		/// </summary>
		/// <param name="pic">The picture whose Liked status should be toggled.</param>
		internal static void ToggleLikedStatus(Picture pic)
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
			{
				pic.Liked ^= true;
				db.Update(pic);
			}
		}

		/// <summary>
		/// Updates the specified contact in the database.
		/// </summary>
		/// <param name="contact">The contact to be updated.</param>
		/// <exception cref="ArgumentException">Thrown if another contact with the parameter contact's phone number
		/// already exists in the database.</exception>
		internal static void Update(Contact contact)
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
			{
				if(db.Table<Contact>().Where(c => c.Id == contact.Id).Single() == contact)
					db.Update(contact);
				else if(db.Table<Contact>().Any(c => c == contact) || contact == Contact.Bzga)
					throw new ArgumentException("Ein Kontakt mit dieser Telefonnummer ist bereits verzeichnet.");
				else
					db.Update(contact);
			}
		}

		/// <summary>
		/// Deletes the specified contact in the database.
		/// </summary>
		/// <param name="contact">The contact to be deleted.</param>
		internal static void Delete(Contact contact)
		{
			using(var db = new SQLiteConnection(AppSettings.DatabaseFilePath))
				db.Delete(contact);
		}
	}
}