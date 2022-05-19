﻿namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
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

        void UpdateAdminUserFailedLoginCount(int adminId, int updatedCount);

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

        void UpdateUserDetailsLastChecked(DateTime detailsLastChecked, int userId);

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

        void UpdateDelegateAccountCentreSpecificDetailsLastChecked(DateTime detailsLastChecked, int userId);

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

        public void UpdateUserDetailsLastChecked(DateTime detailsLastChecked, int userId)
        {
            connection.Execute(
                @"UPDATE Users
                        SET
                            DetailsLastChecked = @detailsLastChecked
                        WHERE ID = @userId",
                new { detailsLastChecked, userId }
            );
        }
    }
}
