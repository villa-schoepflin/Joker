using System;

namespace Joker.AppInterface
{
	/// <summary>
	/// A custom API for the shared code that abstracts away the platform-specific push notification services of the
	/// target systems specifically for use in this app.
	/// </summary>
	public interface IPlatformNotifier
	{
		/// <summary>
		/// Schedules a notification indicating the current limit has expired and a new one should be set.
		/// </summary>
		/// <param name="timeSetting">The time at which the notification should appear.</param>
		void ScheduleLimitExpired(DateTime timeSetting);

		/// <summary>
		/// Removes the notification that indicates that the current limit has expired.
		/// </summary>
		void CancelLimitExpired();

		/// <summary>
		/// Schedules a notification indicating that a new picture is available to see.
		/// </summary>
		/// <param name="timeSetting">The time at which the notification should appear.</param>
		void ScheduleNewPicture(DateTime timeSetting);

		/// <summary>
		/// Schedules a notification reminding the user to always record acts of gambling within the app.
		/// </summary>
		/// <param name="interval">The interval for the time the notification should appear.</param>
		void ScheduleGambleReminder(TimeSpan interval);

		/// <summary>
		/// Schedules a notification reminding the user about the current state of their limit.
		/// </summary>
		/// <param name="interval">The interval for the time the notification should appear.</param>
		void ScheduleLimitReminder(TimeSpan interval);
	}
}
