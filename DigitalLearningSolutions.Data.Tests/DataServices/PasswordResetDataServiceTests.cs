namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;

    public class PasswordResetDataServiceTests
    {
        private const string HashNotYetInDb = "HashThatDoesNotExistInTheDatabase";
        private SqlConnection connection = null!;
        private PasswordResetDataService service = null!;
        private UserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
            service = new PasswordResetDataService(connection, new NullLogger<PasswordResetDataService>());
        }

        [Test]
        public async Task Can_Create_And_Find_A_Password_Reset_For_User()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var createTime = new DateTime(2021, 1, 1);
            var expiryTime = new DateTime(2021, 1, 1);
            var testUser = UserTestHelper.GetDefaultUserAccount();
            var resetPasswordCreateModel = new ResetPasswordCreateModel(
                createTime,
                "ResetPasswordHash",
                testUser.Id,
                expiryTime            );

            // When
            service.CreatePasswordReset(resetPasswordCreateModel);

            var match = await service.FindMatchingResetPasswordEntityWithUserDetailsAsync(
                testUser.PrimaryEmail,
                resetPasswordCreateModel.Hash
            );

            // Then
            match.Should().NotBe(null);

            match!.UserId.Should().Be(testUser.Id);
            match.Email.Should().Be(testUser.PrimaryEmail);

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
            var expiryTime = new DateTime(2021, 1, 1);
            var testUser = UserTestHelper.GetDefaultUserAccount();
            var resetPasswordCreateModel = new ResetPasswordCreateModel(
                createTime,
                "ResetPasswordHash",
                testUser.Id,
                expiryTime
            );

            // When
            service.CreatePasswordReset(resetPasswordCreateModel);
            var match = await service.FindMatchingResetPasswordEntityWithUserDetailsAsync(
                emailToCheck,
                resetPasswordCreateModel.Hash
            );

            // Then
            match.Should().Be(null);
        }

        [Test]
        public async Task Does_Not_Match_Reset_Passwords_If_No_Reset_Password_With_Given_Hash()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given

            var createTime = new DateTime(2021, 1, 1);
            var expiryTime = new DateTime(2021, 1, 1);
            var testUser = UserTestHelper.GetDefaultUserAccount();
            var resetPasswordCreateModel = new ResetPasswordCreateModel(
                createTime,
                "NormalHash",
                testUser.Id,
                expiryTime
            );

            // When
            service.CreatePasswordReset(resetPasswordCreateModel);
            var match = await service.FindMatchingResetPasswordEntityWithUserDetailsAsync(
                testUser.PrimaryEmail,
                HashNotYetInDb
            );

            // Then
            match.Should().Be(null);
        }

        [Test]
        public async Task Removing_reset_hash_sets_ResetPasswordId_to_null_for_user()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var user = UserTestHelper.GetDefaultUserAccount();
            var resetPasswordId = await GivenResetPasswordWithHashExistsForUsersAsync(HashNotYetInDb, user);

            // When
            await service.RemoveResetPasswordAsync(resetPasswordId);

            // Then
            var userAfterRemoval = userDataService.GetUserAccountById(user.Id);
            userAfterRemoval.Should().NotBeNull();
            userAfterRemoval!.ResetPasswordId.Should().BeNull();
        }

        [Test]
        public async Task Removing_reset_hash_from_user_removes_ResetPassword_entity()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var user = UserTestHelper.GetDefaultUserAccount();
            var resetPasswordId = await GivenResetPasswordWithHashExistsForUsersAsync(HashNotYetInDb, user);

            // When
            await service.RemoveResetPasswordAsync(resetPasswordId);

            // Then
            var matchingResetPasswords = connection.GetResetPasswordById(resetPasswordId);
            matchingResetPasswords.Should().BeEmpty();
        }

        /// <summary>
        ///     Adds reset password entity for a list of users.
        /// </summary>
        /// <param name="hash">Reset hash.</param>
        /// <param name="user">A UserAccount.</param>
        /// <returns>The id of the reset password entity.</returns>
        private async Task<int> GivenResetPasswordWithHashExistsForUsersAsync(
            string hash,
            UserAccount user
        )
        {
            service.CreatePasswordReset(new ResetPasswordCreateModel(DateTime.UtcNow, hash, user.Id, DateTime.UtcNow.AddHours(2)));

            var resetPasswordId = await connection.GetResetPasswordIdByHashAsync(hash);

            await connection.SetResetPasswordIdForUserAsync(user, resetPasswordId);

            return resetPasswordId;
        }
    }
}
