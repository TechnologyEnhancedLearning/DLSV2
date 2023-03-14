namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Attributes;

    public class PasswordViewModel
    {
        [MinLength(8, ErrorMessage = CommonValidationErrorMessages.PasswordMinLength)]
        [MaxLength(100, ErrorMessage = CommonValidationErrorMessages.PasswordMaxLength)]
        [RegularExpression(
            CommonValidationErrorMessages.PasswordRegex,
            ErrorMessage = CommonValidationErrorMessages.PasswordInvalidCharacters
        )]
        [CommonPasswords(CommonValidationErrorMessages.PasswordTooCommon)]
        [UserName(CommonValidationErrorMessages.PasswordSimilarUsername)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
