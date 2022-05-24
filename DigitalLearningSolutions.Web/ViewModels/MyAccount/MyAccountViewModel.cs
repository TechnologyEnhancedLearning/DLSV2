namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class MyAccountViewModel
    {
        public MyAccountViewModel(
            AdminUser? adminUser,
            DelegateUser? delegateUser,
            CentreRegistrationPromptsWithAnswers? customPrompts,
            DlsSubApplication dlsSubApplication
        )
        {
            FirstName = adminUser?.FirstName ?? delegateUser?.FirstName;
            Surname = adminUser?.LastName ?? delegateUser?.LastName;
            User = adminUser?.EmailAddress ?? delegateUser?.EmailAddress;
            ProfilePicture = adminUser?.ProfileImage ?? delegateUser?.ProfileImage;
            Centre = adminUser?.CentreName ?? delegateUser?.CentreName;
            DelegateNumber = delegateUser?.CandidateNumber;
            JobGroup = delegateUser?.JobGroupName;
            CentreEmail = delegateUser?.EmailAddress;
            DateRegistered = delegateUser?.DateRegistered?.ToString("dd-MM-yyyy");
            ProfessionalRegistrationNumber = delegateUser == null
                ? null
                : PrnStringHelper.GetPrnDisplayString(
                    delegateUser.HasBeenPromptedForPrn,
                    delegateUser.ProfessionalRegistrationNumber
                );

            DelegateRegistrationPrompts = new List<DelegateRegistrationPrompt>();
            if (customPrompts != null)
            {
                DelegateRegistrationPrompts = customPrompts.CustomPrompts.Select(
                        cp =>
                            new DelegateRegistrationPrompt(
                                cp.RegistrationField.Id,
                                cp.PromptText,
                                cp.Mandatory,
                                cp.Answer
                            )
                    )
                    .ToList();
            }

            DlsSubApplication = dlsSubApplication;
        }

        public string? Centre { get; set; }

        public string? User { get; set; }

        public string? DelegateNumber { get; set; }

        public string? FirstName { get; set; }

        public string? Surname { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public string? JobGroup { get; set; }

        public string? ProfessionalRegistrationNumber { get; set; }

        public string? CentreEmail { get; set; }

        public string? DateRegistered { get; set; }

        public List<DelegateRegistrationPrompt> DelegateRegistrationPrompts { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
