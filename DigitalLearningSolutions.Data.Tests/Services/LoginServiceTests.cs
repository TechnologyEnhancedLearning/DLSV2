namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private LoginService loginService = null!;
        private IUserService userService = null!;
        private IUserVerificationService userVerificationService = null!;

        [SetUp]
        public void Setup()
        {
            userVerificationService = A.Fake<IUserVerificationService>(x => x.Strict());
            userService = A.Fake<IUserService>(x => x.Strict());

            loginService = new LoginService(userService, userVerificationService);
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
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(new List<AdminUser>(), new List<DelegateUser>()));

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
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(new List<AdminUser>(), new List<DelegateUser>()));
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
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(new List<AdminUser>(), new List<DelegateUser>()));
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
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(new List<AdminUser>(), new List<DelegateUser>()));
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
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(new List<AdminUser>(), new List<DelegateUser> { delegateUser }));

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
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(adminUsers, new List<DelegateUser>()));

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
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(new List<AdminUser> { adminUser }, new List<DelegateUser> { delegateUser }));

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
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(new List<AdminUser> { adminUser }, new List<DelegateUser>()));
            A.CallTo(() => userService.ResetFailedLoginCount(adminUser)).DoesNothing();
            A.CallTo(
                () => userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(
                    adminUser,
                    Password
                )
            ).Returns(new List<DelegateUser>());
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
                A.CallTo(
                    () => userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUser(
                        A<DelegateUser?>._,
                        A<string>._
                    )
                ).MustNotHaveHappened();
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
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(new List<AdminUser>(), new List<DelegateUser> { delegateUser }));
            A.CallTo(
                () => userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUser(
                    delegateUser,
                    Password
                )
            ).Returns(linkedAdminUser);
            A.CallTo(
                () => userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(
                    null,
                    Password
                )
            ).Returns(new List<DelegateUser>());
            A.CallTo(() => userService.GetUsersWithActiveCentres(linkedAdminUser, A<List<DelegateUser>>._))
                .Returns((linkedAdminUser, new List<DelegateUser> { delegateUser }));
            A.CallTo(() => userService.GetUserCentres(linkedAdminUser, A<List<DelegateUser>>._)).Returns(
                new List<CentreUserDetails>
                    { new CentreUserDetails(linkedAdminUser.CentreId, linkedAdminUser.CentreName, true) }
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
    }
}
