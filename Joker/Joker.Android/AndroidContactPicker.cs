using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Provider;

using Joker.ApplicationLayer;
using Joker.BusinessLogic;

[assembly: Xamarin.Forms.Dependency(typeof(Joker.Droid.AndroidContactPicker))]
namespace Joker.Droid
{
	/// <summary>
	/// Contains Android-specific functionality to interact with the contact list.
	/// </summary>
	public class AndroidContactPicker : IPlatformContactPicker
	{
		/// <summary>
		/// The constant request code associated with the contact-picking activity.
		/// </summary>
		internal const int RequestCode = 0;

		/// <summary>
		/// The callback handler for the asynchronous task through which the contact is picked.
		/// </summary>
		private static TaskCompletionSource<Contact> Callback;

		/// <summary>
		/// Opens a result-based activity from the main activity to the Android contact list
		/// allowing the user to pick a single contact with name and phone number.
		/// </summary>
		/// <returns>The contact contained in an asynchronous task result.</returns>
		public Task<Contact> PickContact()
		{
			Callback = new TaskCompletionSource<Contact>();

			var intent = new Intent(Intent.ActionPick);
			intent.SetType(ContactsContract.CommonDataKinds.Phone.ContentType);
			MainActivity.Instance.StartActivityForResult(intent, RequestCode);

			return Callback.Task;
		}

		/// <summary>
		/// Handles all actions for when the contact-picking activity was cancelled or a contact was picked.
		/// </summary>
		/// <param name="intent">The intent containing the picked contact if one was picked.</param>
		internal static void Finish(Intent intent)
		{
			if(intent == null)
				return;

			string[] projection =
			{
				ContactsContract.ContactNameColumns.DisplayNamePrimary,
				ContactsContract.CommonDataKinds.Phone.Number
			};
			var data = Application.Context.ContentResolver.Query(intent.Data, projection, null, null, null);
			data.MoveToFirst();
			Callback.SetResult(new Contact
			{
				Name = data.GetString(data.GetColumnIndex(projection[0])),
				PhoneNumber = data.GetString(data.GetColumnIndex(projection[1]))
			});
		}
	}
}