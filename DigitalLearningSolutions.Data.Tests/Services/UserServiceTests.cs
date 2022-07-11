namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
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

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            groupsService = A.Fake<IGroupsService>();
            centreContractAdminUsageService = A.Fake<ICentreContractAdminUsageService>();
            sessionDataService = A.Fake<ISessionDataService>();
            logger = A.Fake<Logger<IUserService>>();
            clockService = A.Fake<IClockService>();
            configuration = A.Fake<IConfiguration>();

            userService = new UserService(
                userDataService,
                groupsService,
                centreContractAdminUsageService,
                sessionDataService,
                logger,
                clockService,
                configuration
            );
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
        public void GetDelegateById_returns_user_from_data_service()
        {
            // Given
            var expectedDelegateEntity = UserTestHelper.GetDefaultDelegateEntity();
            A.CallTo(() => userDataService.GetDelegateById(expectedDelegateEntity.DelegateAccount.Id))
                .Returns(expectedDelegateEntity);

            // When
            var returnedDelegateEntity = userService.GetDelegateById(expectedDelegateEntity.DelegateAccount.Id);

            // Then
            returnedDelegateEntity.Should().BeEquivalentTo(expectedDelegateEntity);
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
        public void
            UpdateUserDetailsAndCentreSpecificDetails_with_null_delegate_details_only_updates_user_and_centre_email()
        {
            // Given
            const int centreId = 1;
            const string centreEmail = "test@email.com";
            const bool shouldUpdateProfileImage = true;
            var accountDetailsData = UserTestHelper.GetDefaultAccountDetailsData();
            var now = DateTime.Now;
            A.CallTo(() => clockService.UtcNow).Returns(now);

            // When
            userService.UpdateUserDetailsAndCentreSpecificDetails(
                accountDetailsData,
                null,
                centreEmail,
                centreId,
                shouldUpdateProfileImage
            );

            // Then
            A.CallTo(
                    () => userDataService.UpdateUser(
                        accountDetailsData.FirstName,
                        accountDetailsData.Surname,
                        accountDetailsData.Email,
                        accountDetailsData.ProfileImage,
                        accountDetailsData.ProfessionalRegistrationNumber,
                        accountDetailsData.HasBeenPromptedForPrn,
                        accountDetailsData.JobGroupId,
                        now,
                        accountDetailsData.UserId,
                        shouldUpdateProfileImage
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                    () => userDataService.SetCentreEmail(
                        accountDetailsData.UserId,
                        centreId,
                        centreEmail,
                        A<IDbTransaction?>._
                    )
                )
                .MustHaveHappened();
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void UpdateUserDetailsAndCentreSpecificDetails_with_non_null_delegate_details_updates_delegate_details()
        {
            // Given
            const string answer1 = "answer1";
            const string answer2 = "answer2";
            const string answer3 = "answer3";
            const string answer4 = "answer4";
            const string answer5 = "answer5";
            const string answer6 = "answer6";
            const bool shouldUpdateProfileImage = true;
            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount();
            var accountDetailsData = UserTestHelper.GetDefaultAccountDetailsData();
            var delegateDetailsData = new DelegateDetailsData(
                delegateAccount.Id,
                answer1,
                answer2,
                answer3,
                answer4,
                answer5,
                answer6
            );

            var now = DateTime.Now;
            A.CallTo(() => clockService.UtcNow).Returns(now);

            // When
            userService.UpdateUserDetailsAndCentreSpecificDetails(
                accountDetailsData,
                delegateDetailsData,
                null,
                1,
                shouldUpdateProfileImage
            );

            // Then
            A.CallTo(
                    () => userDataService.UpdateUser(
                        accountDetailsData.FirstName,
                        accountDetailsData.Surname,
                        accountDetailsData.Email,
                        accountDetailsData.ProfileImage,
                        accountDetailsData.ProfessionalRegistrationNumber,
                        accountDetailsData.HasBeenPromptedForPrn,
                        accountDetailsData.JobGroupId,
                        now,
                        accountDetailsData.UserId,
                        shouldUpdateProfileImage
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                    () => userDataService.UpdateDelegateUserCentrePrompts(
                        delegateAccount.Id,
                        answer1,
                        answer2,
                        answer3,
                        answer4,
                        answer5,
                        answer6,
                        now
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                () => groupsService.SynchroniseUserChangesWithGroups(
                    delegateDetailsData.DelegateId,
                    accountDetailsData,
                    A<RegistrationFieldAnswers>.That.Matches(
                        rfa => rfa.JobGroupId == accountDetailsData.JobGroupId &&
                               rfa.Answer1 == answer1 &&
                               rfa.Answer2 == answer2 &&
                               rfa.Answer3 == answer3 &&
                               rfa.Answer4 == answer4 &&
                               rfa.Answer5 == answer5 &&
                               rfa.Answer6 == answer6
                    ),
                    null
                )
            ).MustHaveHappened();
        }

        [Test]
        public void ResetFailedLoginCountByUserId_resets_count()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount(failedLoginCount: 4);

            // When
            userService.ResetFailedLoginCountByUserId(userAccount.Id);

            // Then
            A.CallTo(() => userDataService.UpdateUserFailedLoginCount(userAccount.Id, 0)).MustHaveHappened();
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
                {
                    FirstName = "include", Approved = true, SelfReg = false, Password = null, EmailAddress = "email"
                },
                new DelegateUserCard
                    { FirstName = "include", Approved = true, SelfReg = false, Password = "", EmailAddress = "email" },
                new DelegateUserCard
                    { FirstName = "skip", Approved = false, SelfReg = false, Password = null, EmailAddress = "email" },
                new DelegateUserCard
                    { FirstName = "skip", Approved = true, SelfReg = true, Password = null, EmailAddress = "email" },
                new DelegateUserCard
                    { FirstName = "skip", Approved = true, SelfReg = false, Password = "pw", EmailAddress = "email" },
                new DelegateUserCard
                    { FirstName = "skip", Approved = true, SelfReg = false, Password = null, EmailAddress = "" },
                new DelegateUserCard
                    { FirstName = "skip", Approved = true, SelfReg = false, Password = null, EmailAddress = null },
            };
            A.CallTo(() => userDataService.GetDelegateUserCardsByCentreId(101)).Returns(testDelegates);

            // When
            var result = userService.GetDelegateUserCardsForWelcomeEmail(101).ToList();

            // Then
            result.Should().HaveCount(2);
            result[0].FirstName.Should().Be("include");
            result[1].FirstName.Should().Be("include");
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
        public void
            ShouldForceDetailsCheck_returns_false_when_delegate_account_details_last_checked_is_beyond_threshold_but_inactive()
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
                    { UserTestHelper.GetDefaultDelegateAccount(centreSpecificDetailsLastChecked: sevenMonthsAgo, active: false) }
            );

            // When
            var result = userService.ShouldForceDetailsCheck(userEntity, 2);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetAllCentreEmailsForUser_returns_centre_email_list(bool isEmpty)
        {
            // Given
            const int userId = 1;
            const string centreName = "centre name";
            const string centreEmail = "centre@email.com";

            var centreEmailList = new List<(string centreName, string? centreEmail)> { (centreName, centreEmail) };
            A.CallTo(() => userDataService.GetAllCentreEmailsForUser(userId)).Returns(
                isEmpty ? new List<(string centreName, string? centreSpecificEmail)>() : centreEmailList
            );

            // When
            var result = userService.GetAllCentreEmailsForUser(userId);

            // Then
            result.Should().BeEquivalentTo(
                isEmpty ? new List<(string centreName, string? centreEmail)>() : centreEmailList
            );
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

        [Test]
        [TestCase(null)]
        [TestCase("unverified@primary.email")]
        public void GetUnverifiedEmailsForUser_returns_unverified_primary_email(string? primaryEmail)
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount(
                emailVerified: primaryEmail == null ? DateTime.Now : (DateTime?)null,
                primaryEmail: "unverified@primary.email"
            );
            A.CallTo(() => userDataService.GetUserAccountById(userAccount.Id)).Returns(userAccount);
            A.CallTo(() => userDataService.GetAdminAccountsByUserId(userAccount.Id)).Returns(new List<AdminAccount>());
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(userAccount.Id))
                .Returns(new List<DelegateAccount>());
            A.CallTo(() => userDataService.GetUnverifiedCentreEmailsForUser(userAccount.Id))
                .Returns(new List<(int, string, string)>());

            // When
            var result = userService.GetUnverifiedEmailsForUser(userAccount.Id);

            // Then
            result.primaryEmail.Should().BeEquivalentTo(primaryEmail);
        }

        [Test]
        public void GetUnverifiedEmailsForUser_returns_centre_emails_for_active_accounts()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var activeAdminAccount = UserTestHelper.GetDefaultAdminAccount(centreId: 1, active: true);
            var inactiveAdminAccount = UserTestHelper.GetDefaultAdminAccount(centreId: 2, active: false);
            var activeDelegateAccount = UserTestHelper.GetDefaultDelegateAccount(centreId: 3, active: true);
            var inactiveDelegateAccount = UserTestHelper.GetDefaultDelegateAccount(centreId: 4, active: false);
            A.CallTo(() => userDataService.GetUserAccountById(userAccount.Id)).Returns(userAccount);
            A.CallTo(() => userDataService.GetAdminAccountsByUserId(userAccount.Id)).Returns(
                new List<AdminAccount> { activeAdminAccount, inactiveAdminAccount }
            );
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(userAccount.Id))
                .Returns(new List<DelegateAccount> { activeDelegateAccount, inactiveDelegateAccount });
            A.CallTo(() => userDataService.GetUnverifiedCentreEmailsForUser(userAccount.Id))
                .Returns(
                    new List<(int, string, string)>
                    {
                        (1, "centre1", "centre@1.email"), (2, "centre2", "centre@2.email"),
                        (3, "centre3", "centre@3.email"), (4, "centre4", "centre@4.email")
                    }
                );

            // When
            var result = userService.GetUnverifiedEmailsForUser(userAccount.Id);

            // Then
            result.centreEmails.Count().Should().Be(2);
            result.centreEmails.Should().Contain(("centre1", "centre@1.email"));
            result.centreEmails.Should().Contain(("centre3", "centre@3.email"));
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
