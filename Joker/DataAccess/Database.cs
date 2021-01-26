using System;
using System.Collections.Generic;
using System.Linq;
using Joker.AppInterface;
using Joker.BusinessLogic;
using SQLite;

namespace Joker.DataAccess
{
	/// <summary>
	/// A utility class containing all methods that interact with the app's local database.
	/// </summary>
	internal static class Database
	{
		/// <summary>
		/// Necessary for a bugfix where limit duration values were corrupted after the update from 1.0.14 to 1.0.15 due
		/// to some parsing error or other change in the SQLite library.
		/// </summary>
		static Database()
		{
			if(!AppSettings.WelcomeTourCompleted)
				return;

			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			bool corrupted = false;
			var limits = db.Table<Limit>().ToArray();
			foreach(var limit in limits)
				if(limit.Duration.TotalDays < 1)
				{
					limit.Duration = TimeSpan.FromDays(limit.Duration.Ticks);
					corrupted = true;
				}
			if(corrupted)
				_ = db.UpdateAll(limits);
		}

		internal static void CreateTables()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			_ = db.CreateTable<Limit>();
			_ = db.CreateTable<Gamble>();
			_ = db.CreateTable<Contact>();
			_ = db.CreateTable<Picture>();
		}

		internal static void Insert(Limit limit)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			_ = db.Insert(limit);
		}

		internal static void Insert(Gamble gamble)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			/* Prevents the insertion of Gamble objects when there is no limit set before the gamble's time property as
			 * this wouldn't make sense for the app's use cases. */
			if(!db.Table<Limit>().Any(l => l.Time < gamble.Time))
				throw new ArgumentException(Text.GambleMustBeAfterFirstLimit);

			/* If there already exists an entry in the database with the same time as the argument, the milliseconds of
			 * the timestamp will continually be incremented. This is to keep the Time property (primary key) unique
			 * without raising exceptions. */
			while(db.Table<Gamble>().Any(g => g.Time == gamble.Time))
				gamble.Time = gamble.Time.AddMilliseconds(1);

			_ = db.Insert(gamble);
		}

		internal static void Insert(Contact contact)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			if(db.Table<Contact>().Any(c => c == contact) || contact == Contact.Bzga)
				throw new ArgumentException(Text.ContactAlreadyExists);
			_ = db.Insert(contact);
		}

		internal static bool InsertPictureFromRandomAsset()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			// All asset paths in the PictureFeed folder are put into an array.
			string[] files = App.Assembly.GetManifestResourceNames();
			files = files.Where(name => name.StartsWith(Folders.PictureAssets)).ToArray();

			/* If the count of pictures in the Picture table is not less than the number of asset paths, the method
			 * returns false because there would be no more new pictures to add. */
			if(db.Table<Picture>().Count() >= files.Length)
				return false;

			/* If there are still image resources available, select a random asset path that isn't in the database yet,
			 * wrap it in a Picture instance, insert it and return true if a row was added. */
			Random random = new();
			Picture pic;
			do
				pic = new(files[random.Next(0, files.Length)]);
			while(db.Table<Picture>().Any(p => p.FilePath == pic.FilePath));

			return db.Insert(pic) > 0;
		}

		internal static int CountLimits()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return db.Table<Limit>().Count();
		}

		internal static TimelineRecord[] AllGamblesAndLimits()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			List<TimelineRecord> list = new();
			list.AddRange(db.Table<Gamble>());
			list.AddRange(db.Table<Limit>());
			return list.OrderByDescending(tr => tr.Time).ToArray();
		}

		internal static Contact[] AllContacts()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return db.Table<Contact>().Reverse().ToArray();
		}

		internal static List<Picture> AllPictures()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return db.Table<Picture>().ToList();
		}

		internal static Picture[] LikedPictures()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return db.Table<Picture>().Where(p => p.Liked).ToArray();
		}

		internal static Limit MostRecentLimit()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return db.Table<Limit>().OrderByDescending(l => l.Time).First();
		}

		internal static Picture MostRecentPicture()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return db.Table<Picture>().OrderByDescending(p => p.TimeAdded).First();
		}

		internal static bool NoGambleAfterMostRecentLimit()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return !db.Table<Gamble>().Any(g => g.Time > MostRecentLimit().Time);
		}

		internal static Limit NextLimitAfter(Limit limit)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return db.Table<Limit>().Where(l => l.Time > limit.Time).FirstOrDefault();
		}

		internal static Gamble[] AllGamblesWithinLimit(Limit limit)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			TableQuery<Gamble> query;

			// Finds the next limits after this one, chronologically.
			var nextLimits = db.Table<Limit>().Where(l => l.Time > limit.Time);

			// Finds all gambles that lie chronologically between the parameter limit and the limit after it.
			if(nextLimits.Any())
			{
				var nextLimit = nextLimits.First();
				query = db.Table<Gamble>().Where(g => g.Time > limit.Time && g.Time < nextLimit.Time);
			}
			/* If the next limit after the argument can't be found (because it doesn't exist yet), only the gambles
			 * directly after the argument are queried, without an upper bound. */
			else
				query = db.Table<Gamble>().Where(g => g.Time > limit.Time);
			return query.ToArray();
		}

		internal static decimal CalcBalance(Limit limit)
		{
			// Subtracts the amounts of all gambles captured by the query from the given limit.
			return AllGamblesWithinLimit(limit).Aggregate(limit.Amount, (limitAmount, g) => limitAmount - g.Amount);
		}

		internal static decimal CalcPreviousLimitBalance()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return CalcBalance(db.Table<Limit>().OrderByDescending(l => l.Time).ElementAt(1));
		}

		internal static decimal CalcRemainingLimit(Gamble gamble)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			/* First, the most recent limit before the argument is selected by querying all limits whose times are
			 * earlier than the argument's time, then ordering them descending by time and selecting the first element.
			 */
			var prevLimit = db.Table<Limit>().Where(l => l.Time < gamble.Time).OrderByDescending(l => l.Time).First();

			/* Then, all gambles whose times are later than the last limit's time up to the argument are selected from
			 * the Gamble table and their amounts are sequentially subtracted from the last limit's amount. */
			return db.Table<Gamble>().Where(g => g.Time > prevLimit.Time && g.Time <= gamble.Time)
				.Aggregate(prevLimit.Amount, (prevLimAmount, g) => prevLimAmount - g.Amount);
		}

		internal static void ToggleLikedStatus(Picture pic)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			pic.Liked ^= true;
			_ = db.Update(pic);
		}

		internal static void Update(Gamble gamble)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			_ = db.Update(gamble);
		}

		internal static void Update(Contact contact)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			if(db.Table<Contact>().Where(c => c.Id == contact.Id).Single() == contact)
				_ = db.Update(contact);
			else if(db.Table<Contact>().Any(c => c == contact) || contact == Contact.Bzga)
				throw new ArgumentException(Text.ContactAlreadyExists);
			else
				_ = db.Update(contact);
		}

		internal static void Delete(Contact contact)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			_ = db.Delete(contact);
		}

		internal static void Delete(Picture pic)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			_ = db.Delete(pic);
		}
	}
}
