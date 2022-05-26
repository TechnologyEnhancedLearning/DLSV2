namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Common;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class UserServiceTests
    {
        private ICentreContractAdminUsageService centreContractAdminUsageService = null!;
        private IClockService clockService = null!;
        private IConfiguration configuration = null!;
        private IGroupsService groupsService = null!;
        private ILogger<IUserService> logger = null!;
        private ISessionDataService sessionDataService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;
        private IUserVerificationService userVerificationService = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            groupsService = A.Fake<IGroupsService>();
            userVerificationService = A.Fake<IUserVerificationService>();
            centreContractAdminUsageService = A.Fake<ICentreContractAdminUsageService>();
            sessionDataService = A.Fake<ISessionDataService>();
            logger = A.Fake<Logger<IUserService>>();
            clockService = A.Fake<IClockService>();
            configuration = A.Fake<IConfiguration>();

            userService = new UserService(
                userDataService,
                groupsService,
                userVerificationService,
                centreContractAdminUsageService,
                sessionDataService,
                logger,
                clockService,
                configuration
            );
        }

        [Test]
        public void GetUsersByUsername_Returns_admin_user_and_delegate_users()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._))
                .Returns(expectedAdminUser);
            A.CallTo(() => userDataService.GetDelegateUsersByUsername(A<string>._))
                .Returns(new List<DelegateUser> { expectedDelegateUser });

            // When
            var (returnedAdminUser, returnedDelegateUsers) = userService.GetUsersByUsername("Username");

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
            returnedDelegateUsers.FirstOrDefault().Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetUsersById_Returns_admin_user_and_delegate_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(expectedAdminUser);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(expectedDelegateUser);

            // When
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

            // When
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

            // When
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
        public void GetDelegateUserById_returns_user_from_data_service()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userDataService.GetDelegateUserById(expectedDelegateUser.Id)).Returns(expectedDelegateUser);

            // When
            var returnedDelegateUser = userService.GetDelegateUserById(expectedDelegateUser.Id);

            // Then
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetDelegateUserById_returns_null_from_data_service()
        {
            // Given
            const int delegateId = 1;
            A.CallTo(() => userDataService.GetDelegateUserById(delegateId)).Returns(null);

            // When
            var returnedDelegateUser = userService.GetDelegateUserById(delegateId);

            // Then
            returnedDelegateUser.Should().BeNull();
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
                UserTestHelper.GetDefaultDelegateUser(4, centreName: "Fourth Centre", centreActive: true),
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
                UserTestHelper.GetDefaultDelegateUser(4, centreName: "Fourth Centre", centreActive: false),
            };

            // When
            var (resultAdminUser, resultDelegateUsers) =
                userService.GetUsersWithActiveCentres(inputAdminAccount, inputDelegateList);

            // Then
            Assert.IsNull(resultAdminUser);
            Assert.That(resultDelegateUsers.IsNullOrEmpty);
        }

        [Test]
        public void UpdateUserAccountDetailsForAllUsers_with_null_delegate_only_updates_admin()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";
            var professionalRegNumber = "test-1234";
            var accountDetailsData =
                new MyAccountDetailsData(
                    adminUser.Id,
                    null,
                    firstName,
                    lastName,
                    email,
                    1,
                    professionalRegNumber,
                    true,
                    null
                );

            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(A<int>._)).Returns(new List<DelegateAccount>());

            A.CallTo(
                    () => userDataService.UpdateUser(
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        null,
                        A<string>._,
                        A<bool>._,
                        A<int>._,
                        A<DateTime>._,
                        A<int>._
                    )
                )
                .DoesNothing();

            // When
            userService.UpdateUserDetailsAndCentreSpecificDetails(adminUser.Id, accountDetailsData);

            // Then
            A.CallTo(
                    () => userDataService.UpdateUser(
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        null,
                        A<string>._,
                        A<bool>._,
                        A<int>._,
                        A<DateTime>._,
                        A<int>._
                    )
                )
                .MustHaveHappened();
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void UpdateUserAccountDetailsForAllUsers_with_null_admin_only_updates_delegate()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount();
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";
            var professionalRegNumber = "123-number";
            var accountDetailsData =
                new MyAccountDetailsData(
                    null,
                    delegateUser.Id,
                    firstName,
                    lastName,
                    email,
                    1,
                    professionalRegNumber,
                    true,
                    null
                );
            var centreAnswersData = new CentreAnswersData(2, 1, null, null, null, null, null, null, null);

            A.CallTo(() => userDataService.GetDelegateUserById(delegateAccount.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(A<int>._))
                .Returns(new List<DelegateAccount>{delegateAccount});
            A.CallTo(
                    () => userDataService.UpdateUser(
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        null,
                        A<string>._,
                        A<bool>._,
                        A<int>._,
                        A<DateTime>._,
                        A<int>._
                    )
                )
                .DoesNothing();
            A.CallTo(
                () => groupsService.SynchroniseUserChangesWithGroups(
                    A<DelegateUser>._,
                    A<MyAccountDetailsData>._,
                    A<CentreAnswersData>._
                )
            ).DoesNothing();

            // When
            userService.UpdateUserDetailsAndCentreSpecificDetails(
                UserTestHelper.GetDefaultAdminUser().Id,
                accountDetailsData,
                centreAnswersData
            );

            // Then
            A.CallTo(
                    () => userDataService.UpdateUser(
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        null,
                        A<string>._,
                        A<bool>._,
                        A<int>._,
                        A<DateTime>._,
                        A<int>._
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                    () => userDataService.UpdateDelegateUserCentrePrompts(
                        2,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        A<DateTime>._
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                () => groupsService.SynchroniseUserChangesWithGroups(
                    delegateUser,
                    accountDetailsData,
                    centreAnswersData
                )
            ).MustHaveHappened();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void UpdateUserAccountDetailsForAllUsers_with_both_admin_and_delegate_updates_both()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var adminUser = UserTestHelper.GetDefaultAdminAccount();
            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount();
            var firstName = "TestFirstName";
            var lastName = "TestLastName";
            var email = "test@email.com";
            var professionalRegNumber = "test-1234";
            var accountDetailsData =
                new MyAccountDetailsData(
                    adminUser.Id,
                    delegateAccount.Id,
                    firstName,
                    lastName,
                    email,
                    1,
                    professionalRegNumber,
                    true,
                    null
                );
            var centreAnswersData = new CentreAnswersData(2, 1, null, null, null, null, null, null, null);

            A.CallTo(() => userDataService.GetDelegateUserById(delegateAccount.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(A<int>._))
                .Returns(new List<DelegateAccount>{delegateAccount});
            A.CallTo(
                    () => userDataService.UpdateUser(
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        null,
                        A<string>._,
                        A<bool>._,
                        A<int>._,
                        A<DateTime>._,
                        A<int>._
                    )
                )
                .DoesNothing();
            A.CallTo(
                () => groupsService.SynchroniseUserChangesWithGroups(
                    A<DelegateUser>._,
                    A<MyAccountDetailsData>._,
                    A<CentreAnswersData>._
                )
            ).DoesNothing();

            // When
            userService.UpdateUserDetailsAndCentreSpecificDetails(adminUser.Id, accountDetailsData, centreAnswersData);

            // Then
            A.CallTo(
                    () => userDataService.UpdateUser(
                        A<string>._,
                        A<string>._,
                        A<string>._,
                        null,
                        A<string>._,
                        A<bool>._,
                        A<int>._,
                        A<DateTime>._,
                        A<int>._
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                    () => userDataService.UpdateDelegateUserCentrePrompts(
                        2,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        A<DateTime>._
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                () => groupsService.SynchroniseUserChangesWithGroups(
                    delegateUser,
                    accountDetailsData,
                    centreAnswersData
                )
            ).MustHaveHappened();
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
            var result = userService.NewEmailAddressIsValid(email, adminUser.Id, delegateUser.Id, adminUser.Id);

            // Then
            result.Should().BeTrue();
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).MustNotHaveHappened();
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).MustNotHaveHappened();
        }

        [Test]
        public void NewEmailAddressIsValid_returns_true_with_same_admin_at_different_centre_with_email()
        {
            // Given
            const string email = "email@test.com";
            const string oldEmail = "oldemail@test.com";
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: oldEmail);
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: oldEmail);
            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).Returns(new List<DelegateUser>());

            // When
            var result = userService.NewEmailAddressIsValid(email, adminUser.Id, delegateUser.Id, adminUser.Id);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void NewEmailAddressIsValid_returns_true_with_same_delegate_at_different_centre_with_email()
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
                (new List<DelegateUser> { delegateUser });
            A.CallTo(() => userDataService.GetUserIdFromUsername(oldEmail)).Returns(adminUser.Id);

            // When
            var result = userService.NewEmailAddressIsValid(email, adminUser.Id, delegateUser.Id, adminUser.Id);

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
            var result = userService.NewEmailAddressIsValid(email, adminUser.Id, null, adminUser.Id);

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
            var result = userService.NewEmailAddressIsValid(
                email,
                null,
                delegateUser.Id,
                UserTestHelper.GetDefaultAdminUser().Id
            );

            // Then
            result.Should().BeTrue();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).MustNotHaveHappened();
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).MustNotHaveHappened();
        }

        [Test]
        public void NewEmailAddressIsValid_returns_true_with_unchanged_email_with_different_case()
        {
            // Given
            const string email = "email@test.com";
            const string capsEmail = "Email@test.com";
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: email);
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: capsEmail);
            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);

            // When
            var result = userService.NewEmailAddressIsValid(email, adminUser.Id, delegateUser.Id, adminUser.Id);

            // Then
            result.Should().BeTrue();
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).MustNotHaveHappened();
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).MustNotHaveHappened();
        }

        [Test]
        public void IsDelegateEmailValidForCentre_should_return_false_if_user_at_centre_has_email()
        {
            // Given
            const string email = "email@test.com";
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).Returns
                (new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(3, emailAddress: email, centreId: 3) });

            // When
            var result = userService.IsDelegateEmailValidForCentre(email, 3);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void IsDelegateEmailValidForCentre_should_return_true_if_user_not_at_centre_has_email()
        {
            // Given
            const string email = "email@test.com";
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).Returns
                (new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(3, emailAddress: email, centreId: 4) });

            // When
            var result = userService.IsDelegateEmailValidForCentre(email, 3);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void IsDelegateEmailValidForCentre_should_return_true_if_no_user_has_email()
        {
            // Given
            const string email = "email@test.com";
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email)).Returns
                (new List<DelegateUser>());

            // When
            var result = userService.IsDelegateEmailValidForCentre(email, 3);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void ResetFailedLoginCount_resets_count()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount(failedLoginCount: 4);

            // When
            userService.ResetFailedLoginCount(userAccount);

            // Then
            A.CallTo(() => userDataService.UpdateUserFailedLoginCount(userAccount.Id, 0)).MustHaveHappened();
        }

        [Test]
        public void ResetFailedLoginCount_doesnt_call_data_service_with_FailedLoginCount_of_zero()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount(failedLoginCount: 0);

            // When
            userService.ResetFailedLoginCount(userAccount);

            // Then
            A.CallTo(() => userDataService.UpdateUserFailedLoginCount(userAccount.Id, 0)).MustNotHaveHappened();
        }

        [Test]
        public void IncrementFailedLoginCount_updates_count_to_expected_value()
        {
            // Given
            const int expectedCount = 5;
            var userAccount = UserTestHelper.GetDefaultUserAccount(failedLoginCount: expectedCount);

            // When
            userService.UpdateFailedLoginCount(userAccount);

            // Then
            A.CallTo(() => userDataService.UpdateUserFailedLoginCount(userAccount.Id, expectedCount))
                .MustHaveHappened();
        }

        [Test]
        public void GetDelegateUserCardsForWelcomeEmail_returns_correctly_filtered_list_of_delegates()
        {
            // Given
            var testDelegates = new List<DelegateUserCard>
            {
                new DelegateUserCard
                    { AliasId = "include", Approved = true, SelfReg = false, Password = null, EmailAddress = "email" },
                new DelegateUserCard
                    { AliasId = "include", Approved = true, SelfReg = false, Password = "", EmailAddress = "email" },
                new DelegateUserCard
                    { AliasId = "skip", Approved = false, SelfReg = false, Password = null, EmailAddress = "email" },
                new DelegateUserCard
                    { AliasId = "skip", Approved = true, SelfReg = true, Password = null, EmailAddress = "email" },
                new DelegateUserCard
                    { AliasId = "skip", Approved = true, SelfReg = false, Password = "pw", EmailAddress = "email" },
                new DelegateUserCard
                    { AliasId = "skip", Approved = true, SelfReg = false, Password = null, EmailAddress = "" },
                new DelegateUserCard
                    { AliasId = "skip", Approved = true, SelfReg = false, Password = null, EmailAddress = null },
            };
            A.CallTo(() => userDataService.GetDelegateUserCardsByCentreId(101)).Returns(testDelegates);

            // When
            var result = userService.GetDelegateUserCardsForWelcomeEmail(101).ToList();

            // Then
            result.Should().HaveCount(2);
            result[0].AliasId.Should().Be("include");
            result[1].AliasId.Should().Be("include");
        }

        [Test]
        public void UpdateAdminUserPermissions_edits_roles_when_spaces_available()
        {
            // Given
            var currentAdminUser = UserTestHelper.GetDefaultAdminUser(
                isContentCreator: false,
                isTrainer: false,
                importOnly: false,
                isContentManager: false
            );
            var numberOfAdmins = CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators();
            GivenAdminDataReturned(numberOfAdmins, currentAdminUser);
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true);

            // When
            userService.UpdateAdminUserPermissions(currentAdminUser.Id, adminRoles, 0);

            // Then
            AssertAdminPermissionsCalledCorrectly(currentAdminUser.Id, adminRoles, 0);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void UpdateAdminUserPermissions_edits_roles_when_spaces_unavailable_but_user_already_on_role(
            bool importOnly
        )
        {
            // Given
            var currentAdminUser = UserTestHelper.GetDefaultAdminUser(
                isContentCreator: true,
                isTrainer: true,
                importOnly: importOnly,
                isContentManager: true
            );

            var numberOfAdmins = GetFullCentreContractAdminUsage();
            GivenAdminDataReturned(numberOfAdmins, currentAdminUser);
            var adminRoles = new AdminRoles(true, true, true, true, true, true, importOnly);

            // When
            userService.UpdateAdminUserPermissions(currentAdminUser.Id, adminRoles, 0);

            // Then
            AssertAdminPermissionsCalledCorrectly(currentAdminUser.Id, adminRoles, 0);
        }

        [Test]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        [TestCase(false, false, true)]
        public void UpdateAdminUserPermissions_throws_exception_when_spaces_unavailable_and_user_not_on_role(
            bool newIsTrainer,
            bool newIsContentCreator,
            bool newImportOnly
        )
        {
            // Given
            var currentAdminUser = UserTestHelper.GetDefaultAdminUser(
                isContentCreator: false,
                isTrainer: false,
                importOnly: false,
                isContentManager: false
            );
            var numberOfAdmins = GetFullCentreContractAdminUsage();
            GivenAdminDataReturned(numberOfAdmins, currentAdminUser);
            var adminRoles = new AdminRoles(true, true, newIsContentCreator, true, newIsTrainer, true, newImportOnly);

            // Then
            Assert.Throws<AdminRoleFullException>(
                () => userService.UpdateAdminUserPermissions(
                    currentAdminUser.Id,
                    adminRoles,
                    0
                )
            );
            AssertAdminPermissionUpdateMustNotHaveHappened();
        }

        [Test]
        public void UpdateUserAccountDetailsViaDelegateAccount_updates_admin_user_if_found_by_email()
        {
            // Given
            const string email = "test@email.com";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: email);
            var adminUser = UserTestHelper.GetDefaultAdminUser(emailAddress: email);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email))
                .Returns(new List<DelegateUser> { delegateUser });
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(adminUser);
            var editDelegateDetailsData = new EditDelegateDetailsData(
                delegateUser.Id,
                delegateUser.FirstName!,
                delegateUser.LastName,
                delegateUser.EmailAddress!,
                delegateUser.AliasId,
                null,
                true
            );
            var centreAnswersData = new CentreAnswersData(
                delegateUser.CentreId,
                delegateUser.JobGroupId,
                null,
                delegateUser.Answer1,
                delegateUser.Answer2,
                delegateUser.Answer3,
                delegateUser.Answer4,
                delegateUser.Answer5,
                delegateUser.Answer6
            );

            // When
            userService.UpdateUserAccountDetailsViaDelegateAccount(editDelegateDetailsData, centreAnswersData);

            // Then
            A.CallTo(
                () => userDataService.UpdateAdminUser(
                    editDelegateDetailsData.FirstName,
                    editDelegateDetailsData.Surname,
                    editDelegateDetailsData.Email,
                    adminUser.ProfileImage,
                    adminUser.Id
                )
            ).MustHaveHappened();
        }

        [Test]
        public void UpdateUserAccountDetailsViaDelegateAccount_does_not_update_admin_user_if_not_found_by_email()
        {
            // Given
            const string email = "test@email.com";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: email);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email))
                .Returns(new List<DelegateUser> { delegateUser });
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(null);
            var editDelegateDetailsData = new EditDelegateDetailsData(
                delegateUser.Id,
                delegateUser.FirstName!,
                delegateUser.LastName,
                delegateUser.EmailAddress!,
                delegateUser.AliasId,
                null,
                true
            );
            var centreAnswersData = new CentreAnswersData(
                delegateUser.CentreId,
                delegateUser.JobGroupId,
                null,
                delegateUser.Answer1,
                delegateUser.Answer2,
                delegateUser.Answer3,
                delegateUser.Answer4,
                delegateUser.Answer5,
                delegateUser.Answer6
            );

            // When
            userService.UpdateUserAccountDetailsViaDelegateAccount(editDelegateDetailsData, centreAnswersData);

            // Then
            A.CallTo(
                () => userDataService.UpdateAdminUser(
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<byte[]?>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void UpdateUserAccountDetailsViaDelegateAccount_updates_name_and_email_on_all_found_delegates()
        {
            // Given
            const string email = "test@email.com";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: email);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(3, emailAddress: email);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email))
                .Returns(new List<DelegateUser> { delegateUser, secondDelegateUser });
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(null);
            var editDelegateDetailsData = new EditDelegateDetailsData(
                delegateUser.Id,
                delegateUser.FirstName!,
                delegateUser.LastName,
                delegateUser.EmailAddress!,
                delegateUser.AliasId,
                null,
                true
            );
            var centreAnswersData = new CentreAnswersData(
                delegateUser.CentreId,
                delegateUser.JobGroupId,
                null,
                delegateUser.Answer1,
                delegateUser.Answer2,
                delegateUser.Answer3,
                delegateUser.Answer4,
                delegateUser.Answer5,
                delegateUser.Answer6
            );

            // When
            userService.UpdateUserAccountDetailsViaDelegateAccount(editDelegateDetailsData, centreAnswersData);

            // Then
            A.CallTo(
                () => userDataService.UpdateUserDetails(
                    editDelegateDetailsData.FirstName,
                    editDelegateDetailsData.Surname,
                    editDelegateDetailsData.Email,
                    centreAnswersData.JobGroupId,
                    A<int>._
                )
            ).MustHaveHappened();
        }

        [Test]
        public void UpdateUserAccountDetailsViaDelegateAccount_calls_UpdateDelegateProfessionalRegistrationNumber()
        {
            // Given
            const string email = "test@email.com";
            const string prn = "PRNNUMBER";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: email);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(3, emailAddress: email);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email))
                .Returns(new List<DelegateUser> { delegateUser, secondDelegateUser });
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(null);
            var editDelegateDetailsData = new EditDelegateDetailsData(
                delegateUser.Id,
                delegateUser.FirstName!,
                delegateUser.LastName,
                delegateUser.EmailAddress!,
                delegateUser.AliasId,
                prn,
                true
            );
            var centreAnswersData = new CentreAnswersData(
                delegateUser.CentreId,
                delegateUser.JobGroupId,
                null,
                delegateUser.Answer1,
                delegateUser.Answer2,
                delegateUser.Answer3,
                delegateUser.Answer4,
                delegateUser.Answer5,
                delegateUser.Answer6
            );

            // When
            userService.UpdateUserAccountDetailsViaDelegateAccount(editDelegateDetailsData, centreAnswersData);

            // Then
            A.CallTo(
                () => userDataService.UpdateDelegateProfessionalRegistrationNumber(
                    delegateUser.Id,
                    prn,
                    true
                )
            ).MustHaveHappened();
        }

        [Test]
        public void UpdateUserAccountDetailsViaDelegateAccount_updates_single_account_if_no_email_set()
        {
            // Given
            const string email = "";
            const string prn = "PRNNUMBER";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: email);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(3, emailAddress: email);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateUser.Id)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(email))
                .Returns(new List<DelegateUser> { delegateUser, secondDelegateUser });
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(email)).Returns(null);
            var editDelegateDetailsData = new EditDelegateDetailsData(
                delegateUser.Id,
                delegateUser.FirstName!,
                delegateUser.LastName,
                delegateUser.EmailAddress!,
                delegateUser.AliasId,
                prn,
                true
            );
            var centreAnswersData = new CentreAnswersData(
                delegateUser.CentreId,
                delegateUser.JobGroupId,
                null,
                delegateUser.Answer1,
                delegateUser.Answer2,
                delegateUser.Answer3,
                delegateUser.Answer4,
                delegateUser.Answer5,
                delegateUser.Answer6
            );

            // When
            userService.UpdateUserAccountDetailsViaDelegateAccount(editDelegateDetailsData, centreAnswersData);

            // Then
            A.CallTo(
                () => userDataService.UpdateAdminUser(
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<byte[]?>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            A.CallTo(
                () => userDataService.UpdateUserDetails(
                    editDelegateDetailsData.FirstName,
                    editDelegateDetailsData.Surname,
                    editDelegateDetailsData.Email,
                    centreAnswersData.JobGroupId,
                    A<int>._
                )
            ).MustHaveHappened();
            A.CallTo(
                () => userDataService.UpdateDelegateAccount(
                    editDelegateDetailsData.DelegateId,
                    delegateUser.Active,
                    centreAnswersData.Answer1,
                    centreAnswersData.Answer2,
                    centreAnswersData.Answer3,
                    centreAnswersData.Answer4,
                    centreAnswersData.Answer5,
                    centreAnswersData.Answer6
                )
            ).MustHaveHappened();
        }

        [Test]
        public void GetSupervisorsAtCentre_returns_expected_admins()
        {
            // Given
            var adminUsers = Builder<AdminUser>.CreateListOfSize(10)
                .TheFirst(5).With(au => au.IsSupervisor = true)
                .TheRest().With(au => au.IsSupervisor = false).Build().ToList();
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(A<int>._)).Returns(adminUsers);

            // When
            var result = userService.GetSupervisorsAtCentre(1).ToList();

            // Then
            result.Should().HaveCount(5);
            result.All(au => au.IsSupervisor).Should().BeTrue();
        }

        [Test]
        public void GetSupervisorsAtCentreForCategory_returns_expected_admins()
        {
            // Given
            var adminUsers = Builder<AdminUser>.CreateListOfSize(10)
                .TheFirst(3)
                .With(au => au.IsSupervisor = true)
                .With(au => au.CategoryId = 1)
                .TheNext(2)
                .With(au => au.IsSupervisor = true)
                .With(au => au.CategoryId = null)
                .TheNext(3)
                .With(au => au.IsSupervisor = true)
                .With(au => au.CategoryId = 2)
                .TheRest().With(au => au.IsSupervisor = false).Build().ToList();
            A.CallTo(() => userDataService.GetAdminUsersByCentreId(A<int>._)).Returns(adminUsers);

            // When
            var result = userService.GetSupervisorsAtCentreForCategory(1, 1).ToList();

            // Then
            result.Should().HaveCount(5);
            result.Should().OnlyContain(au => au.IsSupervisor);
            result.Should().OnlyContain(au => au.CategoryId == null || au.CategoryId == 1);
        }

        [Test]
        public void UpdateDelegateLhLoginWarningDismissalStatus_calls_data_service_with_correct_parameters()
        {
            // Given
            const int delegateId = 1;
            const bool status = true;

            // When
            userService.UpdateDelegateLhLoginWarningDismissalStatus(delegateId, status);

            // Then
            A.CallTo(() => userDataService.UpdateDelegateLhLoginWarningDismissalStatus(delegateId, status))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void DeactivateOrDeleteAdmin_calls_deactivate_if_admin_has_admin_sessions()
        {
            // Given
            const int adminId = 1;
            A.CallTo(() => sessionDataService.HasAdminGotSessions(1)).Returns(true);

            // When
            userService.DeactivateOrDeleteAdmin(adminId);

            // Them
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.DeactivateAdmin(adminId)).MustHaveHappenedOnceExactly();
                A.CallTo(() => userDataService.DeleteAdminAccount(adminId)).MustNotHaveHappened();
            }
        }

        [Test]
        public void DeactivateOrDeleteAdmin_calls_delete_if_admin_does_not_have_admin_sessions()
        {
            // Given
            const int adminId = 1;
            A.CallTo(() => sessionDataService.HasAdminGotSessions(1)).Returns(false);

            // When
            userService.DeactivateOrDeleteAdmin(adminId);

            // Them
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.DeleteAdminAccount(adminId)).MustHaveHappenedOnceExactly();
                A.CallTo(() => userDataService.DeactivateAdmin(adminId)).MustNotHaveHappened();
            }
        }

        [Test]
        public void DeactivateOrDeleteAdmin_calls_deactivate_if_delete_throws_exception()
        {
            // Given
            const int adminId = 1;
            A.CallTo(() => sessionDataService.HasAdminGotSessions(1)).Returns(false);
            A.CallTo(() => userDataService.DeleteAdminAccount(adminId)).Throws(new Exception());

            // When
            userService.DeactivateOrDeleteAdmin(adminId);

            // Them
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.DeleteAdminAccount(adminId)).MustHaveHappenedOnceExactly();
                A.CallTo(() => userDataService.DeactivateAdmin(adminId)).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetUsersByEmailAddress_returns_no_results_for_no_address(string? address)
        {
            // Given
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(new AdminUser());
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(A<string>._))
                .Returns(new List<DelegateUser> { new DelegateUser() });

            // When
            var result = userService.GetUsersByEmailAddress(address);

            // Then
            result.adminUser.Should().BeNull();
            result.delegateUsers.Should().BeEmpty();
        }

        [Test]
        public void GetUserById_returns_null_when_no_user_account_found()
        {
            // Given
            const int userId = 2;
            A.CallTo(() => userDataService.GetUserAccountById(userId)).Returns(null);

            // When
            var result = userService.GetUserById(userId);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                A.CallTo(() => userDataService.GetAdminAccountsByUserId(A<int>._)).MustNotHaveHappened();
                A.CallTo(() => userDataService.GetDelegateAccountsByUserId(A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void GetUserById_returns_populated_user_entity_when_accounts_are_returned()
        {
            // Given
            const int userId = 2;
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var adminAccounts = Builder<AdminAccount>.CreateListOfSize(5).Build();
            var delegateAccounts = Builder<DelegateAccount>.CreateListOfSize(7).Build();
            A.CallTo(() => userDataService.GetUserAccountById(userId)).Returns(userAccount);
            A.CallTo(() => userDataService.GetAdminAccountsByUserId(A<int>._)).Returns(adminAccounts);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(A<int>._)).Returns(delegateAccounts);

            // When
            var result = userService.GetUserById(userId);

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.UserAccount.Should().BeEquivalentTo(userAccount);
                result.AdminAccounts.Should().BeEquivalentTo(adminAccounts);
                result.DelegateAccounts.Should().BeEquivalentTo(delegateAccounts);
            }
        }

        [Test]
        public void GetUserByUsername_returns_null_and_does_not_call_GetUserById_when_no_user_account_found()
        {
            // Given
            const string username = "username";
            A.CallTo(() => userDataService.GetUserIdFromUsername(username)).Returns(null);

            // When
            var result = userService.GetUserByUsername(username);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                A.CallTo(() => userDataService.GetUserAccountById(A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void GetUserByUsername_returns_populated_user_entity_when_accounts_are_found()
        {
            // Given
            const int userId = 2;
            const string username = "username";
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var adminAccounts = Builder<AdminAccount>.CreateListOfSize(5).Build();
            var delegateAccounts = Builder<DelegateAccount>.CreateListOfSize(7).Build();
            A.CallTo(() => userDataService.GetUserIdFromUsername(username)).Returns(userId);
            A.CallTo(() => userDataService.GetUserAccountById(userId)).Returns(userAccount);
            A.CallTo(() => userDataService.GetAdminAccountsByUserId(A<int>._)).Returns(adminAccounts);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(A<int>._)).Returns(delegateAccounts);

            // When
            var result = userService.GetUserById(userId);

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result!.UserAccount.Should().BeEquivalentTo(userAccount);
                result.AdminAccounts.Should().BeEquivalentTo(adminAccounts);
                result.DelegateAccounts.Should().BeEquivalentTo(delegateAccounts);
            }
        }

        [Test]
        [TestCase("test@test", false)]
        [TestCase("testtest", true)]
        [TestCase("@testtest", true)]
        [TestCase("testtest@", true)]
        public void ShouldForceDetailsCheck_returns_expected_result_depending_on_user_account_primary_email(
            string email,
            bool expectedResult
        )
        {
            // Given
            var now = new DateTime(2022, 5, 5);
            var yesterday = now.AddDays(-1);
            A.CallTo(() => configuration["MonthsToPromptUserDetailsCheck"]).Returns("6");
            A.CallTo(() => clockService.UtcNow).Returns(now);
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(primaryEmail: email, detailsLastChecked: yesterday),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                    { UserTestHelper.GetDefaultDelegateAccount(centreSpecificDetailsLastChecked: yesterday) }
            );

            // When
            var result = userService.ShouldForceDetailsCheck(userEntity, 2);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        public void ShouldForceDetailsCheck_returns_true_when_user_account_details_last_checked_is_beyond_threshold()
        {
            // Given
            var now = new DateTime(2022, 5, 5);
            var yesterday = now.AddDays(-1);
            var sevenMonthsAgo = now.AddMonths(-7);
            A.CallTo(() => configuration["MonthsToPromptUserDetailsCheck"]).Returns("6");
            A.CallTo(() => clockService.UtcNow).Returns(now);
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(detailsLastChecked: sevenMonthsAgo),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                    { UserTestHelper.GetDefaultDelegateAccount(centreSpecificDetailsLastChecked: yesterday) }
            );

            // When
            var result = userService.ShouldForceDetailsCheck(userEntity, 2);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void
            ShouldForceDetailsCheck_returns_true_when_delegate_account_details_last_checked_is_beyond_threshold()
        {
            // Given
            var now = new DateTime(2022, 5, 5);
            var yesterday = now.AddDays(-1);
            var sevenMonthsAgo = now.AddMonths(-7);
            A.CallTo(() => configuration["MonthsToPromptUserDetailsCheck"]).Returns("6");
            A.CallTo(() => clockService.UtcNow).Returns(now);
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(detailsLastChecked: yesterday),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                    { UserTestHelper.GetDefaultDelegateAccount(centreSpecificDetailsLastChecked: sevenMonthsAgo) }
            );

            // When
            var result = userService.ShouldForceDetailsCheck(userEntity, 2);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void ShouldForceDetailsCheck_returns_false_when_all_details_are_valid_or_below_threshold()
        {
            // Given
            var now = new DateTime(2022, 5, 5);
            var yesterday = now.AddDays(-1);
            A.CallTo(() => configuration["MonthsToPromptUserDetailsCheck"]).Returns("6");
            A.CallTo(() => clockService.UtcNow).Returns(now);
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(detailsLastChecked: yesterday),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                    { UserTestHelper.GetDefaultDelegateAccount(centreSpecificDetailsLastChecked: yesterday) }
            );

            // When
            var result = userService.ShouldForceDetailsCheck(userEntity, 2);

            // Then
            result.Should().BeFalse();
        }

        private void AssertAdminPermissionsCalledCorrectly(
            int adminId,
            AdminRoles adminRoles,
            int categoryId
        )
        {
            A.CallTo(
                () => userDataService.UpdateAdminUserPermissions(
                    adminId,
                    adminRoles.IsCentreAdmin,
                    adminRoles.IsSupervisor,
                    adminRoles.IsNominatedSupervisor,
                    adminRoles.IsTrainer,
                    adminRoles.IsContentCreator,
                    adminRoles.IsContentManager,
                    adminRoles.ImportOnly,
                    categoryId
                )
            ).MustHaveHappened();
        }

        private void AssertAdminPermissionUpdateMustNotHaveHappened()
        {
            A.CallTo(
                () => userDataService.UpdateAdminUserPermissions(
                    A<int>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        private void GivenAdminDataReturned(CentreContractAdminUsage numberOfAdmins, AdminUser adminUser)
        {
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(adminUser);
            A.CallTo(() => centreContractAdminUsageService.GetCentreAdministratorNumbers(A<int>._))
                .Returns(numberOfAdmins);
        }

        private CentreContractAdminUsage GetFullCentreContractAdminUsage()
        {
            return CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(
                trainerSpots: 3,
                trainers: 3,
                ccLicenceSpots: 4,
                ccLicences: 4,
                cmsAdministrators: 5,
                cmsAdministratorSpots: 5,
                cmsManagerSpots: 6,
                cmsManagers: 6
            );
        }
    }
}
