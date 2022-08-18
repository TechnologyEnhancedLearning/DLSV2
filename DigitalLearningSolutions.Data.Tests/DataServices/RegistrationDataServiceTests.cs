namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class RegistrationDataServiceTests
    {
        private IClockUtility clockUtility = null!;
        private SqlConnection connection = null!;
        private IEmailVerificationDataService emailVerificationDataService = null!;
        private IUserDataService fakeUserDataService = null!;
        private ILogger<IRegistrationDataService> logger = null!;
        private INotificationPreferencesDataService notificationPreferencesDataService = null!;
        private RegistrationDataService service = null!;
        private RegistrationDataService serviceWithFakeUserDataService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
            fakeUserDataService = A.Fake<IUserDataService>();
            emailVerificationDataService = A.Fake<IEmailVerificationDataService>();
            clockUtility = A.Fake<IClockUtility>();
            logger = A.Fake<ILogger<IRegistrationDataService>>();
            service = new RegistrationDataService(
                connection,
                userDataService,
                emailVerificationDataService,
                clockUtility,
                logger
            );
            serviceWithFakeUserDataService = new RegistrationDataService(
                connection,
                fakeUserDataService,
                emailVerificationDataService,
                clockUtility,
                logger
            );
            notificationPreferencesDataService = new NotificationPreferencesDataService(connection);
        }

        [Test]
        public void RegisterNewUserAndDelegateAccount_sets_all_fields_correctly_on_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            const bool isRegisteredByAdmin = false;
            var dateTime = new DateTime(2022, 6, 16, 9, 41, 30);
            A.CallTo(() => clockUtility.UtcNow).Returns(dateTime);

            // Given
            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    centre: 3,
                    activeUser: false,
                    active: true,
                    approved: true,
                    isSelfRegistered: false
                );

            // When
            var (delegateId, candidateNumber) = service.RegisterNewUserAndDelegateAccount(
                delegateRegistrationModel,
                false,
                isRegisteredByAdmin
            );

            // Then
            var delegateEntity = userDataService.GetDelegateById(delegateId);
            using (new AssertionScope())
            {
                delegateEntity!.UserAccount.FirstName.Should().Be(delegateRegistrationModel.FirstName);
                delegateEntity.UserAccount.LastName.Should().Be(delegateRegistrationModel.LastName);
                delegateEntity.UserAccount.PrimaryEmail.Should().Be(delegateRegistrationModel.PrimaryEmail);
                delegateEntity.UserAccount.PasswordHash.Should().BeEmpty();
                delegateEntity.UserAccount.TermsAgreed.Should().BeNull();
                delegateEntity.UserAccount.DetailsLastChecked.Should().Be(dateTime);
                delegateEntity.UserAccount.Active.Should().BeFalse();
                delegateEntity.DelegateAccount.CentreId.Should().Be(delegateRegistrationModel.Centre);
                delegateEntity.DelegateAccount.Answer1.Should().Be(delegateRegistrationModel.Answer1);
                delegateEntity.DelegateAccount.Answer2.Should().Be(delegateRegistrationModel.Answer2);
                delegateEntity.DelegateAccount.Answer3.Should().Be(delegateRegistrationModel.Answer3);
                delegateEntity.DelegateAccount.Answer4.Should().Be(delegateRegistrationModel.Answer4);
                delegateEntity.DelegateAccount.Answer5.Should().Be(delegateRegistrationModel.Answer5);
                delegateEntity.DelegateAccount.Answer6.Should().Be(delegateRegistrationModel.Answer6);
                delegateEntity.DelegateAccount.Approved.Should().BeTrue();
                delegateEntity.DelegateAccount.Active.Should().BeTrue();
                delegateEntity.DelegateAccount.SelfReg.Should().BeFalse();
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
            A.CallTo(() => clockUtility.UtcNow).Returns(dateTime);

            // When
            var (delegateId, candidateNumber) = service.RegisterNewUserAndDelegateAccount(
                delegateRegistrationModel,
                true,
                false
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
        public void
            RegisterNewUserAndDelegateAccount_sets_email_verified_to_null_if_delegate_is_self_registered()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const string centreSpecificEmail = "centre@email.com";
            const bool registeredByAdmin = false;
            var delegateRegistrationModel = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                centre: 3,
                centreSpecificEmail: centreSpecificEmail
            );
            var currentTime = new DateTime(2022, 6, 16, 9, 41, 30);
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            var (delegateId, _) = service.RegisterNewUserAndDelegateAccount(
                delegateRegistrationModel,
                true,
                registeredByAdmin
            );

            // Then
            var delegateEntity = userDataService.GetDelegateById(delegateId);
            using (new AssertionScope())
            {
                delegateEntity!.UserCentreDetails!.Email.Should().Be(centreSpecificEmail);
                delegateEntity.UserCentreDetails.EmailVerified.Should().BeNull();
            }
        }

        [Test]
        public void
            RegisterNewUserAndDelegateAccount_sets_email_verified_to_current_time_if_registered_by_admin()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const string centreSpecificEmail = "centre@email.com";
            const bool registeredByAdmin = true;
            var delegateRegistrationModel = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                centre: 3,
                centreSpecificEmail: centreSpecificEmail
            );
            var currentTime = new DateTime(2022, 6, 16, 9, 41, 30);
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            var (delegateId, _) = service.RegisterNewUserAndDelegateAccount(
                delegateRegistrationModel,
                true,
                registeredByAdmin
            );

            // Then
            var delegateEntity = userDataService.GetDelegateById(delegateId);
            using (new AssertionScope())
            {
                delegateEntity!.UserCentreDetails!.Email.Should().Be(centreSpecificEmail);
                delegateEntity.UserCentreDetails.EmailVerified.Should().Be(currentTime);
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
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);
            const int userId = 2;

            // When
            var (delegateId, candidateNumber) =
                service.RegisterDelegateAccountAndCentreDetailForExistingUser(
                    delegateRegistrationModel,
                    userId,
                    currentTime,
                    null
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
                    currentTime,
                    null
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
        [TestCase(false)]
        [TestCase(true)]
        public void RegisterDelegateAccountAndCentreDetailForExistingUser_updates_centre_email_when_email_is_updating(
            bool isEmailVerified
        )
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const int userId = 2;
            const int centreId = 3;
            const string centreEmail = "centre@email.com";
            var currentTime = new DateTime(2022, 2, 2);

            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    centreId,
                    centreSpecificEmail: centreEmail
                );

            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            serviceWithFakeUserDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                currentTime,
                new PossibleEmailUpdate
                {
                    OldEmail = null,
                    NewEmail = centreEmail,
                    NewEmailIsVerified = isEmailVerified,
                }
            );

            // Then
            A.CallTo(
                    () => fakeUserDataService.SetCentreEmail(
                        userId,
                        centreId,
                        centreEmail,
                        isEmailVerified ? currentTime : (DateTime?)null,
                        A<IDbTransaction?>._
                    )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void
            RegisterDelegateAccountAndCentreDetailForExistingUser_does_not_update_centre_email_when_email_is_not_updating(
                bool emailIsVerified
            )
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const int userId = 2;
            const int centreId = 3;
            const string centreEmail = "centre@email.com";
            var currentTime = new DateTime(2022, 2, 2);

            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    centreId,
                    centreSpecificEmail: centreEmail
                );

            // When
            serviceWithFakeUserDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                currentTime,
                new PossibleEmailUpdate
                {
                    OldEmail = centreEmail,
                    NewEmail = centreEmail,
                    NewEmailIsVerified = emailIsVerified,
                }
            );

            // Then
            A.CallTo(
                    () => fakeUserDataService.SetCentreEmail(
                        A<int>._,
                        A<int>._,
                        A<string?>._,
                        A<DateTime?>._,
                        A<IDbTransaction?>._
                    )
                )
                .MustNotHaveHappened();
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
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);
            var userBeforeUpdate = userDataService.GetUserAccountById(userId);
            var delegateBeforeUpdate = userDataService.GetDelegateAccountById(existingDelegateId);

            // When
            service.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                existingDelegateId,
                currentTime,
                new PossibleEmailUpdate
                {
                    OldEmail = null,
                    NewEmail = delegateRegistrationModel.CentreSpecificEmail,
                    NewEmailIsVerified = false,
                }
            );

            // Then
            using (new AssertionScope())
            {
                var userAfterUpdate = userDataService.GetUserAccountById(userId);
                var delegateAfterUpdate = userDataService.GetDelegateAccountById(existingDelegateId);

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
                delegateAfterUpdate.Active.Should().Be(delegateRegistrationModel.CentreAccountIsActive);
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
            const int userId = 281052;
            const int existingDelegateId = 142559;
            const int centreId = 121;
            var currentTime = new DateTime(2022, 06, 27, 11, 03, 12);
            var newCentreEmail = "newCentreEmailTest@test.com";
            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    centreId,
                    centreSpecificEmail: newCentreEmail
                );
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            service.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                existingDelegateId,
                currentTime,
                new PossibleEmailUpdate
                {
                    OldEmail = null,
                    NewEmail = delegateRegistrationModel.CentreSpecificEmail,
                    NewEmailIsVerified = false,
                }
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
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);
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
                currentTime,
                new PossibleEmailUpdate
                {
                    OldEmail = existingEmail,
                    NewEmail = null,
                    NewEmailIsVerified = false,
                }
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
        [TestCase(false)]
        [TestCase(true)]
        public void ReregisterDelegateAccountAndCentreDetailForExistingUser_updates_centre_email_when_email_is_updating(
            bool isEmailVerified
        )
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const int userId = 2;
            const int centreId = 3;
            const int delegateId = 4;
            const string oldCentreEmail = "old@centre.email";
            const string newCentreEmail = "new@centre.email";
            var currentTime = new DateTime(2022, 2, 2);

            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    centreId,
                    centreSpecificEmail: newCentreEmail
                );

            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            serviceWithFakeUserDataService.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                delegateId,
                currentTime,
                new PossibleEmailUpdate
                {
                    OldEmail = oldCentreEmail,
                    NewEmail = newCentreEmail,
                    NewEmailIsVerified = isEmailVerified,
                }
            );

            // Then
            A.CallTo(
                    () => fakeUserDataService.SetCentreEmail(
                        userId,
                        centreId,
                        newCentreEmail,
                        isEmailVerified ? currentTime : (DateTime?)null,
                        A<IDbTransaction?>._
                    )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void
            ReregisterDelegateAccountAndCentreDetailForExistingUser_does_not_update_centre_email_when_email_is_not_updating(
                bool emailIsVerified
            )
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const int userId = 2;
            const int centreId = 3;
            const int delegateId = 4;
            const string centreEmail = "centre@email.com";
            var currentTime = new DateTime(2022, 2, 2);

            var delegateRegistrationModel =
                RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    centreId,
                    centreSpecificEmail: centreEmail
                );

            // When
            serviceWithFakeUserDataService.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                delegateId,
                currentTime,
                new PossibleEmailUpdate
                {
                    OldEmail = centreEmail,
                    NewEmail = centreEmail,
                    NewEmailIsVerified = emailIsVerified,
                }
            );

            // Then
            A.CallTo(
                    () => fakeUserDataService.SetCentreEmail(
                        A<int>._,
                        A<int>._,
                        A<string?>._,
                        A<DateTime?>._,
                        A<IDbTransaction?>._
                    )
                )
                .MustNotHaveHappened();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void RegisterAdmin_updates_centre_email_when_email_is_updating(bool isEmailVerified)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const int userId = 2;
            const int centreId = 3;
            const string centreEmail = "centre@email.com";
            var currentTime = new DateTime(2022, 2, 2);

            var adminAccountRegistrationModel = new AdminAccountRegistrationModel(
                RegistrationModelTestHelper.GetDefaultAdminRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    centreId,
                    centreEmail,
                    categoryId: 1
                ),
                userId
            );

            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            serviceWithFakeUserDataService.RegisterAdmin(
                adminAccountRegistrationModel,
                new PossibleEmailUpdate
                {
                    OldEmail = null,
                    NewEmail = centreEmail,
                    NewEmailIsVerified = isEmailVerified,
                }
            );

            // Then
            A.CallTo(
                    () => fakeUserDataService.SetCentreEmail(
                        userId,
                        centreId,
                        centreEmail,
                        isEmailVerified ? currentTime : (DateTime?)null,
                        A<IDbTransaction?>._
                    )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void RegisterAdmin_does_not_update_centre_email_when_email_is_not_updating(bool emailIsVerified)
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const int userId = 2;
            const int centreId = 3;
            const string centreEmail = "centre@email.com";
            var currentTime = new DateTime(2022, 2, 2);

            var adminAccountRegistrationModel = new AdminAccountRegistrationModel(
                RegistrationModelTestHelper.GetDefaultAdminRegistrationModel(
                    "forename",
                    "surname",
                    "test@gmail.com",
                    centreId,
                    centreEmail,
                    categoryId: 1
                ),
                userId
            );

            // When
            serviceWithFakeUserDataService.RegisterAdmin(
                adminAccountRegistrationModel,
                new PossibleEmailUpdate
                {
                    OldEmail = centreEmail,
                    NewEmail = centreEmail,
                    NewEmailIsVerified = emailIsVerified,
                }
            );

            // Then
            A.CallTo(
                    () => fakeUserDataService.SetCentreEmail(
                        A<int>._,
                        A<int>._,
                        A<string?>._,
                        A<DateTime?>._,
                        A<IDbTransaction?>._
                    )
                )
                .MustNotHaveHappened();
        }

        [Test]
        public async Task
            ReregisterDelegateAccountAndCentreDetailForExistingUser_sets_email_verified_to_current_time_if_user_has_already_verified_new_email()
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
                    centreSpecificEmail: newCentreEmail,
                    isSelfRegistered: true
                );

            var possibleEmailUpdate = new PossibleEmailUpdate
            {
                OldEmail = "old@email.com",
                NewEmail = newCentreEmail,
                NewEmailIsVerified = true,
            };

            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            service.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                delegateRegistrationModel,
                userId,
                existingDelegateId,
                currentTime,
                possibleEmailUpdate
            );

            // Then
            using (new AssertionScope())
            {
                var userCentreDetails = connection.GetEmailAndVerifiedDateFromUserCentreDetails(userId, centreId);
                userCentreDetails.email.Should().Be(newCentreEmail);
                userCentreDetails.emailVerified.Should().Be(currentTime);
            }
        }

        [Test]
        public void RegisterAdmin_sets_all_fields_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel =
                RegistrationModelTestHelper.GetDefaultCentreManagerAccountRegistrationModel(
                    categoryId: 1
                );
            A.CallTo(() => clockUtility.UtcNow).Returns(DateTime.UtcNow);

            // When
            var id = service.RegisterAdmin(registrationModel, null);

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
        public void RegisterAdmin_sets_notification_preferences_correctly_on_centre_manager_admin_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var registrationModel =
                RegistrationModelTestHelper.GetDefaultCentreManagerAccountRegistrationModel(
                    categoryId: 1
                );
            A.CallTo(() => clockUtility.UtcNow).Returns(DateTime.UtcNow);

            // When
            var id = service.RegisterAdmin(registrationModel, null);

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

        [Test]
        public void RegisterAdmin_sets_email_verified_to_current_time_if_user_has_already_verified_new_email()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const string centreSpecificEmail = "centre@email.com";
            var registrationModel =
                RegistrationModelTestHelper.GetDefaultCentreManagerAccountRegistrationModel(
                    centreSpecificEmail: centreSpecificEmail
                );
            var currentTime = new DateTime(2022, 6, 16, 9, 41, 30);
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            var possibleEmailUpdate = new PossibleEmailUpdate
            {
                OldEmail = "old@email.com",
                NewEmail = centreSpecificEmail,
                NewEmailIsVerified = true,
            };

            // When
            var id = service.RegisterAdmin(registrationModel, possibleEmailUpdate);

            // Then
            var user = userDataService.GetAdminById(id)!;
            using (new AssertionScope())
            {
                user.UserCentreDetails!.Email.Should().Be(centreSpecificEmail);
                user.UserCentreDetails.EmailVerified.Should().Be(currentTime);
            }
        }

        [Test]
        public void RegisterAdmin_sets_email_verified_to_null_if_email_is_not_already_verified_for_user()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            const string centreSpecificEmail = "centre@email.com";
            var registrationModel =
                RegistrationModelTestHelper.GetDefaultCentreManagerAccountRegistrationModel(
                    centreSpecificEmail: centreSpecificEmail
                );

            var possibleEmailUpdate = new PossibleEmailUpdate
            {
                OldEmail = "old@email.com",
                NewEmail = centreSpecificEmail,
                NewEmailIsVerified = false,
            };

            // When
            var id = service.RegisterAdmin(registrationModel, possibleEmailUpdate);

            // Then
            var user = userDataService.GetAdminById(id)!;
            using (new AssertionScope())
            {
                user.UserCentreDetails!.Email.Should().Be(centreSpecificEmail);
                user.UserCentreDetails.EmailVerified.Should().BeNull();
            }
        }
    }
}
