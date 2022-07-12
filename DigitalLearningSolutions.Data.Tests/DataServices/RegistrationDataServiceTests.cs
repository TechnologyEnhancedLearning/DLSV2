namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class RegistrationDataServiceTests
    {
        private IClockService clockService = null!;
        private SqlConnection connection = null!;
        private INotificationPreferencesDataService notificationPreferencesDataService = null!;
        private RegistrationDataService service = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            clockService = A.Fake<IClockService>();
            service = new RegistrationDataService(connection, clockService);
            userDataService = new UserDataService(connection);
            notificationPreferencesDataService = new NotificationPreferencesDataService(connection);
        }

        [Test]
        public async Task Sets_all_fields_correctly_on_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var delegateRegistrationModel = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(centre: 3);

            // When
            var candidateNumber = service.RegisterDelegate(delegateRegistrationModel);
            var user = await connection.GetDelegateUserByCandidateNumberAsync(candidateNumber);

            // Then
            user.FirstName.Should().Be(delegateRegistrationModel.FirstName);
            user.LastName.Should().Be(delegateRegistrationModel.LastName);
            user.EmailAddress.Should().Be(delegateRegistrationModel.Email);
            user.CentreId.Should().Be(delegateRegistrationModel.Centre);
            user.Answer1.Should().Be(delegateRegistrationModel.Answer1);
            user.Answer2.Should().Be(delegateRegistrationModel.Answer2);
            user.Answer3.Should().Be(delegateRegistrationModel.Answer3);
            user.Answer4.Should().Be(delegateRegistrationModel.Answer4);
            user.Answer5.Should().Be(delegateRegistrationModel.Answer5);
            user.Answer6.Should().Be(delegateRegistrationModel.Answer6);
        }

        [Test]
        public async Task RegisterAdmin_sets_all_fields_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();
            var currentTime = new DateTime(2022, 6, 17, 14, 05, 30);
            A.CallTo(() => clockService.UtcNow).Returns(currentTime);

            // When
            service.RegisterAdmin(registrationModel, true);

            // Then
            var user = userDataService.GetAdminUserByEmailAddress(registrationModel.Email)!;
            var tcAgreed = await connection.GetTCAgreedByAdminIdAsync(user.Id);
            using (new AssertionScope())
            {
                user.FirstName.Should().Be(registrationModel.FirstName);
                user.LastName.Should().Be(registrationModel.LastName);
                user.CentreId.Should().Be(registrationModel.Centre);
                user.Password.Should().Be(registrationModel.PasswordHash);
                user.IsCentreAdmin.Should().BeTrue();
                user.IsCentreManager.Should().BeTrue();
                tcAgreed.Should().Be(currentTime);
            }
        }

        [Test]
        public async Task RegisterAdmin_Sets_tc_agreed_to_null_when_registerJourneyContainsTermsAndConditions_is_false()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();
            var currentTime = new DateTime(2022, 6, 17, 14, 05, 30);
            A.CallTo(() => clockService.UtcNow).Returns(currentTime);

            // When
            service.RegisterAdmin(registrationModel, false);

            // Then
            var user = userDataService.GetAdminUserByEmailAddress(registrationModel.Email)!;
            var tcAgreed = await connection.GetTCAgreedByAdminIdAsync(user.Id);
            user.FirstName.Should().Be(registrationModel.FirstName);
            user.LastName.Should().Be(registrationModel.LastName);
            user.CentreId.Should().Be(registrationModel.Centre);
            user.Password.Should().Be(registrationModel.PasswordHash);
            user.IsCentreAdmin.Should().BeTrue();
            user.IsCentreManager.Should().BeTrue();
            tcAgreed.Should().BeNull();
        }

        [Test]
        public void Sets_notification_preferences_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            service.RegisterAdmin(registrationModel, false);

            // Then
            var user = userDataService.GetAdminUserByEmailAddress(registrationModel.Email)!;
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
