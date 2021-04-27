namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;

    public class MyAccountViewModel
    {
        public MyAccountViewModel(
            AdminUser? adminUser,
            DelegateUser? delegateUser,
            CentreCustomPrompts? customPrompts)
        {
            FirstName = adminUser?.FirstName ?? delegateUser?.FirstName;
            Surname = adminUser?.LastName ?? delegateUser?.LastName;
            User = adminUser?.EmailAddress ?? delegateUser?.EmailAddress;
            ProfilePicture = adminUser?.ProfileImage ?? delegateUser?.ProfileImage;
            Centre = adminUser?.CentreName ?? delegateUser?.CentreName;
            DelegateNumber = delegateUser?.CandidateNumber;
            JobGroup = delegateUser?.JobGroupName;

            CustomFields = new List<CustomFieldViewModel>();

            TryAddCustomFieldViewModelToList(1, customPrompts?.CustomField1, delegateUser?.Answer1);
            TryAddCustomFieldViewModelToList(2, customPrompts?.CustomField2, delegateUser?.Answer2);
            TryAddCustomFieldViewModelToList(3, customPrompts?.CustomField3, delegateUser?.Answer3);
            TryAddCustomFieldViewModelToList(4, customPrompts?.CustomField4, delegateUser?.Answer4);
            TryAddCustomFieldViewModelToList(5, customPrompts?.CustomField5, delegateUser?.Answer5);
            TryAddCustomFieldViewModelToList(6, customPrompts?.CustomField6, delegateUser?.Answer6);
        }

        private void TryAddCustomFieldViewModelToList(int fieldNumber, CustomPrompt? customPrompt, string? answer)
        {
            if (customPrompt != null)
            {
                CustomFields.Add(new CustomFieldViewModel(fieldNumber, customPrompt.CustomPromptText, customPrompt.Mandatory, answer));
            }
        }

        public string? Centre { get; set; }

        public string? User { get; set; }

        public string? DelegateNumber { get; set; }

        public string? FirstName { get; set; }

        public string? Surname { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public string? JobGroup { get; set; }

        public List<CustomFieldViewModel> CustomFields { get; set; }
    }
}
