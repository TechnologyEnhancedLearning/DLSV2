namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditDetailsFormData : EditAccountDetailsFormDataBase
    {
        [Required(ErrorMessage = "Enter your email")]
        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(ErrorMessage = CommonValidationErrorMessages.WhitespaceInEmail)]
        public string? Email { get; set; }
    }
}
