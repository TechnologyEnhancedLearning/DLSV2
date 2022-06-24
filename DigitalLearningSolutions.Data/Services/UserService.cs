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

    public interface IUserService
    {
        (AdminUser? adminUser, DelegateUser? delegateUser) GetUsersById(int? adminId, int? delegateId);

        DelegateEntity? GetDelegateById(int id);

        DelegateUser? GetDelegateUserById(int delegateId);

        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress);

        List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId);

        void UpdateUserDetailsAndCentreSpecificDetails(
            EditAccountDetailsData editAccountDetailsData,
            DelegateDetailsData? delegateDetailsData,
            string? centreEmail,
            int centreId,
            bool changeMadeBySameUser
        );

        bool NewEmailAddressIsValid(string emailAddress, int userId);

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

        (string? primaryEmail, IEnumerable<(string centreName, string centreEmail)> centreEmails)
            GetUnverifiedEmailsForUser(int userId);

        AdminEntity? GetAdminById(int adminId);
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

        public UserService(
            IUserDataService userDataService,
            IGroupsService groupsService,
            ICentreContractAdminUsageService centreContractAdminUsageService,
            ISessionDataService sessionDataService,
            ILogger<IUserService> logger,
            IClockService clockService,
            IConfiguration configuration
        )
        {
            this.userDataService = userDataService;
            this.groupsService = groupsService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
            this.sessionDataService = sessionDataService;
            this.logger = logger;
            this.clockService = clockService;
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

        public bool NewEmailAddressIsValid(string emailAddress, int userId)
        {
            return !userDataService.EmailIsInUseByOtherUser(userId, emailAddress);
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

        public (string? primaryEmail, IEnumerable<(string centreName, string centreEmail)> centreEmails)
            GetUnverifiedEmailsForUser(int userId)
        {
            var userEntity = GetUserById(userId);

            if (userEntity == null)
            {
                return (null, new List<(string centreName, string centreEmail)>());
            }

            var unverifiedPrimaryEmail = userEntity.UserAccount.EmailVerified == null
                ? userEntity.UserAccount.PrimaryEmail
                : null;

            var unverifiedCentreEmails = userDataService.GetUnverifiedCentreEmailsForUser(userId).Where(
                tuple =>
                    userEntity.AdminAccounts.Any(account => account.CentreId == tuple.centreId && account.Active) ||
                    userEntity.DelegateAccounts.Any(account => account.CentreId == tuple.centreId && account.Active)
            ).Select(tuple => (tuple.centreName, tuple.centreEmail));

            return (unverifiedPrimaryEmail, unverifiedCentreEmails);
        }

        public AdminEntity? GetAdminById(int adminId)
        {
            return userDataService.GetAdminById(adminId);
        }

        public void UpdateUserDetailsAndCentreSpecificDetails(
            EditAccountDetailsData editAccountDetailsData,
            DelegateDetailsData? delegateDetailsData,
            string? centreEmail,
            int centreId,
            bool changeMadeBySameUser
        )
        {
            var detailsLastChecked = clockService.UtcNow;

            if (delegateDetailsData != null)
            {
                groupsService.SynchroniseUserChangesWithGroups(
                    delegateDetailsData.DelegateId,
                    editAccountDetailsData,
                    new RegistrationFieldAnswers(delegateDetailsData, editAccountDetailsData.JobGroupId)
                );

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
            }

            userDataService.UpdateUser(
                editAccountDetailsData.FirstName,
                editAccountDetailsData.Surname,
                editAccountDetailsData.Email,
                editAccountDetailsData.ProfileImage,
                editAccountDetailsData.ProfessionalRegistrationNumber,
                editAccountDetailsData.HasBeenPromptedForPrn,
                editAccountDetailsData.JobGroupId,
                detailsLastChecked,
                editAccountDetailsData.UserId,
                changeMadeBySameUser
            );

            userDataService.SetCentreEmail(
                editAccountDetailsData.UserId,
                centreId,
                centreEmail
            );
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
