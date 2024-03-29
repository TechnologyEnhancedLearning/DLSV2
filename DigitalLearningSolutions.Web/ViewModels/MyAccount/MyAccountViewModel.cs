﻿namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
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
            int? centreId,
            string? centreName,
            string? centreSpecificEmail,
            CentreRegistrationPromptsWithAnswers? customPrompts,
            List<(int centreId, string centreName, string? centreSpecificEmail)> allCentreSpecificEmails,
            List<(int centreId, string centreName, string? centreSpecificEmail)> unverifiedCentreEmails,
            DlsSubApplication dlsSubApplication,
            string switchCentreReturnUrl,
            List<string> roles = null
        )
        {
            CentreId = centreId;
            FirstName = userAccount.FirstName;
            Surname = userAccount.LastName;
            PrimaryEmail = userAccount.PrimaryEmail;
            ProfilePicture = userAccount.ProfileImage;
            CentreName = centreName;
            DelegateNumber = delegateAccount?.CandidateNumber;
            JobGroup = userAccount.JobGroupName;
            CentreSpecificEmail = centreSpecificEmail;
            DateRegistered = delegateAccount?.DateRegistered.ToString(DateHelper.StandardDateFormat);
            ProfessionalRegistrationNumber = PrnHelper.GetPrnDisplayString(
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

            AllCentreSpecificEmails = allCentreSpecificEmails;
            DlsSubApplication = dlsSubApplication;
            SwitchCentreReturnUrl = switchCentreReturnUrl;
            PrimaryEmailIsVerified = userAccount.EmailVerified != null;
            UnverifiedCentreEmails = unverifiedCentreEmails;
            NumberOfUnverifiedEmails = (PrimaryEmailIsVerified ? 0 : 1) + UnverifiedCentreEmails.Count;
            Roles = roles;
        }

        public int? CentreId { get; set; }

        public string? CentreName { get; set; }

        public string PrimaryEmail { get; set; }

        public bool PrimaryEmailIsVerified { get; }

        public string? DelegateNumber { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public string JobGroup { get; set; }

        public string ProfessionalRegistrationNumber { get; set; }

        public string? CentreSpecificEmail { get; set; }

        public string? DateRegistered { get; set; }

        public List<DelegateRegistrationPrompt> DelegateRegistrationPrompts { get; set; }

        public List<(int centreId, string centreName, string? centreSpecificEmail)> AllCentreSpecificEmails
        {
            get;
            set;
        }

        public List<(int centreId, string centreName, string? centreSpecificEmail)> UnverifiedCentreEmails { get; set; }

        public int NumberOfUnverifiedEmails { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }

        public string SwitchCentreReturnUrl { get; set; }
        public List<string> Roles { get; set; }
    }
}
