namespace Joker.BusinessLogic
{
	/// <summary>
	/// Represents a simplified phone contact with a name and one phone number.
	/// </summary>
	public sealed class Contact
	{
		/// <summary>
		/// Contact information for the Bundeszentrale für gesundheitliche Aufklärung.
		/// </summary>
		internal static readonly Contact Bzga = new Contact
		{
			Name = "Hotline der BZgA",
			PhoneNumber = "+49 800 1372700"
		};

		/// <summary>
		/// Contact information for the publisher of this app.
		/// </summary>
		internal static readonly Contact VillaSchoepflin = new Contact
		{
			Name = "Villa Schöpflin gGmbH",
			PhoneNumber = "+49 7621 9149090"
		};

		/// <summary>
		/// The name associated with this contact.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The phone number associated with this contact.
		/// </summary>
		public string PhoneNumber { get; set; }

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