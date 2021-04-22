namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
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

            CustomField1 = PopulateCustomFieldViewModel(customPrompts?.CustomField1, delegateUser?.Answer1);
            CustomField2 = PopulateCustomFieldViewModel(customPrompts?.CustomField2, delegateUser?.Answer2);
            CustomField3 = PopulateCustomFieldViewModel(customPrompts?.CustomField3, delegateUser?.Answer3);
            CustomField4 = PopulateCustomFieldViewModel(customPrompts?.CustomField4, delegateUser?.Answer4);
            CustomField5 = PopulateCustomFieldViewModel(customPrompts?.CustomField5, delegateUser?.Answer5);
            CustomField6 = PopulateCustomFieldViewModel(customPrompts?.CustomField6, delegateUser?.Answer6);
        }

        private CustomFieldViewModel? PopulateCustomFieldViewModel(CustomPrompt? customPrompt, string? answer)
        {
            if (customPrompt == null)
            {
                return null;
            }

            return new CustomFieldViewModel(customPrompt.CustomPromptText, customPrompt.Mandatory, answer);
        }

        public string? Centre { get; set; }

        public string? User { get; set; }

        public string? DelegateNumber { get; set; }

        public string? FirstName { get; set; }

        public string? Surname { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public string? JobGroup { get; set; }

        public CustomFieldViewModel? CustomField1 { get; set; }

        public CustomFieldViewModel? CustomField2 { get; set; }

        public CustomFieldViewModel? CustomField3 { get; set; }

        public CustomFieldViewModel? CustomField4 { get; set; }

        public CustomFieldViewModel? CustomField5 { get; set; }

        public CustomFieldViewModel? CustomField6 { get; set; }
    }
}
