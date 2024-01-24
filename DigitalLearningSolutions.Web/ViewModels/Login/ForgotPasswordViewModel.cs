using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.Login
{
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "You need to enter your email address")]
        [MaxLength(100, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(ErrorMessage = CommonValidationErrorMessages.WhitespaceInEmail)]
        public string EmailAddress { get; set; }
    }
}
