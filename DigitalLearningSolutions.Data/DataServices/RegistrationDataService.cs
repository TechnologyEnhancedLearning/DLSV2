namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Logging;

    public interface IRegistrationDataService
    {
        (int delegateId, string candidateNumber) RegisterNewUserAndDelegateAccount(
            DelegateRegistrationModel delegateRegistrationModel,
            bool registerJourneyContainsTermsAndConditions
        );

        int RegisterAdmin(AdminAccountRegistrationModel registrationModel);

        (int delegateId, string candidateNumber) RegisterDelegateAccountAndCentreDetailForExistingUser(
            DelegateRegistrationModel delegateRegistrationModel,
            int userId,
            DateTime currentTime,
            IDbTransaction? transaction = null
        );

        void ReregisterDelegateAccountAndCentreDetailForExistingUser(
            DelegateRegistrationModel delegateRegistrationModel,
            int userId,
            int delegateId,
            DateTime currentTime
        );
    }

    public class RegistrationDataService : IRegistrationDataService
    {
        private readonly IClockUtility clockUtility;
        private readonly IDbConnection connection;
        private readonly ILogger<IRegistrationDataService> logger;
        private readonly IUserDataService userDataService;

        public RegistrationDataService(
            IDbConnection connection,
            IUserDataService userDataService,
            IClockUtility clockUtility,
            ILogger<IRegistrationDataService> logger,
            IGroupsService groupsService
        )
        {
            this.connection = connection;
            this.userDataService = userDataService;
            this.clockUtility = clockUtility;
            this.logger = logger;
            this.groupsService = groupsService;
        }

        public (int delegateId, string candidateNumber) RegisterNewUserAndDelegateAccount(
            DelegateRegistrationModel delegateRegistrationModel,
            bool registerJourneyContainsTermsAndConditions
        )
        {
            connection.EnsureOpen();
            using var transaction = connection.BeginTransaction();

            var currentTime = clockUtility.UtcNow;

            var userIdToLinkDelegateAccountTo = RegisterUserAccount(
                delegateRegistrationModel,
                currentTime,
                registerJourneyContainsTermsAndConditions,
                transaction
            );

            var (delegateId, candidateNumber) = RegisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userIdToLinkDelegateAccountTo,
                currentTime,
                transaction
            );

            transaction.Commit();

            return (delegateId, candidateNumber);
        }

        public (int delegateId, string candidateNumber) RegisterDelegateAccountAndCentreDetailForExistingUser(
            DelegateRegistrationModel delegateRegistrationModel,
            int userId,
            DateTime currentTime,
            IDbTransaction? transaction = null
        )
        {
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

            var (delegateId, candidateNumber) = RegisterDelegateAccountAndReturnCandidateNumberAndDelegateId(
                delegateRegistrationModel,
                userId,
                currentTime,
                transaction
            );

            if (transactionShouldBeClosed)
            {
                transaction.Commit();
            }

            return (delegateId, candidateNumber);
        }

        public void ReregisterDelegateAccountAndCentreDetailForExistingUser(
            DelegateRegistrationModel delegateRegistrationModel,
            int userId,
            int delegateId,
            DateTime currentTime
        )
        {
            connection.EnsureOpen();
            var transaction = connection.BeginTransaction();

            userDataService.SetCentreEmail(
                userId,
                delegateRegistrationModel.Centre,
                delegateRegistrationModel.CentreSpecificEmail,
                transaction
            );

            ReregisterDelegateAccount(
                delegateRegistrationModel,
                delegateId,
                currentTime,
                transaction
            );

            transaction.Commit();
        }

        public int RegisterAdmin(AdminAccountRegistrationModel registrationModel)
        {
            connection.EnsureOpen();
            using var transaction = connection.BeginTransaction();

            RegisterCentreDetailForExistingUser(
                registrationModel.CentreId,
                registrationModel.CentreSpecificEmail,
                registrationModel.UserId,
                transaction
            );

            var adminValues = new
            {
                registrationModel.UserId,
                centreID = registrationModel.CentreId,
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
                new { adminUserId, roles = registrationModel.GetNotificationRoles() },
                transaction
            );

            transaction.Commit();

            return adminUserId;
        }

        private int RegisterUserAccount(
            DelegateRegistrationModel delegateRegistrationModel,
            DateTime currentTime,
            bool registerJourneyContainsTermsAndConditions,
            IDbTransaction transaction
        )
        {
            var userValues = new
            {
                delegateRegistrationModel.FirstName,
                delegateRegistrationModel.LastName,
                delegateRegistrationModel.PrimaryEmail,
                delegateRegistrationModel.JobGroup,
                delegateRegistrationModel.UserIsActive,
                delegateRegistrationModel.ProfessionalRegistrationNumber,
                PasswordHash = string.Empty,
                TermsAgreed = registerJourneyContainsTermsAndConditions ? currentTime : (DateTime?)null,
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
                        TermsAgreed,
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
                        @userIsActive,
                        @termsAgreed,
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

        private (int delegateId, string candidateNumber) RegisterDelegateAccountAndReturnCandidateNumberAndDelegateId(
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
                    FROM DelegateAccounts
                    WHERE (LEFT(CandidateNumber, 2) = @initials)
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
                delegateRegistrationModel.CentreAccountIsActive,
                delegateRegistrationModel.IsExternalRegistered,
                delegateRegistrationModel.IsSelfRegistered,
                CentreSpecificDetailsLastChecked = currentTime,
            };

            var delegateId = 0;
            try
            {
                delegateId = connection.QuerySingle<int>(
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
                        @centreAccountIsActive,
                        @isExternalRegistered,
                        @isSelfRegistered,
                        @centreSpecificDetailsLastChecked
                    )",
                    candidateValues,
                    transaction
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting new DelegateAccount record");
                transaction.Rollback();
                throw;
            }

            return (delegateId, candidateNumber);
        }

        private void ReregisterDelegateAccount(
            DelegateRegistrationModel delegateRegistrationModel,
            int delegateId,
            DateTime currentTime,
            IDbTransaction transaction
        )
        {
            var newDelegateValues = new
            {
                delegateId,
                delegateRegistrationModel.Answer1,
                delegateRegistrationModel.Answer2,
                delegateRegistrationModel.Answer3,
                delegateRegistrationModel.Answer4,
                delegateRegistrationModel.Answer5,
                delegateRegistrationModel.Answer6,
                delegateRegistrationModel.Approved,
                delegateRegistrationModel.CentreAccountIsActive,
                CentreSpecificDetailsLastChecked = currentTime,
            };

            connection.Execute(
                @"UPDATE DelegateAccounts SET
                            Answer1 = @answer1,
                            Answer2 = @answer2,
                            Answer3 = @answer3,
                            Answer4 = @answer4,
                            Answer5 = @answer5,
                            Answer6 = @answer6,
                            Approved = @approved,
                            Active = @centreAccountIsActive,
                            CentreSpecificDetailsLastChecked = @centreSpecificDetailsLastChecked
                        WHERE ID = @delegateId",
                newDelegateValues,
                transaction
            );
        }
    }
}
