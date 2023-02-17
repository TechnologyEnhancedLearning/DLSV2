﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.SetDelegatePassword
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;

    public class SetDelegatePasswordViewModel
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
        [UserName(CommonValidationErrorMessages.PasswordSimilarUsername)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public ReturnPageQuery? ReturnPageQuery { get; set; }
    }
}
