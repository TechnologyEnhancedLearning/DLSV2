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
            service = new RegistrationDataService(connection);
            userDataService = new UserDataService(connection);
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
        }

        [Test]
        public void Sets_all_fields_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            service.RegisterAdmin(registrationModel);

            // Then
            var user = userDataService.GetAdminUserByEmailAddress(registrationModel.PrimaryEmail)!;
            user.FirstName.Should().Be(registrationModel.FirstName);
            user.LastName.Should().Be(registrationModel.LastName);
            user.CentreId.Should().Be(registrationModel.Centre);
            user.Password.Should().Be(registrationModel.PasswordHash);
            user.IsCentreAdmin.Should().BeTrue();
            user.IsCentreManager.Should().BeTrue();
        }

        [Test]
        public void Sets_notification_preferences_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            service.RegisterAdmin(registrationModel);

            // Then
            var user = userDataService.GetAdminUserByEmailAddress(registrationModel.PrimaryEmail)!;
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
