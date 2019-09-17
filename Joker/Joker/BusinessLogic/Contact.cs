using System;

using SQLite;

namespace Joker.BusinessLogic
{
	/// <summary>
	/// Represents a simplified phone contact with a name and one phone number.
	/// </summary>
	[Table("Contact")]
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

		/// <summary>
		/// Contact information for the Bundeszentrale für gesundheitliche Aufklärung.
		/// </summary>
		internal static readonly Contact Bzga = new Contact
		{
			Name = "Hotline der Bundeszentrale für gesundheitliche Aufklärung",
			PhoneNumber = "+49 800 1372700",
			MarkedAsExpert = true
		};

		/// <summary>
		/// The numeric identifier for a contact in the database.
		/// </summary>
		[PrimaryKey, AutoIncrement, Column("Id")] public long Id { get; set; }

		/// <summary>
		/// The name associated with this contact.
		/// </summary>
		[Column("Name")] public string Name { get; set; }

		/// <summary>
		/// The phone number associated with this contact.
		/// </summary>
		[Column("PhoneNumber")] public string PhoneNumber { get; set; }

		/// <summary>
		/// Indicates whether this contact is marked by the user as a professional or expert such as the
		/// number of a counseling center.
		/// </summary>
		[Column("MarkedAsExpert")] public bool MarkedAsExpert { get; set; }

		/// <summary>
		/// The constructor that should be used when a contact is created from user input.
		/// </summary>
		/// <param name="name">The name as supplied from the user.</param>
		/// <param name="phoneNumber">The phone number as supplied from the user.</param>
		/// <param name="markedAsExpert">Indicates whether the user has marked this contact as a professional
		/// contact.</param>
		/// <exception cref="ArgumentException">Thrown if name or phone number aren't within allowed bounds.</exception>
		public Contact(string name, string phoneNumber, bool markedAsExpert)
		{
			if(name.Length > MaxNameLength || string.IsNullOrEmpty(name))
				throw new ArgumentException($"Der Name sollte zwischen 1 und {MaxNameLength} Zeichen lang sein.");
			if(phoneNumber.Length > MaxPhoneNumberLength || string.IsNullOrEmpty(phoneNumber))
				throw new ArgumentException("Der Eintrag der Telefonnummer sollte zwischen 1 und "
					+ MaxPhoneNumberLength + " Zeichen lang sein.");

			Name = name.Trim();
			PhoneNumber = phoneNumber.Trim();
			MarkedAsExpert = markedAsExpert;
		}

		/// <summary>
		/// The constructor to be used by SQLite database interactions and direct data manipulation.
		/// </summary>
		public Contact() { }

		/// <summary>
		/// Compares two contacts on whether their phone numbers are the same, eliminating whitespace for comparison.
		/// </summary>
		/// <param name="left">The left operand on comparing equality.</param>
		/// <param name="right">The right operand on comparing equality.</param>
		/// <returns>Whether the contacts are equal by their phone number.</returns>
		public static bool operator ==(Contact left, Contact right)
		{
			return left.PhoneNumber.Replace(" ", "") == right.PhoneNumber.Replace(" ", "");
		}

		/// <summary>
		/// Compares two contacts on whether their phone numbers are different, eliminating whitespace for comparison.
		/// </summary>
		/// <param name="left">The left operand on comparing inequality.</param>
		/// <param name="right">The right operand on comparing inequality.</param>
		/// <returns>Whether the contacts are different by their phone number.</returns>
		public static bool operator !=(Contact left, Contact right)
		{
			return left.PhoneNumber.Replace(" ", "") != right.PhoneNumber.Replace(" ", "");
		}

		/// <summary>
		/// Creates an identical clone of a contact.
		/// </summary>
		/// <returns>A deep copy of the contact.</returns>
		public Contact Copy()
		{
			return new Contact { Id = Id, Name = Name, PhoneNumber = PhoneNumber, MarkedAsExpert = MarkedAsExpert };
		}

		/// <summary> 
		/// Contains the properties of a contact.
		/// </summary>
		/// <returns>A one-line string that represents this contact.</returns>
		public override string ToString()
		{
			return $"Name: {Name}  |  Phone number: {PhoneNumber}";
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>True if the specified object is equal to the current object, otherwise false.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}