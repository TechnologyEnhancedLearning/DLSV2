namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class PasswordDataServiceTests
    {
        private const string PasswordHashNotYetInDb = "I haven't used this password before!";
        private SqlConnection connection = null!;
        private PasswordDataService passwordDataService = null!;
        private UserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            passwordDataService = new PasswordDataService(connection);
            userDataService = new UserDataService(connection);
        }

        [Test]
        public void SetPasswordByCandidateNumber_should_set_password_correctly()
        {
            using var transaction = new TransactionScope();
            try
            {
                // When
                var password = "hashedPassword";
                var candidateNumber = "KW1";
                passwordDataService.SetPasswordByCandidateNumber(candidateNumber, password);
                var result = userDataService.GetDelegateUserById(1)!.Password;

                // Then
                result.Should().Be(password);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public async Task SetPasswordByUserIdAsync_sets_password_for_matching_user()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingUser = UserTestHelper.GetDefaultUserAccount();
            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordByUserIdAsync(existingUser.Id, newPasswordHash);

            // Then
            userDataService.GetUserAccountById(existingUser.Id)!.PasswordHash.Should()
                .Be(PasswordHashNotYetInDb);
        }

        [Test]
        public async Task SetPasswordByUserIdAsync_does_not_set_password_for_all_users()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingUser = UserTestHelper.GetDefaultUserAccount();
            var existingUserPassword = existingUser.PasswordHash;
            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordByUserIdAsync(-1, newPasswordHash);

            // Then
            userDataService.GetUserAccountById(existingUser.Id)!.PasswordHash.Should()
                .Be(existingUserPassword);
        }

        [Test]
        public async Task SetOldPasswordsToNullByUserIdAsync_nullifies_old_passwords_for_matching_user()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var userWithMultipleDelegateAccounts = await connection.GetUserWithMultipleDelegateAccountsAsync();
            await connection.SetDelegateAccountOldPasswordsForUserAsync(userWithMultipleDelegateAccounts);

            foreach (var delegateAccount in userDataService.GetDelegateAccountsByUserId(
                userWithMultipleDelegateAccounts.Id
            ))
            {
                delegateAccount.OldPassword.Should().NotBe(null);
            }

            // When
            await passwordDataService.SetOldPasswordsToNullByUserIdAsync(userWithMultipleDelegateAccounts.Id);

            // Then
            foreach (var delegateAccount in userDataService.GetDelegateAccountsByUserId(
                userWithMultipleDelegateAccounts.Id
            ))
            {
                delegateAccount.OldPassword.Should().Be(null);
            }
        }
    }
}
