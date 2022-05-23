namespace DigitalLearningSolutions.Data.Tests.Services
{
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
        public void AttemptLogin_returns_invalid_username_when_no_account_found_for_username()
        {
            // Given
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(null);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.InvalidUsername);
                result.UserEntity.Should().BeNull();
                result.CentreToLogInto.Should().BeNull();
            }
        }

        [Test]
        public void AttemptLogin_returns_inactive_account_when_user_account_found_has_Active_false()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(active: false),
                new List<AdminAccount>(),
                new List<DelegateAccount>()
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.InactiveAccount);
                result.UserEntity.Should().BeNull();
                result.CentreToLogInto.Should().BeNull();
            }
        }

        [Test]
        public void
            AttemptLogin_returns_mismatched_passwords_when_user_account_found_has_several_delegates_with_missmatched_passwords()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(),
                    UserTestHelper.GetDefaultDelegateAccount(3, centreId: 101),
                }
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new[] { 2 }, new[] { 3 }));

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.AccountsHaveMismatchedPasswords);
                result.UserEntity.Should().BeNull();
                result.CentreToLogInto.Should().BeNull();
            }
        }

        [Test]
        [TestCase(LoginAttemptResult.InvalidPassword, 0)]
        [TestCase(LoginAttemptResult.InvalidPassword, 3)]
        [TestCase(LoginAttemptResult.AccountLocked, 4)]
        [TestCase(LoginAttemptResult.AccountLocked, 5)]
        [TestCase(LoginAttemptResult.AccountLocked, 10)]
        public void
            AttemptLogin_returns_expected_result_and_increments_failed_login_count_when_user_account_found_does_not_match_any_passwords(
                LoginAttemptResult expectedResult,
                int currentFailedLoginCount
            )
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(failedLoginCount: currentFailedLoginCount),
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount() },
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(),
                    UserTestHelper.GetDefaultDelegateAccount(3, centreId: 101),
                }
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(false, new List<int>(), new[] { 2, 3 }));
            A.CallTo(() => userService.UpdateFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(expectedResult);
                if (expectedResult == LoginAttemptResult.InvalidPassword)
                {
                    result.UserEntity.Should().BeNull();
                }
                else
                {
                    result.UserEntity.Should().BeEquivalentTo(userEntity);
                }

                result.CentreToLogInto.Should().BeNull();
                A.CallTo(() => userService.UpdateFailedLoginCount(userEntity.UserAccount)).MustHaveHappened();
            }
        }

        [Test]
        public void
            AttemptLogin_resets_failed_login_count_when_user_account_is_not_yet_locked_and_successfully_logging_in()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(),
                }
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.LogIntoSingleCentre);
                result.UserEntity.Should().Be(userEntity);
                result.CentreToLogInto.Should().Be(userEntity.DelegateAccounts.Single().CentreId);
                A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).MustHaveHappened();
            }
        }

        [Test]
        public void
            AttemptLogin_returns_locked_account_when_user_account_is_already_locked_and_correct_password_is_provided()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(failedLoginCount: 5),
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount() },
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(),
                }
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.AccountLocked);
                result.UserEntity.Should().Be(userEntity);
                result.CentreToLogInto.Should().BeNull();
                A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).MustNotHaveHappened();
            }
        }

        [Test]
        public void
            AttemptLogin_returns_choose_a_centre_when_logging_into_single_inactive_centre_and_successfully_logging_in()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount(centreActive: false) }
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.ChooseACentre);
                result.UserEntity.Should().Be(userEntity);
                result.CentreToLogInto.Should().BeNull();
            }
        }

        [Test]
        public void
            AttemptLogin_returns_choose_a_centre_when_logging_into_single_inactive_account_and_successfully_logging_in()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount(active: false) }
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.ChooseACentre);
                result.UserEntity.Should().Be(userEntity);
                result.CentreToLogInto.Should().BeNull();
            }
        }

        [Test]
        public void
            AttemptLogin_returns_choose_a_centre_when_logging_into_unapproved_account_and_successfully_logging_in()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount(approved: false) }
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.ChooseACentre);
                result.UserEntity.Should().Be(userEntity);
                result.CentreToLogInto.Should().BeNull();
            }
        }

        [Test]
        public void
            AttemptLogin_returns_choose_a_centre_when_user_account_has_no_centres_and_successfully_logging_in()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount>()
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.ChooseACentre);
                result.UserEntity.Should().Be(userEntity);
                result.CentreToLogInto.Should().BeNull();
            }
        }
    }
}
