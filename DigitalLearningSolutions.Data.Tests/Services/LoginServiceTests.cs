namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
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
        public void AttemptLogin_throws_InvalidOperationException_with_multiple_verified_admins()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var secondAdminUser = UserTestHelper.GetDefaultAdminUser(8, emailAddress: "test@test.com");
            var adminUsers = new List<AdminUser> { adminUser, secondAdminUser };
            GivenAdminUsersAreVerified(adminUsers);

            // Then
            Assert.Throws<InvalidOperationException>(() => loginService.AttemptLogin(Username, Password));
        }

        [Test]
        public void
            AttemptLogin_throws_LoginWithMultipleEmailsException_when_verified_admin_email_and_delegate_email_dont_match()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            GivenAdminUserAndDelegateUserAreVerified(adminUser, delegateUser);

            // Then
            var exception =
                Assert.Throws<LoginWithMultipleEmailsException>(() => loginService.AttemptLogin(Username, Password));
            exception.Message.Should().Be("Not all accounts have the same email");
        }

        [Test]
        public void AttemptLogin_throws_LoginWithMultipleEmailsException_when_multiple_delegate_emails_dont_match()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: "test@test.com");
            var delegateUsers = new List<DelegateUser> { delegateUser, secondDelegateUser };
            GivenDelegateUsersAreVerified(delegateUsers);

            // Then
            var exception =
                Assert.Throws<LoginWithMultipleEmailsException>(() => loginService.AttemptLogin(Username, Password));
            exception.Message.Should().Be("Not all accounts have the same email");
        }

        [Test]
        public void AttemptLogin_returns_invalid_password_with_no_verified_delegate_accounts_and_no_admin_account()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var delegateUsers = new List<DelegateUser> { delegateUser };
            GivenDelegateAccountsFailedVerification(delegateUsers);

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
        public void
            AttemptLogin_returns_invalid_password_with_no_verified_admin_account_and_no_delegate_accounts_and_increments_login_count()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            GivenAdminAccountFailedVerification(adminUser);

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
            GivenAdminAccountFailedVerification(adminUser);

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
            GivenAdminAccountFailedVerification(adminUser);

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
            var delegateUsers = new List<DelegateUser> { delegateUser };
            GivenDelegateUsersAreVerified(delegateUsers);

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
        public void AttemptLogin_returns_inactive_centre_if_all_centres_are_inactive()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(2, 3);
            var delegateUsers = new List<DelegateUser> { delegateUser, secondDelegateUser };
            GivenDelegateUsersAreVerified(delegateUsers);
            GivenNoLinkedAccountsFound();
            GivenNoActiveCentresAreFound();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.InactiveCentre);
                result.LogInAdmin.Should().Be(null);
                result.LogInDelegates.Should().BeEmpty();
            }
        }

        [Test]
        public void
            AttemptLogin_does_not_find_linked_admins_if_verified_admin_already_found_and_no_linked_delegates_and_returns_single_centre_login_result()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: "email@test.com");
            var adminUsers = new List<AdminUser> { adminUser };
            GivenAdminUsersAreVerified(adminUsers);
            GivenResetFailedLoginCountDoesNothing(adminUser);
            GivenNoLinkedDelegateAccountsFound();
            AdminUserHasActiveCentre(adminUser);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                        A<List<DelegateUser>>._,
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
            AttemptLogin_does_not_find_linked_admins_if_verified_admin_already_found_but_linked_delegates_are_found_and_returns_single_centre_login_result()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: "email@test.com");
            var adminUsers = new List<AdminUser> { adminUser };

            var linkedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(emailAddress: "email@test.com") };

            GivenAdminUsersAreVerified(adminUsers);
            GivenResetFailedLoginCountDoesNothing(adminUser);
            GivenLinkedDelegateAccountsFound(linkedDelegateUsers);
            AdminUserHasActiveCentre(adminUser);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                        A<List<DelegateUser>>._,
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
            var delegateUsers = new List<DelegateUser> { delegateUser };
            GivenDelegateUsersAreVerified(delegateUsers);
            GivenLinkedAdminUserIsFound(linkedAdminUser);
            GivenNoLinkedDelegateAccountsFound();
            GivenSingleActiveCentreIsFound(linkedAdminUser, delegateUsers);

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

        [Test]
        public void
            AttemptLogin_does_not_use_linked_admin_if_admin_is_locked_and_single_delegate_exists_and_returns_single_centre_login_result()
        {
            // Given
            var linkedAdminUser = UserTestHelper.GetDefaultAdminUser(
                emailAddress: "email@test.com",
                failedLoginCount: 6
            );
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var delegateUsers = new List<DelegateUser> { delegateUser };
            GivenDelegateUsersAreVerified(delegateUsers);
            GivenLinkedAdminUserIsFound(linkedAdminUser);
            GivenNoLinkedDelegateAccountsFound();
            GivenDelegateUserHasActiveCentre(delegateUser);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.LogIntoSingleCentre);
                result.LogInAdmin.Should().BeNull();
                result.LogInDelegates.Single().Should().Be(delegateUser);
            }
        }

        [Test]
        public void
            AttemptLogin_does_not_increment_failed_count_for_locked_admin_if_delegate_exists_and_returns_single_centre_login_result()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: "email@test.com", failedLoginCount: 6);
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            GivenAdminUserAndDelegateUserAreVerified(adminUser, delegateUser);
            GivenNoLinkedAccountsFound();
            GivenDelegateUserHasActiveCentre(delegateUser);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.IncrementFailedLoginCount(adminUser)).MustNotHaveHappened();
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.LogIntoSingleCentre);
                result.LogInAdmin.Should().BeNull();
                result.LogInDelegates.Single().Should().Be(delegateUser);
            }
        }

        [Test]
        public void AttemptLogin_find_multiple_delegates_and_returns_choose_a_centre_login_result()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(2, 3);
            var delegateUsers = new List<DelegateUser> { delegateUser, secondDelegateUser };
            GivenDelegateUsersAreVerified(delegateUsers);
            GivenNoLinkedAccountsFound();
            GivenMultipleActiveCentresAreFound(null, delegateUsers);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.ChooseACentre);
                result.LogInAdmin.Should().Be(null);
                result.LogInDelegates.Should().BeEquivalentTo(delegateUsers);
            }
        }

        [Test]
        public void AttemptLogin_find_admin_and_delegate_at_different_centres_and_returns_choose_a_centre_login_result()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: "email@test.com");
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(3);
            var delegateUsers = new List<DelegateUser> { delegateUser };
            GivenAdminUserAndDelegateUserAreVerified(adminUser, delegateUser);
            GivenResetFailedLoginCountDoesNothing(adminUser);
            GivenNoLinkedAccountsFound();
            GivenMultipleActiveCentresAreFound(adminUser, delegateUsers);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.ChooseACentre);
                result.LogInAdmin.Should().Be(adminUser);
                result.LogInDelegates.Should().BeEquivalentTo(delegateUsers);
            }
        }

        private void GivenAdminUsersFoundByUsername(List<AdminUser> adminUsers)
        {
            A.CallTo(() => userService.GetUsersByUsername(Username)).Returns((adminUsers, new List<DelegateUser>()));
        }

        private void GivenDelegateUsersFoundByUserName(List<DelegateUser> delegateUsers)
        {
            A.CallTo(() => userService.GetUsersByUsername(Username)).Returns((new List<AdminUser>(), delegateUsers));
        }

        private void GivenAdminUserAndDelegateUserAreVerified(AdminUser adminUser, DelegateUser delegateUser)
        {
            A.CallTo(() => userService.GetUsersByUsername(Username)).Returns(
                (new List<AdminUser> { adminUser }, new List<DelegateUser> { delegateUser })
            );
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(
                    new UserAccountSet(new List<AdminUser> { adminUser }, new List<DelegateUser> { delegateUser })
                );
        }

        public void GivenDelegateAccountsFailedVerification(List<DelegateUser> delegateUsers)
        {
            GivenDelegateUsersFoundByUserName(delegateUsers);
            GivenNoAccountsAreVerified();
        }

        public void GivenAdminAccountFailedVerification(AdminUser adminUser)
        {
            var adminUsers = new List<AdminUser> { adminUser };
            GivenAdminUsersFoundByUsername(adminUsers);
            GivenNoAccountsAreVerified();
            A.CallTo(() => userService.IncrementFailedLoginCount(adminUser)).DoesNothing();
        }

        private void GivenNoAccountsAreVerified()
        {
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(new List<AdminUser>(), new List<DelegateUser>()));
        }

        private void GivenDelegateUsersAreVerified(List<DelegateUser> delegateUsers)
        {
            GivenDelegateUsersFoundByUserName(delegateUsers);
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(new List<AdminUser>(), delegateUsers));
        }

        private void GivenAdminUsersAreVerified(List<AdminUser> adminUsers)
        {
            GivenAdminUsersFoundByUsername(adminUsers);
            A.CallTo(() => userVerificationService.VerifyUsers(Password, A<List<AdminUser>>._, A<List<DelegateUser>>._))
                .Returns(new UserAccountSet(adminUsers, new List<DelegateUser>()));
        }

        private void GivenNoLinkedAccountsFound()
        {
            A.CallTo(
                () => userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                    A<List<DelegateUser>>._,
                    Password
                )
            ).Returns(null);
            GivenNoLinkedDelegateAccountsFound();
        }

        private void GivenNoLinkedDelegateAccountsFound()
        {
            A.CallTo(
                () => userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(A<AdminUser?>._, Password)
            ).Returns(new List<DelegateUser>());
        }

        private void GivenLinkedDelegateAccountsFound(List<DelegateUser> delegateUsers)
        {
            A.CallTo(
                () => userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(A<AdminUser?>._, Password)
            ).Returns(delegateUsers);
        }

        private void GivenLinkedAdminUserIsFound(AdminUser? linkedAdminUser)
        {
            A.CallTo(
                () => userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUsers(
                    A<List<DelegateUser>>._,
                    Password
                )
            ).Returns(linkedAdminUser);
        }

        private void GivenResetFailedLoginCountDoesNothing(AdminUser adminUser)
        {
            A.CallTo(() => userService.ResetFailedLoginCount(adminUser)).DoesNothing();
        }

        private void AdminUserHasActiveCentre(AdminUser adminUser)
        {
            A.CallTo(() => userService.GetUsersWithActiveCentres(adminUser, A<List<DelegateUser>>._))
                .Returns((adminUser, new List<DelegateUser>()));
            A.CallTo(() => userService.GetUserCentres(adminUser, A<List<DelegateUser>>._)).Returns(
                new List<CentreUserDetails> { new CentreUserDetails(adminUser.CentreId, adminUser.CentreName, true) }
            );
        }

        private void GivenDelegateUserHasActiveCentre(DelegateUser delegateUser)
        {
            A.CallTo(() => userService.GetUsersWithActiveCentres(null, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser> { delegateUser }));
            A.CallTo(() => userService.GetUserCentres(null, A<List<DelegateUser>>._)).Returns(
                new List<CentreUserDetails> { new CentreUserDetails(delegateUser.CentreId, delegateUser.CentreName) }
            );
        }

        private void GivenNoActiveCentresAreFound()
        {
            A.CallTo(() => userService.GetUsersWithActiveCentres(null, A<List<DelegateUser>>._))
                .Returns((null, new List<DelegateUser>()));
            A.CallTo(() => userService.GetUserCentres(null, A<List<DelegateUser>>._))
                .Returns(new List<CentreUserDetails>());
        }

        private void GivenSingleActiveCentreIsFound(AdminUser? adminUser, List<DelegateUser> delegateUsers)
        {
            const int centreId = 2;
            const string centreName = "Test Centre";
            A.CallTo(() => userService.GetUsersWithActiveCentres(adminUser, A<List<DelegateUser>>._))
                .Returns((adminUser, delegateUsers));
            A.CallTo(() => userService.GetUserCentres(adminUser, A<List<DelegateUser>>._)).Returns(
                new List<CentreUserDetails> { new CentreUserDetails(centreId, centreName, adminUser != null) }
            );
        }

        private void GivenMultipleActiveCentresAreFound(AdminUser? adminUser, List<DelegateUser> delegateUsers)
        {
            const int centreId = 2;
            const string centreName = "Test Centre";
            const int secondCentreId = 3;
            const string secondCentreName = "Other Centre";
            A.CallTo(() => userService.GetUsersWithActiveCentres(adminUser, A<List<DelegateUser>>._))
                .Returns((adminUser, delegateUsers));
            A.CallTo(() => userService.GetUserCentres(adminUser, A<List<DelegateUser>>._)).Returns(
                new List<CentreUserDetails>
                {
                    new CentreUserDetails(centreId, centreName),
                    new CentreUserDetails(secondCentreId, secondCentreName, adminUser != null)
                }
            );
        }
    }
}
