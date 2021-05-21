﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class PasswordDataServiceTests
    {
        private PasswordDataService passwordDataService;
        private UserDataService userDataService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            passwordDataService = new PasswordDataService(connection);
            userDataService = new UserDataService(connection);
        }

        [Test]
        public void Set_password_by_candidate_number_should_set_password_correctly()
        {
            using var transaction = new TransactionScope();
            try
            {
                // When
                var password = "hashedPassword";
                var candidateNumber = "KW1";
                passwordDataService.SetPasswordByCandidateNumber(candidateNumber, password);
                var result = userDataService.GetDelegateUserById(1)?.Password;

                // Then
                result.Should().Be(password);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public async Task Setting_password_by_email_sets_password_for_matching_admins()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingAdminUser = UserTestHelper.GetDefaultAdminUser();
            var newPasswordHash = "I haven't used this password before!";

            // When
            await this.passwordDataService.SetPasswordByEmailAsync(existingAdminUser.EmailAddress!, newPasswordHash);

            // Then
            this.userDataService.GetAdminUserById(existingAdminUser.Id)?.Password.Should()
                .Be("I haven't used this password before!");
        }

        [Test]
        public async Task Setting_password_by_email_does_not_set_password_for_all_admins()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingAdminUser = UserTestHelper.GetDefaultAdminUser();
            var existingAdminUserPassword = existingAdminUser.Password;
            var newPasswordHash = "I haven't used this password before!";

            // When
            await this.passwordDataService.SetPasswordByEmailAsync("random.email@address.com", newPasswordHash);

            // Then
            this.userDataService.GetAdminUserById(existingAdminUser.Id)?.Password.Should()
                .Be(existingAdminUserPassword);
        }

        [Test]
        public async Task Setting_password_by_email_sets_password_for_matching_candidates()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingCandidate = UserTestHelper.GetDefaultDelegateUser();
            var newPasswordHash = "I haven't used this password before!";

            // When
            await this.passwordDataService.SetPasswordByEmailAsync(existingCandidate.EmailAddress!, newPasswordHash);

            // Then
            this.userDataService.GetDelegateUserById(existingCandidate.Id)?.Password.Should()
                .Be("I haven't used this password before!");
        }

        [Test]
        public async Task Setting_password_by_email_does_not_set_password_for_all_candidates()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingCandidate = UserTestHelper.GetDefaultDelegateUser();
            var existingCandidatePassword = existingCandidate.Password;
            var newPasswordHash = "I haven't used this password before!";

            // When
            await this.passwordDataService.SetPasswordByEmailAsync("random.email@address.com", newPasswordHash);

            // Then
            this.userDataService.GetDelegateUserById(existingCandidate.Id)?.Password.Should()
                .Be(existingCandidatePassword);
        }
    }
}
