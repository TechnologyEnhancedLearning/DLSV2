﻿namespace DigitalLearningSolutions.Data.Tests.Services
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
        public void
            AttemptLogin_returns_invalid_password_and_increments_failed_login_count_when_user_account_found_does_not_match_any_passwords()
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
                .Returns(new UserEntityVerificationResult(false, new List<int>(), new[] { 2, 3 }));
            A.CallTo(() => userService.UpdateFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.InvalidPassword);
                result.UserEntity.Should().BeNull();
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
                    UserTestHelper.GetDefaultDelegateAccount()
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
    }
}
