namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Threading.Tasks;
    using System.Transactions;
    using Dapper;
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
        private PasswordDataService passwordDataService = null!;
        private UserDataService userDataService = null!;
        private SqlConnection? connection;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
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
        public async Task Setting_password_by_email_sets_password_for_matching_candidate()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingDelegate = UserTestHelper.GetDefaultDelegateUser();
            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordByEmailAsync(existingDelegate.EmailAddress!, newPasswordHash);

            // Then
            userDataService.GetDelegateUserById(existingDelegate.Id)?.Password.Should()
                .Be(PasswordHashNotYetInDb);
        }

        [Test]
        public async Task SetPasswordForUsers_can_set_password_for_multiple_delegates()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingDelegate = UserTestHelper.GetDefaultDelegateUser();
            var newDelegate = Builder<DelegateUser>.CreateNew()
                .With(d => d.EmailAddress = existingDelegate.EmailAddress)
                .With(d => d.CentreId = existingDelegate.CentreId)
                .Build();
            GivenDelegateUserIsInDatabase(newDelegate);

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
        public async Task Setting_password_by_email_does_not_set_password_for_all_delegates()
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var existingDelegate = UserTestHelper.GetDefaultDelegateUser();
            var existingDelegatePassword = existingDelegate.Password;
            var newPasswordHash = PasswordHashNotYetInDb;

            // When
            await passwordDataService.SetPasswordByEmailAsync("random.email@address.com", newPasswordHash);

            // Then
            userDataService.GetDelegateUserById(existingDelegate.Id)?.Password.Should()
                .Be(existingDelegatePassword);
        }

        [Test]
        public async Task Setting_password_for_user_account_set_changes_password()
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
        public async Task Can_set_password_for_delegate_only()
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

        private void GivenDelegateUserIsInDatabase(DelegateUser user)
        {
            connection.Execute(
                @"insert into Candidates (Active, CentreId, LastName, DateRegistered, CandidateNumber, JobGroupID,
                    Approved, ExternalReg, SelfReg, SkipPW, PublicSkypeLink)
                  values (@Active, @CentreId, @LastName, @DateRegistered, @CandidateNumber, @JobGroupID,
                    @Approved, @ExternalReg, @SelfReg, @SkipPW, @PublicSkypeLink);",
                new
                {
                    user.Active,
                    user.CentreId,
                    user.LastName,
                    user.DateRegistered,
                    user.CandidateNumber,
                    user.JobGroupId,
                    user.Approved,
                    ExternalReg = false,
                    SelfReg = false,
                    SkipPW = false,
                    PublicSkypeLink = false,
                }
            );
        }
    }
}
