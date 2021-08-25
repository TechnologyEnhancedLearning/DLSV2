namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using NUnit.Framework;

    public class UserServiceTests
    {
        private ILoginService loginService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

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
        public void GetUsersWithActiveCentres_returns_users_with_active_centres()
        {
            // Given
            var inputAdminAccount =
                UserTestHelper.GetDefaultAdminUser(1, centreName: "First Centre", centreActive: true);
            var inputDelegateList = new List<DelegateUser>
            {
                UserTestHelper.GetDefaultDelegateUser(2, centreName: "Second Centre", centreActive: true),
                UserTestHelper.GetDefaultDelegateUser(3, centreName: "Third Centre", centreActive: true),
                UserTestHelper.GetDefaultDelegateUser(4, centreName: "Fourth Centre", centreActive: true)
            };
            var expectedDelegateIds = new List<int> { 2, 3, 4 };

            // When
            var (resultAdminUser, resultDelegateUsers) =
                userService.GetUsersWithActiveCentres(inputAdminAccount, inputDelegateList);
            var resultDelegateIds = resultDelegateUsers.Select(du => du.Id).ToList();

            // Then
            Assert.That(resultAdminUser.IsSameOrEqualTo(inputAdminAccount));
            Assert.That(resultDelegateIds.SequenceEqual(expectedDelegateIds));
        }

        [Test]
        public void GetUsersWithActiveCentres_does_not_return_users_with_inactive_centres()
        {
            // Given
            var inputAdminAccount =
                UserTestHelper.GetDefaultAdminUser(1, centreName: "First Centre", centreActive: false);
            var inputDelegateList = new List<DelegateUser>
            {
                UserTestHelper.GetDefaultDelegateUser(2, centreName: "Second Centre", centreActive: false),
                UserTestHelper.GetDefaultDelegateUser(3, centreName: "Third Centre", centreActive: false),
                UserTestHelper.GetDefaultDelegateUser(4, centreName: "Fourth Centre", centreActive: false)
            };

            // When
            var (resultAdminUser, resultDelegateUsers) =
                userService.GetUsersWithActiveCentres(inputAdminAccount, inputDelegateList);

            // Then
            Assert.IsNull(resultAdminUser);
            Assert.That(resultDelegateUsers.IsNullOrEmpty);
        }

        [Test]
        public void GetUserCentres_returns_centres_correctly_ordered()
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
            string password = "password";
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";
            var accountDetailsData =
                new AccountDetailsData(adminUser.Id, null, password, firstName, lastName, email, null);

            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(adminUser.EmailAddress!)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(new List<DelegateUser>());
            A.CallTo(() => loginService.VerifyUsers(password, adminUser, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(adminUser, new List<DelegateUser>()));
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, null, A<int>._))
                .DoesNothing();

            // When
            userService.UpdateUserAccountDetails(accountDetailsData);

            // Then
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, null, A<int>._))
                .MustHaveHappened();
            A.CallTo(() => userDataService.UpdateDelegateUsers(A<string>._, A<string>._, A<string>._, null, A<int[]>._))
                .MustNotHaveHappened();
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void TryUpdateUsers_with_null_admin_only_updates_delegate()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            string password = "password";
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";
            var accountDetailsData =
                new AccountDetailsData(null, delegateUser.Id, password, firstName, lastName, email, null);
            var centreAnswersData = new CentreAnswersData(2, 1, null, null, null, null, null, null);

            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress!)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(delegateUser.EmailAddress!))
                .Returns(new List<DelegateUser> { delegateUser });
            A.CallTo(() => loginService.VerifyUsers(password, null, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(null, new List<DelegateUser> { delegateUser }));
            A.CallTo(() => userDataService.UpdateDelegateUsers(A<string>._, A<string>._, A<string>._, null, A<int[]>._))
                .DoesNothing();

            // When
            userService.UpdateUserAccountDetails(accountDetailsData, centreAnswersData);

            // Then
            A.CallTo(() => userDataService.UpdateDelegateUsers(A<string>._, A<string>._, A<string>._, null, A<int[]>._))
                .MustHaveHappened();
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, null, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(() => userDataService.UpdateDelegateUserCentrePrompts(2, 1, null, null, null, null, null, null))
                .MustHaveHappened();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void TryUpdateUsers_with_both_admin_and_delegate_updates_both()
        {
            // Given
            string signedInEmail = "oldtest@email.com";
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: signedInEmail);
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: signedInEmail);
            string password = "password";
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";
            var accountDetailsData =
                new AccountDetailsData(adminUser.Id, delegateUser.Id, password, firstName, lastName, email, null);
            var centreAnswersData = new CentreAnswersData(2, 1, null, null, null, null, null, null);

            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(signedInEmail)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(signedInEmail))
                .Returns(new List<DelegateUser> { delegateUser });
            A.CallTo(() => loginService.VerifyUsers(password, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(adminUser, new List<DelegateUser> { delegateUser }));
            A.CallTo(() => userDataService.UpdateDelegateUsers(A<string>._, A<string>._, A<string>._, null, A<int[]>._))
                .DoesNothing();
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, null, A<int>._))
                .DoesNothing();

            // When
            userService.UpdateUserAccountDetails(accountDetailsData, centreAnswersData);

            // Then
            A.CallTo(() => userDataService.UpdateDelegateUsers(A<string>._, A<string>._, A<string>._, null, A<int[]>._))
                .MustHaveHappened();
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, null, A<int>._))
                .MustHaveHappened();
            A.CallTo(() => userDataService.UpdateDelegateUserCentrePrompts(2, 1, null, null, null, null, null, null))
                .MustHaveHappened();
        }

        [Test]
        public void TryUpdateUsers_with_incorrect_password_doesnt_update()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            string signedInEmail = "oldtest@email.com";
            string password = "incorrectPassword";
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";
            var accountDetailsData =
                new AccountDetailsData(adminUser.Id, delegateUser.Id, password, firstName, lastName, email, null);
            var centreAnswersData = new CentreAnswersData(2, 1, null, null, null, null, null, null);

            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(signedInEmail)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(signedInEmail))
                .Returns(new List<DelegateUser>());
            A.CallTo(() => loginService.VerifyUsers(password, A<AdminUser>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet());

            // When
            userService.UpdateUserAccountDetails(accountDetailsData, centreAnswersData);

            // Then
            A.CallTo(() => userDataService.UpdateDelegateUsers(A<string>._, A<string>._, A<string>._, null, A<int[]>._))
                .MustNotHaveHappened();
            A.CallTo(() => userDataService.UpdateAdminUser(A<string>._, A<string>._, A<string>._, null, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(
                    () => userDataService.UpdateDelegateUserCentrePrompts(
                        A<int>._,
                        A<int>._,
                        A<string?>._,
                        A<string?>._,
                        A<string?>._,
                        A<string?>._,
                        A<string?>._,
                        A<string?>._
                    )
                )
                .MustNotHaveHappened();
        }

        [Test]
        public void NewEmailAddressIsValid_returns_true_with_unchanged_email()
        {
            // Given
            const string email = "email@test.com";
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: email);
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: email);
            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);

            // When
            var result = userService.NewEmailAddressIsValid(email, 7, 2, 2);

            // Then
            result.Should().BeTrue();
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).MustNotHaveHappened();
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).MustNotHaveHappened();
        }

        [Test]
        public void NewEmailAddressIsValid_returns_false_with_existing_admin_with_email()
        {
            // Given
            const string email = "email@test.com";
            const string oldEmail = "oldemail@test.com";
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: oldEmail);
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: oldEmail);
            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email))
                .Returns(UserTestHelper.GetDefaultAdminUser(1, emailAddress: email));
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).Returns(new List<DelegateUser>());

            // When
            var result = userService.NewEmailAddressIsValid(email, 7, 2, 2);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void NewEmailAddressIsValid_returns_false_with_existing_delegate_at_centre_with_email()
        {
            // Given
            const string email = "email@test.com";
            const string oldEmail = "oldemail@test.com";
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: oldEmail);
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: oldEmail);
            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).Returns
                (new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(3, emailAddress: email) });

            // When
            var result = userService.NewEmailAddressIsValid(email, 7, 2, 2);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void NewEmailAddressIsValid_returns_true_with_existing_delegate_at_different_centre_with_email()
        {
            // Given
            const string email = "email@test.com";
            const string oldEmail = "oldemail@test.com";
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: oldEmail);
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: oldEmail);
            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).Returns
                (new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(3, emailAddress: email, centreId: 3) });

            // When
            var result = userService.NewEmailAddressIsValid(email, 7, 2, 2);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void NewEmailAddressIsValid_returns_true_for_admin_only_with_unchanged_email()
        {
            // Given
            const string email = "email@test.com";
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: email);
            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);

            // When
            var result = userService.NewEmailAddressIsValid(email, 7, null, 2);

            // Then
            result.Should().BeTrue();
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).MustNotHaveHappened();
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).MustNotHaveHappened();
        }

        [Test]
        public void NewEmailAddressIsValid_returns_true_for_delegate_only_with_unchanged_email()
        {
            // Given
            const string email = "email@test.com";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: email);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);

            // When
            var result = userService.NewEmailAddressIsValid(email, null, 2, 2);

            // Then
            result.Should().BeTrue();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).MustNotHaveHappened();
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).MustNotHaveHappened();
        }

        [Test]
        public void ResetFailedLoginCount_resets_count()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(failedLoginCount: 4);

            // When
            userService.ResetFailedLoginCount(adminUser);

            // Then
            A.CallTo(() => userDataService.UpdateAdminUserFailedLoginCount(adminUser.Id, 0)).MustHaveHappened();
        }

        [Test]
        public void ResetFailedLoginCount_doesnt_call_data_service_with_FailedLoginCount_of_zero()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(failedLoginCount: 0);

            // When
            userService.ResetFailedLoginCount(adminUser);

            // Then
            A.CallTo(() => userDataService.UpdateAdminUserFailedLoginCount(adminUser.Id, 0)).MustNotHaveHappened();
        }

        [Test]
        public void IncrementFailedLoginCount_updates_count_to_expected_value()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(failedLoginCount: 4);
            const int expectedCount = 5;

            // When
            userService.IncrementFailedLoginCount(adminUser);

            // Then
            A.CallTo(() => userDataService.UpdateAdminUserFailedLoginCount(adminUser.Id, expectedCount)).MustHaveHappened();
        }
    }
}
