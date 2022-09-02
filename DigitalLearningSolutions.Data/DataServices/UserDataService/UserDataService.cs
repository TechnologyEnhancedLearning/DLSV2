namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

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
            int? categoryId
        );

        void UpdateUserFailedLoginCount(int userId, int updatedCount);

        DelegateEntity? GetDelegateById(int id);

        DelegateEntity? GetDelegateByCandidateNumber(string candidateNumber);

        IEnumerable<DelegateEntity> GetUnapprovedDelegatesByCentreId(int centreId);

        DelegateUser? GetDelegateUserById(int id);

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
    }

    public partial class UserDataService : IUserDataService
    {
        private const string BaseSelectUserQuery =
            @"SELECT
                u.ID,
                u.PrimaryEmail,
                u.PasswordHash,
                u.FirstName,
                u.LastName,
                u.JobGroupID,
                jg.JobGroupName,
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
                u.DetailsLastChecked
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
    }
}
