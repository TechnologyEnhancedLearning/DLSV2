namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class LoginServiceTests
    {
        private const string Username = "Username";
        private const string Password = "Password";
        private ICryptoService cryptoService = null!;
        private LoginService loginService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>(x => x.Strict());
            cryptoService = A.Fake<ICryptoService>(x => x.Strict());
            userService = A.Fake<IUserService>(x => x.Strict());

            loginService = new LoginService(userService, userDataService, cryptoService);
        }

        [Test]
        public void AttemptLogin_returns_no_account_found_with_no_accounts()
        {
            // Given
            A.CallTo(() => userService.GetUsersByUsername(Username))
                .Returns((new List<AdminUser>(), new List<DelegateUser>()));

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.InvalidUsername);
                result.LogInAdmin.Should().BeNull();
                result.LogInDelegates.Should().BeEmpty();
            }
        }

        [Test]
        public void AttemptLogin_returns_invalid_password_with_no_verified_delegate_accounts()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userService.GetUsersByUsername(Username))
                .Returns((new List<AdminUser>(), new List<DelegateUser> { delegateUser }));
            A.CallTo(() => cryptoService.VerifyHashedPassword(delegateUser.Password!, Password))
                .Returns(false);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.InvalidPassword);
                result.LogInAdmin.Should().BeNull();
                result.LogInDelegates.Should().BeEmpty();
            }
        }

        [Test]
        public void AttemptLogin_returns_invalid_password_with_no_verified_admin_accounts_and_increments_login_count()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userService.GetUsersByUsername(Username))
                .Returns((new List<AdminUser> { adminUser }, new List<DelegateUser>()));
            A.CallTo(() => cryptoService.VerifyHashedPassword(adminUser.Password!, Password))
                .Returns(false);
            A.CallTo(() => userService.IncrementFailedLoginCount(adminUser)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.IncrementFailedLoginCount(adminUser)).MustHaveHappened();
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.InvalidPassword);
                result.LogInAdmin.Should().BeNull();
                result.LogInDelegates.Should().BeEmpty();
            }
        }

        [Test]
        public void AttemptLogin_returns_locked_account_and_increments_login_count_when_account_is_about_to_be_locked()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(failedLoginCount: 4);
            A.CallTo(() => userService.GetUsersByUsername(Username))
                .Returns((new List<AdminUser> { adminUser }, new List<DelegateUser>()));
            A.CallTo(() => cryptoService.VerifyHashedPassword(adminUser.Password!, Password))
                .Returns(false);
            A.CallTo(() => userService.IncrementFailedLoginCount(adminUser)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.IncrementFailedLoginCount(adminUser)).MustHaveHappened();
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.AccountLocked);
                result.LogInAdmin.Should().Be(adminUser);
                result.LogInDelegates.Should().BeEmpty();
            }
        }

        [Test]
        public void AttemptLogin_returns_locked_account_and_increments_login_count_when_account_was_already_locked()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(failedLoginCount: 6);
            A.CallTo(() => userService.GetUsersByUsername(Username))
                .Returns((new List<AdminUser> { adminUser }, new List<DelegateUser>()));
            A.CallTo(() => cryptoService.VerifyHashedPassword(adminUser.Password!, Password))
                .Returns(false);
            A.CallTo(() => userService.IncrementFailedLoginCount(adminUser)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.IncrementFailedLoginCount(adminUser)).MustHaveHappened();
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.AccountLocked);
                result.LogInAdmin.Should().Be(adminUser);
                result.LogInDelegates.Should().BeEmpty();
            }
        }

        [Test]
        public void AttemptLogin_returns_unapproved_account_when_verified_delegate_account_is_unapproved()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(approved: false);
            A.CallTo(() => userService.GetUsersByUsername(Username))
                .Returns((new List<AdminUser>(), new List<DelegateUser> { delegateUser }));
            A.CallTo(() => cryptoService.VerifyHashedPassword(delegateUser.Password!, Password))
                .Returns(true);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.AccountNotApproved);
                result.LogInAdmin.Should().BeNull();
                result.LogInDelegates.Should().BeEmpty();
            }
        }

        [Test]
        public void AttemptLogin_throws_exception_with_multiple_verified_admins()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var secondAdminUser = UserTestHelper.GetDefaultAdminUser(8, emailAddress: "test@test.com");
            var adminUsers = new List<AdminUser> { adminUser, secondAdminUser };
            A.CallTo(() => userService.GetUsersByUsername(Username))
                .Returns((adminUsers, new List<DelegateUser>()));
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string?>._, Password))
                .Returns(true);

            // Then
            Assert.Throws<InvalidOperationException>(() => loginService.AttemptLogin(Username, Password));
        }

        [Test]
        public void AttemptLogin_throws_exception_with_accounts_with_mismatched_emails()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userService.GetUsersByUsername(Username))
                .Returns((new List<AdminUser> { adminUser }, new List<DelegateUser> { delegateUser }));
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string?>._, Password))
                .Returns(true);

            // Then
            var exception = Assert.Throws<Exception>(() => loginService.AttemptLogin(Username, Password));
            exception.Message.Should().Be("Not all accounts have the same email");
        }

        [Test]
        public void
            AttemptLogin_does_not_find_linked_admins_if_verified_admin_already_found_and_no_linked_delegates_and_returns_single_centre_login_result()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: "email@test.com");
            A.CallTo(() => userService.GetUsersByUsername(Username))
                .Returns((new List<AdminUser> { adminUser }, new List<DelegateUser>()));
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string?>._, Password))
                .Returns(true);
            A.CallTo(() => userService.ResetFailedLoginCount(adminUser)).DoesNothing();
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(new List<DelegateUser>());
            A.CallTo(() => userService.GetUsersWithActiveCentres(adminUser, A<List<DelegateUser>>._))
                .Returns((adminUser, new List<DelegateUser>()));
            A.CallTo(() => userService.GetUserCentres(adminUser, A<List<DelegateUser>>._)).Returns(
                new List<CentreUserDetails> { new CentreUserDetails(adminUser.CentreId, adminUser.CentreName, true) }
            );

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).MustNotHaveHappened();
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.LogIntoSingleCentre);
                result.LogInAdmin.Should().Be(adminUser);
                result.LogInDelegates.Should().BeEmpty();
            }
        }

        [Test]
        public void
            AttemptLogin_finds_linked_admin_if_no_admin_and_single_delegate_exists_and_returns_single_centre_login_result()
        {
            // Given
            var linkedAdminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: "email@test.com");
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userService.GetUsersByUsername(Username))
                .Returns((new List<AdminUser>(), new List<DelegateUser> { delegateUser }));
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string?>._, Password))
                .Returns(true);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress!))
                .Returns(linkedAdminUser);
            A.CallTo(() => userService.GetUsersWithActiveCentres(linkedAdminUser, A<List<DelegateUser>>._))
                .Returns((linkedAdminUser, new List<DelegateUser> { delegateUser }));
            A.CallTo(() => userService.GetUserCentres(linkedAdminUser, A<List<DelegateUser>>._)).Returns(
                new List<CentreUserDetails> { new CentreUserDetails(linkedAdminUser.CentreId, linkedAdminUser.CentreName, true) }
            );

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.LogIntoSingleCentre);
                result.LogInAdmin.Should().Be(linkedAdminUser);
                result.LogInDelegates.Single().Should().Be(delegateUser);
            }
        }

        #region old tests

        [Test]
        public void VerifyUsers_Returns_verified_admin_user()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            A.CallTo(() => cryptoService.VerifyHashedPassword("Automatically Verified", A<string>._)).Returns(true);
            var adminUser = UserTestHelper.GetDefaultAdminUser(password: "Automatically Verified");
            var adminUsers = new List<AdminUser> { adminUser };

            //When
            var (verifiedAdminUsers, _) = loginService.VerifyUsers(
                "password",
                adminUsers,
                new List<DelegateUser>()
            );

            // Then
            Assert.AreEqual(adminUser, verifiedAdminUsers.First());
        }

        [Test]
        public void VerifyUsers_Does_not_return_unverified_admin_user()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);

            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var adminUsers = new List<AdminUser> { adminUser };

            //When
            var (verifiedAdminUsers, _) = loginService.VerifyUsers(
                "password",
                adminUsers,
                new List<DelegateUser>()
            );

            // Then
            Assert.IsEmpty(verifiedAdminUsers);
        }

        [Test]
        public void VerifyUsers__Returns_verified_delegate_users()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            A.CallTo(() => cryptoService.VerifyHashedPassword("Automatically Verified", A<string>._)).Returns(true);

            var firstDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 1);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Fails Verification", id: 2);
            var thirdDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 3);

            var delegateUsers = new List<DelegateUser>
            {
                firstDelegateUser,
                secondDelegateUser,
                thirdDelegateUser
            };

            var adminUsers = new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() };

            //When
            var (_, verifiedDelegateUsers) = loginService.VerifyUsers(
                "password",
                adminUsers,
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

            var firstDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 1);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Fails Verification", id: 2);
            var thirdDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 3);

            var delegateUsers = new List<DelegateUser>
            {
                firstDelegateUser,
                secondDelegateUser,
                thirdDelegateUser
            };

            var adminUsers = new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() };

            //When
            var (_, verifiedDelegateUsers) = loginService.VerifyUsers(
                "password",
                adminUsers,
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

            var firstDelegateUser = UserTestHelper.GetDefaultDelegateUser(1);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            var thirdDelegateUser = UserTestHelper.GetDefaultDelegateUser(3);

            var delegateUsers = new List<DelegateUser>
            {
                firstDelegateUser,
                secondDelegateUser,
                thirdDelegateUser
            };

            var adminUsers = new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() };

            //When
            var (_, verifiedDelegateUsers) = loginService.VerifyUsers(
                "password",
                adminUsers,
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
            var adminUsers = new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() };

            //When
            var (_, returnedDelegateList) = loginService.VerifyUsers(
                "password",
                adminUsers,
                new List<DelegateUser> { delegateUserWithoutPassword }
            );

            // Then
            Assert.IsEmpty(returnedDelegateList);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_delegate_email_and_alias_is_empty()
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
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(null);

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
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(associatedAdminUser);
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
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(associatedAdminUser);
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
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(associatedAdminUser);
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
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_verified_admin_account_associated_with_delegate_by_email_with_single_admin_matched_twice()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: "test@test.com");
            var delegateUsers = new List<DelegateUser> { delegateUser, delegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress!))
                .Returns(associatedAdminUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(secondDelegateUser.EmailAddress!))
                .Returns(associatedAdminUser);
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
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: "test@test.com");
            var delegateUsers = new List<DelegateUser> { delegateUser, secondDelegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser();
            var secondAssociatedAdminUser = UserTestHelper.GetDefaultAdminUser(1);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress!))
                .Returns(associatedAdminUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(secondDelegateUser.EmailAddress!))
                .Returns(secondAssociatedAdminUser);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // Then
            Assert.Throws<InvalidOperationException>(
                () => loginService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                    delegateUsers,
                    "password"
                )
            );
        }

        #endregion
    }
}
