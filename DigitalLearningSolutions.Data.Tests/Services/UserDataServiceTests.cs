namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class UserDataServiceTests
    {
        private IUserDataService userDataService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
        }

        [Test]
        public void GetAdminUserByUsername_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            //When
            var returnedAdminUser = userDataService.GetAdminUserByUsername("Username");

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetDelegateUsersByUsername_Returns_delegate_user()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();

            //When
            var returnedDelegateUsers = userDataService.GetDelegateUsersByUsername("SV1234");

            // Then
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetAdminUserById_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var returnedAdminUser = userDataService.GetAdminUserById(7);

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }

        [Test]
        public void GetDelegateUserById_Returns_delegate_users()
        {
            // Given
            var expectedDelegateUsers = UserTestHelper.GetDefaultDelegateUser(jobGroupName: "Nursing / midwifery");

            //When
            var returnedDelegateUser = userDataService.GetDelegateUserById(2);

            // Then
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUsers);
        }

        [Test]
        public void UpdateAdminUser_updates_user()
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Given
                    var firstName = "TestFirstName";
                    var lastName = "TestLastName";
                    var email = "test@email.com";
                    var adminUser = UserTestHelper.GetDefaultAdminUser();
                    adminUser.FirstName = firstName;
                    adminUser.LastName = lastName;
                    adminUser.EmailAddress = email;

                    // When
                    userDataService.UpdateAdminUser(adminUser);
                    var updatedUser = userDataService.GetAdminUserById(7);

                    // Then
                    updatedUser.FirstName.Should().BeEquivalentTo(firstName);
                    updatedUser.LastName.Should().BeEquivalentTo(lastName);
                    updatedUser.EmailAddress.Should().BeEquivalentTo(email);
                }
                finally
                {
                    transaction.Dispose();
                }
            }
        }

        [Test]
        public void UpdateDelegateUser_updates_user()
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Given
                    var firstName = "TestFirstName";
                    var lastName = "TestLastName";
                    var email = "test@email.com";
                    var delegateUser = UserTestHelper.GetDefaultDelegateUser();
                    delegateUser.FirstName = firstName;
                    delegateUser.LastName = lastName;
                    delegateUser.EmailAddress = email;

                    // When
                    userDataService.UpdateDelegateUser(delegateUser);
                    var updatedUser = userDataService.GetDelegateUserById(2);

                    // Then
                    updatedUser.FirstName.Should().BeEquivalentTo(firstName);
                    updatedUser.LastName.Should().BeEquivalentTo(lastName);
                    updatedUser.EmailAddress.Should().BeEquivalentTo(email);
                }
                finally
                {
                    transaction.Dispose();
                }
            }
        }
    }
}
