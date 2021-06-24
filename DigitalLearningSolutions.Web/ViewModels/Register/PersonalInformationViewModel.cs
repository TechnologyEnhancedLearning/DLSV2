﻿namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class PersonalInformationViewModel
    {
        [Required(ErrorMessage = "Enter your first name")]
        [MaxLength(250, ErrorMessage = "First name must be 250 characters or fewer")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter your last name")]
        [MaxLength(250, ErrorMessage = "Last name must be 250 characters or fewer")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter your email address")]
        [MaxLength(250, ErrorMessage = "Email address must be 250 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [NoWhitespace("Email address must not contain any whitespace characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Select a centre")]
        public int? Centre { get; set; }

        public bool IsCentreSpecificRegistration { get; set; }

        public string? CentreName { get; set; }

        public IEnumerable<SelectListItem> CentreOptions { get; set; } = new List<SelectListItem>();
    }
}
