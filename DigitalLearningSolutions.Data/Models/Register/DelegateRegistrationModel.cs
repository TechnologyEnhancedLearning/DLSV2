namespace DigitalLearningSolutions.Data.Models.Register
{
    using System;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Data.Models.User;

    public class DelegateRegistrationModel : RegistrationModel
    {
        public DelegateRegistrationModel(
            string firstName,
            string lastName,
            string primaryEmail,
            string? centreSpecificEmail,
            int centre,
            int jobGroup,
            string? passwordHash,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6,
            bool isSelfRegistered,
            bool centreAccountIsActive,
            bool userIsActive,
            string? professionalRegistrationNumber,
            bool approved = false,
            DateTime? notifyDate = null
        ) : base(
            firstName,
            lastName,
            primaryEmail,
            centreSpecificEmail,
            centre,
            passwordHash,
            centreAccountIsActive,
            approved,
            professionalRegistrationNumber,
            jobGroup
        )
        {
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
            Answer4 = answer4;
            Answer5 = answer5;
            Answer6 = answer6;
            NotifyDate = notifyDate;
            IsSelfRegistered = isSelfRegistered;
            UserIsActive = userIsActive;
        }

        public DelegateRegistrationModel(
            string firstName,
            string lastName,
            string primaryEmail,
            string? centreSpecificEmail,
            int centre,
            int jobGroup,
            string? passwordHash,
            bool centreAccountIsActive,
            bool userIsActive,
            bool approved,
            string? professionalRegistrationNumber
        ) : base(
            firstName,
            lastName,
            primaryEmail,
            centreSpecificEmail,
            centre,
            passwordHash,
            centreAccountIsActive,
            approved,
            professionalRegistrationNumber,
            jobGroup
        )
        {
            UserIsActive = userIsActive;
        }

        public DelegateRegistrationModel(
            DelegateTableRow row,
            int centreId,
            DateTime? welcomeEmailDate
        ) : this(
            row.FirstName!,
            row.LastName!,
            Guid.NewGuid().ToString(),
            row.Email,
            centreId,
            row.JobGroupId!.Value,
            null,
            row.Answer1,
            row.Answer2,
            row.Answer3,
            row.Answer4,
            row.Answer5,
            row.Answer6,
            false,
            row.Active!.Value,
            false,
            row.Prn,
            true,
            welcomeEmailDate
        ) { }

        public DelegateRegistrationModel(
            UserAccount userAccount,
            InternalDelegateRegistrationModel internalDelegateRegistrationModel,
            bool approved = false,
            bool isSelfRegistered = true
        ) : base(
            userAccount.FirstName,
            userAccount.LastName,
            userAccount.PrimaryEmail,
            internalDelegateRegistrationModel.CentreSpecificEmail,
            internalDelegateRegistrationModel.Centre,
            userAccount.PasswordHash,
            true,
            approved,
            userAccount.ProfessionalRegistrationNumber,
            userAccount.JobGroupId
        )
        {
            Answer1 = internalDelegateRegistrationModel.Answer1;
            Answer2 = internalDelegateRegistrationModel.Answer2;
            Answer3 = internalDelegateRegistrationModel.Answer3;
            Answer4 = internalDelegateRegistrationModel.Answer4;
            Answer5 = internalDelegateRegistrationModel.Answer5;
            Answer6 = internalDelegateRegistrationModel.Answer6;
            IsSelfRegistered = isSelfRegistered;
            UserIsActive = true;
        }

        public string? Answer1 { get; }

        public string? Answer2 { get; }

        public string? Answer3 { get; }

        public string? Answer4 { get; }

        public string? Answer5 { get; }

        public string? Answer6 { get; }

        public DateTime? NotifyDate { get; }

        public bool IsSelfRegistered { get; }

        public bool UserIsActive { get; }

        public bool IsExternalRegistered => !Approved;
    }
}
