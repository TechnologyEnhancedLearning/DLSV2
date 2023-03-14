namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.SetDelegatePassword
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class SetDelegatePasswordViewModel : IValidatableObject
    {
        public SetDelegatePasswordViewModel() { }

        public SetDelegatePasswordViewModel(
            string name,
            int delegateId,
            bool isFromViewDelegatePage = false,
            ReturnPageQuery? returnPageQuery = null
        )
        {
            Name = name;
            DelegateId = delegateId;
            IsFromViewDelegatePage = isFromViewDelegatePage;
            ReturnPageQuery = returnPageQuery;
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
        [CommonPasswords(CommonValidationErrorMessages.PasswordTooCommon)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public ReturnPageQuery? ReturnPageQuery { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            if(Password != null && Password != string.Empty)
            {
                var passwordLower = Password.ToLower();
                var firstnameLower = Name.ToLower().Split(' ').First();
                var lastnameLower = Name.ToLower().Split(' ').Last();

                if (passwordLower.Contains(firstnameLower) || passwordLower.Contains(lastnameLower))
                {
                    errors.Add(new ValidationResult(CommonValidationErrorMessages.PasswordSimilarUsername));
                }
            }

            return errors;
        }

    }
}
