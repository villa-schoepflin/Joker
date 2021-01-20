namespace Joker.BusinessLogic
{
	/// <summary>
	/// This class lists short string constants that are displayed to the user in various parts of the app.
	/// </summary>
	internal static class Alerts
	{
		public const string Cancel
			= "Abbrechen";

		public const string ContactAboutToBeDeleted
			= "Soll dieser Kontakt gelöscht werden?";

		public const string ContactNameTooLong
			= "Der Name sollte zwischen 1 und {0} Zeichen lang sein.";

		public const string ContactPhoneNumberTooLong
			= "Der Eintrag der Telefonnummer sollte zwischen 1 und {0} Zeichen lang sein.";

		public const string ContactNotDeletable
			= "Dieser Kontakt ist fest eingespeichert.";

		public const string ContactPermissionDenied
			= "Du hast der App leider die Erlaubnis verweigert, auf Deine Kontakte zuzugreifen.";

		public const string ContactAlreadyExists
			= "Diese Nummer ist bereits in der App verzeichnet.";

		public const string ContactWithoutPhoneNumber
			= "Dieser Kontakt hat keine Telefonnummer.";

		public const string GambleMustBeAfterFirstLimit
			= "Ausgaben dürfen zeitlich nicht vor dem ersten Limit liegen.";

		public const string GambleTimeInFuture
			= "Der Zeitpunkt der Ausgabe sollte nicht in der Zukunft liegen.";

		public const string InputEmpty
			= "Deine Angabe darf hier nicht leer sein.";

		public const string InputTooLong
			= "Deine Angabe hier darf nicht länger als {0} sein.";

		public const string LimitDurationBounds
			= "Die Dauer eines Limits sollte zwischen {0} und {1} Tagen liegen.";

		public const string MonetaryValueBounds
			= "Der Betrag muss zwischen {0:C} und {1:C} liegen.";

		public const string MonetaryValueInvalid
			= "Das ist kein gültiger Geldbetrag.";

		public const string NameSaved
			= "Die App spricht Dich ab jetzt mit {0} an.";

		public const string No
			= "Nein";

		public const string NumberInvalid
			= "Das ist keine gültige Zahl.";

		public const string Ok
			= "Ok";

		public const string PasswordAboutToBeDeleted
			= "Möchtest Du den Passwortschutz entfernen?";

		public const string PasswordContainsSpaces
			= "Das Passwort darf keine Leerzeichen enthalten.";

		public const string PasswordContainsSpecialChars
			= "Das Passwort darf nur Groß-/Kleinbuchstaben und Zahlen enthalten.";

		public const string PasswordDeleted
			= "Du brauchst jetzt kein Passwort mehr für die App.";

		public const string PasswordSaved
			= "Ab jetzt wird Dich die App nach dem Passwort fragen.";

		public const string PasswordTooLong
			= "Das Passwort darf maximal {0} Zeichen lang sein.";

		public const string PictureIntervalBounds
			= "Die Zeit zwischen neuen Bildern sollte zwischen {0} und {1} Tagen liegen.";

		public const string PictureIntervalSaved
			= "Die Einstellung wird nach dem nächsten Bild berücksichtigt.";

		public const string CharsRemaining
			= "noch {0} Zeichen";

		public const string ReminderIntervalBounds
			= "Die Zeit zwischen den Erinnerungen muss zwischen {0} und {1} Stunden liegen.";

		public const string ReminderIntervalSaved
			= "Die nächste Erinnerung kommt in circa {0} Stunden.";

		public const string SavedToGallery
			= "In der Galerie gespeichert.";

		public const string SecurityAttributesNotSaved
			= "Bitte speichere zuerst die Sicherheitsfragen und Antworten.";

		public const string SecurityAttributeSaved
			= "Die Sicherheitsfrage und Antwort wurden gespeichert.";

		public const string StoragePermissionDenied
			= "Die App ist nicht berechtigt auf den Speicher zuzugreifen.";

		public const string TimeSpanInvalid
			= "Das ist keine erkennbare Dauer.";

		public const string TitleOnPasswordDeleted
			= "Passwort entfernt";

		public const string TitleOnPasswordSaved
			= "Passwort erstellt";

		public const string TitleOnSaved
			= "Gespeichert";

		public const string TitleOnSecurityProposals
			= "Vorschläge für Sicherheitsfragen:";

		public const string Yes
			= "Ja";
	}
}
