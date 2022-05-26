namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class RegistrationDataServiceTests
    {
        private SqlConnection connection = null!;
        private INotificationPreferencesDataService notificationPreferencesDataService = null!;
        private RegistrationDataService service = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
            service = new RegistrationDataService(connection, userDataService);
            notificationPreferencesDataService = new NotificationPreferencesDataService(connection);
        }

        [Test]
        public async Task RegisterNewUserAndDelegateAccount_sets_all_fields_correctly_on_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var delegateRegistrationModel = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(centre: 3);

            // When
            var candidateNumber = service.RegisterNewUserAndDelegateAccount(delegateRegistrationModel);
            var user = await connection.GetDelegateUserByCandidateNumberAsync(candidateNumber);

            // Then
            user.FirstName.Should().Be(delegateRegistrationModel.FirstName);
            user.LastName.Should().Be(delegateRegistrationModel.LastName);
            user.EmailAddress.Should().Be(delegateRegistrationModel.PrimaryEmail);
            user.CentreId.Should().Be(delegateRegistrationModel.Centre);
            user.Answer1.Should().Be(delegateRegistrationModel.Answer1);
            user.Answer2.Should().Be(delegateRegistrationModel.Answer2);
            user.Answer3.Should().Be(delegateRegistrationModel.Answer3);
            user.Answer4.Should().Be(delegateRegistrationModel.Answer4);
            user.Answer5.Should().Be(delegateRegistrationModel.Answer5);
            user.Answer6.Should().Be(delegateRegistrationModel.Answer6);
            candidateNumber.Should().Be("TU67");
            user.CandidateNumber.Should().Be("TU67");
        }

        [Test]
        public void RegisterNewUserAndDelegateAccount_has_consistent_sequential_candidate_numbers()
        {
            try
            {
                // Given
                var models = new[]
                {
                    RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                        firstName: "Xavier",
                        lastName: "Quondam",
                        centre: 3,
                        email: "fake1",
                        secondaryEmail: "XQfake1"
                    ),
                    RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                        firstName: "Xavier",
                        lastName: "Quondam",
                        centre: 3,
                        email: "fake2",
                        secondaryEmail: "XQfake2"
                    ),
                    RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                        firstName: "Xavier",
                        lastName: "Quondam",
                        centre: 3,
                        email: "fake3",
                        secondaryEmail: "XQfake3"
                    ),
                    RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                        firstName: "Xavier",
                        lastName: "Quondam",
                        centre: 3,
                        email: "fake4",
                        secondaryEmail: "XQfake4"
                    ),
                    RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                        firstName: "Xavier",
                        lastName: "Quondam",
                        centre: 3,
                        email: "fake5",
                        secondaryEmail: "XQfake5"
                    ),
                };
                var actions = models.Select(GetRegistrationAction).ToArray();

                Func<Task> userLookup1 = async () => await connection.GetDelegateUserByCandidateNumberAsync("XQ1");
                Func<Task> userLookup2 = async () => await connection.GetDelegateUserByCandidateNumberAsync("XQ2");
                Func<Task> userLookup3 = async () => await connection.GetDelegateUserByCandidateNumberAsync("XQ3");
                Func<Task> userLookup4 = async () => await connection.GetDelegateUserByCandidateNumberAsync("XQ4");
                Func<Task> userLookup5 = async () => await connection.GetDelegateUserByCandidateNumberAsync("XQ5");
                Func<Task> userLookup6 = async () => await connection.GetDelegateUserByCandidateNumberAsync("XQ6");
                Func<Task> userLookup7 = async () => await connection.GetDelegateUserByCandidateNumberAsync("XQ7");

                // When
                Parallel.Invoke(actions);

                // Then
                using (new AssertionScope())
                {
                    userLookup1.Should().NotThrow();
                    userLookup2.Should().NotThrow();
                    userLookup3.Should().NotThrow();
                    userLookup4.Should().NotThrow();
                    userLookup5.Should().NotThrow();
                    userLookup6.Should().Throw<InvalidOperationException>()
                        .WithMessage("Sequence contains no elements");
                    userLookup7.Should().Throw<InvalidOperationException>()
                        .WithMessage("Sequence contains no elements");
                }
            }
            // we clean up manually due to difficulties in parallel invocation of data service methods inside a transaction.
            // if the test is failing, check the cleanup is working correctly.
            finally
            {
                connection.Execute("DELETE FROM UserCentreDetails WHERE Email LIKE 'XQfake%'");
                connection.Execute("DELETE FROM DelegateAccounts WHERE CandidateNumber LIKE 'XQ%'");
                connection.Execute("DELETE FROM Users WHERE FirstName = 'Xavier' AND LastName = 'Quondam'");
            }
        }

        private Action GetRegistrationAction(DelegateRegistrationModel model)
        {
            var newConnection = ServiceTestHelper.GetDatabaseConnection();
            var newUserDataService = new UserDataService(newConnection);
            var newService = new RegistrationDataService(newConnection, newUserDataService);

            void Action() => newService.RegisterNewUserAndDelegateAccount(model);
            return Action;
        }

        [Test]
        public void Sets_all_fields_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel =
                RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel(
                    categoryId: 1
                );

            // When
            var id = service.RegisterAdmin(registrationModel, 4046);

            // Then
            var user = userDataService.GetAdminUserById(id)!;
            user.CentreId.Should().Be(registrationModel.Centre);
            user.IsCentreAdmin.Should().Be(registrationModel.IsCentreAdmin);
            user.IsCentreManager.Should().Be(registrationModel.IsCentreManager);
            user.Approved.Should().Be(registrationModel.Approved);
            user.Active.Should().Be(registrationModel.Active);
            user.IsContentCreator.Should().Be(registrationModel.IsContentCreator);
            user.IsContentManager.Should().Be(registrationModel.IsContentManager);
            user.ImportOnly.Should().Be(registrationModel.ImportOnly);
            user.IsTrainer.Should().Be(registrationModel.IsTrainer);
            user.IsSupervisor.Should().Be(registrationModel.IsSupervisor);
            user.IsNominatedSupervisor.Should().Be(registrationModel.IsNominatedSupervisor);
        }

        [Test]
        public void Sets_notification_preferences_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel =
                RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel(
                    categoryId: 1
                );

            // When
            var id = service.RegisterAdmin(registrationModel, 4046);

            // Then
            var user = userDataService.GetAdminUserById(id)!;
            var preferences = notificationPreferencesDataService.GetNotificationPreferencesForAdmin(user.Id).ToList();
            preferences.Count.Should().Be(7);
            preferences.Should().ContainSingle(n => n.NotificationId.Equals(1) && !n.Accepted);
            preferences.Should().ContainSingle(n => n.NotificationId.Equals(2) && n.Accepted);
            preferences.Should().ContainSingle(n => n.NotificationId.Equals(3) && n.Accepted);
            preferences.Should().ContainSingle(n => n.NotificationId.Equals(4) && !n.Accepted);
            preferences.Should().ContainSingle(n => n.NotificationId.Equals(5) && n.Accepted);
            preferences.Should().ContainSingle(n => n.NotificationId.Equals(6) && !n.Accepted);
            preferences.Should().ContainSingle(n => n.NotificationId.Equals(7) && !n.Accepted);
        }
    }
}
