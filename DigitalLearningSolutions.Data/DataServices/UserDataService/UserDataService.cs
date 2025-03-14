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
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.SuperAdmin;

    public interface IUserDataService
    {
        AdminEntity? GetAdminById(int id);

        IEnumerable<AdminEntity> GetActiveAdminsByCentreId(int centreId);

        IEnumerable<AdminEntity> GetAdminsByCentreId(int centreId);

        AdminUser? GetAdminUserById(int id);

        List<AdminUser> GetAdminUsersByCentreId(int centreId);
        List<AdminUser> GetAdminUsersAtCentreForCategory(int centreId, int categoryId);

        AdminUser? GetAdminUserByEmailAddress(string emailAddress);

        int GetNumberOfAdminsAtCentre(int centreId);

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


        void DeactivateDelegateUser(int delegateId);

        void UpdateUserDetails(string firstName, string surname, string primaryEmail, int jobGroupId, int userId);

        DelegateUserCard? GetDelegateUserCardById(int id);

        List<DelegateUserCard> GetDelegateUserCardsByCentreId(int centreId);
        List<DelegateUserCard> GetDelegateUserCardsForExportByCentreId(String searchString, string sortBy, string sortDirection, int centreId,
                                    string isActive, string isPasswordSet, string isAdmin, string isUnclaimed, string isEmailVerified, string registrationType, int jobGroupId,
                                    int? groupId, string answer1, string answer2, string answer3, string answer4, string answer5, string answer6, int exportQueryRowLimit, int currentRun);
        int GetCountDelegateUserCardsForExportByCentreId(String searchString, string sortBy, string sortDirection, int centreId,
                                     string isActive, string isPasswordSet, string isAdmin, string isUnclaimed, string isEmailVerified, string registrationType, int jobGroupId,
                                     int? groupId, string answer1, string answer2, string answer3, string answer4, string answer5, string answer6);

        (IEnumerable<DelegateUserCard>, int) GetDelegateUserCards(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection, int centreId,
                                    string isActive, string isPasswordSet, string isAdmin, string isUnclaimed, string isEmailVerified, string registrationType, int jobGroupId,
                                    int? groupId, string answer1, string answer2, string answer3, string answer4, string answer5, string answer6);

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

        int? GetUserLearningHubAuthId(int userId);

        void SetDelegateUserLearningHubAuthId(int delegateId, int learningHubAuthId);

        void SetUserLearningHubAuthId(int userId, int learningHubAuthId);

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

        string GetEmailVerificationHash(int ID);

        IEnumerable<(int centreId, string centreEmail, string EmailVerificationHashID)> GetUnverifiedCentreEmailsForUserList(int userId);

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
        (IEnumerable<SuperAdminDelegateAccount>, int) GetAllDelegates(
      string search, int offset, int rows, int? delegateId, string accountStatus, string lhlinkStatus, int? centreId, int failedLoginThreshold
      );
        IEnumerable<AdminEntity> GetAllAdminsExport(
      string search, int offset, int rows, int? adminId, string userStatus, string role, int? centreId, int failedLoginThreshold, int exportQueryRowLimit, int currentRun
      );

        bool PrimaryEmailIsInUseAtCentre(string email, int centreId);

        void UpdateAdminStatus(int adminId, bool active);

        void UpdateAdminUserAndSpecialPermissions(
            int adminId, bool isCentreAdmin, bool isSupervisor, bool isNominatedSupervisor, bool isTrainer,
            bool isContentCreator,
            bool isContentManager,
            bool importOnly,
            int? categoryId,
            bool isCentreManager,
            bool isSuperAdmin,
            bool isReportsViewer,
            bool isLocalWorkforceManager,
            bool isFrameworkDeveloper,
            bool isWorkforceManager
        );

        int GetUserIdFromAdminId(int adminId);
        void UpdateAdminCentre(int adminId, int centreId);
        bool IsUserAlreadyAdminAtCentre(int? userId, int centreId);

        void LinkAdminAccountToNewUser(
            int currentUserIdForAdminAccount,
            int newUserIdForAdminAccount,
            int centreId
        );
        int RessultCount(int adminId, string search, int? centreId, string userStatus, int failedLoginThreshold, string role);

        public void DeleteUserAndAccounts(int userId);

        public bool PrimaryEmailInUseAtCentres(string email);

        public int? GetUserIdFromLearningHubAuthId(int learningHubAuthId);
       void DeactivateAdminAccount(int userId, int centreId);
        int? CheckDelegateIsActive(int delegateId);
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
                u.ProfessionalRegistrationNumber,
                u.ProfileImage,
                u.Active,
                u.LastAccessed,
                u.ResetPasswordID,
                u.TermsAgreed,
                u.FailedLoginCount,
                u.HasBeenPromptedForPrn,
                u.LearningHubAuthId,
                u.HasDismissedLhLoginWarning,
                u.EmailVerified,
                u.DetailsLastChecked,
                u.EmailVerificationHashID,
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
        private readonly ILogger<UserDataService> logger;

        public UserDataService(IDbConnection connection, ILogger<UserDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
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

        public string GetEmailVerificationHash(int ID)
        {
            var EmailVerificationHash = connection.QuerySingle<string?>(
               @"SELECT EmailVerificationHash FROM EmailVerificationHashes WHERE ID = @ID",
               new { ID }
           );
            return EmailVerificationHash!;
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
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
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
            string trimmedFirstName = firstName.Trim();
            string trimmedLastName = lastName.Trim();
            connection.Execute(
                @"UPDATE Users
                  SET
                  FirstName = @trimmedFirstName,
                  LastName = @trimmedLastName,
                  PrimaryEmail = @primaryEmail,
                  JobGroupId = @jobGroupId,
                  ProfessionalRegistrationNumber = @prnNumber,
                  EmailVerified = @emailVerified
                WHERE ID = @userId",
                new { trimmedFirstName, trimmedLastName, primaryEmail, jobGroupId, prnNumber, emailVerified, userId }
            );
        }
        public (IEnumerable<SuperAdminDelegateAccount>, int) GetAllDelegates(
      string search, int offset, int rows, int? delegateId, string accountStatus, string lhlinkStatus, int? centreId, int failedLoginThreshold
      )
        {
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
            }
            string BaseSelectQuery = @$"SELECT
                da.ID,
                da.Active,
                da.CentreID,
                ce.CentreName,
                ce.Active AS CentreActive,
                da.DateRegistered,
                da.CandidateNumber,
                da.Approved,
                da.SelfReg,
                da.UserID,
                da.RegistrationConfirmationHash,
                u.ID as UserId,
                 u.PrimaryEmail AS EmailAddress,
                u.FirstName,
                u.LastName,
                u.Active as UserActive,
                u.LearningHubAuthID,
                u.EmailVerified,
                ucd.ID as UserCentreDetailID,
                ucd.UserID,
                ucd.CentreID,
                ucd.Email as CentreEmail,
                ucd.EmailVerified as CentreEmailVerified,
                (SELECT ID
                    FROM AdminAccounts aa
                        WHERE aa.UserID = da.UserID
                            AND aa.CentreID = da.CentreID
                            AND aa.Active = 1
                ) AS AdminID
            FROM DelegateAccounts AS da WITH (NOLOCK)
            INNER JOIN Centres AS ce WITH (NOLOCK) ON ce.CentreId = da.CentreID
            INNER JOIN Users AS u WITH (NOLOCK) ON u.ID = da.UserID
            LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = u.ID
            AND ucd.CentreId = da.CentreID
            INNER JOIN JobGroups AS jg WITH (NOLOCK) ON jg.JobGroupID = u.JobGroupID";
            string condition = $@" WHERE ((@delegateId = 0) OR (da.ID = @delegateId)) 	AND (u.FirstName + ' ' + u.LastName + ' ' + u.PrimaryEmail + ' ' + COALESCE(ucd.Email, '') + ' ' + COALESCE(da.CandidateNumber, '') LIKE N'%' + @search + N'%')
                                    AND ((ce.CentreID = @centreId) OR (@centreId= 0)) 
                                    AND ((@accountStatus = 'Any') OR (@accountStatus = 'Active' AND da.Active = 1 AND u.Active =1) OR (@accountStatus = 'Inactive' AND (u.Active = 0 OR da.Active =0)) 
	                                    OR(@accountStatus = 'Approved' AND  da.Approved =1) OR (@accountStatus = 'Unapproved' AND  da.Approved =0)
	                                    OR (@accountStatus = 'Claimed' AND  da.RegistrationConfirmationHash is  null) OR (@accountStatus = 'Unclaimed' AND da.RegistrationConfirmationHash is not null))
                                    AND ((@lhlinkStatus = 'Any') OR (@lhlinkStatus = 'Linked' AND u.LearningHubAuthID IS NOT NULL) OR (@lhlinkStatus = 'Not linked' AND u.LearningHubAuthID IS NULL))";

            string sql = @$"{BaseSelectQuery}{condition} ORDER BY LTRIM(u.LastName), LTRIM(u.FirstName)
                            OFFSET @offset ROWS
                            FETCH NEXT @rows ROWS ONLY";
            IEnumerable<SuperAdminDelegateAccount> delegateEntity = connection.Query<SuperAdminDelegateAccount>(
                sql,
            new { delegateId, search, centreId, accountStatus, lhlinkStatus, offset, rows },
                commandTimeout: 3000
            );

            int ResultCount = connection.ExecuteScalar<int>(
                            @$"SELECT  COUNT(*) AS Matches
                            FROM DelegateAccounts AS da WITH (NOLOCK)
                            INNER JOIN Centres AS ce WITH (NOLOCK) ON ce.CentreId = da.CentreID
                            INNER JOIN Users AS u WITH (NOLOCK) ON u.ID = da.UserID
                            LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = u.ID
                            AND ucd.CentreId = da.CentreID
                            INNER JOIN JobGroups AS jg WITH (NOLOCK) ON jg.JobGroupID = u.JobGroupID {condition}",
                new { delegateId, search, centreId, accountStatus, failedLoginThreshold, lhlinkStatus },
                commandTimeout: 3000
            );
            return (delegateEntity, ResultCount);
        }

        public void DeleteUserAndAccounts(int userId)
        {
            var numberOfAffectedRows = connection.Execute(
            @"
	           BEGIN TRY
	                BEGIN TRANSACTION

                        DELETE FROM aspProgress WHERE ProgressID IN (SELECT ProgressID FROM Progress WHERE CandidateID in (SELECT ID FROM DelegateAccounts where UserID = @userId))

                        DELETE FROM Progress WHERE CandidateID IN (SELECT ID FROM DelegateAccounts where UserID = @userId)

                        DELETE FROM ReportSelfAssessmentActivityLog where UserID = @userId

                        DELETE FROM SelfAssessmentResultSupervisorVerifications WHERE CandidateAssessmentSupervisorID IN ( SELECT ID
                        FROM CandidateAssessmentSupervisors where CandidateAssessmentID IN (select ID from CandidateAssessments where DelegateUserID = @userId))

                        DELETE FROM CandidateAssessmentSupervisorVerifications WHERE CandidateAssessmentSupervisorID IN ( SELECT ID
                        FROM CandidateAssessmentSupervisors where CandidateAssessmentID IN (select ID from CandidateAssessments where DelegateUserID = @userId))

                        DELETE FROM CandidateAssessmentSupervisors where CandidateAssessmentID IN (select ID from CandidateAssessments where DelegateUserID = @userId)

                        DELETE FROM CandidateAssessmentOptionalCompetencies WHERE CandidateAssessmentID IN (select ID from CandidateAssessments where DelegateUserID = @userId)

                        DELETE from CandidateAssessments where DelegateUserID = @userId

                        DELETE FROM SupervisorDelegates WHERE DelegateUserID = @userId

				        DELETE FROM UserCentreDetails WHERE UserID = @userId

				        DELETE FROM AdminAccounts WHERE UserID = @userId

				        DELETE FROM DelegateAccounts WHERE UserID = @userId

				        DELETE FROM Users WHERE ID = @userId

			        COMMIT TRANSACTION
		        END TRY
		        BEGIN CATCH
                    IF @@TRANCOUNT<>0
	                BEGIN
		                ROLLBACK TRANSACTION
	                END
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
                $"db delete user failed for User ID: {userId}";
                throw new DeleteUserException(message);
            }
        }

        public bool PrimaryEmailInUseAtCentres(string email)
        {
            return connection.QueryFirst<int>(
               @$"SELECT COUNT(*)
                    FROM UserCentreDetails
                    WHERE Email = @email ",
               new { email }
           ) > 0;
        }

        public int? GetUserIdFromLearningHubAuthId(int learningHubAuthId)
        {
            var query = $"SELECT DISTINCT u.ID " +
                $"FROM Users AS u " +
                $"WHERE u.LearningHubAuthId = {learningHubAuthId}" +
                $"ORDER BY u.ID";
            var userId = connection.Query<int?>(
                query
            ).FirstOrDefault();

            return userId;
        }

        public int? GetUserLearningHubAuthId(int userId)
        {
            return connection.Query<int?>(
                @"SELECT LearningHubAuthId
                    FROM Users
                    WHERE ID = @userId",
                new { userId }
                ).Single();
        }

        public void SetUserLearningHubAuthId(int userId, int learningHubAuthId)
        {
            connection.Execute(
                @"UPDATE Users
                    SET LearningHubAuthId = @learningHubAuthId
                    WHERE ID = @userId",
                new { userId, learningHubAuthId });
        }
    }
}
