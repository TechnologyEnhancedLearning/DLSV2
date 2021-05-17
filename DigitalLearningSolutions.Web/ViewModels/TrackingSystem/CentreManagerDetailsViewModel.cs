﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models;

    public class CentreManagerDetailsViewModel
    {
        public CentreManagerDetailsViewModel(){ }

        public CentreManagerDetailsViewModel(Centre centre)
        {
            FirstName = centre.ContactForename;
            LastName = centre.ContactSurname;
            Email = centre.ContactEmail;
            Telephone = centre.ContactTelephone;
        }

        [Required(ErrorMessage = "Please enter a first name.")]
        [MaxLength(250, ErrorMessage = "Please enter a first name that is less than 250 characters.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name.")]
        [MaxLength(250, ErrorMessage = "Please enter a last name that is less than 250 characters.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please enter an email address.")]
        [MaxLength(250, ErrorMessage = "Please enter an email address that is less than 250 characters.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        [MaxLength(250, ErrorMessage = "Please enter a valid phone number")]
        public string? Telephone { get; set; }
    }
}
