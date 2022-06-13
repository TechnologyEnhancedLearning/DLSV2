﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Services;

    public interface IRegistrationDataService
    {
        string RegisterNewUserAndDelegateAccount(DelegateRegistrationModel delegateRegistrationModel);

        (int delegateId, string candidateNumber) RegisterDelegateAccountAndCentreDetailForExistingUser(
            DelegateRegistrationModel delegateRegistrationModel,
            int userId,
            DateTime currentTime,
            IDbTransaction? transaction = null
        );

        int RegisterAdmin(AdminRegistrationModel registrationModel, int userId);
    }

    public class RegistrationDataService : IRegistrationDataService
    {
        private readonly IClockService clockService;
        private readonly IDbConnection connection;
        private readonly IUserDataService userDataService;

        public RegistrationDataService(
            IDbConnection connection,
            IUserDataService userDataService,
            IClockService clockService
        )
        {
            this.connection = connection;
            this.userDataService = userDataService;
            this.clockService = clockService;
        }

        public string RegisterNewUserAndDelegateAccount(DelegateRegistrationModel delegateRegistrationModel)
        {
            // TODO HEEDLS-900: this method previously returned error codes as well as candidate numbers.
            // any code that calls it and handled those errors on the basis of the codes needs to be updated
            connection.EnsureOpen();
            using var transaction = connection.BeginTransaction();

            var currentTime = clockService.UtcNow;

            var userIdToLinkDelegateAccountTo = RegisterUserAccount(
                delegateRegistrationModel,
                currentTime,
                transaction
            );

            var (_, candidateNumber) = RegisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userIdToLinkDelegateAccountTo,
                currentTime,
                transaction
            );

            transaction.Commit();

            return candidateNumber;
        }

        public (int delegateId, string candidateNumber) RegisterDelegateAccountAndCentreDetailForExistingUser(
            DelegateRegistrationModel delegateRegistrationModel,
            int userId,
            DateTime currentTime,
            IDbTransaction? transaction = null
        )
        {
            // TODO HEEDLS-900: this method previously returned error codes as well as candidate numbers.
            // any code that calls it and handled those errors on the basis of the codes needs to be updated

            var transactionShouldBeClosed = false;
            if (transaction == null)
            {
                connection.EnsureOpen();
                transaction = connection.BeginTransaction();
                transactionShouldBeClosed = true;
            }

            RegisterCentreDetailForExistingUser(
                delegateRegistrationModel.Centre,
                delegateRegistrationModel.CentreSpecificEmail,
                userId,
                transaction
            );

            var (delegateId, candidateNumber) = RegisterDelegateAccountAndReturnCandidateNumber(
                delegateRegistrationModel,
                userId,
                currentTime,
                transaction
            );

            // TODO HEEDLS-874 deal with group assignment

            if (transactionShouldBeClosed)
            {
                transaction.Commit();
            }

            return (delegateId, candidateNumber);
        }

        public int RegisterAdmin(AdminRegistrationModel registrationModel, int userId)
        {
            connection.EnsureOpen();
            using var transaction = connection.BeginTransaction();

            RegisterCentreDetailForExistingUser(
                registrationModel.Centre,
                registrationModel.CentreSpecificEmail,
                userId,
                transaction
            );

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
                        IsNominatedSupervisor
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
                        @isNominatedSupervisor
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

        private int RegisterUserAccount(
            DelegateRegistrationModel delegateRegistrationModel,
            DateTime currentTime,
            IDbTransaction transaction
        )
        {
            var userValues = new
            {
                delegateRegistrationModel.FirstName,
                delegateRegistrationModel.LastName,
                delegateRegistrationModel.PrimaryEmail,
                delegateRegistrationModel.JobGroup,
                delegateRegistrationModel.Active,
                PasswordHash = "temp",
                ProfessionalRegistrationNumber = (string?)null,
                DetailsLastChecked = currentTime,
            };

            return connection.QuerySingle<int>(
                @"INSERT INTO Users
                    (
                        PrimaryEmail,
                        PasswordHash,
                        FirstName,
                        LastName,
                        JobGroupID,
                        ProfessionalRegistrationNumber,
                        Active,
                        DetailsLastChecked
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
                        @active,
                        @detailsLastChecked
                    )",
                userValues,
                transaction
            );
        }

        private void RegisterCentreDetailForExistingUser(
            int centreId,
            string? centreSpecificEmail,
            int userId,
            IDbTransaction transaction
        )
        {
            if (!string.IsNullOrWhiteSpace(centreSpecificEmail))
            {
                userDataService.SetCentreEmail(
                    userId,
                    centreId,
                    centreSpecificEmail,
                    transaction
                );
            }
        }

        private (int delegateId, string candidateNumber) RegisterDelegateAccountAndReturnCandidateNumber(
            DelegateRegistrationModel delegateRegistrationModel,
            int userIdToLinkDelegateAccountTo,
            DateTime currentTime,
            IDbTransaction transaction
        )
        {
            var initials = (delegateRegistrationModel.FirstName.Substring(0, 1) +
                            delegateRegistrationModel.LastName.Substring(0, 1)).ToUpper();

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
                userId = userIdToLinkDelegateAccountTo,
                CentreId = delegateRegistrationModel.Centre,
                DateRegistered = currentTime,
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
                CentreSpecificDetailsLastChecked = currentTime,
            };

            var delegateId = connection.QuerySingle<int>(
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
                        CentreSpecificDetailsLastChecked
                    )
                    OUTPUT Inserted.ID
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
                        @centreSpecificDetailsLastChecked
                    )",
                candidateValues,
                transaction
            );
            return (delegateId, candidateNumber);
        }
    }
}
