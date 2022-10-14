namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;

    public interface IUserService
    {
        (AdminUser? adminUser, DelegateUser? delegateUser) GetUsersById(int? adminId, int? delegateId);

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

        UserEntity? GetUserById(int userId);

        UserEntity? GetUserByUsername(string username);

        public UserAccount? GetUserAccountById(int userId);

        UserAccount? GetUserAccountByEmailAddress(string emailAddress);

        string? GetCentreEmail(int userId, int centreId);

        IEnumerable<(int centreId, string centreName, string? centreSpecificEmail)> GetAllActiveCentreEmailsForUser(
            int userId
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

        public (AdminUser?, DelegateUser?) GetUsersById(int? userAdminId, int? userDelegateId)
        {
            AdminUser? adminUser = null;

            if (userAdminId != null)
            {
                adminUser = userDataService.GetAdminUserById(userAdminId.Value);
            }

            DelegateUser? delegateUser = null;

            if (userDelegateId != null)
            {
                delegateUser = userDataService.GetDelegateUserById(userDelegateId.Value);
            }

            return (adminUser, delegateUser);
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
                user => user.Approved && !user.SelfReg && string.IsNullOrEmpty(user.Password) &&
                        !string.IsNullOrEmpty(user.EmailAddress)
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
                (int)categoryId,
                adminRoles.IsCentreManager
            );
        }

        public IEnumerable<AdminUser> GetSupervisorsAtCentre(int centreId)
        {
            return userDataService.GetAdminUsersByCentreId(centreId).Where(au => au.IsSupervisor);
        }

        public IEnumerable<AdminUser> GetSupervisorsAtCentreForCategory(int centreId, int categoryId)
        {
            return userDataService.GetAdminUsersByCentreId(centreId).Where(au => au.IsSupervisor)
                .Where(au => au.CategoryId == categoryId || au.CategoryId == null);
        }

        public bool DelegateUserLearningHubAccountIsLinked(int delegateId)
        {
            return userDataService.GetDelegateUserLearningHubAuthId(delegateId).HasValue;
        }

        public int? GetDelegateUserLearningHubAuthId(int delegateId)
        {
            return userDataService.GetDelegateUserLearningHubAuthId(delegateId);
        }

        public void UpdateDelegateLhLoginWarningDismissalStatus(int delegateId, bool status)
        {
            userDataService.UpdateDelegateLhLoginWarningDismissalStatus(delegateId, status);
        }

        public void DeactivateOrDeleteAdmin(int adminId)
        {
            if (sessionDataService.HasAdminGotSessions(adminId))
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
            GetAllActiveCentreEmailsForUser(int userId)
        {
            return userDataService.GetAllActiveCentreEmailsForUser(userId);
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
                    var emailVerified = emailVerificationDataService.AccountEmailIsVerifiedForUser(userId, email)
                        ? currentTime
                        : (DateTime?)null;

                    userDataService.SetCentreEmail(userId, centreId, email, emailVerified);
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
    }
}
