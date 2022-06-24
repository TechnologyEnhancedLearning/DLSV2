namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class RegistrationDataServiceTests
    {
        private IClockService clockService = null!;
        private SqlConnection connection = null!;
        private ILogger<IRegistrationDataService> logger = null!;
        private INotificationPreferencesDataService notificationPreferencesDataService = null!;
        private RegistrationDataService service = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
            clockService = A.Fake<IClockService>();
            logger = A.Fake<ILogger<IRegistrationDataService>>();
            service = new RegistrationDataService(connection, userDataService, clockService, logger);
            notificationPreferencesDataService = new NotificationPreferencesDataService(connection);
        }

        [Test]
        public void RegisterNewUserAndDelegateAccount_sets_all_fields_correctly_on_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var dateTime = new DateTime(2022, 6, 16, 9, 41, 30);
            A.CallTo(() => clockService.UtcNow).Returns(dateTime);

            // Given
            var delegateRegistrationModel = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(centre: 3);

            // When
            var candidateNumber = service.RegisterNewUserAndDelegateAccount(
                delegateRegistrationModel,
                false
            );

            // Then
            var delegateEntity = userDataService.GetDelegateByCandidateNumber(candidateNumber);
            using (new AssertionScope())
            {
                delegateEntity!.UserAccount.FirstName.Should().Be(delegateRegistrationModel.FirstName);
                delegateEntity.UserAccount.LastName.Should().Be(delegateRegistrationModel.LastName);
                delegateEntity.UserAccount.PrimaryEmail.Should().Be(delegateRegistrationModel.PrimaryEmail);
                delegateEntity.UserAccount.TermsAgreed.Should().BeNull();
                delegateEntity.UserAccount.DetailsLastChecked.Should().Be(dateTime);
                delegateEntity.DelegateAccount.CentreId.Should().Be(delegateRegistrationModel.Centre);
                delegateEntity.DelegateAccount.Answer1.Should().Be(delegateRegistrationModel.Answer1);
                delegateEntity.DelegateAccount.Answer2.Should().Be(delegateRegistrationModel.Answer2);
                delegateEntity.DelegateAccount.Answer3.Should().Be(delegateRegistrationModel.Answer3);
                delegateEntity.DelegateAccount.Answer4.Should().Be(delegateRegistrationModel.Answer4);
                delegateEntity.DelegateAccount.Answer5.Should().Be(delegateRegistrationModel.Answer5);
                delegateEntity.DelegateAccount.Answer6.Should().Be(delegateRegistrationModel.Answer6);
                candidateNumber.Should().Be("TU67");
                delegateEntity.DelegateAccount.CandidateNumber.Should().Be("TU67");
                delegateEntity.DelegateAccount.CentreSpecificDetailsLastChecked.Should().Be(dateTime);
            }
        }

        [Test]
        public void RegisterNewUserAndDelegateAccount_sets_all_fields_correctly_when_registerJourneyHasTerms_is_true()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var delegateRegistrationModel = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(centre: 3);
            var dateTime = new DateTime(2022, 6, 16, 9, 41, 30);
            A.CallTo(() => clockService.UtcNow).Returns(dateTime);

            // When
            var candidateNumber = service.RegisterNewUserAndDelegateAccount(
                delegateRegistrationModel,
                true
            );

            // Then
            var delegateEntity = userDataService.GetDelegateByCandidateNumber(candidateNumber);
            using (new AssertionScope())
            {
                delegateEntity!.UserAccount.FirstName.Should().Be(delegateRegistrationModel.FirstName);
                delegateEntity.UserAccount.LastName.Should().Be(delegateRegistrationModel.LastName);
                delegateEntity.UserAccount.PrimaryEmail.Should().Be(delegateRegistrationModel.PrimaryEmail);
                delegateEntity.UserAccount!.TermsAgreed.Should().Be(dateTime);
                delegateEntity.UserAccount.DetailsLastChecked.Should().Be(dateTime);
                delegateEntity.DelegateAccount.CentreId.Should().Be(delegateRegistrationModel.Centre);
                delegateEntity.DelegateAccount.Answer1.Should().Be(delegateRegistrationModel.Answer1);
                delegateEntity.DelegateAccount.Answer2.Should().Be(delegateRegistrationModel.Answer2);
                delegateEntity.DelegateAccount.Answer3.Should().Be(delegateRegistrationModel.Answer3);
                delegateEntity.DelegateAccount.Answer4.Should().Be(delegateRegistrationModel.Answer4);
                delegateEntity.DelegateAccount.Answer5.Should().Be(delegateRegistrationModel.Answer5);
                delegateEntity.DelegateAccount.Answer6.Should().Be(delegateRegistrationModel.Answer6);
                candidateNumber.Should().Be("TU67");
                delegateEntity.DelegateAccount.CandidateNumber.Should().Be("TU67");
                delegateEntity.DelegateAccount.CentreSpecificDetailsLastChecked.Should().Be(dateTime);
            }
        }

        [Test]
        public async Task
            RegisterDelegateAccountAndCentreDetailForExistingUser_sets_all_fields_correctly_on_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    3
                );
            var currentTime = DateTime.Now;
            const int userId = 2;

            // When
            var (delegateId, candidateNumber) =
                service.RegisterDelegateAccountAndCentreDetailForExistingUser(
                    delegateRegistrationModel,
                    userId,
                    currentTime
                );

            // Then
            var user = await connection.GetDelegateUserByCandidateNumberAsync(candidateNumber);
            using (new AssertionScope())
            {
                user.Id.Should().Be(delegateId);
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
                user.DateRegistered.Should().BeCloseTo(currentTime, 100);
                candidateNumber.Should().Be("FS352");
                user.CandidateNumber.Should().Be("FS352");
            }
        }

        [Test]
        public async Task
            RegisterDelegateAccountAndCentreDetailForExistingUser_does_not_create_UserCentreDetails_with_null_centre_specific_email()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    3,
                    centreSpecificEmail: null
                );
            var currentTime = DateTime.Now;
            const int userId = 2;

            // When
            var (delegateId, candidateNumber) =
                service.RegisterDelegateAccountAndCentreDetailForExistingUser(
                    delegateRegistrationModel,
                    userId,
                    currentTime
                );

            // Then
            var userCentreDetailsCount = connection.QuerySingle<int>(
                "SELECT COUNT(*) FROM UserCentreDetails WHERE CentreID = 3 AND UserID = 2 AND Email IS NOT NULL"
            );
            var user = await connection.GetDelegateUserByCandidateNumberAsync(candidateNumber);
            user.Id.Should().Be(delegateId);
            userCentreDetailsCount.Should().Be(0);
            candidateNumber.Should().Be("FS352");
            user.CandidateNumber.Should().Be("FS352");
        }

        [Test]
        public void Sets_all_fields_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel =
                RegistrationModelTestHelper.GetDefaultCentreManagerAccountRegistrationModel(
                    categoryId: 1
                );

            // When
            var id = service.RegisterAdmin(registrationModel);

            // Then
            var user = userDataService.GetAdminUserById(id)!;
            using (new AssertionScope())
            {
                user.CentreId.Should().Be(registrationModel.CentreId);
                user.IsCentreAdmin.Should().Be(registrationModel.IsCentreAdmin);
                user.IsCentreManager.Should().Be(registrationModel.IsCentreManager);
                user.Active.Should().Be(registrationModel.Active);
                user.IsContentCreator.Should().Be(registrationModel.IsContentCreator);
                user.IsContentManager.Should().Be(registrationModel.IsContentManager);
                user.ImportOnly.Should().Be(registrationModel.ImportOnly);
                user.IsTrainer.Should().Be(registrationModel.IsTrainer);
                user.IsSupervisor.Should().Be(registrationModel.IsSupervisor);
                user.IsNominatedSupervisor.Should().Be(registrationModel.IsNominatedSupervisor);
            }
        }

        [Test]
        public void Sets_notification_preferences_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel =
                RegistrationModelTestHelper.GetDefaultCentreManagerAccountRegistrationModel(
                    categoryId: 1
                );

            // When
            var id = service.RegisterAdmin(registrationModel);

            // Then
            var user = userDataService.GetAdminUserById(id)!;
            var preferences = notificationPreferencesDataService.GetNotificationPreferencesForAdmin(user.Id).ToList();
            using (new AssertionScope())
            {
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
}
