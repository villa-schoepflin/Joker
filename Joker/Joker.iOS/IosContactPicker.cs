using System;
using System.Threading.Tasks;

using Contacts;
using ContactsUI;
using Foundation;
using UIKit;

using Joker.AppInterface;
using Joker.BusinessLogic;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.iOS.IosContactPicker))]
namespace Joker.iOS
{
	/// <summary>
	/// Contains iOS-specific functionality to interact with the contact list.
	/// </summary>
	public class IosContactPicker : CNContactPickerDelegate, IPlatformContactPicker
	{
		/// <summary>
		/// The callback handler for the asynchronous task through which the contact is picked.
		/// </summary>
		private static TaskCompletionSource<Contact> Callback;

		/// <summary>
		/// Implicit constructor which instantiates this interface implementation as an iOS delegate.
		/// </summary>
		public IosContactPicker() { }

		/// <summary>
		/// Constructor used by the runtime only to create managed representations of unmanaged objects.
		/// </summary>
		/// <param name="handle">Pointer to this object.</param>
		public IosContactPicker(IntPtr handle) : base(handle) { }

		/// <summary>
		/// Opens a view to the iOS contact list allowing the user to pick a single contact with name
		/// and phone number.
		/// </summary>
		/// <returns>The contact contained in an asynchronous task result.</returns>
		public Task<Contact> PickContact()
		{
			Callback = new TaskCompletionSource<Contact>();

			// These 3 statements fix the bug that forces the app's color scheme over foreign view controllers.
			UINavigationBar.Appearance.BarTintColor = UIColor.White;
			UINavigationBar.Appearance.TintColor = UIColor.Red;
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes() { TextColor = UIColor.Black });

			var picker = new CNContactPickerViewController() { Delegate = this };
			UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(picker, true, null);

			return Callback.Task;
		}

		/// <summary>
		/// Handles the callback for when the contact list view is cancelled.
		/// </summary>
		/// <param name="picker">The view controller that was cancelled.</param>
		public override void ContactPickerDidCancel(CNContactPickerViewController picker)
		{
			AppDelegate.CorrectNavigationBarColors();
		}

		/// <summary>
		/// Handles the callback for when a contact was picked from the contact list.
		/// </summary>
		/// <param name="picker">The view controller where the contact was picked.</param>
		/// <param name="contact">The iOS-specific contact object that was picked.</param>
		public override void DidSelectContact(CNContactPickerViewController picker, CNContact contact)
		{
			AppDelegate.CorrectNavigationBarColors();

			// if the contact doesn't have any phone numbers, return with a null result
			if(contact.PhoneNumbers.Length == 0)
				return;

			Callback.SetResult(new Contact
			{
				Name = $"{contact.GivenName} {contact.FamilyName}",
				PhoneNumber = contact.PhoneNumbers[0].Value.ValueForKey(new NSString("digits")).ToString()
			});
		}
	}
}