namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
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
        [DatapointSource]
        public UserType[] UserTypes = { UserType.AdminUser, UserType.DelegateUser };

        private PasswordResetDataService service = null!;
        private SqlConnection connection = null!;

        [SetUp]
        public void SetUp()
        {
            this.connection = ServiceTestHelper.GetDatabaseConnection();
            this.service = new PasswordResetDataService(connection, new NullLogger<PasswordResetDataService>());
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
            var resetPasswordCreateModel = new ResetPasswordCreateModel(
                createTime,
                "ResetPasswordHash",
                testDelegateUser.Id,
                userType);

            // When
            service.CreatePasswordReset(resetPasswordCreateModel);
            var matches = await service.FindMatchingResetPasswordEntitiesWithUserDetailsAsync(
                testDelegateUser.EmailAddress,
                resetPasswordCreateModel.Hash);

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
            var resetPasswordCreateModel = new ResetPasswordCreateModel(
                createTime,
                "ResetPasswordHash",
                testDelegateUser.Id,
                UserType.DelegateUser);

            // When
            service.CreatePasswordReset(resetPasswordCreateModel);
            var matches = await service.FindMatchingResetPasswordEntitiesWithUserDetailsAsync(
                emailToCheck,
                resetPasswordCreateModel.Hash);

            // Then
            matches.Count.Should().Be(0);
        }

        [Test]
        public async Task Does_Not_Match_Reset_Passwords_If_No_Reset_Password_With_Given_Hash()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var hashToCheck = "HashThatDoesNotExistInTheDatabase";

            var createTime = new DateTime(2021, 1, 1);
            var testDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            var resetPasswordCreateModel = new ResetPasswordCreateModel(
                createTime,
                "NormalHash",
                testDelegateUser.Id,
                UserType.DelegateUser);

            // When
            service.CreatePasswordReset(resetPasswordCreateModel);
            var matches = await service.FindMatchingResetPasswordEntitiesWithUserDetailsAsync(
                testDelegateUser.EmailAddress,
                hashToCheck);

            // Then
            matches.Count.Should().Be(0);
        }

        [Test]
        public async Task Removing_reset_hash_sets_ResetPasswordId_to_null_for_admin_user()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var hash = "HashThatDoesNotExistInTheDatabase";
            await this.GivenResetPasswordWithHashExists(hash);
            var resetPasswordId = await this.GetResetPasswordIdByHash(hash);
            var userId = UserTestHelper.GetDefaultAdminUser(resetPasswordId: resetPasswordId).Id;
            await this.GivenUserHasResetPasswordId(resetPasswordId, new UserReference(userId, UserType.AdminUser));

            // When
            await this.service.RemoveResetPasswordAsync(resetPasswordId);

            // Then
            var userAfterRemoval = (await this.connection.QueryAsync<AdminUser>(
                "SELECT * FROM AdminUsers WHERE AdminID = @UserId;",
                new { UserId = userId })).Single();
            userAfterRemoval.ResetPasswordId.Should().BeNull();
        }

        [Test]
        public async Task Removing_reset_hash_sets_ResetPasswordId_to_null_for_delegate_user()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var hash = "HashThatDoesNotExistInTheDatabase";
            await this.GivenResetPasswordWithHashExists(hash);
            var resetPasswordId = await this.GetResetPasswordIdByHash(hash);
            var userId = UserTestHelper.GetDefaultDelegateUser(resetPasswordId: resetPasswordId).Id;
            await this.GivenUserHasResetPasswordId(resetPasswordId, new UserReference(userId, UserType.DelegateUser));

            // When
            await this.service.RemoveResetPasswordAsync(resetPasswordId);

            // Then
            var userAfterRemoval = (await this.connection.QueryAsync<AdminUser>(
                "SELECT * FROM Candidates WHERE CandidateID = @UserId;",
                new { UserId = userId })).Single();
            userAfterRemoval.ResetPasswordId.Should().BeNull();
        }

        [Test]
        public async Task Removing_reset_hash_from_admin_user_removes_ResetPassword_entity()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var hash = "HashThatDoesNotExistInTheDatabase";
            await this.GivenResetPasswordWithHashExists(hash);
            var resetPasswordId = await this.GetResetPasswordIdByHash(hash);
            var userId = UserTestHelper.GetDefaultDelegateUser(resetPasswordId: resetPasswordId).Id;
            await this.GivenUserHasResetPasswordId(resetPasswordId, new UserReference(userId, UserType.DelegateUser));

            // When
            await this.service.RemoveResetPasswordAsync(resetPasswordId);

            // Then
            var matchingResetPasswords = this.connection.Query<ResetPassword>(
                "SELECT * FROM ResetPassword WHERE ID = @ResetPasswordId",
                new { ResetPasswordId = resetPasswordId });
            matchingResetPasswords.Should().BeEmpty();
        }

        private async Task GivenResetPasswordWithHashExists(string hash)
        {
            await this.connection.ExecuteAsync(
                "INSERT INTO ResetPassword (ResetPasswordHash, PasswordResetDateTime) VALUES (@Hash, @CurrentTime);",
                new { Hash = hash, CurrentTime = DateTime.UtcNow });
        }

        private async Task<int> GetResetPasswordIdByHash(string hash)
        {
            var resetPasswordId = (await this.connection.QueryAsync<int>(
                "SELECT Id FROM ResetPassword WHERE ResetPasswordHash = @Hash;",
                new { Hash = hash })).Single();
            return resetPasswordId;
        }

        private async Task GivenUserHasResetPasswordId(int resetPasswordId, UserReference user)
        {
            await this.connection.ExecuteAsync(
                $"UPDATE {user.UserType.TableName} SET ResetPasswordId = @ResetPasswordId WHERE {user.UserType.IdColumnName} = @UserId;",
                new { ResetPasswordId = resetPasswordId, UserId = user.Id });
        }
    }
}
