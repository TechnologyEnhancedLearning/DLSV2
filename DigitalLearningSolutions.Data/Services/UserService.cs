namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Delegate = DigitalLearningSolutions.Data.Models.User.Delegate;

    public interface IUserService
    {
        (AdminUser? adminUser, List<DelegateUser> delegateUsers) GetUsersByEmailAddress(string emailAddress);

        (AdminUser? adminUser, DelegateUser? delegateUser) GetUsersById(int? adminId, int? delegateId);

        Delegate? GetDelegateById(int id);

        DelegateUser? GetDelegateUserById(int delegateId);

        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress);

        List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId);

        (AdminUser?, List<DelegateUser>) GetUsersWithActiveCentres(
            AdminUser? adminUser,
            List<DelegateUser> delegateUsers
        );

        void UpdateUserDetailsAndCentreSpecificDetails(
            MyAccountDetailsData myAccountDetailsData,
            DelegateDetailsData? delegateDetailsData,
            string? centreEmail,
            int centreId
        );

        bool NewEmailAddressIsValid(string emailAddress, int userId);

        bool IsPasswordValid(int? adminId, int? delegateId, string password);

        bool IsDelegateEmailValidForCentre(string email, int centreId);

        bool EmailIsInUse(string email);

        void ResetFailedLoginCount(UserAccount userAccount);

        void ResetFailedLoginCountByUserId(int userId);

        void UpdateFailedLoginCount(UserAccount userAccount);

        public IEnumerable<DelegateUserCard> GetDelegateUserCardsForWelcomeEmail(int centreId);

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

        UserAccount? GetUserByEmailAddress(string emailAddress);

        string? GetCentreEmail(int userId, int centreId);

        bool ShouldForceDetailsCheck(UserEntity userEntity, int centreIdToCheck);
    }

    public class UserService : IUserService
    {
        private readonly ICentreContractAdminUsageService centreContractAdminUsageService;
        private readonly IClockService clockService;
        private readonly IConfiguration configuration;
        private readonly IGroupsService groupsService;
        private readonly ILogger<IUserService> logger;
        private readonly ISessionDataService sessionDataService;
        private readonly IUserDataService userDataService;
        private readonly IUserVerificationService userVerificationService;

        public UserService(
            IUserDataService userDataService,
            IGroupsService groupsService,
            IUserVerificationService userVerificationService,
            ICentreContractAdminUsageService centreContractAdminUsageService,
            ISessionDataService sessionDataService,
            ILogger<IUserService> logger,
            IClockService clockService,
            IConfiguration configuration
        )
        {
            this.userDataService = userDataService;
            this.groupsService = groupsService;
            this.userVerificationService = userVerificationService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
            this.sessionDataService = sessionDataService;
            this.logger = logger;
            this.clockService = clockService;
            this.configuration = configuration;
        }

        public (AdminUser? adminUser, List<DelegateUser> delegateUsers) GetUsersByEmailAddress(string? emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return (null, new List<DelegateUser>());
            }

            var adminUser = userDataService.GetAdminUserByEmailAddress(emailAddress);
            var delegateUsers = userDataService.GetDelegateUsersByEmailAddress(emailAddress);

            return (adminUser, delegateUsers);
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

        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress)
        {
            return string.IsNullOrWhiteSpace(emailAddress)
                ? new List<DelegateUser>()
                : userDataService.GetDelegateUsersByEmailAddress(emailAddress);
        }

        public List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId)
        {
            return userDataService.GetDelegatesNotRegisteredForGroupByGroupId(groupId, centreId);
        }

        public (AdminUser?, List<DelegateUser>) GetUsersWithActiveCentres(
            AdminUser? adminUser,
            List<DelegateUser> delegateUsers
        )
        {
            var adminUserWithActiveCentre = adminUser?.CentreActive == true ? adminUser : null;
            var delegateUsersWithActiveCentres = delegateUsers.Where(du => du.CentreActive).ToList();
            return (adminUserWithActiveCentre, delegateUsersWithActiveCentres);
        }

        public void UpdateUserDetailsAndCentreSpecificDetails(
            MyAccountDetailsData myAccountDetailsData,
            DelegateDetailsData? delegateDetailsData,
            string? centreEmail,
            int centreId
        )
        {
            var detailsLastChecked = clockService.UtcNow;

            userDataService.UpdateUser(
                myAccountDetailsData.FirstName,
                myAccountDetailsData.Surname,
                myAccountDetailsData.Email,
                myAccountDetailsData.ProfileImage,
                myAccountDetailsData.ProfessionalRegistrationNumber,
                myAccountDetailsData.HasBeenPromptedForPrn,
                myAccountDetailsData.JobGroupId,
                detailsLastChecked,
                myAccountDetailsData.UserId
            );

            userDataService.SetCentreEmail(
                myAccountDetailsData.UserId,
                centreId,
                centreEmail
            );

            if (delegateDetailsData != null)
            {
                userDataService.UpdateDelegateUserCentrePrompts(
                    delegateDetailsData.DelegateId,
                    delegateDetailsData.Answer1,
                    delegateDetailsData.Answer2,
                    delegateDetailsData.Answer3,
                    delegateDetailsData.Answer4,
                    delegateDetailsData.Answer5,
                    delegateDetailsData.Answer6,
                    detailsLastChecked
                );

                groupsService.SynchroniseUserChangesWithGroups(
                    delegateDetailsData.DelegateId,
                    myAccountDetailsData,
                    new RegistrationFieldAnswers(delegateDetailsData, myAccountDetailsData.JobGroupId)
                );
            }
        }

        public bool NewEmailAddressIsValid(string emailAddress, int userId)
        {
            return !userDataService.EmailIsInUseByOtherUser(userId, emailAddress);
        }

        public bool IsPasswordValid(int? adminId, int? delegateId, string password)
        {
            var verifiedLinkedUsersAccounts = GetVerifiedLinkedUsersAccounts(adminId, delegateId, password);

            return verifiedLinkedUsersAccounts.Any();
        }

        public bool IsDelegateEmailValidForCentre(string email, int centreId)
        {
            var duplicateUsers = userDataService.GetDelegateUsersByEmailAddress(email)
                .Where(u => u.CentreId == centreId);

            return !duplicateUsers.Any();
        }

        public bool EmailIsInUse(string email)
        {
            return userDataService.AnyEmailsInSetAreAlreadyInUse(new[] { email });
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
                categoryId
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

        public UserAccount? GetUserByEmailAddress(string emailAddress)
        {
            return userDataService.GetUserAccountByEmailAddress(emailAddress);
        }

        public Delegate? GetDelegateById(int id)
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
            var now = clockService.UtcNow;
            var monthThresholdToForceCheck = configuration.GetMonthsToPromptUserDetailsCheck();

            if (userEntity.UserAccount.DetailsLastChecked == null ||
                userEntity.UserAccount.DetailsLastChecked.Value.AddMonths(monthThresholdToForceCheck) < now)
            {
                return true;
            }

            return delegateAccount != null &&
                   (delegateAccount.CentreSpecificDetailsLastChecked == null ||
                    delegateAccount.CentreSpecificDetailsLastChecked.Value.AddMonths(monthThresholdToForceCheck) < now);
        }

        public string? GetCentreEmail(int userId, int centreId)
        {
            return userDataService.GetCentreEmail(userId, centreId);
        }

        private UserAccountSet GetVerifiedLinkedUsersAccounts(
            int? adminId,
            int? delegateId,
            string password
        )
        {
            var (loggedInAdminUser, loggedInDelegateUser) = GetUsersById(adminId, delegateId);

            var signedInEmailIfAny = loggedInAdminUser?.EmailAddress ?? loggedInDelegateUser?.EmailAddress;

            if (string.IsNullOrWhiteSpace(signedInEmailIfAny))
            {
                var loggedInDelegateUsers = loggedInDelegateUser != null
                    ? new List<DelegateUser> { loggedInDelegateUser }
                    : new List<DelegateUser>();

                return userVerificationService.VerifyUsers(password, loggedInAdminUser, loggedInDelegateUsers);
            }

            var (adminUser, delegateUsers) = GetUsersByEmailAddress(signedInEmailIfAny);

            return userVerificationService.VerifyUsers(password, adminUser, delegateUsers);
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
