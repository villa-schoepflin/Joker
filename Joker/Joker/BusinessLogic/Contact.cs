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
			MarkedAsProfessional = true
		};

		/// <summary>
		/// The name associated with this contact.
		/// </summary>
		[Column("Name")] public string Name { get; set; }

		/// <summary>
		/// The phone number associated with this contact.
		/// </summary>
		[PrimaryKey, Column("PhoneNumber")] public string PhoneNumber { get; set; }

		/// <summary>
		/// Indicates whether this contact is marked by the user as a professional or expert such as the
		/// number of a counseling center.
		/// </summary>
		[Column("MarkedAsProfessional")] public bool MarkedAsProfessional { get; set; }

		/// <summary>
		/// The standard constructor that should be used every time a new contact is about to be added.
		/// </summary>
		/// <param name="name">The name as supplied from the user.</param>
		/// <param name="phoneNumber">The phone number as supplied from the user.</param>
		/// <param name="markedAsProfessional">Indicates whether the user has marked this contact as a professional
		/// contact.</param>
		/// <exception cref="ArgumentException">Thrown if name or phone number are not within allowed length 
		/// bounds.</exception>
		public Contact(string name, string phoneNumber, bool markedAsProfessional)
		{
			name = name.Trim();
			phoneNumber = phoneNumber.Trim();

			if(name.Length > MaxNameLength || string.IsNullOrEmpty(name))
				throw new ArgumentException($"Der Name sollte zwischen 1 und {MaxNameLength} Zeichen lang sein.");
			if(phoneNumber.Length > MaxPhoneNumberLength || string.IsNullOrEmpty(phoneNumber))
				throw new ArgumentException("Der Eintrag der Telefonnummer sollte zwischen 1 und "
					+ MaxPhoneNumberLength + " Zeichen lang sein.");

			Name = name;
			PhoneNumber = phoneNumber;
			MarkedAsProfessional = markedAsProfessional;
		}

		/// <summary>
		/// This constructor only exists for SQLite to be able to return collections of contacts from the database.
		/// It should never be used to instantiate a contact directly within the app.
		/// </summary>
		public Contact() { }

		/// <summary>
		/// Compares two contacts on whether their phone numbers are the same after removing whitespace.
		/// </summary>
		/// <param name="other">The contact whose phone number will be compared.</param>
		/// <returns>Whether the contacts have identical phone numbers.</returns>
		public bool SamePhoneNumber(Contact other)
		{
			return PhoneNumber.Replace(" ", "") == other.PhoneNumber.Replace(" ", "");
		}

		/// <summary> 
		/// Contains the properties of a contact.
		/// </summary>
		/// <returns>A one-line string that represents this contact.</returns>
		public override string ToString()
		{
			return $"Name: {Name}  |  Phone number: {PhoneNumber}";
		}
	}
}