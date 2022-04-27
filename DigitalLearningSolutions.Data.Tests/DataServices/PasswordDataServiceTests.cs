namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FizzWare.NBuilder;
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
        public async Task SetPasswordByEmailAsync_sets_password_for_matching_user()
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
        public async Task SetPasswordByEmailAsync_does_not_set_password_for_all_users()
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
        public async Task SetPasswordForUsersAsync_can_set_password_for_multiple_delegates()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingDelegate = UserTestHelper.GetDefaultDelegateUser();
            var newDelegate = Builder<DelegateUser>.CreateNew()
                .With(d => d.EmailAddress = "unique.email@test1234.com")
                .With(d => d.CentreId = existingDelegate.CentreId)
                .With(d => d.LastName = existingDelegate.LastName)
                .With(d => d.JobGroupId = existingDelegate.JobGroupId)
                .Build();
            UserTestHelper.GivenDelegateUserIsInDatabase(newDelegate, connection);

            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordForUsersAsync(
                new[] { existingDelegate.ToUserReference(), newDelegate.ToUserReference() },
                newPasswordHash
            );

            // Then
            userDataService.GetDelegateUserById(existingDelegate.Id)?.Password.Should()
                .Be(PasswordHashNotYetInDb);
            userDataService.GetDelegateUserById(newDelegate.Id)?.Password.Should()
                .Be(PasswordHashNotYetInDb);
        }

        [Test]
        public async Task SetPasswordForUsersAsync_changes_password_for_given_users()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingDelegateRef = UserTestHelper.GetDefaultDelegateUser().ToUserReference();
            var existingAdminRef = UserTestHelper.GetDefaultAdminUser().ToUserReference();
            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordForUsersAsync(
                new[] { existingDelegateRef, existingAdminRef },
                newPasswordHash
            );

            // Then
            userDataService.GetAdminUserById(existingAdminRef.Id)?.Password.Should().Be(newPasswordHash);
            userDataService.GetDelegateUserById(existingDelegateRef.Id)?.Password.Should()
                .Be(newPasswordHash);
        }

        [Test]
        public async Task SetPasswordForUsersAsync_can_set_password_for_delegate_only()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingDelegateRef = UserTestHelper.GetDefaultDelegateUser().ToUserReference();
            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordForUsersAsync(
                new[] { existingDelegateRef },
                newPasswordHash
            );

            // Then
            userDataService.GetDelegateUserById(UserTestHelper.GetDefaultDelegateUser().Id)?.Password.Should()
                .Be(newPasswordHash);
            userDataService.GetAdminUserById(UserTestHelper.GetDefaultAdminUser().Id)?.Password.Should()
                .NotBe(newPasswordHash);
        }
    }
}
