namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserService
    {
        (AdminUser?, List<DelegateUser>) GetUsersByUsername(string username);

        (AdminUser? adminUser, List<DelegateUser> delegateUsers) GetUsersByEmailAddress(string emailAddress);

        (AdminUser? adminUser, DelegateUser? delegateUser) GetUsersById(int? adminId, int? delegateId);

        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress);

        (AdminUser?, List<DelegateUser>) GetUsersWithActiveCentres(
            AdminUser? adminUser,
            List<DelegateUser> delegateUsers
        );

        List<CentreUserDetails> GetUserCentres(AdminUser? adminUser, List<DelegateUser> delegateUsers);

        void UpdateUserAccountDetailsForAllVerifiedUsers(
            MyAccountDetailsData myAccountDetailsData,
            CentreAnswersData? centreAnswersData = null
        );

        bool NewEmailAddressIsValid(string emailAddress, int? adminUserId, int? delegateUserId, int centreId);

        UserAccountSet GetVerifiedLinkedUsersAccounts(int? adminId, int? delegateId, string password);

        bool IsPasswordValid(int? adminId, int? delegateId, string password);

        bool IsDelegateEmailValidForCentre(string email, int centreId);

        void ResetFailedLoginCount(AdminUser adminUser);

        void IncrementFailedLoginCount(AdminUser adminUser);

        public IEnumerable<DelegateUserCard> GetDelegateUserCardsForWelcomeEmail(int centreId);

        void UpdateAdminUserPermissions(
            int adminId,
            AdminRoles adminRoles,
            int categoryId
        );

        bool NewAliasIsValid(string? aliasId, int delegateUserId, int centreId);

        void UpdateUserAccountDetailsViaDelegateAccount(
            EditDelegateDetailsData editDelegateDetailsData,
            CentreAnswersData centreAnswersData
        );

        IEnumerable<AdminUser> GetSupervisorsAtCentre(int centreId);

        IEnumerable<AdminUser> GetSupervisorsAtCentreForCategory(int centreId, int categoryId);
    }

    public class UserService : IUserService
    {
        private readonly ICentreContractAdminUsageService centreContractAdminUsageService;
        private readonly IGroupsService groupsService;
        private readonly IUserDataService userDataService;
        private readonly IUserVerificationService userVerificationService;

        public UserService(
            IUserDataService userDataService,
            IGroupsService groupsService,
            IUserVerificationService userVerificationService,
            ICentreContractAdminUsageService centreContractAdminUsageService
        )
        {
            this.userDataService = userDataService;
            this.groupsService = groupsService;
            this.userVerificationService = userVerificationService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
        }

        public (AdminUser?, List<DelegateUser>) GetUsersByUsername(string username)
        {
            var adminUser = userDataService.GetAdminUserByUsername(username);
            List<DelegateUser> delegateUsers = userDataService.GetDelegateUsersByUsername(username);

            return (adminUser, delegateUsers);
        }

        public (AdminUser? adminUser, List<DelegateUser> delegateUsers) GetUsersByEmailAddress(string emailAddress)
        {
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

        public (AdminUser?, List<DelegateUser>) GetUsersWithActiveCentres(
            AdminUser? adminUser,
            List<DelegateUser> delegateUsers
        )
        {
            var adminUserWithActiveCentre = adminUser?.CentreActive == true ? adminUser : null;
            var delegateUsersWithActiveCentres = delegateUsers.Where(du => du.CentreActive).ToList();
            return (adminUserWithActiveCentre, delegateUsersWithActiveCentres);
        }

        public List<CentreUserDetails> GetUserCentres(AdminUser? adminUser, List<DelegateUser> delegateUsers)
        {
            var availableCentres = delegateUsers
                .Select(
                    du =>
                        new CentreUserDetails(du.CentreId, du.CentreName, adminUser?.CentreId == du.CentreId, true)
                )
                .ToList();

            if (adminUser != null && availableCentres.All(c => c.CentreId != adminUser.CentreId))
            {
                availableCentres.Add(new CentreUserDetails(adminUser.CentreId, adminUser.CentreName, true));
            }

            return availableCentres.OrderByDescending(ac => ac.IsAdmin).ThenBy(ac => ac.CentreName).ToList();
        }

        public void UpdateUserAccountDetailsForAllVerifiedUsers(
            MyAccountDetailsData myAccountDetailsData,
            CentreAnswersData? centreAnswersData = null
        )
        {
            var (verifiedAdminUser, verifiedDelegateUsers) =
                GetVerifiedLinkedUsersAccounts(
                    myAccountDetailsData.AdminId,
                    myAccountDetailsData.DelegateId,
                    myAccountDetailsData.Password
                );

            if (verifiedAdminUser != null)
            {
                userDataService.UpdateAdminUser(
                    myAccountDetailsData.FirstName,
                    myAccountDetailsData.Surname,
                    myAccountDetailsData.Email,
                    myAccountDetailsData.ProfileImage,
                    verifiedAdminUser.Id
                );
            }

            if (verifiedDelegateUsers.Count != 0)
            {
                var delegateIds = verifiedDelegateUsers.Select(d => d.Id).ToArray();
                userDataService.UpdateDelegateUsers(
                    myAccountDetailsData.FirstName,
                    myAccountDetailsData.Surname,
                    myAccountDetailsData.Email,
                    myAccountDetailsData.ProfileImage,
                    myAccountDetailsData.ProfessionRegistrationNumber,
                    myAccountDetailsData.HasBeenPromptedForPrn,
                    delegateIds
                );

                var oldDelegateDetails =
                    verifiedDelegateUsers.SingleOrDefault(u => u.Id == myAccountDetailsData.DelegateId);

                if (oldDelegateDetails != null && centreAnswersData != null)
                {
                    userDataService.UpdateDelegateUserCentrePrompts(
                        myAccountDetailsData.DelegateId!.Value,
                        centreAnswersData.JobGroupId,
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

            var (adminUsersWithNewEmail, delegateUsersWithNewEmail) = GetUsersByEmailAddress(emailAddress);

            return adminUsersWithNewEmail == null && delegateUsersWithNewEmail.Count(u => u.CentreId == centreId) == 0;
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

        public void ResetFailedLoginCount(AdminUser adminUser)
        {
            if (adminUser.FailedLoginCount != 0)
            {
                userDataService.UpdateAdminUserFailedLoginCount(adminUser.Id, 0);
            }
        }

        public void IncrementFailedLoginCount(AdminUser adminUser)
        {
            userDataService.UpdateAdminUserFailedLoginCount(adminUser.Id, adminUser.FailedLoginCount + 1);
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
                adminRoles.IsTrainer,
                adminRoles.IsContentCreator,
                adminRoles.IsContentManager,
                adminRoles.ImportOnly,
                categoryId
            );
        }

        public bool NewAliasIsValid(string? aliasId, int delegateUserId, int centreId)
        {
            if (aliasId == null)
            {
                return true;
            }

            var delegateUsers = userDataService.GetDelegateUsersByAliasId(aliasId);
            return !delegateUsers.Any(du => du.Id != delegateUserId && du.CentreId == centreId);
        }

        public void UpdateUserAccountDetailsViaDelegateAccount(
            EditDelegateDetailsData editDelegateDetailsData,
            CentreAnswersData centreAnswersData
        )
        {
            var delegateUser = userDataService.GetDelegateUserById(editDelegateDetailsData.DelegateId);
            var (adminUser, delegateUsers) = GetUsersByEmailAddress(delegateUser!.EmailAddress!);

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

            var delegateIds = delegateUsers.Select(d => d.Id).ToArray();
            userDataService.UpdateDelegateAccountDetails(
                editDelegateDetailsData.FirstName,
                editDelegateDetailsData.Surname,
                editDelegateDetailsData.Email,
                delegateIds
            );

            userDataService.UpdateDelegate(
                editDelegateDetailsData.DelegateId,
                editDelegateDetailsData.FirstName,
                editDelegateDetailsData.Surname,
                centreAnswersData.JobGroupId,
                delegateUser.Active,
                centreAnswersData.Answer1,
                centreAnswersData.Answer2,
                centreAnswersData.Answer3,
                centreAnswersData.Answer4,
                centreAnswersData.Answer5,
                centreAnswersData.Answer6,
                editDelegateDetailsData.Alias,
                editDelegateDetailsData.Email
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
