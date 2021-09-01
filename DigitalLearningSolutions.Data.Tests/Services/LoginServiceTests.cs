namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using NUnit.Framework;

    public class LoginServiceTests
    {
        private ICryptoService cryptoService = null!;
        private LoginService loginService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            cryptoService = A.Fake<ICryptoService>();

            loginService = new LoginService(userDataService, cryptoService);
        }

        [Test]
        public void VerifyUsers_Returns_verified_admin_user()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            A.CallTo(() => cryptoService.VerifyHashedPassword("Automatically Verified", A<string>._)).Returns(true);

            //When
            var adminUser = UserTestHelper.GetDefaultAdminUser(password: "Automatically Verified");
            var (verifiedAdminUser, _) = loginService.VerifyUsers(
                "password",
                adminUser,
                new List<DelegateUser>()
            );

            // Then
            Assert.AreEqual(adminUser, verifiedAdminUser);
        }

        [Test]
        public void VerifyUsers_Does_not_return_unverified_admin_user()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);

            //When
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var (verifiedAdminUser, _) = loginService.VerifyUsers(
                "password",
                adminUser,
                new List<DelegateUser>()
            );

            // Then
            Assert.IsNull(verifiedAdminUser);
        }

        [Test]
        public void VerifyUsers__Returns_verified_delegate_users()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            A.CallTo(() => cryptoService.VerifyHashedPassword("Automatically Verified", A<string>._)).Returns(true);

            //When
            var firstDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 1);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Fails Verification", id: 2);
            var thirdDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 3);

            var delegateUsers = new List<DelegateUser>
            {
                firstDelegateUser,
                secondDelegateUser,
                thirdDelegateUser
            };

            var (_, verifiedDelegateUsers) = loginService.VerifyUsers(
                "password",
                UserTestHelper.GetDefaultAdminUser(),
                delegateUsers
            );

            // Then
            Assert.Contains(firstDelegateUser, verifiedDelegateUsers);
            Assert.Contains(thirdDelegateUser, verifiedDelegateUsers);
        }

        [Test]
        public void VerifyUsers_Filters_out_unverified_delegate_users()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            A.CallTo(() => cryptoService.VerifyHashedPassword("Automatically Verified", A<string>._)).Returns(true);

            //When
            var firstDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 1);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Fails Verification", id: 2);
            var thirdDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 3);

            var delegateUsers = new List<DelegateUser>
            {
                firstDelegateUser,
                secondDelegateUser,
                thirdDelegateUser
            };

            var (_, verifiedDelegateUsers) = loginService.VerifyUsers(
                "password",
                UserTestHelper.GetDefaultAdminUser(),
                delegateUsers
            );

            // Then
            Assert.IsFalse(verifiedDelegateUsers.Contains(secondDelegateUser));
        }

        [Test]
        public void VerifyUsers_Returns_no_delegates_if_all_unverified()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);

            //When
            var firstDelegateUser = UserTestHelper.GetDefaultDelegateUser(1);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            var thirdDelegateUser = UserTestHelper.GetDefaultDelegateUser(3);

            var delegateUsers = new List<DelegateUser>
            {
                firstDelegateUser,
                secondDelegateUser,
                thirdDelegateUser
            };

            var (_, verifiedDelegateUsers) = loginService.VerifyUsers(
                "password",
                UserTestHelper.GetDefaultAdminUser(),
                delegateUsers
            );

            // Then
            Assert.IsEmpty(verifiedDelegateUsers);
        }

        [Test]
        public void VerifyUsers_Returns_no_delegate_users_when_delegate_stored_password_is_empty()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            var delegateUserWithoutPassword = UserTestHelper.GetDefaultDelegateUser(password: string.Empty);

            //When
            var (_, returnedDelegateList) = loginService.VerifyUsers(
                "password",
                UserTestHelper.GetDefaultAdminUser(),
                new List<DelegateUser> { delegateUserWithoutPassword }
            );

            // Then
            Assert.IsEmpty(returnedDelegateList);
        }

        [Test]
        public void GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_delegate_email_and_alias_is_empty()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: null, aliasId: null);
            var delegateUsers = new List<DelegateUser> { delegateUser };

            // When
            var returnedAdminUser = loginService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.IsNull(returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_no_admin_account_is_associated_with_delegate()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var delegateUsers = new List<DelegateUser> { delegateUser };
            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(null);

            // When
            var returnedAdminUser = loginService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.IsNull(returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_admin_account_associated_only_by_email_is_at_different_centre()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(centreId: 2);
            var delegateUsers = new List<DelegateUser> { delegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 5);
            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(associatedAdminUser);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedAdminUser = loginService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.IsNull(returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_admin_account_associated_only_by_alias_is_at_different_centre()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(centreId: 2, emailAddress: null, aliasId: "Test");
            var delegateUsers = new List<DelegateUser> { delegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 5);
            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(associatedAdminUser);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedAdminUser = loginService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.IsNull(returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_verified_admin_account_associated_with_delegate_by_email()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var delegateUsers = new List<DelegateUser> { delegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(associatedAdminUser);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedAdminUser = loginService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.AreEqual(associatedAdminUser, returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_verified_admin_account_associated_with_delegate_by_alias()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: null, aliasId: "Test");
            var delegateUsers = new List<DelegateUser> { delegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(associatedAdminUser);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedAdminUser = loginService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.AreEqual(associatedAdminUser, returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_verified_admin_account_associated_with_delegate_by_email_and_alias()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(aliasId: "Test");
            var delegateUsers = new List<DelegateUser> { delegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserByUsername(A<string>._)).Returns(associatedAdminUser);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedAdminUser = loginService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.AreEqual(associatedAdminUser, returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_throws_exception_with_multiple_different_matching_admins()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(aliasId: "Test");
            var delegateUsers = new List<DelegateUser> { delegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser();
            var secondAssociatedAdminUser = UserTestHelper.GetDefaultAdminUser(id: 1);
            A.CallTo(() => userDataService.GetAdminUserByUsername(delegateUser.EmailAddress!)).Returns(associatedAdminUser);
            A.CallTo(() => userDataService.GetAdminUserByUsername(delegateUser.AliasId!)).Returns(secondAssociatedAdminUser);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // Then
            Assert.Throws<InvalidOperationException>(() => loginService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            ));
        }
    }
}
