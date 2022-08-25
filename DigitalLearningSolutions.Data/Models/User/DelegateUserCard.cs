namespace DigitalLearningSolutions.Data.Models.User
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class DelegateUserCard : DelegateUser
    {
        public DelegateUserCard() { }

        public DelegateUserCard(DelegateEntity delegateEntity)
        {
            Id = delegateEntity.DelegateAccount.Id;
            CentreId = delegateEntity.DelegateAccount.CentreId;
            CentreName = delegateEntity.DelegateAccount.CentreName;
            Active = delegateEntity.DelegateAccount.Active;
            Approved = delegateEntity.DelegateAccount.Approved;
            CentreActive = delegateEntity.DelegateAccount.CentreActive;
            FirstName = delegateEntity.UserAccount.FirstName;
            LastName = delegateEntity.UserAccount.LastName;
            EmailAddress = delegateEntity.UserCentreDetails?.Email ?? delegateEntity.UserAccount.PrimaryEmail;
            Password = delegateEntity.UserAccount.PasswordHash;
            CandidateNumber = delegateEntity.DelegateAccount.CandidateNumber;
            DateRegistered = delegateEntity.DelegateAccount.DateRegistered;
            JobGroupId = delegateEntity.UserAccount.JobGroupId;
            JobGroupName = delegateEntity.UserAccount.JobGroupName;
            Answer1 = delegateEntity.DelegateAccount.Answer1;
            Answer2 = delegateEntity.DelegateAccount.Answer2;
            Answer3 = delegateEntity.DelegateAccount.Answer3;
            Answer4 = delegateEntity.DelegateAccount.Answer4;
            Answer5 = delegateEntity.DelegateAccount.Answer5;
            Answer6 = delegateEntity.DelegateAccount.Answer6;
            HasBeenPromptedForPrn = delegateEntity.UserAccount.HasBeenPromptedForPrn;
            ProfessionalRegistrationNumber = delegateEntity.UserAccount.ProfessionalRegistrationNumber;
            HasDismissedLhLoginWarning = delegateEntity.UserAccount.HasDismissedLhLoginWarning;
            SelfReg = delegateEntity.DelegateAccount.SelfReg;
            ExternalReg = delegateEntity.DelegateAccount.ExternalReg;
            AdminId = delegateEntity.AdminId;
            RegistrationConfirmationHash = delegateEntity.DelegateAccount.RegistrationConfirmationHash;
        }

        public bool SelfReg { get; set; }
        public bool ExternalReg { get; set; }
        public int? AdminId { get; set; }
        public bool IsPasswordSet => !string.IsNullOrEmpty(Password);
        public bool IsRegistrationComplete => RegistrationConfirmationHash == null;
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
