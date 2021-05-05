namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class UserServiceTests
    {
        private IUserDataService userDataService;
        private ILoginService loginService;
        private IUserService userService;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            loginService = A.Fake<ILoginService>();
            userService = new UserService(userDataService, loginService);
        }

        [Test]
        public void GetUsersByUsername_Returns_admin_user_and_delegate_users()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();
            var expectedDelegateUsers = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(expectedAdminUser);
            A.CallTo(() => userDataService.GetDelegateUsersByUsername(A<string>._))
                .Returns(new List<DelegateUser> { expectedDelegateUsers });

            //When
            var (returnedAdminUser, returnedDelegateUsers) = userService.GetUsersByUsername("Username");

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUsers);
        }

        [Test]
        public void GetUsersById_Returns_admin_user_and_delegate_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(expectedAdminUser);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(expectedDelegateUser);

            //When
            var (returnedAdminUser, returnedDelegateUser) = userService.GetUsersById(1, 2);

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetUsersById_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(expectedAdminUser);

            //When
            var (returnedAdminUser, returnedDelegateUser) = userService.GetUsersById(1, null);

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
            returnedDelegateUser.Should().BeNull();
        }

        [Test]
        public void GetUsersById_Returns_delegate_user()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(expectedDelegateUser);

            //When
            var (returnedAdminUser, returnedDelegateUser) = userService.GetUsersById(null, 2);

            // Then
            returnedAdminUser.Should().BeNull();
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetUsersById_Returns_nulls_with_unexpected_input()
        {
            // When
            var (returnedAdminUser, returnedDelegateUser) =
                userService.GetUsersById(null, null);

            // Then
            returnedAdminUser.Should().BeNull();
            returnedDelegateUser.Should().BeNull();
        }

        [Test]
        public void GetUsersByUsername_with_admin_id_fetches_associated_delegate_users()
        {
            // Given
            var testAdmin = UserTestHelper.GetDefaultAdminUser(emailAddress: "TestAccountAssociation@email.com");
            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._))
                .Returns(testAdmin);
            A.CallTo(() => userDataService.GetDelegateUsersByUsername(A<string>._))
                .Returns(new List<DelegateUser>());

            // When
            userService.GetUsersByUsername("Admin Id");

            // Then
            A.CallTo(() => userDataService.GetDelegateUsersByUsername("TestAccountAssociation@email.com"))
                .MustHaveHappened();
        }

        [Test]
        public void GetAvailableCentres_returns_centres_correctly_ordered()
        {
            // Given
            var inputDelegateList = new List<DelegateUser>
            {
                UserTestHelper.GetDefaultDelegateUser(centreId: 1, centreName: "First Centre"),
                UserTestHelper.GetDefaultDelegateUser(centreId: 3, centreName: "Third Centre"),
                UserTestHelper.GetDefaultDelegateUser(centreId: 4, centreName: "Fourth Centre")
            };
            var inputAdminAccount = UserTestHelper.GetDefaultAdminUser(centreId: 2, centreName: "Second Centre");
            // Expect Admin first, alphabetical after
            var expectedIdOrder = new List<int> { 2, 1, 4, 3 };

            // When
            var result = userService.GetUserCentres(inputAdminAccount, inputDelegateList);
            var resultIdOrder = result.Select(details => details.CentreId).ToList();

            // Then
            Assert.That(resultIdOrder.SequenceEqual(expectedIdOrder));
        }

        [Test]
        public void TryUpdateUsers_with_null_delegate_only_updates_admin()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            string signedInEmail = "oldtest@email.com";
            string password = "password";
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";
            
            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUsersByUsername(A<string>._))
                .Returns(new List<DelegateUser> ());
            A.CallTo(() => loginService.VerifyUsers(password, adminUser, A<List<DelegateUser>>._))
                .Returns((adminUser, new List<DelegateUser>()));
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, A<int>._)).DoesNothing();

            // When
            var result = userService.TryUpdateUserAccountDetails(password, signedInEmail, firstName, lastName, email);

            // Then
            result.Should().BeTrue();
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, A<int>._))
                .MustHaveHappened();
            A.CallTo(() => userDataService.UpdateDelegateUser(A<string>._, A<string>._, A<string>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void TryUpdateUsers_with_null_admin_only_updates_delegate()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            string signedInEmail = "oldtest@email.com";
            string password = "password";
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";

            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateUsersByUsername(A<string>._))
                .Returns(new List<DelegateUser> { delegateUser });
            A.CallTo(() => loginService.VerifyUsers(password, null, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser> { delegateUser }));
            A.CallTo(() => userDataService.UpdateDelegateUser(A<string>._, A<string>._, A<string>._, A<int>._)).DoesNothing();

            // When
            var result = userService.TryUpdateUserAccountDetails(password, signedInEmail, firstName, lastName, email);

            // Then
            result.Should().BeTrue();
            A.CallTo(() => userDataService.UpdateDelegateUser(A<string>._, A<string>._, A<string>._, A<int>._))
                .MustHaveHappened();
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void TryUpdateUsers_with_both_admin_and_delegate_updates_both()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            string signedInEmail = "oldtest@email.com";
            string password = "password";
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";

            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUsersByUsername(A<string>._))
                .Returns(new List<DelegateUser> { delegateUser });
            A.CallTo(() => loginService.VerifyUsers(password, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((adminUser, new List<DelegateUser> { delegateUser }));
            A.CallTo(() => userDataService.UpdateDelegateUser(A<string>._, A<string>._, A<string>._, A<int>._)).DoesNothing();
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, A<int>._)).DoesNothing();

            // When
            var result = userService.TryUpdateUserAccountDetails(password, signedInEmail, firstName, lastName, email);

            // Then
            result.Should().BeTrue();
            A.CallTo(() => userDataService.UpdateDelegateUser(A<string>._, A<string>._, A<string>._, A<int>._))
                .MustHaveHappened();
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, A<int>._))
                .MustHaveHappened();
        }

        [Test]
        public void TryUpdateUsers_with_incorrect_password_doesnt_update()
        {
            // Given
            string signedInEmail = "oldtest@email.com";
            string password = "incorrectPassword";
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";
            A.CallTo(() => userDataService.GetAdminUserByUsername(signedInEmail)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateUsersByUsername(signedInEmail))
                .Returns(new List<DelegateUser>());
            A.CallTo(() => loginService.VerifyUsers(password, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser>()));

            // When
            var result = userService.TryUpdateUserAccountDetails(password, signedInEmail, firstName, lastName, email);

            // Then
            result.Should().BeFalse();
            A.CallTo(() => userDataService.UpdateDelegateUser(A<string>._, A<string>._, A<string>._, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, A<int>._))
                .MustNotHaveHappened();
        }
    }
}
