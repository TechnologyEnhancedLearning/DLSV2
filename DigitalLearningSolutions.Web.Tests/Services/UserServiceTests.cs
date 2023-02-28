namespace DigitalLearningSolutions.Web.Tests.Services
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
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class UserServiceTests
    {
        private ICentreContractAdminUsageService centreContractAdminUsageService = null!;
        private IClockUtility clockUtility = null!;
        private IConfiguration configuration = null!;
        private IEmailVerificationDataService emailVerificationDataService = null!;
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
            emailVerificationDataService = A.Fake<IEmailVerificationDataService>();
            logger = A.Fake<Logger<IUserService>>();
            clockUtility = A.Fake<IClockUtility>();
            configuration = A.Fake<IConfiguration>();

            userService = new UserService(
                userDataService,
                groupsService,
                centreContractAdminUsageService,
                sessionDataService,
                emailVerificationDataService,
                logger,
                clockUtility,
                configuration
            );
        }

        [Test]
        public void GetAdminUserById_Returns_admin_user()
        {
            // Given
            var expectedAdminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(expectedAdminUser);

            // When
            var returnedAdminUser = userService.GetAdminUserByAdminId(1);

            // Then
            returnedAdminUser.Should().BeEquivalentTo(expectedAdminUser);
        }


        [Test]
        public void GetDelegateUserByDelegateUserIdAndCentreId_Returns_delegate_user()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            A.CallTo(() => userDataService.GetDelegateUserByDelegateUserIdAndCentreId(A<int>._, A<int>._)).Returns(expectedDelegateUser);

            // When
            var returnedDelegateUser = userService.GetDelegateUserByDelegateUserIdAndCentreId(2, 0);

            // Then
            returnedDelegateUser.Should().BeEquivalentTo(expectedDelegateUser);
        }

        [Test]
        public void GetAdminUserByAdminId_Returns_nulls_with_unexpected_input()
        {
            // When
            var returnedAdminUser = userService.GetAdminUserByAdminId(null);

            // Then
            returnedAdminUser.Should().BeNull();
        }

        [Test]
        public void GetUsersById_Returns_nulls_with_unexpected_input()
        {
            // When
            var returnedDelegateUser = userService.GetDelegateUserByDelegateUserIdAndCentreId(null, null);

            // Then
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
        [TestCase(false, false, false)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, false)]
        [TestCase(true, true, true)]
        public void
            UpdateUserDetailsAndCentreSpecificDetails_with_null_delegate_details_only_updates_user_and_centre_email(
                bool isPrimaryEmailUpdated,
                bool isCentreEmailUpdated,
                bool changesMadeBySameUser
            )
        {
            // Given
            const int centreId = 1;
            const string centreEmail = "test@email.com";
            var accountDetailsData = UserTestHelper.GetDefaultAccountDetailsData();
            var currentTime = new DateTime(2022, 1, 1);
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            userService.UpdateUserDetailsAndCentreSpecificDetails(
                accountDetailsData,
                null,
                centreEmail,
                centreId,
                isPrimaryEmailUpdated,
                isCentreEmailUpdated,
                changesMadeBySameUser
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
                        currentTime,
                        changesMadeBySameUser ? (DateTime?)null : currentTime,
                        accountDetailsData.UserId,
                        isPrimaryEmailUpdated,
                        changesMadeBySameUser
                    )
                )
                .MustHaveHappened();

            if (isCentreEmailUpdated)
            {
                A.CallTo(
                        () => userDataService.SetCentreEmail(
                            accountDetailsData.UserId,
                            centreId,
                            centreEmail,
                            changesMadeBySameUser ? (DateTime?)null : currentTime,
                            A<IDbTransaction?>._
                        )
                    )
                    .MustHaveHappened();
            }
            else
            {
                A.CallTo(
                        () => userDataService.SetCentreEmail(
                            A<int>._,
                            A<int>._,
                            A<string?>._,
                            A<DateTime?>._,
                            A<IDbTransaction?>._
                        )
                    )
                    .MustNotHaveHappened();
            }

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
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(delegateAccount.Id);
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

            var currentTime = new DateTime(2022, 1, 1);
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateAccount.Id)).Returns(delegateUser);

            // When
            userService.UpdateUserDetailsAndCentreSpecificDetails(
                accountDetailsData,
                delegateDetailsData,
                null,
                1,
                false,
                true,
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
                        currentTime,
                        null,
                        accountDetailsData.UserId,
                        false,
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
                        currentTime
                    )
                )
                .MustHaveHappened();
            A.CallTo(
                () => groupsService.UpdateSynchronisedDelegateGroupsBasedOnUserChanges(
                    delegateDetailsData.DelegateId,
                    accountDetailsData,
                    A<RegistrationFieldAnswers>.That.Matches(
                        rfa =>
                            rfa.JobGroupId == accountDetailsData.JobGroupId &&
                            rfa.Answer1 == answer1 &&
                            rfa.Answer2 == answer2 &&
                            rfa.Answer3 == answer3 &&
                            rfa.Answer4 == answer4 &&
                            rfa.Answer5 == answer5 &&
                            rfa.Answer6 == answer6
                    ),
                    A<RegistrationFieldAnswers>.That.Matches(
                        rfa =>
                            rfa.JobGroupId == delegateUser.JobGroupId &&
                            rfa.CentreId == delegateUser.CentreId &&
                            rfa.Answer1 == delegateUser.Answer1 &&
                            rfa.Answer2 == delegateUser.Answer2 &&
                            rfa.Answer3 == delegateUser.Answer3 &&
                            rfa.Answer4 == delegateUser.Answer4 &&
                            rfa.Answer5 == delegateUser.Answer5 &&
                            rfa.Answer6 == delegateUser.Answer6
                    ),
                    null
                )
            ).MustHaveHappened();
        }

        [Test]
        public void
            UpdateUserDetailsAndCentreSpecificDetails_with_changeMadeBySameUser_true_does_not_set_emailVerified()
        {
            // Given
            const int centreId = 1;
            const string centreEmail = "test@email.com";
            const bool changeMadeBySameUser = true;
            var accountDetailsData = UserTestHelper.GetDefaultAccountDetailsData();
            var currentTime = new DateTime(2022, 1, 1);
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            userService.UpdateUserDetailsAndCentreSpecificDetails(
                accountDetailsData,
                null,
                centreEmail,
                centreId,
                false,
                true,
                changeMadeBySameUser
            );

            // Then
            A.CallTo(
                () => userDataService.SetCentreEmail(
                    accountDetailsData.UserId,
                    centreId,
                    centreEmail,
                    null,
                    A<IDbTransaction?>._
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void UpdateUserDetailsAndCentreSpecificDetails_with_changeMadeBySameUser_false_sets_emailVerified()
        {
            // Given
            const int centreId = 1;
            const string centreEmail = "test@email.com";
            const bool changeMadeBySameUser = false;
            var accountDetailsData = UserTestHelper.GetDefaultAccountDetailsData();
            var currentTime = new DateTime(2022, 1, 1);
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            userService.UpdateUserDetailsAndCentreSpecificDetails(
                accountDetailsData,
                null,
                centreEmail,
                centreId,
                false,
                true,
                changeMadeBySameUser
            );

            // Then
            A.CallTo(
                () => userDataService.SetCentreEmail(
                    accountDetailsData.UserId,
                    centreId,
                    centreEmail,
                    currentTime,
                    A<IDbTransaction?>._
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void UpdateUserDetails_updates_user(bool isPrimaryEmailUpdated)
        {
            // Given
            const bool changesMadeBySameUser = true;
            var accountDetailsData = UserTestHelper.GetDefaultAccountDetailsData();
            var currentTime = new DateTime(2022, 1, 1);
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            userService.UpdateUserDetails(
                accountDetailsData,
                isPrimaryEmailUpdated,
                changesMadeBySameUser,
                currentTime
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
                        currentTime,
                        null,
                        accountDetailsData.UserId,
                        isPrimaryEmailUpdated,
                        changesMadeBySameUser
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void UpdateUserDetails_sets_detailsLastChecked_to_ClockUtility_UtcNow_if_no_argument_provided()
        {
            // Given
            const bool changesMadeBySameUser = true;
            var accountDetailsData = UserTestHelper.GetDefaultAccountDetailsData();
            var currentTime = new DateTime(2022, 1, 1);
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            userService.UpdateUserDetails(
                accountDetailsData,
                false,
                changesMadeBySameUser
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
                        currentTime,
                        null,
                        accountDetailsData.UserId,
                        false,
                        changesMadeBySameUser
                    )
                )
                .MustHaveHappened();

            A.CallTo(() => clockUtility.UtcNow).MustHaveHappened();
        }

        [Test]
        public void
            SetCentreEmails_calls_UserDataService_SetCentreEmail_for_each_item_in_given_dictionary_that_gets_modified()
        {
            // Given
            const int userId = 2;
            var centreEmailsByCentreId = new Dictionary<int, string?>
            {
                { 1, "email@centre1.com" },
                { 2, "email@centre2.com" },
                { 3, null },
            };
            A.CallTo(() => clockUtility.UtcNow).Returns(new DateTime(2022, 5, 5));

            A.CallTo(
                () => userDataService.SetCentreEmail(
                    A<int>._,
                    A<int>._,
                    A<string?>._,
                    A<DateTime?>._,
                    A<IDbTransaction?>._
                )
            ).DoesNothing();

            // When
            userService.SetCentreEmails(userId, centreEmailsByCentreId, new List<UserCentreDetails>());

            // Then
            A.CallTo(
                () => userDataService.SetCentreEmail(userId, 1, "email@centre1.com", null, A<IDbTransaction?>._)
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => userDataService.SetCentreEmail(userId, 2, "email@centre2.com", null, A<IDbTransaction?>._)
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => userDataService.SetCentreEmail(userId, 3, null, null, A<IDbTransaction?>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void SetCentreEmails_does_not_call_data_service_if_given_an_empty_dictionary()
        {
            // Given
            var centreEmailsByCentreId = new Dictionary<int, string?>();

            // When
            userService.SetCentreEmails(2, centreEmailsByCentreId, new List<UserCentreDetails>());

            // Then
            A.CallTo(
                () => userDataService.SetCentreEmail(
                    A<int>._,
                    A<int>._,
                    A<string?>._,
                    A<DateTime?>._,
                    A<IDbTransaction?>._
                )
            ).MustNotHaveHappened();
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
                    FirstName = "include", Approved = true, SelfReg = false, Password = null, EmailAddress = "email",
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
            result.Should().HaveCount(0);
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
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, true);

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
            var adminRoles = new AdminRoles(true, true, true, true, true, true, importOnly, true);

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
            var adminRoles = new AdminRoles(true, true, newIsContentCreator, true, newIsTrainer, true, newImportOnly, true);

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
        public void GetUserAccountById_calls_data_service_and_returns_user_account()
        {
            // Given
            const int userId = 1;
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            A.CallTo(() => userDataService.GetUserAccountById(userId)).Returns(userAccount);

            // When
            var result = userService.GetUserAccountById(userId);

            // Then
            result.Should().BeEquivalentTo(userAccount);
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
            A.CallTo(() => clockUtility.UtcNow).Returns(now);
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
            A.CallTo(() => clockUtility.UtcNow).Returns(now);
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
            A.CallTo(() => clockUtility.UtcNow).Returns(now);
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
            A.CallTo(() => clockUtility.UtcNow).Returns(now);
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(detailsLastChecked: yesterday),
                new List<AdminAccount>(),
                new List<DelegateAccount>
                {
                    UserTestHelper.GetDefaultDelegateAccount(
                        centreSpecificDetailsLastChecked: sevenMonthsAgo,
                        active: false
                    ),
                }
            );

            // When
            var result = userService.ShouldForceDetailsCheck(userEntity, 2);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetAllActiveCentreEmailsForUser_returns_centre_email_list(bool isEmpty)
        {
            // Given
            const int userId = 1;
            const int centreId = 1;
            const string centreName = "centre name";
            const string centreEmail = "centre@email.com";

            var centreEmailList = new List<(int centreId, string centreName, string? centreEmail)>
                { (centreId, centreName, centreEmail) };
            A.CallTo(() => userDataService.GetAllActiveCentreEmailsForUser(userId, false)).Returns(
                isEmpty ? new List<(int centreId, string centreName, string? centreSpecificEmail)>() : centreEmailList
            );

            // When
            var result = userService.GetAllActiveCentreEmailsForUser(userId);

            // Then
            result.Should().BeEquivalentTo(
                isEmpty ? new List<(int centreId, string centreName, string? centreEmail)>() : centreEmailList
            );
        }

        [Test]
        public void ShouldForceDetailsCheck_returns_false_when_all_details_are_valid_or_below_threshold()
        {
            // Given
            var now = new DateTime(2022, 5, 5);
            var yesterday = now.AddDays(-1);
            A.CallTo(() => configuration["MonthsToPromptUserDetailsCheck"]).Returns("6");
            A.CallTo(() => clockUtility.UtcNow).Returns(now);
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
        public void GetUnverifiedEmailsForUser_returns_unverified_primary_email()
        {
            // Given
            const string unverifiedPrimaryEmail = "unverified@primary.email";
            var userAccount = UserTestHelper.GetDefaultUserAccount(
                emailVerified: false,
                primaryEmail: unverifiedPrimaryEmail
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
            result.primaryEmail.Should().BeEquivalentTo(unverifiedPrimaryEmail);
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
                        (3, "centre3", "centre@3.email"), (4, "centre4", "centre@4.email"),
                    }
                );

            // When
            var result = userService.GetUnverifiedEmailsForUser(userAccount.Id);

            // Then
            result.centreEmails.Count.Should().Be(2);
            result.centreEmails.Should().Contain((1, "centre1", "centre@1.email"));
            result.centreEmails.Should().Contain((3, "centre3", "centre@3.email"));
        }

        [Test]
        public void GetEmailVerificationDetails_returns_details_related_to_primary_email_if_code_and_email_match()
        {
            // Given
            const string email = "email@email.com";
            const string code = "code";
            var emailVerificationDetails = new EmailVerificationDetails
            {
                UserId = 1,
                Email = email,
                EmailVerificationHash = code,
                EmailVerified = null,
                EmailVerificationHashCreatedDate = new DateTime(2022, 1, 1),
            };

            A.CallTo(() => userDataService.GetPrimaryEmailVerificationDetails(code)).Returns(emailVerificationDetails);
            A.CallTo(() => userDataService.GetCentreEmailVerificationDetails(code))
                .Returns(new List<EmailVerificationDetails>());

            // When
            var result = userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails(email, code);

            // Then
            result!.Email.Should().Be(email);
        }

        [Test]
        public void GetEmailVerificationDetails_returns_details_related_to_centre_email_if_code_and_email_match()
        {
            // Given
            const string email = "email@email.com";
            const string code = "code";
            var emailVerificationDetails = new EmailVerificationDetails
            {
                UserId = 1,
                Email = email,
                EmailVerificationHash = code,
                EmailVerified = null,
                EmailVerificationHashCreatedDate = new DateTime(2022, 1, 1),
            };

            A.CallTo(() => userDataService.GetPrimaryEmailVerificationDetails(code)).Returns(null);
            A.CallTo(() => userDataService.GetCentreEmailVerificationDetails(code))
                .Returns(new[] { emailVerificationDetails });

            // When
            var result = userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails(email, code);

            // Then
            result!.Email.Should().Be(email);
        }

        [Test]
        public void GetEmailVerificationDetails_returns_null_if_code_and_email_do_not_match()
        {
            // Given
            const string email = "email@email.com";
            const string code = "code";

            A.CallTo(() => userDataService.GetPrimaryEmailVerificationDetails(code)).Returns(null);
            A.CallTo(() => userDataService.GetCentreEmailVerificationDetails(code))
                .Returns(new EmailVerificationDetails[] { });

            // When
            var result = userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails(email, code);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetEmailVerificationDetails_returns_null_if_code_matches_Users_record_with_different_email()
        {
            // Given
            const string code = "code";
            var emailVerificationDetails = new EmailVerificationDetails
            {
                UserId = 1,
                Email = "email@email.com",
                EmailVerificationHash = code,
                EmailVerified = null,
                EmailVerificationHashCreatedDate = new DateTime(2022, 1, 1),
            };

            A.CallTo(() => userDataService.GetPrimaryEmailVerificationDetails(code)).Returns(emailVerificationDetails);
            A.CallTo(() => userDataService.GetCentreEmailVerificationDetails(code))
                .Returns(new List<EmailVerificationDetails>());

            // When
            var result =
                userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails("different@email.com", code);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void
            GetEmailVerificationDetails_returns_null_if_code_matches_UserCentreDetails_record_with_different_email()
        {
            // Given
            const string code = "code";
            var emailVerificationDetails = new EmailVerificationDetails
            {
                UserId = 1,
                Email = "email@email.com",
                EmailVerificationHash = code,
                EmailVerified = null,
                EmailVerificationHashCreatedDate = new DateTime(2022, 1, 1),
            };

            A.CallTo(() => userDataService.GetPrimaryEmailVerificationDetails(code)).Returns(null);
            A.CallTo(() => userDataService.GetCentreEmailVerificationDetails(code))
                .Returns(new[] { emailVerificationDetails });

            // When
            var result =
                userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails("different@email.com", code);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void SetEmailVerified_calls_data_services()
        {
            // Given
            const int userId = 1;
            const string email = "test@email.com";
            var verifiedDateTime = new DateTime(2022, 1, 1);

            // When
            userService.SetEmailVerified(userId, email, verifiedDateTime);

            // Then
            A.CallTo(() => userDataService.SetPrimaryEmailVerified(userId, email, verifiedDateTime))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.SetCentreEmailVerified(userId, email, verifiedDateTime))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetEmailVerificationTransactionData_given_multiple_users_matching_email_hash_throws_exception()
        {
            // Given
            var details = Builder<EmailVerificationDetails>.CreateListOfSize(2).All()
                .With(d => d.Email = "email")
                .With(d => d.EmailVerificationHash = "hash")
                .TheFirst(1).With(d => d.UserId = 1)
                .TheLast(1).With(d => d.UserId = 2)
                .Build();
            A.CallTo(() => userDataService.GetCentreEmailVerificationDetails("hash")).Returns(details);

            // When
            Action gettingData = () =>
                userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails("email", "hash");

            // Then
            gettingData.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void GetEmailVerificationTransactionData_given_only_verified_emails_matched_returns_null()
        {
            // Given
            var emailVerificationDetails = new EmailVerificationDetails
            {
                UserId = 1,
                Email = "email",
                EmailVerificationHash = "code",
                EmailVerified = new DateTime(2022, 1, 1),
                CentreIdIfEmailIsForUnapprovedDelegate = null,
            };

            A.CallTo(() => userDataService.GetPrimaryEmailVerificationDetails("code"))
                .Returns(emailVerificationDetails);
            A.CallTo(() => userDataService.GetCentreEmailVerificationDetails("code"))
                .Returns(new[] { emailVerificationDetails });

            // When
            var result = userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails("email", "code");

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void EmailIsHeldAtCentre_returns_true_if_email_used_as_centre_email()
        {
            // Given
            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email", 1)).Returns(true);

            // When
            var result = userService.EmailIsHeldAtCentre("email", 1);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void EmailIsHeldAtCentre_returns_true_if_used_as_primary_email_by_user_at_centre()
        {
            // Given
            GivenPrimaryEmailHolderDelegateAccountIsAtCentre("email", 1);

            // When
            var result = userService.EmailIsHeldAtCentre("email", 1);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void EmailIsHeldAtCentre_returns_false_if_email_not_used_at_all()
        {
            // Given
            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email", 1)).Returns(false);
            A.CallTo(() => userDataService.GetUserAccountByPrimaryEmail("email")).Returns(null);

            // When
            var result = userService.EmailIsHeldAtCentre("email", 1);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void EmailIsHeldAtCentre_returns_false_if_email_used_only_at_other_centres()
        {
            // Given
            GivenPrimaryEmailHolderDelegateAccountIsAtCentre("email", 2);
            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre("email", 1)).Returns(false);

            // When
            var result = userService.EmailIsHeldAtCentre("email", 1);

            // Then
            result.Should().BeFalse();
        }

        private void GivenPrimaryEmailHolderDelegateAccountIsAtCentre(string emailAddress, int centreId)
        {
            var primaryEmailHolderUserAccount = Builder<UserAccount>.CreateNew()
                .With(u => u.Id = 1)
                .Build();

            var primaryEmailHolderDelegateAccount = Builder<DelegateAccount>.CreateNew()
                .With(da => da.CentreId = centreId)
                .Build();

            A.CallTo(() => userDataService.GetUserAccountByPrimaryEmail(emailAddress))
                .Returns(primaryEmailHolderUserAccount);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(1))
                .Returns(new[] { primaryEmailHolderDelegateAccount });
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
                    categoryId,
                    adminRoles.IsCentreManager
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
                    A<int>._,
                    A<bool>._
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

        [Test]
        public void SetCentreEmails_delete_user_centre_detail_on_empty_email()
        {
            // Given
            const int userId = 2;
            var centreEmailsByCentreId = new Dictionary<int, string?>
            {
                { 1, "" },
                { 2, "" }
            };
            A.CallTo(() => clockUtility.UtcNow).Returns(new DateTime(2022, 5, 5));
            A.CallTo(
                () => userDataService.SetCentreEmail(
                    A<int>._,
                    A<int>._,
                    A<string?>._,
                    A<DateTime?>._,
                    A<IDbTransaction?>._
                )
            ).DoesNothing();

            // When
            userService.SetCentreEmails(userId, centreEmailsByCentreId, new List<UserCentreDetails>());

            // Then
            A.CallTo(
                () => userDataService.DeleteUserCentreDetail(userId, 1)
            ).MustHaveHappenedOnceExactly();

            A.CallTo(
                () => userDataService.DeleteUserCentreDetail(userId, 2)
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void SetCentreEmails_does_not_delete_user_centre_detail_on_valid_email()
        {
            // Given
            const int userId = 2;
            var centreEmailsByCentreId = new Dictionary<int, string?>
            {
                { 1, "email@centre1.com" },
                { 2, "email@centre2.com" },
            };
            A.CallTo(() => clockUtility.UtcNow).Returns(new DateTime(2022, 5, 5));
            A.CallTo(
                () => userDataService.SetCentreEmail(
                    A<int>._,
                    A<int>._,
                    A<string?>._,
                    A<DateTime?>._,
                    A<IDbTransaction?>._
                )
            ).DoesNothing();

            // When
            userService.SetCentreEmails(userId, centreEmailsByCentreId, new List<UserCentreDetails>());

            A.CallTo(
                () => userDataService.DeleteUserCentreDetail(userId, 1)
            ).MustNotHaveHappened();

            A.CallTo(
                () => userDataService.DeleteUserCentreDetail(userId, 2)
            ).MustNotHaveHappened();
        }
    }
}
