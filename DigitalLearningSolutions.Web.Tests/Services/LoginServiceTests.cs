﻿namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;    
    using DigitalLearningSolutions.Data.ViewModels;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class LoginServiceTests
    {
        private const string Username = "Username";
        private const string Password = "Password";

        private static readonly (string?, List<(int centreId, string centreName, string centreEmail)>)
            ResultListingNoEmailsAsUnverified =
                (null, new List<(int centreId, string centreName, string centreEmail)>());

        private static readonly List<int> EmptyListOfCentreIds = new List<int>();

        private LoginService loginService = null!;
        private IUserService userService = null!;
        private IUserVerificationService userVerificationService = null!;
        private ILoginDataService loginDataService = null!;

        [SetUp]
        public void Setup()
        {
            userVerificationService = A.Fake<IUserVerificationService>(x => x.Strict());
            userService = A.Fake<IUserService>(x => x.Strict());

            loginService = new LoginService(userService, userVerificationService, loginDataService);
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
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.InvalidCredentials);
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
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new List<int>(), new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.GetUnverifiedEmailsForUser(userEntity.UserAccount.Id))
                .Returns(ResultListingNoEmailsAsUnverified);
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

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
                .Returns(new UserEntityVerificationResult(true, new List<int>(), new[] { 2 }, new[] { 3 }));

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
            AttemptLogin_returns_invalid_password_when_user_account_password_is_incorrect_and_delegate_old_passwords_null()
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
                .Returns(new UserEntityVerificationResult(false, new[] { 2, 3 }, new List<int>(), new List<int>()));
            A.CallTo(() => userService.UpdateFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.InvalidCredentials);
                result.UserEntity.Should().BeNull();
                result.CentreToLogInto.Should().BeNull();
            }
        }

        [Test]
        public void Valid_creds_for_unclaimed_delegate_returns_unclaimed_delegate()
        {
            // Given
            var unclaimedUserEntity = new UserEntity(
                Builder<UserAccount>.CreateNew().Build(),
                new AdminAccount[] { },
                new[]
                {
                    Builder<DelegateAccount>.CreateNew().With(da => da.RegistrationConfirmationHash = "hash").Build(),
                }
            );
            GivenCredsMatchUserEntity(Username, Password, unclaimedUserEntity);

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            result.LoginAttemptResult.Should().Be(LoginAttemptResult.UnclaimedDelegateAccount);
        }

        [Test]
        [TestCase(LoginAttemptResult.InvalidCredentials, 0)]
        [TestCase(LoginAttemptResult.InvalidCredentials, 3)]
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
                .Returns(new UserEntityVerificationResult(false, new List<int>(), new List<int>(), new[] { 2, 3 }));
            A.CallTo(() => userService.UpdateFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(expectedResult);
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
                    UserTestHelper.GetDefaultDelegateAccount(),
                }
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new List<int>(), new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.GetUnverifiedEmailsForUser(userEntity.UserAccount.Id))
                .Returns(ResultListingNoEmailsAsUnverified);
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
                .Returns(new UserEntityVerificationResult(true, new List<int>(), new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.AccountLocked);
                result.UserEntity.Should().BeNull();
                result.CentreToLogInto.Should().BeNull();
                A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).MustNotHaveHappened();
            }
        }

        [Test]
        [TestCase(true, true, true, true, true, true, false)]
        [TestCase(true, true, false, true, true, true, false)]
        [TestCase(true, false, false, true, true, true, false)]
        [TestCase(true, true, true, true, false, true, false)]
        [TestCase(true, true, true, true, true, false, false)]
        [TestCase(true, true, true, true, false, false, false)]
        [TestCase(true, true, true, false, false, false, false)]
        [TestCase(true, false, false, true, false, true, true)]
        [TestCase(true, false, false, true, true, false, true)]
        [TestCase(true, false, false, true, false, false, true)]
        [TestCase(true, false, false, false, false, false, true)]
        [TestCase(false, true, true, true, true, true, true)]
        [TestCase(false, true, false, true, true, true, true)]
        [TestCase(false, false, false, true, true, true, true)]
        [TestCase(false, true, true, true, false, true, true)]
        [TestCase(false, true, true, true, true, false, true)]
        [TestCase(false, true, true, true, false, false, true)]
        [TestCase(false, true, true, false, false, false, true)]
        [TestCase(false, false, false, true, false, true, true)]
        [TestCase(false, false, false, true, true, false, true)]
        [TestCase(false, false, false, true, false, false, true)]
        [TestCase(false, false, false, false, false, false, true)]
        public void
            AttemptLogin_returns_choose_a_centre_for_single_centre_user_when_centre_inactive_or_when_neither_admin_is_active_nor_delegate_account_is_active_and_approved(
                bool centreActive,
                bool isAdmin,
                bool isActiveAdmin,
                bool isDelegate,
                bool isActiveDelegate,
                bool isApprovedDelegate,
                bool expectChooseACentrePage
            )
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                isAdmin
                    ? new List<AdminAccount>
                        { UserTestHelper.GetDefaultAdminAccount(centreActive: centreActive, active: isActiveAdmin) }
                    : new List<AdminAccount>(),
                isDelegate
                    ? new List<DelegateAccount>
                    {
                        UserTestHelper.GetDefaultDelegateAccount(
                            centreActive: centreActive,
                            active: isActiveDelegate,
                            approved: isApprovedDelegate
                        ),
                    }
                    : new List<DelegateAccount>()
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(
                    new UserEntityVerificationResult(
                        true,
                        new List<int>(),
                        isDelegate ? new List<int> { 2 } : new List<int>(),
                        new List<int>()
                    )
                );
            A.CallTo(() => userService.GetUnverifiedEmailsForUser(userEntity.UserAccount.Id))
                .Returns(ResultListingNoEmailsAsUnverified);
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.UserEntity.Should().Be(userEntity);

                if (expectChooseACentrePage)
                {
                    result.LoginAttemptResult.Should().Be(LoginAttemptResult.ChooseACentre);
                    result.CentreToLogInto.Should().BeNull();
                }
                else
                {
                    result.LoginAttemptResult.Should().NotBe(LoginAttemptResult.ChooseACentre);
                    result.CentreToLogInto.Should().NotBeNull();
                }
            }
        }

        [Test]
        public void AttemptLogin_returns_choose_a_centre_for_multi_centre_user_when_logging_in_with_email()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(1, centreId: 1),
                    UserTestHelper.GetDefaultDelegateAccount(),
                }
            );
            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new List<int>(), new[] { 1, 2 }, new List<int>()));
            A.CallTo(() => userService.GetUnverifiedEmailsForUser(userEntity.UserAccount.Id))
                .Returns(ResultListingNoEmailsAsUnverified);
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.UserEntity.Should().Be(userEntity);
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.ChooseACentre);
                result.CentreToLogInto.Should().BeNull();
            }
        }

        [Test]
        public void AttemptLogin_returns_log_in_to_single_for_multi_centre_user_when_logging_in_with_candidate_number()
        {
            // Given
            const string candidateNumberForLogin = "AB1";
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(
                        1,
                        centreId: 1,
                        candidateNumber: candidateNumberForLogin
                    ),
                    UserTestHelper.GetDefaultDelegateAccount(2, centreId: 2, candidateNumber: "AB2"),
                }
            );
            A.CallTo(() => userService.GetUserByUsername(candidateNumberForLogin)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new List<int>(), new[] { 1, 2 }, new List<int>()));
            A.CallTo(() => userService.GetUnverifiedEmailsForUser(userEntity.UserAccount.Id))
                .Returns(ResultListingNoEmailsAsUnverified);
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(candidateNumberForLogin, Password);

            // Then
            using (new AssertionScope())
            {
                result.UserEntity.Should().Be(userEntity);
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.LogIntoSingleCentre);
                result.CentreToLogInto.Should().Be(1);
            }
        }

        [Test]
        public void
            AttemptLogin_returns_log_in_to_single_for_multi_centre_user_when_logging_in_with_candidate_number_to_bad_delegate_account_but_centre_has_accessible_admin_account()
        {
            // Given
            const string candidateNumberForLogin = "AB1";
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>
                {
                    UserTestHelper.GetDefaultAdminAccount(centreId: 1),
                },
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(
                        1,
                        centreId: 1,
                        candidateNumber: candidateNumberForLogin,
                        active: false
                    ),
                    UserTestHelper.GetDefaultDelegateAccount(2, centreId: 2, candidateNumber: "AB2"),
                }
            );
            A.CallTo(() => userService.GetUserByUsername(candidateNumberForLogin)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new List<int>(), new[] { 1, 2 }, new List<int>()));
            A.CallTo(() => userService.GetUnverifiedEmailsForUser(userEntity.UserAccount.Id))
                .Returns(ResultListingNoEmailsAsUnverified);
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(candidateNumberForLogin, Password);

            // Then
            using (new AssertionScope())
            {
                result.UserEntity.Should().Be(userEntity);
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.LogIntoSingleCentre);
                result.CentreToLogInto.Should().Be(1);
            }
        }

        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        [TestCase(false, false, false)]
        public void
            AttemptLogin_returns_choose_a_centre_for_multi_centre_user_when_user_cannot_log_in_to_centre_from_candidate_number(
                bool delegateAccountActive,
                bool delegateAccountApproved,
                bool centreActive
            )
        {
            // Given
            const string candidateNumberForLogin = "AB1";
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>
                {
                    UserTestHelper.GetDefaultAdminAccount(centreId: 2),
                },
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(
                        1,
                        centreId: 1,
                        candidateNumber: candidateNumberForLogin,
                        active: delegateAccountActive,
                        approved: delegateAccountApproved,
                        centreActive: centreActive
                    ),
                    UserTestHelper.GetDefaultDelegateAccount(2, centreId: 2, candidateNumber: "AB2"),
                }
            );

            A.CallTo(() => userService.GetUserByUsername(candidateNumberForLogin)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new List<int>(), new[] { 1, 2 }, new List<int>()));
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(candidateNumberForLogin, Password);

            // Then
            using (new AssertionScope())
            {
                result.UserEntity.Should().Be(userEntity);
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.ChooseACentre);
                result.CentreToLogInto.Should().BeNull();
            }
        }

        [Test]
        public void
            AttemptLogin_returns_unverified_email_if_primary_email_is_unverified()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(emailVerified: false),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(),
                    UserTestHelper.GetDefaultDelegateAccount(3, centreId: 2, candidateNumber: "AB2"),
                }
            );

            var resultListingPrimaryEmailAsUnverified = ("primary@email.com",
                new List<(int centreId, string centreName, string centreEmail)>());

            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new List<int>(), new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.GetUnverifiedEmailsForUser(userEntity.UserAccount.Id))
                .Returns(resultListingPrimaryEmailAsUnverified);
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.UserEntity.Should().Be(userEntity);
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.UnverifiedEmail);
                result.CentreToLogInto.Should().BeNull();
                A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            AttemptLogin_returns_unverified_email_if_user_is_logging_into_single_centre_and_centre_email_is_unverified()
        {
            // Given
            const int centreId = 1;
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(centreId: 1),
                }
            );

            var resultListingCentreEmailAsUnverified = ((string?)null,
                new List<(int centreId, string centreName, string centreEmail)>
                    { (centreId, "Test Centre", "centre@email.com") });

            A.CallTo(() => userService.GetUserByUsername(Username)).Returns(userEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(Password, userEntity))
                .Returns(new UserEntityVerificationResult(true, new List<int>(), new[] { 2 }, new List<int>()));
            A.CallTo(() => userService.GetUnverifiedEmailsForUser(userEntity.UserAccount.Id))
                .Returns(resultListingCentreEmailAsUnverified);
            A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).DoesNothing();

            // When
            var result = loginService.AttemptLogin(Username, Password);

            // Then
            using (new AssertionScope())
            {
                result.UserEntity.Should().Be(userEntity);
                result.LoginAttemptResult.Should().Be(LoginAttemptResult.UnverifiedEmail);
                result.CentreToLogInto.Should().Be(1);
                A.CallTo(() => userService.ResetFailedLoginCount(userEntity.UserAccount)).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void GetChooseACentreAccountViewModels_combines_admin_and_delegate_accounts_by_centre()
        {
            // Given
            var adminAccountWithDelegate =
                UserTestHelper.GetDefaultAdminAccount(centreId: 1, centreName: "admin+delegate");
            var delegateAccountWithAdmin =
                UserTestHelper.GetDefaultDelegateAccount(centreId: 1, centreName: "admin+delegate");
            var adminAccountInactiveCentre = UserTestHelper.GetDefaultAdminAccount(
                centreId: 2,
                centreName: "admin inactive centre",
                centreActive: false
            );
            var delegateAccountInactive = UserTestHelper.GetDefaultDelegateAccount(
                centreId: 3,
                centreName: "inactive delegate",
                active: false
            );
            var delegateAccountUnapproved = UserTestHelper.GetDefaultDelegateAccount(
                centreId: 4,
                centreName: "unapproved delegate",
                approved: false
            );
            var adminAccountWithUnapprovedDelegate = UserTestHelper.GetDefaultAdminAccount(
                centreId: 5,
                centreName: "admin+unapproved delegate"
            );
            var delegateAccountUnapprovedWithAdmin = UserTestHelper.GetDefaultDelegateAccount(
                centreId: 5,
                centreName: "admin+unapproved delegate",
                approved: false
            );

            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>
                {
                    adminAccountWithDelegate,
                    adminAccountInactiveCentre,
                    adminAccountWithUnapprovedDelegate,
                },
                new List<DelegateAccount>
                {
                    delegateAccountWithAdmin,
                    delegateAccountInactive,
                    delegateAccountUnapproved,
                    delegateAccountUnapprovedWithAdmin,
                }
            );

            // When
            var result = loginService.GetChooseACentreAccountViewModels(userEntity, EmptyListOfCentreIds);

            // Then
            result.Should().BeEquivalentTo(
                new List<ChooseACentreAccountViewModel>
                {
                    new ChooseACentreAccountViewModel(
                        1,
                        "admin+delegate",
                        true,
                        true,
                        true,
                        true,
                        true,
                        false
                    ),
                    new ChooseACentreAccountViewModel(
                        2,
                        "admin inactive centre",
                        false,
                        true,
                        false,
                        false,
                        false,
                        false
                    ),
                    new ChooseACentreAccountViewModel(
                        3,
                        "inactive delegate",
                        true,
                        false,
                        true,
                        true,
                        false,
                        false
                    ),
                    new ChooseACentreAccountViewModel(
                        4,
                        "unapproved delegate",
                        true,
                        false,
                        true,
                        false,
                        true,
                        false
                    ),
                    new ChooseACentreAccountViewModel(
                        5,
                        "admin+unapproved delegate",
                        true,
                        true,
                        true,
                        false,
                        true,
                        false
                    ),
                }
            );
        }

        [Test]
        public void GetChooseACentreAccountViewModels_identifies_accounts_with_unverified_emails_correctly()
        {
            // Given
            var idsOfCentresWithUnverifiedEmails = new List<int> { 1, 2, 3 };
            var unverifiedEmailAdminAccountWithDelegateAccount =
                UserTestHelper.GetDefaultAdminAccount(centreId: 1, centreName: "admin+delegate");
            var unverifiedEmailDelegateAccountWithAdminAccount =
                UserTestHelper.GetDefaultDelegateAccount(centreId: 1, centreName: "admin+delegate");
            var unverifiedEmailAdminAccountOnly =
                UserTestHelper.GetDefaultAdminAccount(centreId: 2, centreName: "admin only");
            var unverifiedEmailDelegateAccountOnly =
                UserTestHelper.GetDefaultDelegateAccount(centreId: 3, centreName: "delegate only");

            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>
                {
                    unverifiedEmailAdminAccountWithDelegateAccount,
                    unverifiedEmailAdminAccountOnly,
                },
                new List<DelegateAccount>
                {
                    unverifiedEmailDelegateAccountWithAdminAccount,
                    unverifiedEmailDelegateAccountOnly,
                }
            );

            // When
            var result = loginService.GetChooseACentreAccountViewModels(userEntity, idsOfCentresWithUnverifiedEmails);

            // Then
            result.Should().BeEquivalentTo(
                new List<ChooseACentreAccountViewModel>
                {
                    new ChooseACentreAccountViewModel(
                        1,
                        "admin+delegate",
                        true,
                        true,
                        true,
                        true,
                        true,
                        true
                    ),
                    new ChooseACentreAccountViewModel(
                        2,
                        "admin only",
                        true,
                        true,
                        false,
                        false,
                        false,
                        true
                    ),
                    new ChooseACentreAccountViewModel(
                        3,
                        "delegate only",
                        true,
                        false,
                        true,
                        true,
                        true,
                        true
                    ),
                }
            );
        }

        [Test]
        public void GetChooseACentreAccountViewModels_omits_inactive_admin_accounts()
        {
            // Given
            var adminAccount = UserTestHelper.GetDefaultAdminAccount(
                centreId: 1,
                centreName: "inactive admin",
                active: false
            );
            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount(centreId: 2, centreName: "delegate");
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount> { adminAccount },
                new List<DelegateAccount> { delegateAccount }
            );

            // When
            var result = loginService.GetChooseACentreAccountViewModels(userEntity, EmptyListOfCentreIds);

            // Then
            result.Should().BeEquivalentTo(
                new List<ChooseACentreAccountViewModel>
                {
                    new ChooseACentreAccountViewModel(2, "delegate", true, false, true, true, true, false),
                }
            );
        }

        [Test]
        public void GetChooseACentreAccountViewModels_returns_empty_list_when_only_inactive_admin_accounts_exist()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount(active: false) },
                new List<DelegateAccount>()
            );

            // When
            var result = loginService.GetChooseACentreAccountViewModels(userEntity, EmptyListOfCentreIds);

            // Then
            result.Should().HaveCount(0);
        }

        [Test]
        public void GetChooseACentreAccountViewModels_returns_empty_list_when_no_admin_or_delegate_accounts()
        {
            // Given
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount>()
            );

            // When
            var result = loginService.GetChooseACentreAccountViewModels(userEntity, EmptyListOfCentreIds);

            // Then
            result.Should().HaveCount(0);
        }

        [Test]
        public void CentreEmailIsVerified_returns_true_when_centre_email_is_verified()
        {
            // Given
            const int userId = 1;
            const int centreId = 2;
            var unverifiedCentreEmails = new List<(int centreId, string centreName, string centreEmail)>
                { (centreId + 1, "Test centre", "centre@email.com") };

            A.CallTo(() => userService.GetUnverifiedEmailsForUser(userId)).Returns((null, unverifiedCentreEmails));

            // When
            var result = loginService.CentreEmailIsVerified(userId, centreId);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void CentreEmailIsVerified_returns_false_when_centre_email_is_unverified()
        {
            // Given
            const int userId = 1;
            const int centreId = 2;
            var unverifiedCentreEmails = new List<(int centreId, string centreName, string centreEmail)>
                { (centreId, "Test centre", "centre@email.com") };

            A.CallTo(() => userService.GetUnverifiedEmailsForUser(userId)).Returns((null, unverifiedCentreEmails));

            // When
            var result = loginService.CentreEmailIsVerified(userId, centreId);

            // Then
            result.Should().BeFalse();
        }

        [TestCase(LoginAttemptResult.AccountLocked, "/login/AccountLocked")]
        [TestCase(LoginAttemptResult.InactiveAccount, "/login/AccountInactive")]
        public void HandleLoginResult_AccountLockedOrInactive(
            LoginAttemptResult loginAttemptResult,
            string url)
        {
            // Given
            var loginResult = new LoginResult(loginAttemptResult);
            loginResult.UserEntity = A.Fake<UserEntity>();
            var context = A.Fake<HttpContext>();
            var scheme = new AuthenticationScheme(
                "TestScheme",
                "Test Scheme",
                typeof(IAuthenticationHandler));
            var options = A.Fake<RemoteAuthenticationOptions>();
            var ticket = A.Fake<AuthenticationTicket>();
            var ticketReceivedContext = new TicketReceivedContext(
                context,
                scheme,
                options,
                ticket);
            var sessionService = A.Fake<ISessionService>();
            var userService = A.Fake<IUserService>();

            // When
             var result = loginService.HandleLoginResult(
                loginResult,
                ticketReceivedContext,
                "",
                sessionService,
                userService,
                "");

            // Then
            result
                .Result
                .Should()
                .Be(url);
        }

        private void GivenCredsMatchUserEntity(string username, string password, UserEntity unclaimedUserEntity)
        {
            A.CallTo(() => userService.GetUserByUsername(username)).Returns(unclaimedUserEntity);
            A.CallTo(() => userVerificationService.VerifyUserEntity(password, unclaimedUserEntity))
                .Returns(
                    new UserEntityVerificationResult(
                        true,
                        unclaimedUserEntity.DelegateAccounts.Select(da => da.Id),
                        new List<int>(),
                        new List<int>()
                    )
                );
        }
    }
}
