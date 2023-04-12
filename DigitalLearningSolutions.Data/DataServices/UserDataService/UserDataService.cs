namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Logging;
    using DocumentFormat.OpenXml.Wordprocessing;

    public interface IUserDataService
    {
        AdminEntity? GetAdminById(int id);

        IEnumerable<AdminEntity> GetActiveAdminsByCentreId(int centreId);

        AdminUser? GetAdminUserById(int id);

        List<AdminUser> GetAdminUsersByCentreId(int centreId);

        AdminUser? GetAdminUserByEmailAddress(string emailAddress);

        int GetNumberOfActiveAdminsAtCentre(int centreId);

        void UpdateAdminUserPermissions(
            int adminId,
            bool isCentreAdmin,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool isTrainer,
            bool isContentCreator,
            bool isContentManager,
            bool importOnly,
            int? categoryId,
            bool isCentreManager
        );

        void UpdateUserFailedLoginCount(int userId, int updatedCount);

        DelegateEntity? GetDelegateById(int id);

        DelegateEntity? GetDelegateByCandidateNumber(string candidateNumber);

        IEnumerable<DelegateEntity> GetUnapprovedDelegatesByCentreId(int centreId);

        DelegateUser? GetDelegateUserById(int id);
        DelegateUser? GetDelegateUserByDelegateUserIdAndCentreId(int delegateUserId, int centreId);

        List<DelegateUser> GetUnapprovedDelegateUsersByCentreId(int centreId);

        void UpdateUser(
            string firstName,
            string surname,
            string primaryEmail,
            byte[]? profileImage,
            string? professionalRegNumber,
            bool hasBeenPromptedForPrn,
            int jobGroupId,
            DateTime detailsLastChecked,
            DateTime? emailVerified,
            int userId,
            bool isPrimaryEmailUpdated,
            bool changeMadeBySameUser = false
        );

        void UpdateDelegateAccount(
            int delegateId,
            bool active,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6
        );

        void ApproveDelegateUsers(params int[] ids);

        void RemoveDelegateAccount(int delegateId);

        int GetNumberOfApprovedDelegatesAtCentre(int centreId);

        void DeactivateDelegateUser(int delegateId);

        void UpdateUserDetails(string firstName, string surname, string primaryEmail, int jobGroupId, int userId);

        DelegateUserCard? GetDelegateUserCardById(int id);

        List<DelegateUserCard> GetDelegateUserCardsByCentreId(int centreId);

        List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId);

        void UpdateDelegateUserCentrePrompts(
            int id,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6,
            DateTime detailsLastChecked
        );

        int GetDelegateCountWithAnswerForPrompt(int centreId, int promptNumber);

        void DeleteAllAnswersForPrompt(int centreId, int promptNumber);

        void DeactivateAdmin(int adminId);

        void ReactivateAdmin(int adminId);

        void ActivateDelegateUser(int delegateId);

        int? GetDelegateUserLearningHubAuthId(int delegateId);

        void SetDelegateUserLearningHubAuthId(int delegateId, int learningHubAuthId);

        void UpdateDelegateLhLoginWarningDismissalStatus(int delegateId, bool status);

        void UpdateDelegateProfessionalRegistrationNumber(
            int delegateId,
            string? professionalRegistrationNumber,
            bool hasBeenPromptedForPrn
        );

        bool PrimaryEmailIsInUse(string email);

        bool PrimaryEmailIsInUseByOtherUser(string email, int userId);

        bool CentreSpecificEmailIsInUseAtCentre(string email, int centreId);

        bool CentreSpecificEmailIsInUseAtCentreByOtherUser(
            string email,
            int centreId,
            int userId
        );

        void DeleteUser(int userId);

        void DeleteAdminAccount(int adminId);

        int? GetUserIdFromUsername(string username);

        int GetUserIdFromDelegateId(int delegateId);

        UserAccount? GetUserAccountById(int userId);

        UserAccount? GetUserAccountByPrimaryEmail(string emailAddress);

        int? GetUserIdByAdminId(int adminId);

        IEnumerable<AdminAccount> GetAdminAccountsByUserId(int userId);

        IEnumerable<DelegateAccount> GetDelegateAccountsByUserId(int userId);

        DelegateAccount? GetDelegateAccountById(int id);

        void SetPrimaryEmailAndActivate(int userId, string email);

        void SetCentreEmail(
            int userId,
            int centreId,
            string? email,
            DateTime? emailVerified,
            IDbTransaction? transaction = null
        );

        string? GetCentreEmail(int userId, int centreId);

        IEnumerable<(int centreId, string centreName, string? centreSpecificEmail)> GetAllActiveCentreEmailsForUser(
            int userId, bool isAll = false
        );

        IEnumerable<(int centreId, string centreName, string? centreSpecificEmail)> GetAllCentreEmailsForUser(
            int userId
        );

        IEnumerable<(int centreId, string centreName, string centreEmail)> GetUnverifiedCentreEmailsForUser(int userId);

        (int? userId, int? centreId, string? centreName)
            GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                string centreSpecificEmail,
                string registrationConfirmationHash
            );

        void SetRegistrationConfirmationHash(int userId, int centreId, string? hash);

        void LinkDelegateAccountToNewUser(
            int currentUserIdForDelegateAccount,
            int newUserIdForDelegateAccount,
            int centreId
        );

        void LinkUserCentreDetailsToNewUser(
            int currentUserIdForUserCentreDetails,
            int newUserIdForUserCentreDetails,
            int centreId
        );

        IEnumerable<UserCentreDetails> GetCentreDetailsForUser(int userId);

        EmailVerificationDetails? GetPrimaryEmailVerificationDetails(string code);

        IEnumerable<EmailVerificationDetails> GetCentreEmailVerificationDetails(string code);

        void SetPrimaryEmailVerified(int userId, string email, DateTime verifiedDateTime);

        void SetCentreEmailVerified(int userId, string email, DateTime verifiedDateTime);

        void DeleteUserCentreDetail(int userId, int centreId);

        (IEnumerable<UserAccountEntity>, int recordCount) GetUserAccounts(
            string search, int offset, int rows, int jobGroupId, string userStatus, string emailStatus, int userId, int failedLoginThreshold
            );

        string GetUserDisplayName(int userId);

        void InactivateUser(int userId);

        void UpdateUserDetailsAccount(string firstName, string lastName, string primaryEmail, int jobGroupId, string? prnNumber, DateTime? emailVerified, int userId);

        void ActivateUser(int userId);

        (IEnumerable<AdminEntity>, int) GetAllAdmins(
       string search, int offset, int rows, int? adminId, string userStatus, string role, int? centreId, int failedLoginThreshold
       );

        bool PrimaryEmailIsInUseAtCentre(string email, int centreId);

        void UpdateAdminStatus(int adminId, bool active);
    }

    public partial class UserDataService : IUserDataService
    {
        private readonly ILogger<UserDataService> logger;
        private const string BaseSelectUserQuery =
            @"SELECT
                u.ID,
                u.PrimaryEmail,
                u.PasswordHash,
                u.FirstName,
                u.LastName,
                u.JobGroupID,
                u.ProfessionalRegistrationNumber,
                u.ProfileImage,
                u.Active,
                u.ResetPasswordID,
                u.TermsAgreed,
                u.FailedLoginCount,
                u.HasBeenPromptedForPrn,
                u.LearningHubAuthId,
                u.HasDismissedLhLoginWarning,
                u.EmailVerified,
                u.DetailsLastChecked,
                jg.JobGroupID,
                jg.JobGroupName
            FROM Users AS u
            INNER JOIN JobGroups AS jg ON jg.JobGroupID = u.JobGroupID";

        private const string BaseSelectUserAccountsQuery =
            @"SELECT
                u.ID,
                u.PrimaryEmail,
                u.EmailVerified,
                u.FirstName,
                u.LastName,
                u.FirstName + ' ' + u.LastName + ' (' + u.PrimaryEmail +')' AS DisplayName,
                u.JobGroupID,
                jg.JobGroupName,
                u.ProfessionalRegistrationNumber,
                u.Active,
                u.LearningHubAuthId
            FROM Users AS u
            INNER JOIN JobGroups AS jg ON jg.JobGroupID = u.JobGroupID";

        private readonly IDbConnection connection;

        public UserDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int? GetUserIdFromUsername(string username)
        {
            var userIds = connection.Query<int?>(
                @"SELECT DISTINCT u.ID
                    FROM Users AS u
                    LEFT JOIN DelegateAccounts AS da ON da.UserID = u.ID
                    WHERE u.PrimaryEmail = @username OR da.CandidateNumber = @username",
                new { username }
            ).ToList();

            if (userIds.Count > 1)
            {
                throw new MultipleUserAccountsFoundException(
                    "Recovered more than 1 User when logging in with username: " + username
                );
            }

            return userIds.SingleOrDefault();
        }

        public int GetUserIdFromDelegateId(int delegateId)
        {
            var userId = connection.QuerySingle<int?>(
                @"SELECT UserID FROM DelegateAccounts WHERE ID = @delegateId",
                new { delegateId }
            );

            if (userId == null)
            {
                throw new UserAccountNotFoundException("No Delegate found with DelegateID: " + delegateId);
            }

            return userId.Value;
        }

        public UserAccount? GetUserAccountById(int userId)
        {
            return connection.Query<UserAccount>(
                @$"{BaseSelectUserQuery} WHERE u.ID = @userId",
                new { userId }
            ).SingleOrDefault();
        }

        public UserAccount? GetUserAccountByPrimaryEmail(string emailAddress)
        {
            return connection.Query<UserAccount>(
                @$"{BaseSelectUserQuery} WHERE u.PrimaryEmail = @emailAddress",
                new { emailAddress }
            ).SingleOrDefault();
        }

        public int? GetUserIdByAdminId(int adminId)
        {
            return connection.Query<int>(
                @"SELECT UserID FROM AdminAccounts WHERE ID = @adminId",
                new { adminId }
            ).SingleOrDefault();
        }

        public void UpdateUserFailedLoginCount(int userId, int updatedCount)
        {
            connection.Execute(
                @"UPDATE Users
                        SET
                            FailedLoginCount = @updatedCount
                        WHERE ID = @userId",
                new { userId, updatedCount }
            );
        }

        public bool PrimaryEmailIsInUse(string email)
        {
            return PrimaryEmailIsInUseQuery(email, null);
        }

        public bool PrimaryEmailIsInUseByOtherUser(string email, int userId)
        {
            return PrimaryEmailIsInUseQuery(email, userId);
        }

        public void SetPrimaryEmailAndActivate(int userId, string email)
        {
            connection.Execute(
                @"UPDATE Users SET PrimaryEmail = @email, Active = 1 WHERE ID = @userId",
                new { email, userId }
            );
        }

        public EmailVerificationDetails? GetPrimaryEmailVerificationDetails(string code)
        {
            return connection.Query<EmailVerificationDetails>(
                @"SELECT
                        u.Id AS UserId,
                        u.PrimaryEmail AS Email,
                        u.EmailVerified,
                        h.EmailVerificationHash,
                        h.CreatedDate AS EmailVerificationHashCreatedDate,
                        NULL AS IsCentreEmailForUnapprovedDelegate
                    FROM Users u
                    JOIN EmailVerificationHashes h ON h.ID = u.EmailVerificationHashID
                    WHERE h.EmailVerificationHash = @code",
                new { code }
            ).SingleOrDefault();
        }

        public void SetPrimaryEmailVerified(int userId, string email, DateTime verifiedDateTime)
        {
            connection.Execute(
                @"UPDATE Users
                    SET EmailVerified = @verifiedDateTime, EmailVerificationHashID = NULL
                    WHERE ID = @userId AND PrimaryEmail = @email AND EmailVerified IS NULL",
                new { userId, email, verifiedDateTime }
            );
        }

        public void DeleteUser(int userId)
        {
            connection.Execute(
                @"DELETE FROM Users WHERE ID = @userId",
                new { userId }
            );
        }

        private bool PrimaryEmailIsInUseQuery(string email, int? userId)
        {
            return connection.QueryFirst<int>(
                @$"SELECT COUNT(*)
                    FROM Users
                    WHERE PrimaryEmail = @email
                    {(userId == null ? "" : "AND Id <> @userId")}",
                new { email, userId }
            ) > 0;
        }

        public (IEnumerable<UserAccountEntity>, int) GetUserAccounts(
        string search, int offset, int rows, int jobGroupId, string userStatus, string emailStatus, int userId, int failedLoginThreshold
        )
        {
            if(!string.IsNullOrEmpty(search))
            {
                search= search.Trim();
            }
            string condition = $@" WHERE ((@userId = 0) OR (u.ID = @userId)) AND 
            (u.FirstName + ' ' + u.LastName + ' ' + u.PrimaryEmail + ' ' + COALESCE(u.ProfessionalRegistrationNumber, '') LIKE N'%' + @search + N'%') AND 
            ((u.JobGroupID = @jobGroupId) OR (@jobGroupId = 0)) AND 
            ((@userStatus = 'Any') OR (@userStatus = 'Active' AND u.Active = 1) OR (@userStatus = 'Inactive' AND u.Active = 0) OR (@userStatus = 'Locked' AND u.FailedLoginCount >= @failedLoginThreshold)) AND 
            ((@emailStatus = 'Any') OR (@emailStatus = 'Verified' AND u.EmailVerified IS NOT NULL) OR (@emailStatus = 'Unverified' AND u.EmailVerified IS NULL))
            ";

            string sql = @$"{BaseSelectUserQuery}{condition} ORDER BY LTRIM(u.LastName), LTRIM(u.FirstName)
                OFFSET @offset ROWS
                FETCH NEXT @rows ROWS ONLY";

            IEnumerable<UserAccountEntity> userAccountEntity = connection.Query<UserAccount, JobGroup, UserAccountEntity>(
                sql,
                (userAccount, jobGroup) => new UserAccountEntity(
                    userAccount, jobGroup),
                new { userId, search, jobGroupId, userStatus, failedLoginThreshold, emailStatus, offset, rows },
                splitOn: "JobGroupID",
                commandTimeout: 3000
            );

            int ResultCount = connection.ExecuteScalar<int>(
                @$"SELECT COUNT(ID) AS Matches
                            FROM   Users AS u INNER JOIN
                            JobGroups AS jg ON u.JobGroupID = jg.JobGroupID {condition}",
                new { userId, search, jobGroupId, userStatus, failedLoginThreshold, emailStatus },
                commandTimeout: 3000
            );
            return (userAccountEntity, ResultCount);
        }

        public string GetUserDisplayName(int userId)
        {
            return connection.Query<string>(
                    @"SELECT
                       u.FirstName + ' ' + u.LastName + ' (' + u.PrimaryEmail +')'
                       FROM Users u
                       WHERE u.ID = @userId",
                    new { userId }
                ).Single();
        }

        public void InactivateUser(int userId)
        {
            var numberOfAffectedRows = connection.Execute(
            @"
               BEGIN TRY
               BEGIN TRANSACTION
                        UPDATE Users SET Active=0 WHERE ID=@UserID

                        UPDATE AdminAccounts SET Active=0 WHERE UserID=@UserID

                        UPDATE DelegateAccounts SET Active=0 WHERE UserID=@UserID

                        DELETE FROM NotificationUsers WHERE AdminUserID IN (SELECT ID FROM AdminAccounts WHERE UserID=@UserID)

                        DELETE FROM NotificationUsers WHERE CandidateID IN (SELECT ID FROM DelegateAccounts WHERE UserID=@UserID)

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
                ",
                new
                {
                    UserID = userId
                }
            );

            if (numberOfAffectedRows == 0)
            {
                string message =
                $"db insert/update failed for User ID: {userId}";
                logger.LogWarning(message);
                throw new InactivateUserUpdateException(message);
            }
        }

        public void ActivateUser(int userId)
        {
            connection.Execute(
            @"UPDATE Users SET Active=1 WHERE ID=@UserID",
                new
                {
                    UserID = userId
                }
            );
        }

        public void UpdateUserDetailsAccount(string firstName, string lastName, string primaryEmail, int jobGroupId, string? prnNumber, DateTime? emailVerified, int userId)
        {
            connection.Execute(
                @"UPDATE Users
                  SET
                  FirstName = @firstName,
                  LastName = @lastName,
                  PrimaryEmail = @primaryEmail,
                  JobGroupId = @jobGroupId,
                  ProfessionalRegistrationNumber = @prnNumber,
                  EmailVerified = @emailVerified
                WHERE ID = @userId",
                new { firstName, lastName, primaryEmail, jobGroupId, prnNumber, emailVerified, userId }
            );
        }
    }
}
