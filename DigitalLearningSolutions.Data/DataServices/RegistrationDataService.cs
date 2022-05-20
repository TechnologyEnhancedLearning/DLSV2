﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IRegistrationDataService
    {
        string RegisterNewUserAndDelegateAccount(DelegateRegistrationModel delegateRegistrationModel);

        int RegisterAdmin(AdminRegistrationModel registrationModel, int userId);
    }

    public class RegistrationDataService : IRegistrationDataService
    {
        private readonly IDbConnection connection;
        private readonly IUserDataService userDataService;

        public RegistrationDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public string RegisterNewUserAndDelegateAccount(DelegateRegistrationModel delegateRegistrationModel)
        {
            // TODO HEEDLS-886: this method previously returned error codes as well as candidate numbers.
            // any code that calls it and handled those errors on the basis of the codes needs to be updated
            connection.EnsureOpen();
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

            if (!string.IsNullOrWhiteSpace(delegateRegistrationModel.SecondaryEmail))
            {
                userDataService.SetCentreEmail(
                    userId,
                    delegateRegistrationModel.Centre,
                    delegateRegistrationModel.SecondaryEmail,
                    transaction
                );
            }

            var initials = delegateRegistrationModel.FirstName.Substring(0, 1) +
                           delegateRegistrationModel.LastName.Substring(0, 1);

            // this SQL is reproduced mostly verbatim from the uspSaveNewCandidate_V10 procedure in the legacy codebase.
            var candidateNumber = connection.QueryFirst<string>(
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
                // TODO HEEDLS-889 currently non-null deprecated columns
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

        public int RegisterAdmin(AdminRegistrationModel registrationModel, int userId)
        {
            connection.EnsureOpen();
            using var transaction = connection.BeginTransaction();

            if (!string.IsNullOrWhiteSpace(registrationModel.SecondaryEmail))
            {
                userDataService.SetCentreEmail(
                    userId,
                    registrationModel.Centre,
                    registrationModel.SecondaryEmail,
                    transaction
                );
            }

            var adminValues = new
            {
                userId,
                centreID = registrationModel.Centre,
                categoryID = registrationModel.CategoryId,
                isCentreAdmin = registrationModel.IsCentreAdmin,
                isCentreManager = registrationModel.IsCentreManager,
                active = registrationModel.Active,
                isContentCreator = registrationModel.IsContentCreator,
                isContentManager = registrationModel.IsContentManager,
                importOnly = registrationModel.ImportOnly,
                isTrainer = registrationModel.IsTrainer,
                isSupervisor = registrationModel.IsSupervisor,
                isNominatedSupervisor = registrationModel.IsNominatedSupervisor,
                // TODO HEEDLS-889 currently non-null deprecated columns
                forename_deprecated = "",
                surname_deprecated = "",
                password_deprecated = ""
            };

            var adminUserId = connection.QuerySingle<int>(
                @"INSERT INTO AdminAccounts
                    (
                        UserID,
                        CentreID,
                        CategoryID,
                        IsCentreAdmin,
                        IsCentreManager,
                        Active,
                        IsContentCreator,
                        IsContentManager,
                        ImportOnly,
                        IsTrainer,
                        IsSupervisor,
                        IsNominatedSupervisor,
                        Forename_deprecated,
                        Surname_deprecated,
                        Password_deprecated
                    )
                    OUTPUT Inserted.ID
                    VALUES
                    (
                        @userId,
                        @centreId,
                        @categoryId,
                        @isCentreAdmin,
                        @isCentreManager,
                        @active,
                        @isContentCreator,
                        @isContentManager,
                        @importOnly,
                        @isTrainer,
                        @isSupervisor,
                        @isNominatedSupervisor,
                        @forename_deprecated,
                        @surname_deprecated,
                        @password_deprecated
                    )",
                adminValues,
                transaction
            );

            connection.Execute(
                @"INSERT INTO NotificationUsers (NotificationId, AdminUserId)
                SELECT N.NotificationId, @adminUserId
                FROM Notifications N INNER JOIN NotificationRoles NR
                ON N.NotificationID = NR.NotificationID
                WHERE RoleID IN @roles AND AutoOptIn = 1",
                new { adminUserId, roles = registrationModel.GetNotificationRoles() }
            );

            transaction.Commit();

            return adminUserId;
        }
    }
}
