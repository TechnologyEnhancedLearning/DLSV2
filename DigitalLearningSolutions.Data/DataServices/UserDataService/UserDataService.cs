namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserDataService
    {
        AdminUser? GetAdminUserById(int id);

        List<AdminUser> GetAdminUsersByCentreId(int centreId);

        AdminUser? GetAdminUserByEmailAddress(string emailAddress);

        int GetNumberOfActiveAdminsAtCentre(int centreId);

        void UpdateAdminUser(string firstName, string surname, string email, byte[]? profileImage, int id);

        void UpdateAdminUserPermissions(
            int adminId,
            bool isCentreAdmin,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool isTrainer,
            bool isContentCreator,
            bool isContentManager,
            bool importOnly,
            int categoryId
        );

        void UpdateUserFailedLoginCount(int userId, int updatedCount);

        DelegateUser? GetDelegateUserById(int id);

        List<DelegateUser> GetDelegateUsersByUsername(string username);

        List<DelegateUser> GetAllDelegateUsersByUsername(string username);

        List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress);

        List<DelegateUser> GetUnapprovedDelegateUsersByCentreId(int centreId);

        void UpdateUser(
            string firstName,
            string surname,
            string primaryEmail,
            byte[]? profileImage,
            string? professionalRegNumber,
            bool hasBeenPromptedForPrn,
            int jobGroupId,
            int userId
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

        DelegateUser? GetDelegateUserByCandidateNumber(string candidateNumber, int centreId);

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
            string? answer6
        );

        int GetDelegateCountWithAnswerForPrompt(int centreId, int promptNumber);

        void DeleteAllAnswersForPrompt(int centreId, int promptNumber);

        void DeactivateAdmin(int adminId);

        void ActivateDelegateUser(int delegateId);

        int? GetDelegateUserLearningHubAuthId(int delegateId);

        void SetDelegateUserLearningHubAuthId(int delegateId, int learningHubAuthId);

        void UpdateDelegateLhLoginWarningDismissalStatus(int delegateId, bool status);

        void UpdateDelegateProfessionalRegistrationNumber(
            int delegateId,
            string? professionalRegistrationNumber,
            bool hasBeenPromptedForPrn
        );

        bool AnyEmailsInSetAreAlreadyInUse(IEnumerable<string?> emails);

        void DeleteAdminAccount(int adminId);

        int? GetUserIdFromUsername(string username);

        UserAccount? GetUserAccountById(int userId);

        IEnumerable<AdminAccount> GetAdminAccountsByUserId(int userId);

        IEnumerable<DelegateAccount> GetDelegateAccountsByUserId(int userId);
    }

    public partial class UserDataService : IUserDataService
    {
        private readonly IDbConnection connection;

        public UserDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public bool AnyEmailsInSetAreAlreadyInUse(IEnumerable<string?> emails)
        {
            return connection.QueryFirst<bool>(
                @"SELECT CASE
                        WHEN EXISTS (SELECT ID FROM USERS WHERE PrimaryEmail IN @emails)
                            OR EXISTS (SELECT ID FROM DelegateAccounts da WHERE da.Email IN @emails AND da.Email IS NOT NULL)
                            OR EXISTS (SELECT ID FROM AdminAccounts aa WHERE aa.Email IN @emails AND aa.Email IS NOT NULL)
                        THEN 1
                        ELSE 0
                        END",
                new { emails }
            );
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

        public UserAccount? GetUserAccountById(int userId)
        {
            return connection.Query<UserAccount>(
                @"SELECT u.ID, 
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
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = u.JobGroupID
                    WHERE u.ID = @userId",
                new { userId }
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
    }
}
