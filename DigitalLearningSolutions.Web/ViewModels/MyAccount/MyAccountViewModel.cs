namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
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
            UserAccount userAccount,
            DelegateAccount? delegateAccount,
            string centreName,
            string? centreEmail,
            CentreRegistrationPromptsWithAnswers? customPrompts,
            DlsSubApplication dlsSubApplication,
            string switchCentreReturnUrl
        )
        {
            FirstName = userAccount.FirstName;
            Surname = userAccount.LastName;
            PrimaryEmail = userAccount.PrimaryEmail;
            ProfilePicture = userAccount.ProfileImage;
            Centre = centreName;
            DelegateNumber = delegateAccount?.CandidateNumber;
            JobGroup = userAccount.JobGroupName;
            CentreEmail = centreEmail;
            DateRegistered = delegateAccount?.DateRegistered.ToString(DateHelper.StandardDateFormat);
            ProfessionalRegistrationNumber = PrnStringHelper.GetPrnDisplayString(
                userAccount.HasBeenPromptedForPrn,
                userAccount.ProfessionalRegistrationNumber
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
            SwitchCentreReturnUrl = switchCentreReturnUrl;
        }

        public string Centre { get; set; }

        public string PrimaryEmail { get; set; }

        public string? DelegateNumber { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public string JobGroup { get; set; }

        public string ProfessionalRegistrationNumber { get; set; }

        public string? CentreEmail { get; set; }

        public string? DateRegistered { get; set; }

        public List<DelegateRegistrationPrompt> DelegateRegistrationPrompts { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }

        public string SwitchCentreReturnUrl { get; set; }
    }
}
