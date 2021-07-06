namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class PersonalInformationViewModel
    {
        public PersonalInformationViewModel() { }

        public PersonalInformationViewModel(RegistrationData data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            Centre = data.Centre;
            Email = data.Email;
        }

        public PersonalInformationViewModel(DelegateRegistrationData data) : this((RegistrationData)data)
        {
            IsCentreSpecificRegistration = data.IsCentreSpecificRegistration;
        }
        public PersonalInformationViewModel(CentreDelegateRegistrationData data) : this((DelegateRegistrationData)data)
        {
            Alias = data.Alias;
        }

        [Required(ErrorMessage = "Enter a first name")]
        [MaxLength(250, ErrorMessage = "First name must be 250 characters or fewer")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter a last name")]
        [MaxLength(250, ErrorMessage = "Last name must be 250 characters or fewer")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter an email address")]
        [MaxLength(250, ErrorMessage = "Email address must be 250 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [NoWhitespace("Email address must not contain any whitespace characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Select a centre")]
        public int? Centre { get; set; }

        [MaxLength(250, ErrorMessage = "Alias must be 250 characters or fewer")]
        public string? Alias { get; set; }

        public bool IsCentreSpecificRegistration { get; set; }

        public string? CentreName { get; set; }

        public IEnumerable<SelectListItem> CentreOptions { get; set; } = new List<SelectListItem>();
    }
}
