﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class RegistrationDataServiceTests
    {
        private IClockService clockService = null!;
        private SqlConnection connection = null!;
        private ILogger<IRegistrationDataService> logger = null!;
        private INotificationPreferencesDataService notificationPreferencesDataService = null!;
        private RegistrationDataService service = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
            clockService = A.Fake<IClockService>();
            logger = A.Fake<ILogger<IRegistrationDataService>>();
            service = new RegistrationDataService(connection, userDataService, clockService, logger);
            notificationPreferencesDataService = new NotificationPreferencesDataService(connection);
        }

        [Test]
        public void RegisterNewUserAndDelegateAccount_sets_all_fields_correctly_on_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var dateTime = new DateTime(2022, 6, 16, 9, 41, 30);
            A.CallTo(() => clockService.UtcNow).Returns(dateTime);

            // Given
            var delegateRegistrationModel = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(centre: 3);

            // When
            var (delegateId, candidateNumber) = service.RegisterNewUserAndDelegateAccount(
                delegateRegistrationModel,
                false
            );

            // Then
            var delegateEntity = userDataService.GetDelegateById(delegateId);
            using (new AssertionScope())
            {
                delegateEntity!.UserAccount.FirstName.Should().Be(delegateRegistrationModel.FirstName);
                delegateEntity.UserAccount.LastName.Should().Be(delegateRegistrationModel.LastName);
                delegateEntity.UserAccount.PrimaryEmail.Should().Be(delegateRegistrationModel.PrimaryEmail);
                delegateEntity.UserAccount.TermsAgreed.Should().BeNull();
                delegateEntity.UserAccount.DetailsLastChecked.Should().Be(dateTime);
                delegateEntity.DelegateAccount.CentreId.Should().Be(delegateRegistrationModel.Centre);
                delegateEntity.DelegateAccount.Answer1.Should().Be(delegateRegistrationModel.Answer1);
                delegateEntity.DelegateAccount.Answer2.Should().Be(delegateRegistrationModel.Answer2);
                delegateEntity.DelegateAccount.Answer3.Should().Be(delegateRegistrationModel.Answer3);
                delegateEntity.DelegateAccount.Answer4.Should().Be(delegateRegistrationModel.Answer4);
                delegateEntity.DelegateAccount.Answer5.Should().Be(delegateRegistrationModel.Answer5);
                delegateEntity.DelegateAccount.Answer6.Should().Be(delegateRegistrationModel.Answer6);
                candidateNumber.Should().Be("TU67");
                delegateEntity.DelegateAccount.CandidateNumber.Should().Be("TU67");
                delegateEntity.DelegateAccount.CentreSpecificDetailsLastChecked.Should().Be(dateTime);
            }
        }

        [Test]
        public void RegisterNewUserAndDelegateAccount_sets_all_fields_correctly_when_registerJourneyHasTerms_is_true()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var delegateRegistrationModel = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(centre: 3);
            var dateTime = new DateTime(2022, 6, 16, 9, 41, 30);
            A.CallTo(() => clockService.UtcNow).Returns(dateTime);

            // When
            var (delegateId, candidateNumber) = service.RegisterNewUserAndDelegateAccount(
                delegateRegistrationModel,
                true
            );

            // Then
            var delegateEntity = userDataService.GetDelegateById(delegateId);
            using (new AssertionScope())
            {
                delegateEntity!.UserAccount.FirstName.Should().Be(delegateRegistrationModel.FirstName);
                delegateEntity.UserAccount.LastName.Should().Be(delegateRegistrationModel.LastName);
                delegateEntity.UserAccount.PrimaryEmail.Should().Be(delegateRegistrationModel.PrimaryEmail);
                delegateEntity.UserAccount!.TermsAgreed.Should().Be(dateTime);
                delegateEntity.UserAccount.DetailsLastChecked.Should().Be(dateTime);
                delegateEntity.DelegateAccount.CentreId.Should().Be(delegateRegistrationModel.Centre);
                delegateEntity.DelegateAccount.Answer1.Should().Be(delegateRegistrationModel.Answer1);
                delegateEntity.DelegateAccount.Answer2.Should().Be(delegateRegistrationModel.Answer2);
                delegateEntity.DelegateAccount.Answer3.Should().Be(delegateRegistrationModel.Answer3);
                delegateEntity.DelegateAccount.Answer4.Should().Be(delegateRegistrationModel.Answer4);
                delegateEntity.DelegateAccount.Answer5.Should().Be(delegateRegistrationModel.Answer5);
                delegateEntity.DelegateAccount.Answer6.Should().Be(delegateRegistrationModel.Answer6);
                candidateNumber.Should().Be("TU67");
                delegateEntity.DelegateAccount.CandidateNumber.Should().Be("TU67");
                delegateEntity.DelegateAccount.CentreSpecificDetailsLastChecked.Should().Be(dateTime);
            }
        }

        [Test]
        public async Task
            RegisterDelegateAccountAndCentreDetailForExistingUser_sets_all_fields_correctly_on_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    3
                );
            var currentTime = DateTime.Now;
            const int userId = 2;

            // When
            var (delegateId, candidateNumber) =
                service.RegisterDelegateAccountAndCentreDetailForExistingUser(
                    delegateRegistrationModel,
                    userId,
                    currentTime
                );

            // Then
            var user = await connection.GetDelegateUserByCandidateNumberAsync(candidateNumber);
            using (new AssertionScope())
            {
                user.Id.Should().Be(delegateId);
                user.FirstName.Should().Be(delegateRegistrationModel.FirstName);
                user.LastName.Should().Be(delegateRegistrationModel.LastName);
                user.EmailAddress.Should().Be(delegateRegistrationModel.PrimaryEmail);
                user.CentreId.Should().Be(delegateRegistrationModel.Centre);
                user.Answer1.Should().Be(delegateRegistrationModel.Answer1);
                user.Answer2.Should().Be(delegateRegistrationModel.Answer2);
                user.Answer3.Should().Be(delegateRegistrationModel.Answer3);
                user.Answer4.Should().Be(delegateRegistrationModel.Answer4);
                user.Answer5.Should().Be(delegateRegistrationModel.Answer5);
                user.Answer6.Should().Be(delegateRegistrationModel.Answer6);
                user.DateRegistered.Should().BeCloseTo(currentTime, 100);
                candidateNumber.Should().Be("FS352");
                user.CandidateNumber.Should().Be("FS352");
            }
        }

        [Test]
        public async Task
            RegisterDelegateAccountAndCentreDetailForExistingUser_does_not_create_UserCentreDetails_with_null_centre_specific_email()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    3,
                    centreSpecificEmail: null
                );
            var currentTime = DateTime.Now;
            const int userId = 2;

            // When
            var (delegateId, candidateNumber) =
                service.RegisterDelegateAccountAndCentreDetailForExistingUser(
                    delegateRegistrationModel,
                    userId,
                    currentTime
                );

            // Then
            var userCentreDetailsCount = connection.QuerySingle<int>(
                "SELECT COUNT(*) FROM UserCentreDetails WHERE CentreID = 3 AND UserID = 2 AND Email IS NOT NULL"
            );
            var user = await connection.GetDelegateUserByCandidateNumberAsync(candidateNumber);
            user.Id.Should().Be(delegateId);
            userCentreDetailsCount.Should().Be(0);
            candidateNumber.Should().Be("FS352");
            user.CandidateNumber.Should().Be("FS352");
        }

        [Test]
        public void ReregisterDelegateAccountAndCentreDetailForExistingUser_sets_all_fields_correctly_on_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    active: true,
                    approved: false
                );
            var currentTime = new DateTime(2022, 06, 27, 11, 03, 12);
            const int userId = 281052;
            const int existingDelegateId = 142559;

            // When
            var userBeforeUpdate = userDataService.GetUserAccountById(userId);
            var delegateBeforeUpdate = userDataService.GetDelegateAccountById(existingDelegateId);
            service.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                existingDelegateId,
                currentTime
            );

            // Then
            var userAfterUpdate = userDataService.GetUserAccountById(userId);
            var delegateAfterUpdate = userDataService.GetDelegateAccountById(existingDelegateId);
            using (new AssertionScope())
            {
                var oldDateRegistered = new DateTime(2014, 12, 24, 10, 44, 53, 257);
                var oldCentreSpecificDetailsLastChecked = new DateTime(2022, 04, 27, 16, 31, 29, 897);
                userBeforeUpdate.Should().NotBeNull();
                delegateBeforeUpdate.Should().NotBeNull();
                userAfterUpdate.Should().NotBeNull();
                delegateAfterUpdate.Should().NotBeNull();
                userBeforeUpdate!.Id.Should().Be(userId);
                userBeforeUpdate.FirstName.Should().Be("xxxx");
                userBeforeUpdate.LastName.Should().Be("xxxx");
                userBeforeUpdate.PrimaryEmail.Should().Be("noe");
                userBeforeUpdate.DetailsLastChecked.Should().BeNull();
                userBeforeUpdate.JobGroupId.Should().Be(4);
                userBeforeUpdate.PasswordHash.Should().BeEmpty();
                userBeforeUpdate.ProfessionalRegistrationNumber.Should().BeNull();

                userAfterUpdate.Should().BeEquivalentTo(userBeforeUpdate);

                delegateBeforeUpdate!.CentreId.Should().Be(121);
                delegateBeforeUpdate.Id.Should().Be(existingDelegateId);
                delegateBeforeUpdate.CandidateNumber.Should().Be("LP497");
                delegateBeforeUpdate.OldPassword.Should().BeNull();
                delegateBeforeUpdate.UserId.Should().Be(userId);
                delegateBeforeUpdate.Answer1.Should().BeNull();
                delegateBeforeUpdate.Answer2.Should().Be("xxxxxxxxxxxx");
                delegateBeforeUpdate.Answer3.Should().BeNull();
                delegateBeforeUpdate.Answer4.Should().BeNull();
                delegateBeforeUpdate.Answer5.Should().BeNull();
                delegateBeforeUpdate.Answer6.Should().BeNull();
                delegateBeforeUpdate.Active.Should().BeFalse();
                delegateBeforeUpdate.Approved.Should().BeTrue();
                delegateBeforeUpdate.ExternalReg.Should().BeFalse();
                delegateBeforeUpdate.SelfReg.Should().BeFalse();
                delegateBeforeUpdate.DateRegistered.Should().Be(oldDateRegistered);
                delegateBeforeUpdate.CentreSpecificDetailsLastChecked.Should().Be(oldCentreSpecificDetailsLastChecked);

                delegateAfterUpdate!.CentreId.Should().Be(delegateBeforeUpdate.CentreId);
                delegateAfterUpdate.Id.Should().Be(existingDelegateId);
                delegateAfterUpdate.CandidateNumber.Should().Be(delegateBeforeUpdate.CandidateNumber);
                delegateAfterUpdate.OldPassword.Should().Be(delegateBeforeUpdate.OldPassword);
                delegateAfterUpdate.UserId.Should().Be(userId);
                delegateAfterUpdate.Answer1.Should().Be(delegateRegistrationModel.Answer1);
                delegateAfterUpdate.Answer2.Should().Be(delegateRegistrationModel.Answer2);
                delegateAfterUpdate.Answer3.Should().Be(delegateRegistrationModel.Answer3);
                delegateAfterUpdate.Answer4.Should().Be(delegateRegistrationModel.Answer4);
                delegateAfterUpdate.Answer5.Should().Be(delegateRegistrationModel.Answer5);
                delegateAfterUpdate.Answer6.Should().Be(delegateRegistrationModel.Answer6);
                delegateAfterUpdate.Active.Should().Be(delegateRegistrationModel.Active);
                delegateAfterUpdate.Approved.Should().Be(delegateRegistrationModel.Approved);
                delegateAfterUpdate.ExternalReg.Should().Be(delegateBeforeUpdate.ExternalReg);
                delegateAfterUpdate.SelfReg.Should().Be(delegateBeforeUpdate.SelfReg);
                delegateAfterUpdate.DateRegistered.Should().Be(delegateBeforeUpdate.DateRegistered);
                delegateAfterUpdate.CentreSpecificDetailsLastChecked.Should().Be(currentTime);
            }
        }

        [Test]
        public void
            ReregisterDelegateAccountAndCentreDetailForExistingUser_does_create_UserCentreDetails_with_non_null_centre_specific_email()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var currentTime = new DateTime(2022, 06, 27, 11, 03, 12);
            const int userId = 281052;
            const int existingDelegateId = 142559;
            const int centreId = 121;
            var newCentreEmail = "newCentreEmailTest@test.com";
            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    centreId,
                    centreSpecificEmail: newCentreEmail
                );

            // When
            service.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                existingDelegateId,
                currentTime
            );

            // Then
            using (new AssertionScope())
            {
                var userCentreDetails = connection.GetEmailAndVerifiedDateFromUserCentreDetails(userId, centreId);
                userCentreDetails.email.Should().Be(newCentreEmail);
                userCentreDetails.emailVerified.Should().BeNull();
            }
        }

        [Test]
        public async Task
            ReregisterDelegateAccountAndCentreDetailForExistingUser_sets_existing_UserCentreDetails_email_to_null_when_input_email_is_null()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var currentTime = new DateTime(2022, 06, 27, 11, 03, 12);
            const int userId = 281052;
            const int existingDelegateId = 142559;
            const int centreId = 121;
            const string existingEmail = "existingEmail@test.com";
            var existingEmailVerified = new DateTime(2022, 10, 4, 12, 12, 12);
            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    centreId,
                    centreSpecificEmail: null
                );

            // When
            await connection.InsertUserCentreDetails(userId, centreId, existingEmail, existingEmailVerified);
            var existingUserCentreDetails = connection.GetEmailAndVerifiedDateFromUserCentreDetails(userId, centreId);

            service.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                existingDelegateId,
                currentTime
            );
            var userCentreDetails = connection.GetEmailAndVerifiedDateFromUserCentreDetails(userId, centreId);

            // Then
            using (new AssertionScope())
            {
                existingUserCentreDetails.email.Should().Be(existingEmail);
                existingUserCentreDetails.emailVerified.Should().Be(existingEmailVerified);
                userCentreDetails.email.Should().BeNull();
                userCentreDetails.emailVerified.Should().BeNull();
            }
        }

        [Test]
        public void Sets_all_fields_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel =
                RegistrationModelTestHelper.GetDefaultCentreManagerAccountRegistrationModel(
                    categoryId: 1
                );

            // When
            var id = service.RegisterAdmin(registrationModel);

            // Then
            var user = userDataService.GetAdminUserById(id)!;
            using (new AssertionScope())
            {
                user.CentreId.Should().Be(registrationModel.CentreId);
                user.IsCentreAdmin.Should().Be(registrationModel.IsCentreAdmin);
                user.IsCentreManager.Should().Be(registrationModel.IsCentreManager);
                user.Active.Should().Be(registrationModel.Active);
                user.IsContentCreator.Should().Be(registrationModel.IsContentCreator);
                user.IsContentManager.Should().Be(registrationModel.IsContentManager);
                user.ImportOnly.Should().Be(registrationModel.ImportOnly);
                user.IsTrainer.Should().Be(registrationModel.IsTrainer);
                user.IsSupervisor.Should().Be(registrationModel.IsSupervisor);
                user.IsNominatedSupervisor.Should().Be(registrationModel.IsNominatedSupervisor);
            }
        }

        [Test]
        public void Sets_notification_preferences_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel =
                RegistrationModelTestHelper.GetDefaultCentreManagerAccountRegistrationModel(
                    categoryId: 1
                );

            // When
            var id = service.RegisterAdmin(registrationModel);

            // Then
            var user = userDataService.GetAdminUserById(id)!;
            var preferences = notificationPreferencesDataService.GetNotificationPreferencesForAdmin(user.Id).ToList();
            using (new AssertionScope())
            {
                preferences.Count.Should().Be(7);
                preferences.Should().ContainSingle(n => n.NotificationId.Equals(1) && !n.Accepted);
                preferences.Should().ContainSingle(n => n.NotificationId.Equals(2) && n.Accepted);
                preferences.Should().ContainSingle(n => n.NotificationId.Equals(3) && n.Accepted);
                preferences.Should().ContainSingle(n => n.NotificationId.Equals(4) && !n.Accepted);
                preferences.Should().ContainSingle(n => n.NotificationId.Equals(5) && n.Accepted);
                preferences.Should().ContainSingle(n => n.NotificationId.Equals(6) && !n.Accepted);
                preferences.Should().ContainSingle(n => n.NotificationId.Equals(7) && !n.Accepted);
            }
        }
    }
}
