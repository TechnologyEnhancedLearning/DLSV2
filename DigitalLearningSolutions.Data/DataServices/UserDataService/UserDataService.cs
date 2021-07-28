﻿namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserDataService
    {
        public AdminUser? GetAdminUserById(int id);
        public DelegateUser? GetDelegateUserById(int id);
        public DelegateUserCard? GetDelegateUserCardById(int id);
        public List<AdminUser> GetAdminUsersByCentreId(int centreId);
        public List<DelegateUserCard> GetDelegateUserCardsByCentreId(int centreId);
        public AdminUser? GetAdminUserByUsername(string username);
        public List<DelegateUser> GetDelegateUsersByUsername(string username);
        public List<DelegateUser> GetAllDelegateUsersByUsername(string username);
        public AdminUser? GetAdminUserByEmailAddress(string emailAddress);
        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress);
        public List<DelegateUser> GetUnapprovedDelegateUsersByCentreId(int centreId);
        public void UpdateAdminUser(string firstName, string surname, string email, byte[]? profileImage, int id);

        public void UpdateDelegateUsers(
            string firstName,
            string surname,
            string email,
            byte[]? profileImage,
            int[] ids
        );

        public void UpdateDelegateUserCentrePrompts(
            int id,
            int jobGroupId,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6
        );

        public void ApproveDelegateUsers(params int[] ids);

        public void RemoveDelegateUser(int delegateId);

        public int GetNumberOfApprovedDelegatesAtCentre(int centreId);

        public int GetNumberOfActiveAdminsAtCentre(int centreId);

        public int GetDelegateCountWithAnswerForPrompt(int centreId, int promptNumber);

        public void DeleteAllAnswersForPrompt(int centreId, int promptNumber);
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
