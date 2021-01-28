using System;
using SQLite;

namespace Joker.BusinessLogic
{
	/// <summary>
	/// Represents a simplified phone contact with a name and one phone number.
	/// </summary>
	[Table(ContactTableName)]
	public sealed class Contact
	{
		/// <summary>
		/// The maximum string length for the contact's name.
		/// </summary>
		public const int MaxNameLength = 60;

		/// <summary>
		/// The maximum string length for the contact's phone number.
		/// </summary>
		public const int MaxPhoneNumberLength = 25;

		internal static readonly Contact[] Prefixed = new[]
		{
			new Contact
			{
				Name = "Hotline der Bundeszentrale für gesundheitliche Aufklärung",
				PhoneNumber = "+49 800 1372700",
				MarkedAsExpert = true
			}
		};

		/// <summary>
		/// The numeric identifier for a contact in the database.
		/// </summary>
		[PrimaryKey, AutoIncrement, Column(IdColumnName)] public long Id { get; set; }

		/// <summary>
		/// The name associated with this contact.
		/// </summary>
		[Column(NameColumnName)] public string Name { get; set; }

		/// <summary>
		/// The phone number associated with this contact.
		/// </summary>
		[Column(PhoneNumberColumnName)] public string PhoneNumber { get; set; }

		/// <summary>
		/// Indicates whether this contact is marked by the user as a professional or expert such as the number of a
		/// counseling center.
		/// </summary>
		[Column(MarkedAsExpertColumnName)] public bool MarkedAsExpert { get; set; }

		/// <summary>
		/// The constructor that should be used when a contact is created from user input.
		/// </summary>
		/// <param name="name">The name as supplied from the user.</param>
		/// <param name="phoneNumber">The phone number as supplied from the user.</param>
		/// <param name="markedAsExpert">Indicates whether the user has marked this contact as a professional.</param>
		/// <exception cref="ArgumentException">Thrown if name or phone number aren't within allowed bounds.</exception>
		public Contact(string name, string phoneNumber, bool markedAsExpert)
		{
			if(name.Length > MaxNameLength || string.IsNullOrEmpty(name))
				throw new ArgumentException(string.Format(Text.ContactNameTooLong, MaxNameLength));

			if(phoneNumber.Length > MaxPhoneNumberLength || string.IsNullOrEmpty(phoneNumber))
				throw new ArgumentException(string.Format(Text.ContactPhoneNumberTooLong, MaxPhoneNumberLength));

			Name = name.Trim();
			PhoneNumber = phoneNumber.Trim();
			MarkedAsExpert = markedAsExpert;
		}

		/// <summary>
		/// The constructor to be used by SQLite database interactions and direct data manipulation.
		/// </summary>
		public Contact()
		{ }

		/// <summary>
		/// Determines whether the specified object is equal to the current contact.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>True if the given object equals the current one, otherwise false.</returns>
		/// <exception cref="InvalidCastException">Thrown if the argument isn't a contact.</exception>
		public override bool Equals(object obj)
		{
			var other = (Contact)obj;
			return PhoneNumber.Replace(" ", "") == other.PhoneNumber.Replace(" ", "");
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region Identifiers for the database schema (DO NOT CHANGE!)
		private const string ContactTableName = "Contact";
		private const string IdColumnName = "Id";
		private const string NameColumnName = "Name";
		private const string PhoneNumberColumnName = "PhoneNumber";
		private const string MarkedAsExpertColumnName = "MarkedAsExpert";
		#endregion
	}
}
