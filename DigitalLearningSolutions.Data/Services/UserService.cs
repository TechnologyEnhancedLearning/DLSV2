namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Logging;

    public interface IUserService
    {
        (AdminUser?, List<DelegateUser>) GetUsersByUsername(string username);

        (AdminUser? adminUser, List<DelegateUser> delegateUsers) GetUsersByEmailAddress(string emailAddress);

        (AdminUser? adminUser, DelegateUser? delegateUser) GetUsersById(int? adminId, int? delegateId);

        DelegateUser? GetDelegateUserById(int delegateId);

        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress);

        List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId);

        (AdminUser?, List<DelegateUser>) GetUsersWithActiveCentres(
            AdminUser? adminUser,
            List<DelegateUser> delegateUsers
        );

        void UpdateUserAccountDetailsForAllUsers(
            MyAccountDetailsData myAccountDetailsData,
            CentreAnswersData? centreAnswersData = null
        );

        bool NewEmailAddressIsValid(string emailAddress, int? adminUserId, int? delegateUserId, int centreId);

        UserAccountSet GetVerifiedLinkedUsersAccounts(int? adminId, int? delegateId, string password);

        bool IsPasswordValid(int? adminId, int? delegateId, string password);

        bool IsDelegateEmailValidForCentre(string email, int centreId);

        void ResetFailedLoginCount(UserAccount userAccount);

        void UpdateFailedLoginCount(UserAccount userAccount);

        public IEnumerable<DelegateUserCard> GetDelegateUserCardsForWelcomeEmail(int centreId);

        void UpdateAdminUserPermissions(
            int adminId,
            AdminRoles adminRoles,
            int categoryId
        );

        void UpdateUserAccountDetailsViaDelegateAccount(
            EditDelegateDetailsData editDelegateDetailsData,
            CentreAnswersData centreAnswersData
        );

        IEnumerable<AdminUser> GetSupervisorsAtCentre(int centreId);

        IEnumerable<AdminUser> GetSupervisorsAtCentreForCategory(int centreId, int categoryId);

        bool DelegateUserLearningHubAccountIsLinked(int delegateId);

        int? GetDelegateUserLearningHubAuthId(int delegateId);

        void UpdateDelegateLhLoginWarningDismissalStatus(int delegateId, bool status);

        void DeactivateOrDeleteAdmin(int adminId);

        UserEntity? GetUserById(int userId);

        UserEntity? GetUserByUsername(string username);
    }

    public class UserService : IUserService
    {
        private readonly ICentreContractAdminUsageService centreContractAdminUsageService;
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
            ILogger<IUserService> logger
        )
        {
            this.userDataService = userDataService;
            this.groupsService = groupsService;
            this.userVerificationService = userVerificationService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
            this.sessionDataService = sessionDataService;
            this.logger = logger;
        }

        public (AdminUser?, List<DelegateUser>) GetUsersByUsername(string username)
        {
            // TODO HEEDLS-887 Check this is the correct behaviour for getting users. This may end up getting cleared up by login fixes
            var adminUser = userDataService.GetAdminUserByEmailAddress(username);
            if (adminUser != null && (!adminUser.Active || !adminUser.Approved))
            {
                adminUser = null;
            }

            var delegateUsers = userDataService.GetDelegateUsersByUsername(username);

            return (adminUser, delegateUsers);
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
            return userDataService.GetDelegateUsersByEmailAddress(emailAddress);
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

        // TODO HEEDLS-887 Make sure this logic is correct with the new account structure
        public void UpdateUserAccountDetailsForAllUsers(
            MyAccountDetailsData myAccountDetailsData,
            CentreAnswersData? centreAnswersData = null
        )
        {
            var (adminUser, delegateUsers) =
                GetLinkedUsersAccounts(
                    myAccountDetailsData.AdminId,
                    myAccountDetailsData.DelegateId
                );

            var userId = adminUser?.Id ?? delegateUsers[0].Id;
            var detailsLastChecked = DateTime.Now;
            userDataService.UpdateUserDetailsLastChecked(detailsLastChecked, userId);

            if (adminUser != null)
            {
                userDataService.UpdateAdminUser(
                    myAccountDetailsData.FirstName,
                    myAccountDetailsData.Surname,
                    myAccountDetailsData.Email,
                    myAccountDetailsData.ProfileImage,
                    adminUser.Id
                );
            }

            if (delegateUsers.Count != 0)
            {
                userDataService.UpdateDelegateAccountCentreSpecificDetailsLastChecked(detailsLastChecked, userId);
                userDataService.UpdateUser(
                    myAccountDetailsData.FirstName,
                    myAccountDetailsData.Surname,
                    myAccountDetailsData.Email,
                    myAccountDetailsData.ProfileImage,
                    myAccountDetailsData.ProfessionalRegistrationNumber,
                    myAccountDetailsData.HasBeenPromptedForPrn,
                    centreAnswersData.JobGroupId,
                    userId
                );

                var oldDelegateDetails =
                    delegateUsers.SingleOrDefault(u => u.Id == myAccountDetailsData.DelegateId);

                if (oldDelegateDetails != null && centreAnswersData != null)
                {
                    userDataService.UpdateDelegateUserCentrePrompts(
                        myAccountDetailsData.DelegateId!.Value,
                        centreAnswersData.Answer1,
                        centreAnswersData.Answer2,
                        centreAnswersData.Answer3,
                        centreAnswersData.Answer4,
                        centreAnswersData.Answer5,
                        centreAnswersData.Answer6
                    );

                    groupsService.SynchroniseUserChangesWithGroups(
                        oldDelegateDetails,
                        myAccountDetailsData,
                        centreAnswersData
                    );
                }
            }
        }

        public bool NewEmailAddressIsValid(string emailAddress, int? adminUserId, int? delegateUserId, int centreId)
        {
            var (adminUser, delegateUser) = GetUsersById(adminUserId, delegateUserId);
            if (!UserEmailHasChanged(adminUser, emailAddress) && !UserEmailHasChanged(delegateUser, emailAddress))
            {
                return true;
            }

            var (adminUserWithNewEmail, delegateUsersWithNewEmail) = GetUsersByEmailAddress(emailAddress);

            return (adminUserWithNewEmail == null || adminUserId == adminUserWithNewEmail.Id) &&
                   delegateUsersWithNewEmail.Count(u => u.Id != delegateUserId) == 0;
        }

        public UserAccountSet GetVerifiedLinkedUsersAccounts(
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

        private UserAccountSet GetLinkedUsersAccounts(int? adminId, int? delegateId)
        {
            var (loggedInAdminUser, loggedInDelegateUser) = GetUsersById(adminId, delegateId);

            var signedInEmailIfAny = loggedInAdminUser?.EmailAddress ?? loggedInDelegateUser?.EmailAddress;

            if (string.IsNullOrWhiteSpace(signedInEmailIfAny))
            {
                var loggedInDelegateUsers = loggedInDelegateUser != null
                    ? new List<DelegateUser> { loggedInDelegateUser }
                    : new List<DelegateUser>();

                return new UserAccountSet(loggedInAdminUser, loggedInDelegateUsers);
            }

            var (adminUser, delegateUsers) = GetUsersByEmailAddress(signedInEmailIfAny);

            return new UserAccountSet(adminUser, delegateUsers);
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

        public void ResetFailedLoginCount(UserAccount userAccount)
        {
            if (userAccount.FailedLoginCount != 0)
            {
                userDataService.UpdateUserFailedLoginCount(userAccount.Id, 0);
            }
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
            int categoryId
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

        // TODO HEEDLS-887 Make sure this logic is correct with the new account structure
        public void UpdateUserAccountDetailsViaDelegateAccount(
            EditDelegateDetailsData editDelegateDetailsData,
            CentreAnswersData centreAnswersData
        )
        {
            var delegateUser = userDataService.GetDelegateUserById(editDelegateDetailsData.DelegateId);
            var (adminUser, delegateUsers) = GetUsersByEmailAddress(delegateUser!.EmailAddress);

            if (adminUser != null)
            {
                userDataService.UpdateAdminUser(
                    editDelegateDetailsData.FirstName,
                    editDelegateDetailsData.Surname,
                    editDelegateDetailsData.Email,
                    adminUser.ProfileImage,
                    adminUser.Id
                );
            }

            userDataService.UpdateUserDetails(
                editDelegateDetailsData.FirstName,
                editDelegateDetailsData.Surname,
                editDelegateDetailsData.Email,
                centreAnswersData.JobGroupId,
                1 // TODO HEEDLS-887 This needs correcting to the correct UserId for the delegate record.
            );

            userDataService.UpdateDelegateAccount(
                editDelegateDetailsData.DelegateId,
                delegateUser.Active,
                centreAnswersData.Answer1,
                centreAnswersData.Answer2,
                centreAnswersData.Answer3,
                centreAnswersData.Answer4,
                centreAnswersData.Answer5,
                centreAnswersData.Answer6
            );

            userDataService.UpdateDelegateProfessionalRegistrationNumber(
                delegateUser.Id,
                editDelegateDetailsData.ProfessionalRegistrationNumber,
                editDelegateDetailsData.HasBeenPromptedForPrn
            );

            groupsService.SynchroniseUserChangesWithGroups(
                delegateUser,
                editDelegateDetailsData,
                centreAnswersData
            );
        }

        public IEnumerable<AdminUser> GetSupervisorsAtCentre(int centreId)
        {
            return userDataService.GetAdminUsersByCentreId(centreId).Where(au => au.IsSupervisor);
        }

        public IEnumerable<AdminUser> GetSupervisorsAtCentreForCategory(int centreId, int categoryId)
        {
            return userDataService.GetAdminUsersByCentreId(centreId).Where(au => au.IsSupervisor)
                .Where(au => au.CategoryId == categoryId || au.CategoryId == 0);
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

            if (!adminAccounts.Any() && !delegateAccounts.Any())
            {
                throw new UserAccountNotFoundException(
                    "No AdminAccounts or DelegateAccounts link to User with ID: " + userId
                );
            }

            return new UserEntity(userAccount, adminAccounts, delegateAccounts);
        }

        public UserEntity? GetUserByUsername(string username)
        {
            var userId = userDataService.GetUserIdFromUsername(username);

            return userId == null ? null : GetUserById(userId.Value);
        }

        public DelegateUser? GetDelegateUserById(int delegateId)
        {
            return userDataService.GetDelegateUserById(delegateId);
        }

        private static bool UserEmailHasChanged(User? user, string emailAddress)
        {
            return user != null && !emailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase);
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
