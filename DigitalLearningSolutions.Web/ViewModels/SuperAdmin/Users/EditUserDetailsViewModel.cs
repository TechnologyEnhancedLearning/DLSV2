using DigitalLearningSolutions.Data.Models.User;
using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Helpers;
using FluentMigrator.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users
{
    public class EditUserDetailsViewModel
    {
        public EditUserDetailsViewModel() { }
        public EditUserDetailsViewModel(UserAccount userAccount)
        {
            Id = userAccount.Id;
            FirstName = userAccount.FirstName;
            LastName = userAccount.LastName;
            JobGroupId = userAccount.JobGroupId;
            ProfessionalRegistrationNumber = userAccount.ProfessionalRegistrationNumber;
            PrimaryEmail = userAccount.PrimaryEmail;
            EmailVerified = userAccount.EmailVerified;
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter a first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter a last name")]
        public string LastName { get; set; }

        public int JobGroupId { get; set; }

        public string? ProfessionalRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Enter a primary email")]
        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(ErrorMessage = CommonValidationErrorMessages.WhitespaceInEmail)]
        public string PrimaryEmail { get; set; }

        public DateTime? EmailVerified { get; set; }

        public bool ResetEmailVerification { get; set; }

        public string? SearchString { get; set; }

        public string? ExistingFilterString { get; set; }

        public int Page { get; set; }
    }
}
