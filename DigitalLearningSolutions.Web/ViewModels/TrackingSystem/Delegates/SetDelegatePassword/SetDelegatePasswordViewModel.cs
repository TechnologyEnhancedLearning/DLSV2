namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.SetDelegatePassword
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;

    public class SetDelegatePasswordViewModel
    {
        public SetDelegatePasswordViewModel() { }

        public SetDelegatePasswordViewModel(
            string name,
            int delegateId,
            int? returnPage,
            bool isFromViewDelegatePage = false
        )
        {
            Name = name;
            DelegateId = delegateId;
            IsFromViewDelegatePage = isFromViewDelegatePage;
            ReturnPage = returnPage;
        }

        public string Name { get; set; }

        public int DelegateId { get; set; }

        public bool IsFromViewDelegatePage { get; set; }

        [Required(ErrorMessage = CommonValidationErrorMessages.PasswordRequired)]
        [MinLength(8, ErrorMessage = CommonValidationErrorMessages.PasswordMinLength)]
        [MaxLength(100, ErrorMessage = CommonValidationErrorMessages.PasswordMaxLength)]
        [RegularExpression(
            CommonValidationErrorMessages.PasswordRegex,
            ErrorMessage = CommonValidationErrorMessages.PasswordInvalidCharacters
        )]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public int? ReturnPage { get; set; }
    }
}
