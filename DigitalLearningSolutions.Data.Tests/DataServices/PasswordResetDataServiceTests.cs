namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;

    public class PasswordResetDataServiceTests
    {
        private const string HashNotYetInDb = "HashThatDoesNotExistInTheDatabase";

        [DatapointSource]
        public UserType[] UserTypes = { UserType.AdminUser, UserType.DelegateUser }; // Used by theories - don't remove!

        private UserDataService userDataService = null!;
        private PasswordResetDataService service = null!;
        private SqlConnection connection = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
            service = new PasswordResetDataService(connection, new NullLogger<PasswordResetDataService>());
        }

        [Theory]
        public async Task Can_Create_And_Find_A_Password_Reset_For_User(UserType userType)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var createTime = new DateTime(2021, 1, 1);
            var testDelegateUser = userType.Equals(UserType.AdminUser)
                ? (User)UserTestHelper.GetDefaultAdminUser()
                : UserTestHelper.GetDefaultDelegateUser();
            var resetPasswordCreateModel = new ResetPasswordCreateModel
            (
                createTime,
                "ResetPasswordHash",
                testDelegateUser.Id,
                userType
            );

            // When
            service.CreatePasswordReset(resetPasswordCreateModel);
            var matches = await service.FindMatchingResetPasswordEntitiesWithUserDetailsAsync
            (
                testDelegateUser.EmailAddress!,
                resetPasswordCreateModel.Hash
            );

            // Then
            matches.Count.Should().Be(1);
            var match = matches.Single();

            match.UserId.Should().Be(testDelegateUser.Id);
            match.Email.Should().Be(testDelegateUser.EmailAddress);
            match.UserType.Should().Be(userType);

            match.Id.Should().BeGreaterThan(0);
            match.ResetPasswordHash.Should().Be(resetPasswordCreateModel.Hash);
            match.PasswordResetDateTime.Should().Be(resetPasswordCreateModel.CreateTime);
        }

        [Test]
        public async Task Does_Not_Match_Reset_Passwords_If_No_User_With_Given_Email()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var emailToCheck = "EmailThat.DoesNotExist@InTheDatabase.com";

            var createTime = new DateTime(2021, 1, 1);
            var testDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            var resetPasswordCreateModel = new ResetPasswordCreateModel
            (
                createTime,
                "ResetPasswordHash",
                testDelegateUser.Id,
                UserType.DelegateUser
            );

            // When
            service.CreatePasswordReset(resetPasswordCreateModel);
            var matches = await service.FindMatchingResetPasswordEntitiesWithUserDetailsAsync
            (
                emailToCheck,
                resetPasswordCreateModel.Hash
            );

            // Then
            matches.Count.Should().Be(0);
        }

        [Test]
        public async Task Does_Not_Match_Reset_Passwords_If_No_Reset_Password_With_Given_Hash()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given

            var createTime = new DateTime(2021, 1, 1);
            var testDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            var resetPasswordCreateModel = new ResetPasswordCreateModel
            (
                createTime,
                "NormalHash",
                testDelegateUser.Id,
                UserType.DelegateUser
            );

            // When
            service.CreatePasswordReset(resetPasswordCreateModel);
            var matches = await service.FindMatchingResetPasswordEntitiesWithUserDetailsAsync
            (
                testDelegateUser.EmailAddress!,
                HashNotYetInDb
            );

            // Then
            matches.Count.Should().Be(0);
        }

        [Test]
        public async Task Removing_reset_hash_sets_ResetPasswordId_to_null_for_admin_user()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var userId = UserTestHelper.GetDefaultAdminUser().Id;
            var userRef = new UserReference(userId, UserType.AdminUser);
            var resetPasswordId = await GivenResetPasswordWithHashExistsForUsersAsync(HashNotYetInDb, new[] { userRef });

            // When
            await service.RemoveResetPasswordAsync(resetPasswordId);

            // Then
            var userAfterRemoval = userDataService.GetAdminUserById(userId);
            userAfterRemoval.Should().NotBeNull();
            userAfterRemoval?.ResetPasswordId.Should().BeNull();
        }

        [Test]
        public async Task Removing_reset_hash_sets_ResetPasswordId_to_null_for_delegate_user()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var userId = UserTestHelper.GetDefaultDelegateUser().Id;
            var userRef = new UserReference(userId, UserType.DelegateUser);
            var resetPasswordId = await GivenResetPasswordWithHashExistsForUsersAsync(HashNotYetInDb, new[] { userRef });

            // When
            await service.RemoveResetPasswordAsync(resetPasswordId);

            // Then
            var userAfterRemoval = userDataService.GetDelegateUserById(userRef.Id);
            userAfterRemoval.Should().NotBeNull();
            userAfterRemoval?.ResetPasswordId.Should().BeNull();
        }

        [Test]
        public async Task Removing_reset_hash_from_admin_user_removes_ResetPassword_entity()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var userId = UserTestHelper.GetDefaultAdminUser().Id;
            var userRef = new UserReference(userId, UserType.AdminUser);
            var resetPasswordId = await GivenResetPasswordWithHashExistsForUsersAsync(HashNotYetInDb, new[] { userRef });

            // When
            await service.RemoveResetPasswordAsync(resetPasswordId);

            // Then
            var matchingResetPasswords = connection.Query<ResetPassword>
            (
                "SELECT * FROM ResetPassword WHERE ID = @ResetPasswordId",
                new { ResetPasswordId = resetPasswordId }
            );
            matchingResetPasswords.Should().BeEmpty();
        }

        [Test]
        public async Task Removing_reset_hash_from_delegate_user_removes_ResetPassword_entity()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var userId = UserTestHelper.GetDefaultDelegateUser().Id;
            var userRef = new UserReference(userId, UserType.DelegateUser);
            var resetPasswordId = await GivenResetPasswordWithHashExistsForUsersAsync(HashNotYetInDb, new[] { userRef });

            // When
            await service.RemoveResetPasswordAsync(resetPasswordId);

            // Then
            var matchingResetPasswords = connection.Query<ResetPassword>
            (
                "SELECT * FROM ResetPassword WHERE ID = @ResetPasswordId",
                new { ResetPasswordId = resetPasswordId }
            );
            matchingResetPasswords.Should().BeEmpty();
        }

        /// <summary>
        /// Adds reset password entity for a list of users.
        /// </summary>
        /// <param name="hash">Reset hash.</param>
        /// <param name="users">Must contain at least 1 user.</param>
        /// <returns>The id of the reset password entity.</returns>
        private async Task<int> GivenResetPasswordWithHashExistsForUsersAsync(string hash, IEnumerable<UserReference> users)
        {
            var userList = users.ToList();

            var firstUser = userList.First();

            service.CreatePasswordReset
                (new ResetPasswordCreateModel(DateTime.UtcNow, hash, firstUser.Id, firstUser.UserType));

            var resetPasswordId = await GetResetPasswordIdByHashAsync(hash);

            foreach (var user in userList.Skip(1))
            {
                await GivenUserHasResetPasswordIdAsync(resetPasswordId, user);
            }

            return resetPasswordId;
        }

        private async Task<int> GetResetPasswordIdByHashAsync(string hash)
        {
            var resetPasswordId = (await connection.QueryAsync<int>
            (
                "SELECT Id FROM ResetPassword WHERE ResetPasswordHash = @Hash;",
                new { Hash = hash }
            )).Single();
            return resetPasswordId;
        }

        private async Task GivenUserHasResetPasswordIdAsync(int resetPasswordId, UserReference user)
        {
            await connection.ExecuteAsync
            (
                $"UPDATE {user.UserType.TableName} SET ResetPasswordId = @ResetPasswordId WHERE {user.UserType.IdColumnName} = @UserId;",
                new { ResetPasswordId = resetPasswordId, UserId = user.Id }
            );
        }
    }
}
