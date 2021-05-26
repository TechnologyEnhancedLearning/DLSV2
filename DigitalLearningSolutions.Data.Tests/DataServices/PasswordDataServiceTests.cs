namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class PasswordDataServiceTests
    {
        private PasswordDataService passwordDataService;
        private UserDataService userDataService;
        private const string PasswordHashNotYetInDb = "I haven't used this password before!";

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
            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordByEmailAsync(existingAdminUser.EmailAddress!, newPasswordHash);

            // Then
            userDataService.GetAdminUserById(existingAdminUser.Id)?.Password.Should()
                .Be(PasswordHashNotYetInDb);
        }

        [Test]
        public async Task Setting_password_by_email_does_not_set_password_for_all_admins()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingAdminUser = UserTestHelper.GetDefaultAdminUser();
            var existingAdminUserPassword = existingAdminUser.Password;
            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordByEmailAsync("random.email@address.com", newPasswordHash);

            // Then
            userDataService.GetAdminUserById(existingAdminUser.Id)?.Password.Should()
                .Be(existingAdminUserPassword);
        }

        [Test]
        public async Task Setting_password_by_email_sets_password_for_matching_candidates()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingCandidate = UserTestHelper.GetDefaultDelegateUser();
            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordByEmailAsync(existingCandidate.EmailAddress!, newPasswordHash);

            // Then
            userDataService.GetDelegateUserById(existingCandidate.Id)?.Password.Should()
                .Be(PasswordHashNotYetInDb);
        }

        [Test]
        public async Task Setting_password_by_email_does_not_set_password_for_all_candidates()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingCandidate = UserTestHelper.GetDefaultDelegateUser();
            var existingCandidatePassword = existingCandidate.Password;
            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordByEmailAsync("random.email@address.com", newPasswordHash);

            // Then
            userDataService.GetDelegateUserById(existingCandidate.Id)?.Password.Should()
                .Be(existingCandidatePassword);
        }
    }
}
