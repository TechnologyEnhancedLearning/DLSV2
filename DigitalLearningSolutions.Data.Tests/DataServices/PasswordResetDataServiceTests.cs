namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;

    public class PasswordResetDataServiceTests
    {
        [DatapointSource]
        public UserType[] UserTypes = {UserType.AdminUser, UserType.DelegateUser};

        private readonly PasswordResetDataService service =
            new PasswordResetDataService(
                ServiceTestHelper.GetDatabaseConnection(),
                new NullLogger<PasswordResetDataService>());

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
    }
}
