﻿using System;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;

using Joker.BusinessLogic;
using Joker.DataAccess;

namespace Joker.UserInterface
{
	/// <summary>
	/// Represents a single contact for display in contact-related pages.
	/// </summary>
	public class ContactViewModel : ViewModel<Page, Contact>
	{
		/// <summary>
		/// The name derived from the contact model.
		/// </summary>
		public string ContactName
		{
			get => Model.Name;
			set => Model.Name = value;
		}

		/// <summary>
		/// The phone number derived from the contact model.
		/// </summary>
		public string PhoneNumber
		{
			get => Model.PhoneNumber;
			set => Model.PhoneNumber = value;
		}

		/// <summary>
		/// The status whether a contact is marked as professional, derived from the contact model.
		/// </summary>
		public bool MarkedAsExpert
		{
			get => Model.MarkedAsExpert;
			set => Model.MarkedAsExpert = value;
		}

		/// <summary>
		/// Indicates whether editing of the contact model is currently enabled.
		/// </summary>
		public bool EditingEnabled { get; set; }

		/// <summary>
		/// Sets the text of the button that toggles the editing status.
		/// </summary>
		public string EditButtonText => EditingEnabled ? "Änderungen speichern" : "Kontakt bearbeiten";

		/// <summary>
		/// The color used to paint the background of the icon frame.
		/// </summary>
		public Color IconBackgroundColor => App.Color(Model.MarkedAsExpert ? "Primary2" : "Primary1");

		/// <summary>
		/// The icon used to differentiate expert contacts from regular contacts.
		/// </summary>
		public ImageSource TypeIcon => ImageSource.FromFile(Model.MarkedAsExpert ? "ui_expert.png" : "ui_contact.png");

		/// <summary>
		/// Navigates the user to a detailed view of the view model's contact.
		/// </summary>
		public ICommand OpenDetailPage => new Command(async () =>
		{
			if(Model != Contact.Bzga)
				await View.Navigation.PushAsync(new ContactInspector(Model.Copy()));
		});

		/// <summary>
		/// Opens the platform's telephone app with the corresponding contact's phone number.
		/// </summary>
		public ICommand CallContact => new Command(() =>
		{
			PhoneDialer.Open(Model.PhoneNumber);
		});

		/// <summary>
		/// Toggles the editing status for the contact detail page.
		/// </summary>
		public ICommand ToggleEditingStatus => new Command(async () =>
		{
			try
			{
				if(EditingEnabled)
				{
					Database.Update(Model);
					App.CurrentContactPage.RefreshContacts();
				}
				EditingEnabled ^= true;
				OnPropertyChanged(nameof(EditingEnabled));
				OnPropertyChanged(nameof(EditButtonText));
			}
			catch(ArgumentException error)
			{
				await View.DisplayAlert(null, error.Message, "Ok");
			}
		});

		/// <summary>
		/// The command by which the model contact will be deleted in the database.
		/// </summary>
		public ICommand DeleteContact => new Command(async () =>
		{
			if(await View.DisplayAlert(null, "Soll dieser Kontakt gelöscht werden?", "Ja", "Nein"))
			{
				Database.Delete(Model);
				await View.Navigation.PopAsync();
				App.CurrentContactPage.RefreshContacts();
			}
		});

		/// <summary>
		/// Constructs a contact view model for the given view.
		/// </summary>
		/// <param name="view">The view for this view model.</param>
		/// <param name="model">The model for this view model.</param>
		public ContactViewModel(Page view, Contact model) : base(view, model) { }
	}
}