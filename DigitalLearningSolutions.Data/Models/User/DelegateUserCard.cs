namespace DigitalLearningSolutions.Data.Models.User
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class DelegateUserCard : DelegateUser
    {
        public DelegateUserCard() { }

        public DelegateUserCard(Delegate delegateUser)
        {
            Id = delegateUser.DelegateAccount.Id;
            CentreId = delegateUser.DelegateAccount.CentreId;
            CentreName = delegateUser.DelegateAccount.CentreName;
            Active = delegateUser.DelegateAccount.Active;
            Approved = delegateUser.DelegateAccount.Approved;
            CentreActive = delegateUser.DelegateAccount.CentreActive;
            FirstName = delegateUser.UserAccount.FirstName;
            LastName = delegateUser.UserAccount.LastName;
            EmailAddress = delegateUser.UserCentreDetails?.Email ?? delegateUser.UserAccount.PrimaryEmail;
            Password = delegateUser.UserAccount.PasswordHash;

            CandidateNumber = delegateUser.DelegateAccount.CandidateNumber;
            DateRegistered = delegateUser.DelegateAccount.DateRegistered;
            JobGroupId = delegateUser.UserAccount.JobGroupId;
            JobGroupName = delegateUser.UserAccount.JobGroupName;
            Answer1 = delegateUser.DelegateAccount.Answer1;
            Answer2 = delegateUser.DelegateAccount.Answer2;
            Answer3 = delegateUser.DelegateAccount.Answer3;
            Answer4 = delegateUser.DelegateAccount.Answer4;
            Answer5 = delegateUser.DelegateAccount.Answer5;
            Answer6 = delegateUser.DelegateAccount.Answer6;
            HasBeenPromptedForPrn = delegateUser.UserAccount.HasBeenPromptedForPrn;
            ProfessionalRegistrationNumber = delegateUser.UserAccount.ProfessionalRegistrationNumber;
            HasDismissedLhLoginWarning = delegateUser.UserAccount.HasDismissedLhLoginWarning;
            SelfReg = delegateUser.DelegateAccount.SelfReg;
            ExternalReg = delegateUser.DelegateAccount.ExternalReg;
        }

        public bool SelfReg { get; set; }
        public bool ExternalReg { get; set; }
        public int? AdminId { get; set; }
        public bool IsPasswordSet => Password != null;
        public bool IsAdmin => AdminId.HasValue;

        public RegistrationType RegistrationType => (SelfReg, ExternalReg) switch
        {
            (true, true) => RegistrationType.SelfRegisteredExternal,
            (true, false) => RegistrationType.SelfRegistered,
            _ => RegistrationType.RegisteredByCentre,
        };

        public static string GetPropertyNameForDelegateRegistrationPromptAnswer(int customPromptNumber)
        {
            return customPromptNumber switch
            {
                1 => nameof(Answer1),
                2 => nameof(Answer2),
                3 => nameof(Answer3),
                4 => nameof(Answer4),
                5 => nameof(Answer5),
                6 => nameof(Answer6),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
