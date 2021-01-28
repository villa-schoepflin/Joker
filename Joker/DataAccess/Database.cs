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

		internal static void Insert(Limit newLimit)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			_ = db.Insert(newLimit);
		}

		internal static void Insert(Gamble newGamble)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			if(!db.Table<Limit>().Any(limit => limit.Time < newGamble.Time))
				throw new ArgumentException(Text.GambleMustBeAfterFirstLimit);

			// Ensures any new gamble is unique, given their time is the primary key.
			var gambles = db.Table<Gamble>();
			while(gambles.Any(gamble => gamble.Time == newGamble.Time))
				newGamble.Time = newGamble.Time.AddMilliseconds(1);

			_ = db.Insert(newGamble);
		}

		internal static void Insert(Contact newContact)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			if(db.Table<Contact>().Contains(newContact) || Contact.Prefixed.Contains(newContact))
				throw new ArgumentException(Text.ContactAlreadyExists);
			_ = db.Insert(newContact);
		}

		internal static bool InsertPictureFromRandomAsset()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			string[] assets = App.Assembly.GetManifestResourceNames();
			assets = assets.Where(name => name.StartsWith(Folders.PictureAssets)).ToArray();

			// Return false if there are no more new pictures available.
			var pics = db.Table<Picture>();
			if(pics.Count() >= assets.Length)
				return false;

			Random random = new();
			Picture newPic;
			do
				newPic = new(assets[random.Next(0, assets.Length)]);
			while(pics.Any(pic => pic.FilePath == newPic.FilePath));

			return db.Insert(newPic) > 0;
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
			return list.OrderByDescending(record => record.Time).ToArray();
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
			return db.Table<Picture>().Where(pic => pic.Liked).ToArray();
		}

		internal static Limit MostRecentLimit()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return db.Table<Limit>().OrderByDescending(limit => limit.Time).First();
		}

		internal static Picture MostRecentPicture()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return db.Table<Picture>().OrderByDescending(pic => pic.TimeAdded).First();
		}

		internal static bool HasGambleAfterMostRecentLimit()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			var mostRecentLimitTime = MostRecentLimit().Time;
			return db.Table<Gamble>().Any(gamble => gamble.Time > mostRecentLimitTime);
		}

		internal static Limit NextLimitAfter(Limit thisLimit)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);
			return db.Table<Limit>().Where(limit => limit.Time > thisLimit.Time).FirstOrDefault();
		}

		internal static Gamble[] AllGamblesWithin(Limit thisLimit)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			var nextLimits = db.Table<Limit>().Where(limit => limit.Time > thisLimit.Time);
			var gambles = db.Table<Gamble>();
			TableQuery<Gamble> gamblesWithin;

			if(nextLimits.Any())
			{
				var nextLimitTime = nextLimits.First().Time;
				gamblesWithin = gambles.Where(gamble => gamble.Time > thisLimit.Time && gamble.Time < nextLimitTime);
			}
			else
				gamblesWithin = gambles.Where(gamble => gamble.Time > thisLimit.Time);

			return gamblesWithin.ToArray();
		}

		internal static decimal CalcBalance(Limit limit)
		{
			Func<decimal, Gamble, decimal> subtract = (limitAmount, gamble) => limitAmount - gamble.Amount;
			return AllGamblesWithin(limit).Aggregate(limit.Amount, subtract);
		}

		internal static decimal CalcPreviousLimitBalance()
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			var prevLimit = db.Table<Limit>().OrderByDescending(limit => limit.Time).ElementAt(1);
			return CalcBalance(prevLimit);
		}

		internal static decimal CalcRemainingLimit(Gamble thisGamble)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			var limitsBeforeGamble = db.Table<Limit>().Where(limit => limit.Time < thisGamble.Time);
			var priorLimit = limitsBeforeGamble.OrderByDescending(limit => limit.Time).First();

			Func<Gamble, bool> inRange = gamble => gamble.Time > priorLimit.Time && gamble.Time <= thisGamble.Time;
			var gamblesInRange = db.Table<Gamble>().Where(inRange);

			Func<decimal, Gamble, decimal> subtract = (priorLimitAmount, gamble) => priorLimitAmount - gamble.Amount;
			return gamblesInRange.Aggregate(priorLimit.Amount, subtract);
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

		internal static void Update(Contact thisContact)
		{
			using SQLiteConnection db = new(AppSettings.DatabaseFilePath);

			var dbContacts = db.Table<Contact>();
			var contactById = dbContacts.Where(contact => contact.Id == thisContact.Id).First();
			if(contactById.Equals(thisContact))
				_ = db.Update(thisContact);
			else if(dbContacts.Contains(thisContact) || Contact.Prefixed.Contains(thisContact))
				throw new ArgumentException(Text.ContactAlreadyExists);
			else
				_ = db.Update(thisContact);
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
