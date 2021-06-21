﻿namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class PersonalInformationViewModel
    {
        [Required(ErrorMessage = "Enter your first name")]
        [MaxLength(100, ErrorMessage = "First name must be at most 100 characters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter your last name")]
        [MaxLength(100, ErrorMessage = "Last name must be at most 100 characters")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter your email address")]
        [EmailAddress(ErrorMessage = "Enter an email address with an @ in it, such as example@domain.com")]
        [MaxLength(100, ErrorMessage = "Email address must be at most 100 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Select a centre")]
        public int? Centre { get; set; }

        public bool IsCentreSpecificRegistration { get; set; }

        public string? CentreName { get; set; }

        public IEnumerable<SelectListItem> CentreOptions { get; set; } = new List<SelectListItem>();
    }
}
