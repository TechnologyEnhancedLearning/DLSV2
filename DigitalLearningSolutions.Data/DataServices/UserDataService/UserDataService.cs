namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserDataService
    {
        AdminUser? GetAdminUserById(int id);
        List<AdminUser> GetAdminUsersByCentreId(int centreId);

        /// <summary>
        ///     Gets a single admin or null by Login or Email Address
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     Thrown in the case where 2 admins are found in the database.
        ///     This should not occur as Login is not an editable column.
        /// </exception>
        AdminUser? GetAdminUserByUsername(string username);
        AdminUser? GetAdminUserByEmailAddress(string emailAddress);
        int GetNumberOfActiveAdminsAtCentre(int centreId);
        void UpdateAdminUser(string firstName, string surname, string email, byte[]? profileImage, int id);

        void UpdateAdminUserPermissions(
            int adminId,
            bool isCentreAdmin,
            bool isSupervisor,
            bool isTrainer,
            bool isContentCreator,
            bool isContentManager,
            bool importOnly,
            int categoryId
        );

        void UpdateAdminUserFailedLoginCount(int adminId, int updatedCount);

        DelegateUser? GetDelegateUserById(int id);
        List<DelegateUser> GetDelegateUsersByUsername(string username);
        List<DelegateUser> GetAllDelegateUsersByUsername(string username);
        List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress);
        List<DelegateUser> GetUnapprovedDelegateUsersByCentreId(int centreId);

        void UpdateDelegateUsers(
            string firstName,
            string surname,
            string email,
            byte[]? profileImage,
            int[] ids
        );

        void UpdateDelegate(
            int delegateId,
            string firstName,
            string lastName,
            int jobGroupId,
            bool active,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6,
            string? aliasId,
            string emailAddress
        );

        void ApproveDelegateUsers(params int[] ids);
        void RemoveDelegateUser(int delegateId);
        int GetNumberOfApprovedDelegatesAtCentre(int centreId);
        DelegateUser? GetDelegateUserByAliasId(string aliasId, int centreId);
        DelegateUser? GetDelegateUserByCandidateNumber(string candidateNumber, int centreId);
        void DeactivateDelegateUser(int delegateId);
        IEnumerable<DelegateUser> GetDelegateUsersByAliasId(string aliasId);
        void UpdateDelegateAccountDetails(string firstName, string surname, string email, int[] ids);

        DelegateUserCard? GetDelegateUserCardById(int id);
        List<DelegateUserCard> GetDelegateUserCardsByCentreId(int centreId);

        List<DelegateUserCard> GetDelegatesNotRegisteredForGroupByGroupId(int groupId, int centreId);

        string? GetGroupNameById(int groupId);

        void UpdateDelegateUserCentrePrompts(
            int id,
            int jobGroupId,
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
    }

    public partial class UserDataService : IUserDataService
    {
        private readonly IDbConnection connection;

        public UserDataService(IDbConnection connection)
        {
            this.connection = connection;
        }
    }
}
