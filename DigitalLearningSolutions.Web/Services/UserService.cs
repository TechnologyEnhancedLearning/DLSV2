namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SuperAdmin;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using DocumentFormat.OpenXml.Office2010.Excel;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;

    public interface IUserService
    {
        AdminUser? GetAdminUserByAdminId(int? adminId);
        DelegateUser? GetDelegateUserByDelegateUserIdAndCentreId(int? delegateUserId, int? centreId);

        DelegateEntity? GetDelegateById(int id);

        DelegateUser? GetDelegateUserById(int delegateId);

        List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId);

        void UpdateUserDetails(
            EditAccountDetailsData editAccountDetailsData,
            bool isPrimaryEmailUpdated,
            bool changeMadeBySameUser,
            DateTime? detailsLastChecked = null
        );

        void UpdateUserDetailsAndCentreSpecificDetails(
            EditAccountDetailsData editAccountDetailsData,
            DelegateDetailsData? delegateDetailsData,
            string? centreEmail,
            int centreId,
            bool isPrimaryEmailUpdated,
            bool isCentreEmailUpdated,
            bool changeMadeBySameUser
        );

        void SetCentreEmails(
            int userId,
            Dictionary<int, string?> centreEmailsByCentreId,
            List<UserCentreDetails> userCentreDetails
        );

        void ResetFailedLoginCount(UserAccount userAccount);

        void ResetFailedLoginCountByUserId(int userId);

        void UpdateFailedLoginCount(UserAccount userAccount);

        IEnumerable<DelegateUserCard> GetDelegateUserCardsForWelcomeEmail(int centreId);

        void UpdateAdminUserPermissions(
            int adminId,
            AdminRoles adminRoles,
            int? categoryId
        );

        IEnumerable<AdminUser> GetSupervisorsAtCentre(int centreId);

        IEnumerable<AdminUser> GetSupervisorsAtCentreForCategory(int centreId, int categoryId);

        bool DelegateUserLearningHubAccountIsLinked(int delegateId);

        int? GetDelegateUserLearningHubAuthId(int delegateId);

        void UpdateDelegateLhLoginWarningDismissalStatus(int delegateId, bool status);

        void DeactivateOrDeleteAdmin(int adminId);

        void DeactivateOrDeleteAdminForSuperAdmin(int adminId);

        UserEntity? GetUserById(int userId);

        string? GetEmailVerificationHashesFromEmailVerificationHashID(int ID);

        public List<(int centreId, string centreEmail, string EmailVerificationHashID)> GetUnverifiedCentreEmailListForUser(int userId);

        UserEntity? GetUserByUsername(string username);

        public UserAccount? GetUserAccountById(int userId);

        UserAccount? GetUserAccountByEmailAddress(string emailAddress);

        string? GetCentreEmail(int userId, int centreId);

        IEnumerable<(int centreId, string centreName, string? centreSpecificEmail)> GetAllActiveCentreEmailsForUser(
            int userId, bool isAll = false
        );

        bool ShouldForceDetailsCheck(UserEntity userEntity, int centreIdToCheck);

        AdminEntity? GetAdminById(int adminId);

        (string? primaryEmail, List<(int centreId, string centreName, string centreEmail)> centreEmails)
            GetUnverifiedEmailsForUser(int userId);

        EmailVerificationTransactionData? GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails(
            string email,
            string code
        );

        void SetEmailVerified(int userId, string email, DateTime verifiedDateTime);

        bool EmailIsHeldAtCentre(string? email, int centreId);

        void ReactivateAdmin(int adminId);

        UserEntity? GetDelegateUserFromLearningHubAuthId(int learningHubAuthId);

        int? GetUserLearningHubAuthId(int userId);
        bool CentreSpecificEmailIsInUseAtCentreByOtherUser(
           string email,
           int centreId,
           int userId
       );
        bool PrimaryEmailIsInUseByOtherUser(string email, int userId);
        IEnumerable<UserCentreDetails> GetCentreDetailsForUser(int userId);
        bool PrimaryEmailIsInUse(string email);
        void SetPrimaryEmailVerified(int userId, string email, DateTime verifiedDateTime);
        (int? userId, int? centreId, string? centreName) GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                string centreSpecificEmail,
                string registrationConfirmationHash
            );
        (IEnumerable<DelegateUserCard>, int) GetDelegateUserCards(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection, int centreId,
                                    string isActive, string isPasswordSet, string isAdmin, string isUnclaimed, string isEmailVerified, string registrationType, int jobGroupId,
                                    int? groupId, string answer1, string answer2, string answer3, string answer4, string answer5, string answer6);

        DelegateUserCard? GetDelegateUserCardById(int id);
        void DeactivateDelegateUser(int delegateId);
        void ActivateDelegateUser(int delegateId);
        int GetUserIdFromDelegateId(int delegateId);
        void DeleteUserAndAccounts(int userId);
        public bool PrimaryEmailInUseAtCentres(string email);
        bool CentreSpecificEmailIsInUseAtCentre(string email, int centreId);
        int? GetUserIdByAdminId(int adminId);
        AdminUser? GetAdminUserByEmailAddress(string emailAddress);
        DelegateAccount? GetDelegateAccountById(int id);
        int? GetUserIdFromUsername(string username);
        IEnumerable<DelegateAccount> GetDelegateAccountsByUserId(int userId);
        void SetCentreEmail(
            int userId,
            int centreId,
            string? email,
            DateTime? emailVerified,
            IDbTransaction? transaction = null
        );
        int GetDelegateCountWithAnswerForPrompt(int centreId, int promptNumber);
        List<AdminUser> GetAdminUsersByCentreId(int centreId);

        AdminUser? GetAdminUserById(int id);
        string GetUserDisplayName(int userId);

        (IEnumerable<SuperAdminDelegateAccount>, int) GetAllDelegates(
      string search, int offset, int rows, int? delegateId, string accountStatus, string lhlinkStatus, int? centreId, int failedLoginThreshold
      );
        void DeleteUserCentreDetail(int userId, int centreId);
        void ApproveDelegateUsers(params int[] ids);
        (IEnumerable<AdminEntity>, int) GetAllAdmins(
       string search, int offset, int rows, int? adminId, string userStatus, string role, int? centreId, int failedLoginThreshold
       );
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
        void DeleteAdminAccount(int adminId);
        void UpdateAdminStatus(int adminId, bool active);
        void UpdateAdminCentre(int adminId, int centreId);
        bool IsUserAlreadyAdminAtCentre(int? userId, int centreId);
        IEnumerable<AdminEntity> GetAdminsByCentreId(int centreId);
        void DeactivateAdmin(int adminId);
        void ActivateUser(int userId);
        void InactivateUser(int userId);
        (IEnumerable<UserAccountEntity>, int recordCount) GetUserAccounts(
           string search, int offset, int rows, int jobGroupId, string userStatus, string emailStatus, int userId, int failedLoginThreshold
           );
        void UpdateUserDetailsAccount(string firstName, string lastName, string primaryEmail, int jobGroupId, string? prnNumber, DateTime? emailVerified, int userId);
        void DeactivateAdminAccount(int userId, int centreId);
        int? CheckDelegateIsActive(int delegateId);
    }

    public class UserService : IUserService
    {
        private readonly ICentreContractAdminUsageService centreContractAdminUsageService;
        private readonly IClockUtility clockUtility;
        private readonly IConfiguration configuration;
        private readonly IEmailVerificationDataService emailVerificationDataService;
        private readonly IGroupsService groupsService;
        private readonly ILogger<IUserService> logger;
        private readonly ISessionDataService sessionDataService;
        private readonly IUserDataService userDataService;

        public UserService(
            IUserDataService userDataService,
            IGroupsService groupsService,
            ICentreContractAdminUsageService centreContractAdminUsageService,
            ISessionDataService sessionDataService,
            IEmailVerificationDataService emailVerificationDataService,
            ILogger<IUserService> logger,
            IClockUtility clockUtility,
            IConfiguration configuration
        )
        {
            this.userDataService = userDataService;
            this.groupsService = groupsService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
            this.sessionDataService = sessionDataService;
            this.emailVerificationDataService = emailVerificationDataService;
            this.logger = logger;
            this.clockUtility = clockUtility;
            this.configuration = configuration;
        }

        public AdminUser? GetAdminUserByAdminId(int? adminId)
        {
            AdminUser? adminUser = null;

            if (adminId != null)
            {
                adminUser = userDataService.GetAdminUserById(adminId.Value);
            }
            return adminUser;
        }
        public DelegateUser? GetDelegateUserByDelegateUserIdAndCentreId(int? delegateUserId, int? centreId)
        {
            DelegateUser? delegateUser = null;

            if (delegateUserId != null && centreId != null)
            {
                delegateUser = userDataService.GetDelegateUserByDelegateUserIdAndCentreId(delegateUserId.Value, centreId.Value);
            }
            return delegateUser;
        }

        public List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId)
        {
            return userDataService.GetDelegatesNotRegisteredForGroupByGroupId(groupId, centreId);
        }

        public void ResetFailedLoginCount(UserAccount userAccount)
        {
            if (userAccount.FailedLoginCount != 0)
            {
                ResetFailedLoginCountByUserId(userAccount.Id);
            }
        }

        public void ResetFailedLoginCountByUserId(int userId)
        {
            userDataService.UpdateUserFailedLoginCount(userId, 0);
        }

        public void UpdateFailedLoginCount(UserAccount userAccount)
        {
            userDataService.UpdateUserFailedLoginCount(userAccount.Id, userAccount.FailedLoginCount);
        }

        public IEnumerable<DelegateUserCard> GetDelegateUserCardsForWelcomeEmail(int centreId)
        {
            return userDataService.GetDelegateUserCardsByCentreId(centreId).Where(
                user => user.Approved && !user.SelfReg && 
                        !string.IsNullOrEmpty(user.EmailAddress)
                        && !Guid.TryParse(user.EmailAddress, out _)
                        && user.RegistrationConfirmationHash != null
            );
        }

        public void UpdateAdminUserPermissions(
            int adminId,
            AdminRoles adminRoles,
            int? categoryId
        )
        {
            if (NewUserRolesExceedAvailableSpots(adminId, adminRoles))
            {
                throw new AdminRoleFullException(
                    "Failed to update admin roles for admin " + adminId +
                    " as one or more of the roles being added to have reached their limit"
                );
            }

            userDataService.UpdateAdminUserPermissions(
                adminId,
                adminRoles.IsCentreAdmin,
                adminRoles.IsSupervisor,
                adminRoles.IsNominatedSupervisor,
                adminRoles.IsTrainer,
                adminRoles.IsContentCreator,
                adminRoles.IsContentManager,
                adminRoles.ImportOnly,
                categoryId,
                adminRoles.IsCentreManager
            );
        }

        public IEnumerable<AdminUser> GetSupervisorsAtCentre(int centreId)
        {
            return userDataService.GetAdminUsersByCentreId(centreId).Where(au => au.IsSupervisor);
        }

        public IEnumerable<AdminUser> GetSupervisorsAtCentreForCategory(int centreId, int categoryId)
        {
            return userDataService.GetAdminUsersAtCentreForCategory(centreId, categoryId);

        }

        public bool DelegateUserLearningHubAccountIsLinked(int delegateId)
        {
            return userDataService.GetDelegateUserLearningHubAuthId(delegateId).HasValue;
        }

        public int? GetDelegateUserLearningHubAuthId(int delegateId)
        {
            return userDataService.GetDelegateUserLearningHubAuthId(delegateId);
        }

        public int? GetUserLearningHubAuthId(int userId)
        {
            return userDataService.GetUserLearningHubAuthId(userId);
        }

        public void UpdateDelegateLhLoginWarningDismissalStatus(int delegateId, bool status)
        {
            userDataService.UpdateDelegateLhLoginWarningDismissalStatus(delegateId, status);
        }

        public void DeactivateOrDeleteAdmin(int adminId)
        {
            if (sessionDataService.HasAdminGotSessions(adminId) || sessionDataService.HasAdminGotReferences(adminId))
            {
                userDataService.DeactivateAdmin(adminId);
            }
            else
            {
                try
                {
                    userDataService.DeleteAdminAccount(adminId);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(
                        ex,
                        $"Error attempting to delete admin {adminId} with no sessions, deactivating them instead."
                    );
                    userDataService.DeactivateAdmin(adminId);
                }
            }
        }

        public void DeactivateOrDeleteAdminForSuperAdmin(int adminId)
        {
            if (sessionDataService.HasAdminGotReferences(adminId))
            {
                userDataService.DeactivateAdmin(adminId);
            }
            else
            {
                try
                {
                    userDataService.DeleteAdminAccount(adminId);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(
                        ex,
                        $"Error attempting to delete admin {adminId} with no sessions, deactivating them instead."
                    );
                    userDataService.DeactivateAdmin(adminId);
                }
            }
        }

        public UserEntity? GetUserById(int userId)
        {
            var userAccount = userDataService.GetUserAccountById(userId);

            if (userAccount == null)
            {
                return null;
            }

            var adminAccounts = userDataService.GetAdminAccountsByUserId(userId).ToList();
            var delegateAccounts = userDataService.GetDelegateAccountsByUserId(userId).ToList();

            return new UserEntity(userAccount, adminAccounts, delegateAccounts);
        }

        public string? GetEmailVerificationHashesFromEmailVerificationHashID(int ID)
        {
            return userDataService.GetEmailVerificationHash(ID);
        }

        public UserEntity? GetUserByUsername(string username)
        {
            var userId = userDataService.GetUserIdFromUsername(username);

            return userId == null ? null : GetUserById(userId.Value);
        }

        public UserAccount? GetUserAccountById(int userId)
        {
            return userDataService.GetUserAccountById(userId);
        }

        public UserAccount? GetUserAccountByEmailAddress(string emailAddress)
        {
            return userDataService.GetUserAccountByPrimaryEmail(emailAddress);
        }

        public DelegateEntity? GetDelegateById(int id)
        {
            return userDataService.GetDelegateById(id);
        }

        public DelegateUser? GetDelegateUserById(int delegateId)
        {
            return userDataService.GetDelegateUserById(delegateId);
        }

        public bool ShouldForceDetailsCheck(UserEntity userEntity, int centreIdToCheck)
        {
            if (!new EmailAddressAttribute().IsValid(userEntity.UserAccount.PrimaryEmail))
            {
                return true;
            }

            var delegateAccount = userEntity.DelegateAccounts.SingleOrDefault(aa => aa.CentreId == centreIdToCheck);
            var now = clockUtility.UtcNow;
            var monthThresholdToForceCheck = ConfigurationExtensions.GetMonthsToPromptUserDetailsCheck(configuration);

            if (userEntity.UserAccount.DetailsLastChecked == null ||
                userEntity.UserAccount.DetailsLastChecked.Value.AddMonths(monthThresholdToForceCheck) < now)
            {
                return true;
            }

            return delegateAccount is { Active: true } &&
                   (delegateAccount.CentreSpecificDetailsLastChecked == null ||
                    delegateAccount.CentreSpecificDetailsLastChecked.Value.AddMonths(monthThresholdToForceCheck) < now);
        }

        public string? GetCentreEmail(int userId, int centreId)
        {
            return userDataService.GetCentreEmail(userId, centreId);
        }

        public IEnumerable<(int centreId, string centreName, string? centreSpecificEmail)>
            GetAllActiveCentreEmailsForUser(int userId, bool isAll = false)
        {
            return userDataService.GetAllActiveCentreEmailsForUser(userId, isAll);
        }

        public (string? primaryEmail, List<(int centreId, string centreName, string centreEmail)> centreEmails)
            GetUnverifiedEmailsForUser(int userId)
        {
            var userEntity = GetUserById(userId);

            if (userEntity == null)
            {
                return (null, new List<(int centreId, string centreName, string centreEmail)>());
            }

            var unverifiedPrimaryEmail = userEntity.UserAccount.EmailVerified == null
                ? userEntity.UserAccount.PrimaryEmail
                : null;

            var unverifiedCentreEmails = userDataService.GetUnverifiedCentreEmailsForUser(userId).Where(
                tuple =>
                    userEntity.AdminAccounts.Any(account => account.CentreId == tuple.centreId && account.Active) ||
                    userEntity.DelegateAccounts.Any(account => account.CentreId == tuple.centreId && account.Active)
            );

            return (unverifiedPrimaryEmail, unverifiedCentreEmails.ToList());
        }

        public List<(int centreId, string centreEmail, string EmailVerificationHashID)>
        GetUnverifiedCentreEmailListForUser(int userId)
        {
            var userEntity = GetUserById(userId);

            if (userEntity == null)
            {
                return new List<(int centreId, string centreName, string centreEmail)>();
            }

            var unverifiedCentreEmails = userDataService.GetUnverifiedCentreEmailsForUserList(userId).Where(
                tuple =>
                    userEntity.AdminAccounts.Any(account => account.CentreId == tuple.centreId && account.Active) ||
                    userEntity.DelegateAccounts.Any(account => account.CentreId == tuple.centreId && account.Active)
            );

            return unverifiedCentreEmails.ToList();
        }

        public AdminEntity? GetAdminById(int adminId)
        {
            return userDataService.GetAdminById(adminId);
        }

        public void UpdateUserDetails(
            EditAccountDetailsData editAccountDetailsData,
            bool isPrimaryEmailUpdated,
            bool changeMadeBySameUser,
            DateTime? detailsLastChecked = null
        )
        {
            var currentTime = clockUtility.UtcNow;
            var emailVerified = changeMadeBySameUser && !emailVerificationDataService.AccountEmailIsVerifiedForUser(
                editAccountDetailsData.UserId,
                editAccountDetailsData.Email
            )
                ? (DateTime?)null
                : currentTime;

            var currentJobGroupId = userDataService.GetUserAccountById(editAccountDetailsData.UserId)!.JobGroupId;

            groupsService.SynchroniseJobGroupsOnOtherCentres(
                    null,
                    editAccountDetailsData.UserId,
                    currentJobGroupId,
                    editAccountDetailsData.JobGroupId,
                    new AccountDetailsData(editAccountDetailsData.FirstName,
                        editAccountDetailsData.Surname,
                        editAccountDetailsData.Email)
                );

            userDataService.UpdateUser(
                editAccountDetailsData.FirstName,
                editAccountDetailsData.Surname,
                editAccountDetailsData.Email,
                editAccountDetailsData.ProfileImage,
                editAccountDetailsData.ProfessionalRegistrationNumber,
                editAccountDetailsData.HasBeenPromptedForPrn,
                editAccountDetailsData.JobGroupId,
                detailsLastChecked ?? currentTime,
                emailVerified,
                editAccountDetailsData.UserId,
                isPrimaryEmailUpdated,
                changeMadeBySameUser
            );
        }

        public void UpdateUserDetailsAndCentreSpecificDetails(
            EditAccountDetailsData editAccountDetailsData,
            DelegateDetailsData? delegateDetailsData,
            string? centreEmail,
            int centreId,
            bool isPrimaryEmailUpdated,
            bool isCentreEmailUpdated,
            bool changeMadeBySameUser
        )
        {
            var currentTime = clockUtility.UtcNow;

            if (delegateDetailsData != null)
            {
                var delegateAccountWithDetails = userDataService.GetDelegateUserById(delegateDetailsData.DelegateId)!;
                groupsService.UpdateSynchronisedDelegateGroupsBasedOnUserChanges(
                    delegateDetailsData.DelegateId,
                    editAccountDetailsData,
                    new RegistrationFieldAnswers(delegateDetailsData, editAccountDetailsData.JobGroupId, centreId),
                    delegateAccountWithDetails.GetRegistrationFieldAnswers(),
                    centreEmail
                );

                userDataService.UpdateDelegateUserCentrePrompts(
                    delegateDetailsData.DelegateId,
                    delegateDetailsData.Answer1,
                    delegateDetailsData.Answer2,
                    delegateDetailsData.Answer3,
                    delegateDetailsData.Answer4,
                    delegateDetailsData.Answer5,
                    delegateDetailsData.Answer6,
                    currentTime
                );
            }

            UpdateUserDetails(editAccountDetailsData, isPrimaryEmailUpdated, changeMadeBySameUser, currentTime);

            if (isCentreEmailUpdated)
            {
                var emailVerified =
                    changeMadeBySameUser &&
                    !emailVerificationDataService.AccountEmailIsVerifiedForUser(
                        editAccountDetailsData.UserId,
                        centreEmail
                    )
                        ? (DateTime?)null
                        : currentTime;

                userDataService.SetCentreEmail(
                    editAccountDetailsData.UserId,
                    centreId,
                    centreEmail,
                    emailVerified
                );
            }
        }

        public void SetCentreEmails(
            int userId,
            Dictionary<int, string?> centreEmailsByCentreId,
            List<UserCentreDetails> userCentreDetails
        )
        {
            var currentTime = clockUtility.UtcNow;

            foreach (var (centreId, email) in centreEmailsByCentreId)
            {
                if (!string.Equals(email, userCentreDetails.SingleOrDefault(ucd => ucd.CentreId == centreId)?.Email))
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        var emailVerified = emailVerificationDataService.AccountEmailIsVerifiedForUser(userId, email)
                            ? currentTime
                            : (DateTime?)null;

                        userDataService.SetCentreEmail(userId, centreId, email, emailVerified);
                    }
                    else
                    {
                        userDataService.DeleteUserCentreDetail(userId, centreId);
                    }
                }
            }
        }

        public EmailVerificationTransactionData? GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails(
            string email,
            string code
        )
        {
            var primaryEmailVerificationDetails = userDataService.GetPrimaryEmailVerificationDetails(code);
            var centreEmailVerificationDetails = userDataService.GetCentreEmailVerificationDetails(code);

            var allVerificationDetails = centreEmailVerificationDetails
                .Concat(new[] { primaryEmailVerificationDetails })
                .Where(details => details != null)
                .ToList();

            var usersMatchedByCode = allVerificationDetails.Select(d => d.UserId).Distinct().ToList();

            if (usersMatchedByCode.Count > 1)
            {
                throw new InvalidOperationException(
                    $"Email verification hash matches multiple users: {string.Join(", ", usersMatchedByCode)}"
                );
            }

            var unverifiedEmailDataMatchingEmail = allVerificationDetails
                .Where(details => details != null && details.Email == email && !details.IsEmailVerified)
                .ToList();

            if (!unverifiedEmailDataMatchingEmail.Any())
            {
                return null;
            }

            // We can assume the hash entity is the same for all the records as we searched by hash
            var hashCreationDate = unverifiedEmailDataMatchingEmail.First().EmailVerificationHashCreatedDate;

            return new EmailVerificationTransactionData(
                email,
                hashCreationDate,
                unverifiedEmailDataMatchingEmail
                    .FirstOrDefault(details => details.CentreIdIfEmailIsForUnapprovedDelegate != null)
                    ?.CentreIdIfEmailIsForUnapprovedDelegate,
                usersMatchedByCode.SingleOrDefault()
            );
        }

        public void SetEmailVerified(int userId, string email, DateTime verifiedDateTime)
        {
            userDataService.SetPrimaryEmailVerified(userId, email, verifiedDateTime);
            userDataService.SetCentreEmailVerified(userId, email, verifiedDateTime);
        }

        public bool EmailIsHeldAtCentre(string email, int centreId)
        {
            var inUseAsCentreEmailAtCentre = userDataService.CentreSpecificEmailIsInUseAtCentre(email!, centreId);

            var primaryEmailOwnerIsAtCentre = EmailIsHeldAsPrimaryEmailByUserAtCentre(email, centreId);

            return inUseAsCentreEmailAtCentre || primaryEmailOwnerIsAtCentre;
        }

        private bool EmailIsHeldAsPrimaryEmailByUserAtCentre(string email, int centreId)
        {
            var primaryEmailOwner = userDataService.GetUserAccountByPrimaryEmail(email);
            var primaryEmailOwnerIsAtCentre = primaryEmailOwner != null && userDataService
                .GetDelegateAccountsByUserId(primaryEmailOwner.Id).Any(da => da.CentreId == centreId);
            return primaryEmailOwnerIsAtCentre;
        }
        private bool NewUserRolesExceedAvailableSpots(
            int adminId,
            AdminRoles adminRoles
        )
        {
            var oldUserDetails = userDataService.GetAdminUserById(adminId)!;
            var currentNumberOfAdmins =
                centreContractAdminUsageService.GetCentreAdministratorNumbers(oldUserDetails.CentreId);

            if (adminRoles.IsTrainer && !oldUserDetails.IsTrainer && currentNumberOfAdmins.TrainersAtOrOverLimit)
            {
                return true;
            }

            if (adminRoles.IsContentCreator && !oldUserDetails.IsContentCreator &&
                currentNumberOfAdmins.CcLicencesAtOrOverLimit)
            {
                return true;
            }

            if (adminRoles.IsCmsAdministrator && !oldUserDetails.IsCmsAdministrator &&
                currentNumberOfAdmins.CmsAdministratorsAtOrOverLimit)
            {
                return true;
            }

            if (adminRoles.IsCmsManager && !oldUserDetails.IsCmsManager &&
                currentNumberOfAdmins.CmsManagersAtOrOverLimit)
            {
                return true;
            }

            return false;
        }

        public void ReactivateAdmin(int adminId)
        {
            userDataService.ReactivateAdmin(adminId);
            int? userId = userDataService.GetUserIdByAdminId(adminId);
            userDataService.ActivateUser(userId.GetValueOrDefault());
        }

        public UserEntity? GetDelegateUserFromLearningHubAuthId(int learningHubAuthId)
        {
            var userId = userDataService.GetUserIdFromLearningHubAuthId(learningHubAuthId);
            return userId == null ? null : GetUserById(userId.Value);
        }

        public bool CentreSpecificEmailIsInUseAtCentreByOtherUser(string email, int centreId, int userId)
        {
            return userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(email, centreId, userId);
        }
        public bool PrimaryEmailIsInUseByOtherUser(string email, int userId)
        {
            return userDataService.PrimaryEmailIsInUseByOtherUser(email, userId);
        }

        public IEnumerable<UserCentreDetails> GetCentreDetailsForUser(int userId)
        {
            return userDataService.GetCentreDetailsForUser(userId);
        }

        public bool PrimaryEmailIsInUse(string email)
        {
            return userDataService.PrimaryEmailIsInUse(email);
        }

        public void SetPrimaryEmailVerified(int userId, string email, DateTime verifiedDateTime)
        {
            userDataService.SetPrimaryEmailVerified(userId, email, verifiedDateTime);
        }

        public (int? userId, int? centreId, string? centreName) GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(string centreSpecificEmail, string registrationConfirmationHash)
        {
            return userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(centreSpecificEmail, registrationConfirmationHash);
        }

        public (IEnumerable<DelegateUserCard>, int) GetDelegateUserCards(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection, int centreId,
                                    string isActive, string isPasswordSet, string isAdmin, string isUnclaimed, string isEmailVerified, string registrationType, int jobGroupId,
                                    int? groupId, string answer1, string answer2, string answer3, string answer4, string answer5, string answer6)
        {
            return userDataService.GetDelegateUserCards(searchString, offSet, itemsPerPage, sortBy, sortDirection, centreId,
                                    isActive, isPasswordSet, isAdmin, isUnclaimed, isEmailVerified, registrationType, jobGroupId,
                                    groupId, answer1, answer2, answer3, answer4, answer5, answer6);
        }
        public DelegateUserCard? GetDelegateUserCardById(int id)
        {
            return userDataService.GetDelegateUserCardById(id);
        }
        public void DeactivateDelegateUser(int delegateId)
        {
            userDataService.DeactivateDelegateUser(delegateId);
        }
        public void ActivateDelegateUser(int delegateId)
        {
            userDataService.ActivateDelegateUser(delegateId);
        }
        public int GetUserIdFromDelegateId(int delegateId)
        {
            return userDataService.GetUserIdFromDelegateId(delegateId);
        }
        public void DeleteUserAndAccounts(int userId)
        {
            userDataService.DeleteUserAndAccounts(userId);
        }
        public bool PrimaryEmailInUseAtCentres(string email)
        {
            return userDataService.PrimaryEmailInUseAtCentres(email);
        }

        public bool CentreSpecificEmailIsInUseAtCentre(string email, int centreId)
        {
            return userDataService.CentreSpecificEmailIsInUseAtCentre(email, centreId);
        }

        public int? GetUserIdByAdminId(int adminId)
        {
            return userDataService.GetUserIdByAdminId(adminId);
        }

        public AdminUser? GetAdminUserByEmailAddress(string emailAddress)
        {
            return userDataService.GetAdminUserByEmailAddress(emailAddress);
        }

        public DelegateAccount? GetDelegateAccountById(int id)
        {
            return userDataService.GetDelegateAccountById(id);
        }

        public int? GetUserIdFromUsername(string username)
        {
            return userDataService.GetUserIdFromUsername(username);
        }

        public IEnumerable<DelegateAccount> GetDelegateAccountsByUserId(int userId)
        {
            return userDataService.GetDelegateAccountsByUserId(userId);
        }
        public void SetCentreEmail(
            int userId,
            int centreId,
            string? email,
            DateTime? emailVerified,
            IDbTransaction? transaction = null)
        {
            userDataService.SetCentreEmail(userId, centreId, email, emailVerified, transaction);
        }

        public int GetDelegateCountWithAnswerForPrompt(int centreId, int promptNumber)
        {
            return userDataService.GetDelegateCountWithAnswerForPrompt(centreId, promptNumber);
        }

        public List<AdminUser> GetAdminUsersByCentreId(int centreId)
        {
            return userDataService.GetAdminUsersByCentreId(centreId);
        }


        public AdminUser? GetAdminUserById(int id)
        {
            return userDataService.GetAdminUserById(id);

        }

        public string GetUserDisplayName(int userId)
        {
            return userDataService.GetUserDisplayName(userId);
        }


        public (IEnumerable<SuperAdminDelegateAccount>, int) GetAllDelegates(string search, int offset, int rows, int? delegateId, string accountStatus, string lhlinkStatus, int? centreId, int failedLoginThreshold)
        {
            return userDataService.GetAllDelegates(search, offset, rows, delegateId, accountStatus, lhlinkStatus, centreId, failedLoginThreshold);
        }

        public void DeleteUserCentreDetail(int userId, int centreId)
        {
            userDataService.DeleteUserCentreDetail(userId, centreId);
        }

        public void ApproveDelegateUsers(params int[] ids)
        {
            userDataService.ApproveDelegateUsers(ids);
        }

        public (IEnumerable<AdminEntity>, int) GetAllAdmins(string search, int offset, int rows, int? adminId, string userStatus, string role, int? centreId, int failedLoginThreshold)
        {
            return userDataService.GetAllAdmins(search, offset, rows, adminId, userStatus, role, centreId, failedLoginThreshold);
        }

        public void UpdateAdminUserAndSpecialPermissions(int adminId, bool isCentreAdmin, bool isSupervisor, bool isNominatedSupervisor, bool isTrainer, bool isContentCreator, bool isContentManager, bool importOnly, int? categoryId, bool isCentreManager, bool isSuperAdmin, bool isReportsViewer, bool isLocalWorkforceManager, bool isFrameworkDeveloper, bool isWorkforceManager)
        {
            userDataService.UpdateAdminUserAndSpecialPermissions(adminId, isCentreAdmin, isSupervisor, isNominatedSupervisor, isTrainer, isContentCreator, isContentManager, importOnly, categoryId, isCentreManager, isSuperAdmin, isReportsViewer, isLocalWorkforceManager, isFrameworkDeveloper, isWorkforceManager);
        }

        public int GetUserIdFromAdminId(int adminId)
        {
            return userDataService.GetUserIdFromAdminId(adminId);
        }

        public void DeleteAdminAccount(int adminId)
        {
            userDataService.DeleteAdminAccount(adminId);
        }

        public void UpdateAdminStatus(int adminId, bool active)
        {
            userDataService.UpdateAdminStatus(adminId, active);
        }

        public void UpdateAdminCentre(int adminId, int centreId)
        {
            userDataService.UpdateAdminCentre(adminId, centreId);
        }

        public bool IsUserAlreadyAdminAtCentre(int? userId, int centreId)
        {
            return userDataService.IsUserAlreadyAdminAtCentre(userId, centreId);
        }

        public IEnumerable<AdminEntity> GetAdminsByCentreId(int centreId)
        {
            return userDataService.GetAdminsByCentreId(centreId);
        }

        public void DeactivateAdmin(int adminId)
        {
            userDataService.DeactivateAdmin(adminId);
        }

        public void ActivateUser(int userId)
        {
            userDataService.ActivateUser(userId);
        }
        public void InactivateUser(int userId)
        {
            userDataService.InactivateUser(userId);
        }
        public (IEnumerable<UserAccountEntity>, int recordCount) GetUserAccounts(string search, int offset, int rows, int jobGroupId, string userStatus, string emailStatus, int userId, int failedLoginThreshold)
        {
            return userDataService.GetUserAccounts(search, offset, rows, jobGroupId, userStatus, emailStatus, userId, failedLoginThreshold);
        }
        public void UpdateUserDetailsAccount(string firstName, string lastName, string primaryEmail, int jobGroupId, string? prnNumber, DateTime? emailVerified, int userId)
        {
            userDataService.UpdateUserDetailsAccount(firstName, lastName, primaryEmail, jobGroupId, prnNumber, emailVerified, userId);
        }
        public void DeactivateAdminAccount(int userId, int centreId)
        {
            userDataService.DeactivateAdminAccount(userId, centreId);
        }
        public int? CheckDelegateIsActive(int delegateId)
        {
            return userDataService.CheckDelegateIsActive(delegateId);
        }
    }
}
