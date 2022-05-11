namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Register;

    public interface IRegistrationDataService
    {
        string RegisterNewUserAndDelegateAccount(DelegateRegistrationModel delegateRegistrationModel);
        int RegisterAdmin(AdminRegistrationModel registrationModel);
    }

    public class RegistrationDataService : IRegistrationDataService
    {
        private readonly IDbConnection connection;

        public RegistrationDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public string RegisterNewUserAndDelegateAccount(DelegateRegistrationModel delegateRegistrationModel)
        {
            // TODO HEEDLS-886: this method previously returned error codes as well as candidate numbers.
            // any code that calls it and handled those errors on the basis of the codes needs to be updated
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var userValues = new
                {
                    delegateRegistrationModel.FirstName,
                    delegateRegistrationModel.LastName,
                    delegateRegistrationModel.PrimaryEmail,
                    delegateRegistrationModel.JobGroup,
                    delegateRegistrationModel.Active,
                    PasswordHash = "temp",
                    ProfessionalRegistrationNumber = (string?)null,
                };

            var userId = connection.QuerySingle<int>(
                @"INSERT INTO Users
                    (
                        PrimaryEmail,
                        PasswordHash,
                        FirstName,
                        LastName,
                        JobGroupID,
                        ProfessionalRegistrationNumber,
                        Active
                    )
                    OUTPUT Inserted.ID
                    VALUES
                    (
                        @primaryEmail,
                        @passwordHash,
                        @firstName,
                        @lastName,
                        @jobGroup,
                        @professionalRegistrationNumber,
                        @active
                    )",
                    userValues,
                    transaction
                );

            var initials = delegateRegistrationModel.FirstName.Substring(0, 1) +
                           delegateRegistrationModel.LastName.Substring(0, 1);

            string candidateNumber;
            // this SQL is reproduced mostly verbatim from the uspSaveNewCandidate_V10 procedure in the legacy codebase.
            candidateNumber = connection.QueryFirst<string>(
                @"DECLARE @_MaxCandidateNumber AS integer
		        SET @_MaxCandidateNumber = (SELECT TOP (1) CONVERT(int, SUBSTRING(CandidateNumber, 3, 250)) AS nCandidateNumber
								FROM      DelegateAccounts WITH (TABLOCKX, HOLDLOCK)
								WHERE     (LEFT(CandidateNumber, 2) = @initials)
								ORDER BY nCandidateNumber DESC)
		        IF @_MaxCandidateNumber IS Null
			        BEGIN
			        SET @_MaxCandidateNumber = 0
			        END
		        SELECT @initials + CONVERT(varchar(100), @_MaxCandidateNumber + 1)",
                new { initials },
                transaction
            );

            var candidateValues = new
            {
                userId,
                CentreId = delegateRegistrationModel.Centre,
                DateRegistered = DateTime.UtcNow,
                candidateNumber,
                Email = delegateRegistrationModel.SecondaryEmail,
                delegateRegistrationModel.Answer1,
                delegateRegistrationModel.Answer2,
                delegateRegistrationModel.Answer3,
                delegateRegistrationModel.Answer4,
                delegateRegistrationModel.Answer5,
                delegateRegistrationModel.Answer6,
                delegateRegistrationModel.Approved,
                delegateRegistrationModel.Active,
                delegateRegistrationModel.IsExternalRegistered,
                delegateRegistrationModel.IsSelfRegistered,
                DetailsLastChecked = DateTime.UtcNow,
                // null-equivalent data for non-nullable deprecated values
                LastName_deprecated = "",
                JobGroupID_deprecated = 0,
                SkipPW_deprecated = false,
                PublicSkypeLink_deprecated = false,
                HasBeenPromptedForPrn_deprecated = false,
                HasDismissedLhLoginWarning_deprecated = false,
            };

            connection.Execute(
                @"INSERT INTO DelegateAccounts
                    (
                        UserID,
                        CentreID,
                        DateRegistered,
                        CandidateNumber,
                        Email,
                        Answer1,
                        Answer2,
                        Answer3,
                        Answer4,
                        Answer5,
                        Answer6,
                        Approved,
                        Active,
                        ExternalReg,
                        SelfReg,
                        CentreSpecificDetailsLastChecked,
                        LastName_deprecated,
                        JobGroupID_deprecated,
                        SkipPW_deprecated,
                        PublicSkypeLink_deprecated,
                        HasBeenPromptedForPrn_deprecated,
                        HasDismissedLhLoginWarning_deprecated
                    )
                    VALUES
                    (
                        @userId,
                        @centreId,
                        @dateRegistered,
                        @candidateNumber,
                        @email,
                        @answer1,
                        @answer2,
                        @answer3,
                        @answer4,
                        @answer5,
                        @answer6,
                        @approved,
                        @active,
                        @isExternalRegistered,
                        @isSelfRegistered,
                        @detailsLastChecked,
                        @lastName_deprecated,
                        @jobGroupID_deprecated,
                        @skipPW_deprecated,
                        @publicSkypeLink_deprecated,
                        @hasBeenPromptedForPrn_deprecated,
                        @hasDismissedLhLoginWarning_deprecated
                    )",
                    candidateValues,
                transaction
                );

            transaction.Commit();

            // TODO HEEDLS-874 deal with group assignment

            return candidateNumber;
        }

        public int RegisterAdmin(AdminRegistrationModel registrationModel)
        {
            var values = new
            {
                forename = registrationModel.FirstName,
                surname = registrationModel.LastName,
                email = registrationModel.PrimaryEmail,
                password = registrationModel.PasswordHash,
                centreID = registrationModel.Centre,
                categoryId = registrationModel.CategoryId,
                centreAdmin = registrationModel.IsCentreAdmin,
                isCentreManager = registrationModel.IsCentreManager,
                approved = registrationModel.Approved,
                active = registrationModel.Active,
                contentCreator = registrationModel.IsContentCreator,
                contentManager = registrationModel.IsContentManager,
                importOnly = registrationModel.ImportOnly,
                trainer = registrationModel.IsTrainer,
                supervisor = registrationModel.IsSupervisor,
                nominatedSupervisor = registrationModel.IsNominatedSupervisor
            };

            using var transaction = new TransactionScope();

            var adminUserId = connection.QuerySingle<int>(
                @"INSERT INTO AdminUsers
                    (
                        Forename,
                        Surname,
                        Email,
                        Password,
                        CentreId,
                        CategoryId,
                        CentreAdmin,
                        IsCentreManager,
                        Approved,
                        Active,
                        ContentCreator,
                        ContentManager,
                        ImportOnly,
                        Trainer,
                        Supervisor,
                        NominatedSupervisor
                    )
                    OUTPUT Inserted.AdminID
                    VALUES
                    (
                        @forename,
                        @surname,
                        @email,
                        @password,
                        @centreId,
                        @categoryId,
                        @centreAdmin,
                        @isCentreManager,
                        @approved,
                        @active,
                        @contentCreator,
                        @contentManager,
                        @importOnly,
                        @trainer,
                        @supervisor,
                        @nominatedSupervisor
                    )",
                values
            );

            connection.Execute(
                @"INSERT INTO NotificationUsers (NotificationId, AdminUserId)
                SELECT N.NotificationId, @adminUserId
                FROM Notifications N INNER JOIN NotificationRoles NR
                ON N.NotificationID = NR.NotificationID
                WHERE RoleID IN @roles AND AutoOptIn = 1",
                new { adminUserId, roles = registrationModel.GetNotificationRoles() }
            );

            transaction.Complete();

            return adminUserId;
        }
    }
}
