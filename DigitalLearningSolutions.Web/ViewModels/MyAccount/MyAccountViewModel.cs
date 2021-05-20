namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class MyAccountViewModel
    {
        public MyAccountViewModel(
            AdminUser? adminUser,
            DelegateUser? delegateUser,
            CentreCustomPromptsWithAnswers? customPrompts)
        {
            FirstName = adminUser?.FirstName ?? delegateUser?.FirstName;
            Surname = adminUser?.LastName ?? delegateUser?.LastName;
            User = adminUser?.EmailAddress ?? delegateUser?.EmailAddress;
            ProfilePicture = adminUser?.ProfileImage ?? delegateUser?.ProfileImage;
            Centre = adminUser?.CentreName ?? delegateUser?.CentreName;
            DelegateNumber = delegateUser?.CandidateNumber;
            JobGroup = delegateUser?.JobGroupName;

            CustomFields = new List<CustomFieldViewModel>();
            if (customPrompts != null)
            {
                CustomFields = customPrompts.CustomPrompts.Select(cp =>
                        new CustomFieldViewModel(cp.CustomPromptNumber, cp.CustomPromptText, cp.Mandatory, cp.Answer))
                    .ToList();
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
