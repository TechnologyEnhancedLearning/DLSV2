namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Notifications;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Models.User;    
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;

    public class RegistrationServiceTests
    {
        private const string ApproverEmail = "approver@email.com";
        private const string ApprovedIpPrefix = "123.456.789";
        private const string NewCandidateNumber = "TU67";
        private const string RefactoredSystemBaseUrl = "refactoredUrl";
        private const string OldSystemBaseUrl = "oldUrl";
        private static readonly (int, string, int) NewDelegateIdAndCandidateNumber = (2, NewCandidateNumber, 61188);

        private ICentresDataService centresDataService = null!;
        private IClockUtility clockUtility = null!;
        private IConfiguration config = null!;
        private IEmailService emailService = null!;
        private IEmailVerificationDataService emailVerificationDataService = null!;
        private IEmailVerificationService emailVerificationService = null!;
        private IGroupsService groupsService = null!;
        private INotificationDataService notificationDataService = null!;
        private IPasswordDataService passwordDataService = null!;
        private IPasswordResetService passwordResetService = null!;
        private IRegistrationDataService registrationDataService = null!;
        private IRegistrationService registrationService = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            registrationDataService = A.Fake<IRegistrationDataService>();
            passwordDataService = A.Fake<IPasswordDataService>();
            passwordResetService = A.Fake<IPasswordResetService>();
            emailService = A.Fake<IEmailService>();
            centresDataService = A.Fake<ICentresDataService>();
            config = A.Fake<IConfiguration>();
            supervisorDelegateService = A.Fake<ISupervisorDelegateService>();
            userService = A.Fake<IUserService>();
            notificationDataService = A.Fake<INotificationDataService>();
            userDataService = A.Fake<IUserDataService>();
            emailVerificationDataService = A.Fake<IEmailVerificationDataService>();
            clockUtility = A.Fake<IClockUtility>();
            groupsService = A.Fake<IGroupsService>();
            emailVerificationService = A.Fake<IEmailVerificationService>();

            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(OldSystemBaseUrl);
            A.CallTo(() => config["AppRootPath"]).Returns(RefactoredSystemBaseUrl);

            A.CallTo(() => centresDataService.GetCentreIpPrefixes(RegistrationModelTestHelper.Centre))
                .Returns(new[] { ApprovedIpPrefix });
            A.CallTo(() => centresDataService.GetCentreManagerDetails(A<int>._))
                .Returns(("Test", "Approver", ApproverEmail));

            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        A<DelegateRegistrationModel>._,
                        false,
                        A<bool>._
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);

            registrationService = new RegistrationService(
                registrationDataService,
                passwordDataService,
                passwordResetService,
                emailService,
                centresDataService,
                config,
                supervisorDelegateService,
                userDataService,
                notificationDataService,
                new NullLogger<RegistrationService>(),
                userService,
                emailVerificationDataService,
                clockUtility,
                groupsService,
                emailVerificationService
            );
        }

        [Test]
        public void Registering_delegate_with_approved_IP_registers_delegate_as_approved()
        {
            // Given
            const string clientIp = ApprovedIpPrefix + ".100";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            var (_, approved) = registrationService.RegisterDelegateForNewUser(
                model,
                clientIp,
                false,
                false
            );

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterNewUserAndDelegateAccount(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved),
                            false,
                            false
                        )
                )
                .MustHaveHappened();
            approved.Should().BeTrue();
        }

        [Test]
        public void Registering_delegate_on_localhost_registers_delegate_as_approved()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                "::1",
                false,
                false
            );

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterNewUserAndDelegateAccount(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved),
                            false,
                            false
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_with_unapproved_IP_registers_delegate_as_unapproved()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                "987.654.321.100",
                false,
                false
            );

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterNewUserAndDelegateAccount(
                            A<DelegateRegistrationModel>.That.Matches(d => !d.Approved),
                            false,
                            false
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_sends_approval_email_with_old_site_approval_link()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenAdminsToNotifyHaveEmails(new[] { ApproverEmail });

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                string.Empty,
                false,
                false
            );

            // Then
            RegistrationRequiresApprovalEmailMustHaveBeenSentTo(ApproverEmail);
        }

        [Test]
        public void Registering_delegate_sends_approval_email_with_refactored_tracking_system_link()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenAdminsToNotifyHaveEmails(new[] { ApproverEmail });

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                string.Empty,
                true,
                false
            );

            // Then
            RegistrationRequiresApprovalEmailMustHaveBeenSentTo(ApproverEmail);
        }

        [Test]
        public void Registering_automatically_approved_does_not_send_email()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                "123.456.789.100",
                false,
                false
            );

            // Then
            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustNotHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_set_password()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                string.Empty,
                false,
                false
            );

            // Then
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.PasswordHash
                    )
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_returns_candidate_number()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            var candidateNumber =
                registrationService.RegisterDelegateForNewUser(
                        model,
                        string.Empty,
                        false,
                        false
                    )
                    .candidateNumber;

            // Then
            candidateNumber.Should().Be(NewCandidateNumber);
        }

        [Test]
        public void Registering_delegate_should_add_CandidateId_to_all_SupervisorDelegate_records_found_by_email()
        {
            // Given
            const int delegateId = 777;
            const int delegateUserId = 270058;
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);

            A.CallTo(
                () => registrationDataService.RegisterNewUserAndDelegateAccount(model, A<bool>._, A<bool>._)
            ).Returns((delegateId, "CANDIDATE_NUMBER", delegateUserId));

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                string.Empty,
                false,
                false,
                999
            );

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>.That.IsSameSequenceAs(supervisorDelegateIds),
                    delegateUserId
                )
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_not_update_any_SupervisorDelegate_records_if_none_found()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenNoPendingSupervisorDelegateRecordsForEmail();

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                string.Empty,
                false,
                false,
                999
            );

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        [TestCase(2, false)]
        [TestCase(0, true)]
        public void Registering_delegate_should_send_SupervisorDelegate_email_if_necessary(
            int supervisorDelegateId,
            bool expectedEmailToBeSent
        )
        {
            // Given
            GivenPendingSupervisorDelegateIdsForEmailAre(new List<int> { 1, 2, 3, 4, 5 });
            GivenAdminsToNotifyHaveEmails(new[] { ApproverEmail });
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                string.Empty,
                false,
                false,
                supervisorDelegateId
            );

            // Then
            RegistrationRequiresApprovalEmailMustHaveBeenSentTo(ApproverEmail, expectedEmailToBeSent ? 1 : 0);
        }

        [Test]
        public void CreateDelegateAccountForNewUser_adds_new_delegate_to_groups()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                string.Empty,
                false,
                false
            );

            // Then
            A.CallTo(
                () => groupsService.AddNewDelegateToAppropriateGroups(
                    NewDelegateIdAndCandidateNumber.Item1,
                    model
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Registering_delegate_generates_verification_hashes_and_sends_emails_to_primary_and_centre_emails()
        {
            // Given
            const string primaryEmail = "primary@email.com";
            const string centreSpecificEmail = "centre@email.com";
            const string clientIp = ApprovedIpPrefix + ".100";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                primaryEmail: primaryEmail,
                centreSpecificEmail: centreSpecificEmail
            );
            var userAccount = UserTestHelper.GetDefaultUserAccount();

            A.CallTo(() => userService.GetUserAccountByEmailAddress(primaryEmail)).Returns(userAccount);
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>._,
                    A<string>._
                )
            ).DoesNothing();

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                clientIp,
                false,
                false
            );

            // Then
            A.CallTo(() => userService.GetUserAccountByEmailAddress(primaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    userAccount,
                    A<List<string>>.That.Matches(
                        list => ListTestHelper.ListOfStringsMatch(
                            list,
                            new List<string> { primaryEmail, centreSpecificEmail }
                        )
                    ),
                    A<string>._
                )
            ).DoesNothing();
        }

        [Test]
        public void
            Registering_delegate_generates_verification_hash_and_sends_email_to_primary_only_if_centre_email_is_null()
        {
            // Given
            const string primaryEmail = "primary@email.com";
            const string? centreSpecificEmail = null;
            const string clientIp = ApprovedIpPrefix + ".100";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                primaryEmail: primaryEmail,
                centreSpecificEmail: centreSpecificEmail
            );
            var userAccount = UserTestHelper.GetDefaultUserAccount();

            A.CallTo(() => userService.GetUserAccountByEmailAddress(A<string>._)).Returns(userAccount);
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    A<UserAccount>._,
                    A<List<string>>._,
                    A<string>._
                )
            ).DoesNothing();

            // When
            registrationService.RegisterDelegateForNewUser(
                model,
                clientIp,
                false,
                false
            );

            // Then
            A.CallTo(() => userService.GetUserAccountByEmailAddress(primaryEmail)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    userAccount,
                    A<List<string>>.That.Matches(
                        list => ListTestHelper.ListOfStringsMatch(
                            list,
                            new List<string> { primaryEmail }
                        )
                    ),
                    A<string>._
                )
            ).DoesNothing();
        }

        [Test]
        public void RegisterDelegateByCentre_adds_new_delegate_to_groups()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.RegisterDelegateByCentre(
                model,
                string.Empty,
                false,
                1,
                0
            );

            // Then
            A.CallTo(
                () => groupsService.AddNewDelegateToAppropriateGroups(
                    NewDelegateIdAndCandidateNumber.Item1,
                    model
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Error_when_registering_delegate_with_duplicate_primary_email()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(() => userDataService.PrimaryEmailIsInUse(model.PrimaryEmail))
                .Returns(true);

            // When
            Action act = () => registrationService.RegisterDelegateForNewUser(
                model,
                string.Empty,
                false,
                false
            );

            // Then
            act.Should().Throw<DelegateCreationFailedException>();
            A.CallTo(
                () =>
                    registrationDataService.RegisterNewUserAndDelegateAccount(
                        A<DelegateRegistrationModel>._,
                        false,
                        A<bool>._
                    )
            ).MustNotHaveHappened();
            A.CallTo(
                () =>
                    emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void Error_when_registering_delegate_with_duplicate_centre_specific_email()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(
                    () => userDataService.CentreSpecificEmailIsInUseAtCentre(model.CentreSpecificEmail!, model.Centre)
                )
                .Returns(true);

            // When
            Action act = () => registrationService.RegisterDelegateForNewUser(
                model,
                string.Empty,
                false,
                false
            );

            // Then
            act.Should().Throw<DelegateCreationFailedException>();
            A.CallTo(
                () =>
                    registrationDataService.RegisterNewUserAndDelegateAccount(
                        A<DelegateRegistrationModel>._,
                        false,
                        A<bool>._
                    )
            ).MustNotHaveHappened();
            A.CallTo(
                () =>
                    emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_delegate_calls_data_service_to_set_prn()
        {
            // Given
            const string clientIp = ApprovedIpPrefix + ".100";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            var (_, approved) = registrationService.RegisterDelegateForNewUser(
                model,
                clientIp,
                false,
                false
            );

            // Then
            A.CallTo(
                    () =>
                        userDataService.UpdateDelegateProfessionalRegistrationNumber(
                            A<int>._,
                            model.ProfessionalRegistrationNumber,
                            true
                        )
                )
                .MustHaveHappenedOnceExactly();
            approved.Should().BeTrue();
        }

        [Test]
        public void RegisterCentreManager_registers_delegate_account_as_approved()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(
                model,
                false
            );

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterNewUserAndDelegateAccount(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved),
                            false,
                            false
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void Registering_admin_delegate_does_not_send_email()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(
                model,
                false
            );

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_admin_delegate_should_set_password()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(
                model,
                false
            );

            // Then
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.PasswordHash
                    )
            ).MustHaveHappened();
        }

        [Test]
        public void RegisterCentreManager_calls_all_relevant_registration_methods()
        {
            // Given
            const int userId = 123;
            var centreManagerModel = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity();

            A.CallTo(
                () => userDataService.GetDelegateByCandidateNumber(NewCandidateNumber)
            ).Returns(delegateEntity);
            A.CallTo(() => userDataService.GetUserIdFromDelegateId(delegateEntity.DelegateAccount.Id)).Returns(userId);

            // When
            registrationService.RegisterCentreManager(
                centreManagerModel,
                false
            );

            // Then
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        A<DelegateRegistrationModel>._,
                        false,
                        false
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.PasswordHash
                    )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                    () => registrationDataService.RegisterAdmin(
                        A<AdminAccountRegistrationModel>.That.Matches(
                            m => m.UserId == userId
                                 && m.CentreSpecificEmail == centreManagerModel.CentreSpecificEmail
                                 && m.CentreId == centreManagerModel.Centre
                                 && m.CategoryId == centreManagerModel.CategoryId
                                 && m.IsCentreAdmin == centreManagerModel.IsCentreAdmin
                                 && m.IsCentreManager == centreManagerModel.IsCentreManager
                                 && m.IsContentManager == centreManagerModel.IsContentManager
                                 && m.IsContentCreator == centreManagerModel.IsContentCreator
                                 && m.IsTrainer == centreManagerModel.IsTrainer
                                 && m.ImportOnly == centreManagerModel.ImportOnly
                                 && m.IsSupervisor == centreManagerModel.IsSupervisor
                                 && m.IsNominatedSupervisor == centreManagerModel.IsNominatedSupervisor
                                 && m.Active == centreManagerModel.CentreAccountIsActive
                        ),
                        A<PossibleEmailUpdate>.That.Matches(
                            update => PossibleEmailUpdateTestHelper.PossibleEmailUpdatesMatch(
                                update,
                                new PossibleEmailUpdate
                                {
                                    OldEmail = null,
                                    NewEmail = centreManagerModel.CentreSpecificEmail,
                                    NewEmailIsVerified = false,
                                }
                            )
                        )
                    )
                )
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => centresDataService.SetCentreAutoRegistered(RegistrationModelTestHelper.Centre))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Error_in_RegisterCentreManager_throws_exception()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();
            var exception = new DelegateCreationFailedException(
                "error message",
                DelegateCreationError.EmailAlreadyInUse
            );
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        A<DelegateRegistrationModel>._,
                        false,
                        false
                    )
                )
                .Throws(exception);

            // When
            Action act = () => registrationService.RegisterCentreManager(
                model,
                false
            );

            // Then
            act.Should().Throw<Exception>();
        }

        [Test]
        public void Error_in_RegisterCentreManager_fails_fast()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();
            var exception = new DelegateCreationFailedException(
                "error message",
                DelegateCreationError.EmailAlreadyInUse
            );
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        A<DelegateRegistrationModel>._,
                        false,
                        false
                    )
                )
                .Throws(exception);

            // When
            Action act = () => registrationService.RegisterCentreManager(
                model,
                false
            );

            // Then
            act.Should().Throw<Exception>();
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        A<DelegateRegistrationModel>._,
                        false,
                        false
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustNotHaveHappened();
            A.CallTo(
                    () => registrationDataService.RegisterAdmin(
                        A<AdminAccountRegistrationModel>._,
                        A<PossibleEmailUpdate>._
                    )
                )
                .MustNotHaveHappened();
            A.CallTo(() => centresDataService.SetCentreAutoRegistered(RegistrationModelTestHelper.Centre))
                .MustNotHaveHappened();
        }

        [Test]
        public void RegisterCentreManager_calls_data_service_to_set_prn()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(
                model,
                false
            );

            // Then
            A.CallTo(
                    () =>
                        userDataService.UpdateDelegateProfessionalRegistrationNumber(
                            A<int>._,
                            model.ProfessionalRegistrationNumber,
                            true
                        )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RegisterDelegateByCentre_sets_password_if_passwordHash_not_null()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        model,
                        false,
                        A<bool>._
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(
                model,
                "",
                false,
                1,
                null
            );

            // Then
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        model,
                        false,
                        A<bool>._
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                    () => passwordDataService.SetPasswordByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.PasswordHash
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void RegisterDelegateByCentre_does_not_set_password_if_passwordHash_is_null()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            model.PasswordHash = null;
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        model,
                        false,
                        A<bool>._
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(
                model,
                "",
                false,
                1,
                null
            );

            // Then
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        model,
                        false,
                        A<bool>._
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_schedules_welcome_email_if_notify_date_set()
        {
            // Given
            const string baseUrl = "base.com";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                passwordHash: null,
                notifyDate: new DateTime(2200, 1, 1)
            );
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        model,
                        false,
                        A<bool>._
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(
                model,
                baseUrl,
                false,
                1,
                null
            );

            // Then
            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    A<int>._,
                    baseUrl,
                    model.NotifyDate!.Value,
                    "RegisterDelegateByCentre_Refactor"
                )
            ).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void RegisterDelegateByCentre_does_not_schedule_welcome_email_if_notify_date_not_set()
        {
            // Given
            const string baseUrl = "base.com";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        model,
                        false,
                        A<bool>._
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(
                model,
                baseUrl,
                false,
                1,
                null
            );

            // Then
            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    A<int>._,
                    A<string>._,
                    A<DateTime>._,
                    A<string>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_should_add_CandidateId_to_all_SupervisorDelegate_records_found_by_email()
        {
            // Given
            const string baseUrl = "base.com";
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        model,
                        false,
                        A<bool>._
                    )
                )
                .Returns((777, NewCandidateNumber, 270058));
            A.CallTo(
                    () => userDataService.GetDelegateByCandidateNumber(NewCandidateNumber)
                )
                .Returns(UserTestHelper.GetDefaultDelegateEntity(777));

            // When
            registrationService.RegisterDelegateByCentre(
                model,
                baseUrl,
                false,
                1,
                null
            );

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>.That.IsSameSequenceAs(supervisorDelegateIds),
                    270058
                )
            ).MustHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_calls_data_service_to_set_prn()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(
                    () => registrationDataService.RegisterNewUserAndDelegateAccount(
                        model,
                        false,
                        A<bool>._
                    )
                )
                .Returns(NewDelegateIdAndCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(
                model,
                "",
                false,
                1,
                null
            );

            // Then
            A.CallTo(
                    () =>
                        userDataService.UpdateDelegateProfessionalRegistrationNumber(
                            A<int>._,
                            model.ProfessionalRegistrationNumber,
                            true
                        )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RegisterDelegateByCentre_throws_if_email_already_held_by_a_user_at_the_centre()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel(
                centreSpecificEmail: "email",
                centre: 1
            );
            A.CallTo(
                    () => userService.EmailIsHeldAtCentre(
                        "email",
                        1
                    )
                )
                .Returns(true);

            // When
            Action action = () => registrationService.RegisterDelegateByCentre(
                model,
                "",
                false,
                1,
                null
            );

            // Then
            action.Should().Throw<DelegateCreationFailedException>().Which.Error.Should()
                .Be(DelegateCreationError.EmailAlreadyInUse);
        }

        [Test]
        public void
            PromoteDelegateToAdmin_reactivates_admin_record_if_inactive_admin_already_exists()
        {
            // Given
            const int categoryId = 1;
            const int userId = 2;
            var adminAccount = UserTestHelper.GetDefaultAdminAccount(
                active: false,
                categoryId: categoryId,
                userId: userId
            );

            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, true);

            A.CallTo(() => userDataService.GetAdminAccountsByUserId(userId)).Returns(new[] { adminAccount });

            // When
            registrationService.PromoteDelegateToAdmin(adminRoles, 1, userId, 2, false);

            // Then
            A.CallTo(() => userDataService.ReactivateAdmin(adminAccount.Id)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => userDataService.UpdateAdminUserPermissions(
                    adminAccount.Id,
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
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => registrationDataService.RegisterAdmin(
                    A<AdminAccountRegistrationModel>._,
                    A<PossibleEmailUpdate>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void PromoteDelegateToAdmin_updates_existing_active_admin_if_admin_at_same_centre_already_exists()
        {
            // Given
            const int categoryId = 1;
            const int userId = 2;
            const int centreId = 2;
            var adminAccount = UserTestHelper.GetDefaultAdminAccount(
                active: true,
                categoryId: categoryId,
                userId: userId
            );
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, true);

            A.CallTo(() => userDataService.GetAdminAccountsByUserId(userId)).Returns(new[] { adminAccount });

            // When
            registrationService.PromoteDelegateToAdmin(adminRoles, categoryId, userId, centreId, false);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.ReactivateAdmin(adminAccount.Id)).MustNotHaveHappened();
                A.CallTo(
                    () => userDataService.UpdateAdminUserPermissions(
                        adminAccount.Id,
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
                ).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => registrationDataService.RegisterAdmin(
                        A<AdminAccountRegistrationModel>._,
                        A<PossibleEmailUpdate>._
                    )
                ).MustNotHaveHappened();
            }
        }

        [Test]
        public void PromoteDelegateToAdmin_updates_existing_admin_if_inactive_admin_at_same_centre_already_exists()
        {
            // Given
            const int categoryId = 1;
            const int userId = 2;
            const int centreId = 2;
            var adminAccount = UserTestHelper.GetDefaultAdminAccount(
                active: false,
                categoryId: categoryId,
                userId: userId
            );
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, true);

            A.CallTo(() => userDataService.GetAdminAccountsByUserId(userId)).Returns(new[] { adminAccount });

            // When
            registrationService.PromoteDelegateToAdmin(adminRoles, categoryId, userId, centreId, false);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.ReactivateAdmin(adminAccount.Id)).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => userDataService.UpdateAdminUserPermissions(
                        adminAccount.Id,
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
                ).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => registrationDataService.RegisterAdmin(
                        A<AdminAccountRegistrationModel>._,
                        A<PossibleEmailUpdate>._
                    )
                ).MustNotHaveHappened();
            }
        }

        [Test]
        public void PromoteDelegateToAdmin_calls_data_service_with_expected_values_if_no_existing_admin()
        {
            // Given
            const int categoryId = 1;
            const int userId = 2;
            const int centreId = 2;
            var activeAdminAtDifferentCentre = UserTestHelper.GetDefaultAdminAccount(centreId: 3, active: true);
            var inactiveAdminAtDifferentCentre = UserTestHelper.GetDefaultAdminAccount(centreId: 4, active: false);
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true, true);
            A.CallTo(() => userDataService.GetAdminAccountsByUserId(userId)).Returns(
                new[] { activeAdminAtDifferentCentre, inactiveAdminAtDifferentCentre }
            );

            // When
            registrationService.PromoteDelegateToAdmin(adminRoles, categoryId, userId, centreId, false);

            // Then
            A.CallTo(
                () => registrationDataService.RegisterAdmin(
                    A<AdminAccountRegistrationModel>.That.Matches(
                        a =>
                            a.CentreId == centreId &&
                            a.Active &&
                            a.IsCentreAdmin == adminRoles.IsCentreAdmin &&
                            a.IsCentreManager == adminRoles.IsCentreManager &&
                            a.IsContentManager == adminRoles.IsContentManager &&
                            a.ImportOnly == adminRoles.IsCmsAdministrator &&
                            a.IsContentCreator == adminRoles.IsContentCreator &&
                            a.IsTrainer == adminRoles.IsTrainer &&
                            a.IsSupervisor == adminRoles.IsSupervisor
                    ),
                    null
                )
            ).MustHaveHappened();
            UpdateToExistingAdminAccountMustNotHaveHappened();
        }

        [Test]
        public void PromoteDelegateToAdmin_with_merge_param_calls_data_service_with_merged_admin_roles()
        {
            // Given
            const bool mergeAdminRoles = true;

            var activeAdmin = UserTestHelper.GetDefaultAdminAccount(centreId: 3, active: true);
            activeAdmin.IsCentreAdmin = true;
            activeAdmin.IsSupervisor = false;
            activeAdmin.IsNominatedSupervisor = true;
            activeAdmin.IsTrainer = false;
            activeAdmin.IsContentCreator = true;
            activeAdmin.IsContentManager = false;
            activeAdmin.ImportOnly = true;
            activeAdmin.IsCentreManager = false;

            var newAdminRoles = new AdminRoles(
                isCentreAdmin: true,
                isSupervisor: true,
                isNominatedSupervisor: true,
                isTrainer: true,
                isContentCreator: false,
                isContentManager: false,
                importOnly: false,
                isCentreManager: false
            );

            A.CallTo(() => userDataService.GetAdminAccountsByUserId(activeAdmin.UserId)).Returns(
                new[] { activeAdmin }
            );

            // When
            registrationService.PromoteDelegateToAdmin(newAdminRoles, activeAdmin.CategoryId, activeAdmin.UserId, activeAdmin.CentreId, mergeAdminRoles);

            // Then
            A.CallTo(
                () => userDataService.UpdateAdminUserPermissions(
                    activeAdmin.Id,
                    true,
                    true,
                    true,
                    true,
                    true,
                    false,
                    true,
                    activeAdmin.CategoryId,
                    false
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(() => registrationDataService.RegisterAdmin(A<AdminAccountRegistrationModel>._, A<PossibleEmailUpdate>._)).MustNotHaveHappened();
        }

        [Test]
        public void PromoteDelegateToAdmin_without_merge_param_calls_data_service_with_overridden_admin_roles()
        {
            // Given
            const bool mergeAdminRoles = false;

            var activeAdmin = UserTestHelper.GetDefaultAdminAccount(centreId: 3, active: true);
            activeAdmin.IsCentreAdmin = true;
            activeAdmin.IsSupervisor = false;
            activeAdmin.IsNominatedSupervisor = true;
            activeAdmin.IsTrainer = false;
            activeAdmin.IsContentCreator = true;
            activeAdmin.IsContentManager = false;
            activeAdmin.ImportOnly = true;
            activeAdmin.IsCentreManager = false;

            var newAdminRoles = new AdminRoles(
                isCentreAdmin: false,
                isSupervisor: false,
                isNominatedSupervisor: false,
                isTrainer: true,
                isContentCreator: false,
                isContentManager: false,
                importOnly: false,
                isCentreManager: false
            );

            A.CallTo(() => userDataService.GetAdminAccountsByUserId(activeAdmin.UserId)).Returns(
                new[] { activeAdmin }
            );

            // When
            registrationService.PromoteDelegateToAdmin(newAdminRoles, activeAdmin.CategoryId, activeAdmin.UserId, activeAdmin.CentreId, mergeAdminRoles);

            // Then
            A.CallTo(
                () => userDataService.UpdateAdminUserPermissions(
                    activeAdmin.Id,
                    false,
                    false,
                    false,
                    true,
                    false,
                    false,
                    false,
                    activeAdmin.CategoryId,
                    false
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(() => registrationDataService.RegisterAdmin(A<AdminAccountRegistrationModel>._, A<PossibleEmailUpdate>._)).MustNotHaveHappened();
        }

        [Test]
        public void CreateDelegateAccountForExistingUser_with_approved_IP_registers_delegate_as_approved()
        {
            // Given
            const int userId = 2;
            const string clientIp = ApprovedIpPrefix + ".100";
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();
            var currentTime = DateTime.Now;
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            var (_, approved, _) = registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                clientIp,
                false
            );

            // Then
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    model.CentreSpecificEmail!,
                    model.Centre,
                    userId
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre(A<string>._, A<int>._))
                .MustNotHaveHappened();

            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved),
                            userId,
                            currentTime,
                            A<PossibleEmailUpdate>.That.Matches(
                                update => PossibleEmailUpdateTestHelper.PossibleEmailUpdatesMatch(
                                    update,
                                    new PossibleEmailUpdate
                                    {
                                        OldEmail = "",
                                        NewEmail = model.CentreSpecificEmail,
                                        NewEmailIsVerified = false,
                                    }
                                )
                            ),
                            null
                        )
                )
                .MustHaveHappened();

            A.CallTo(
                () => registrationDataService.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                    A<DelegateRegistrationModel>._,
                    A<int>._,
                    A<int>._,
                    A<DateTime>._,
                    A<PossibleEmailUpdate>._
                )
            ).MustNotHaveHappened();
            approved.Should().BeTrue();
        }

        [Test]
        public void CreateDelegateAccountForExistingUser_on_localhost_registers_delegate_as_approved()
        {
            // Given
            const int userId = 2;
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();
            var currentTime = DateTime.Now;
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            var (_, approved, _) = registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                "::1",
                false
            );

            // Then
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    model.CentreSpecificEmail!,
                    model.Centre,
                    userId
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre(A<string>._, A<int>._))
                .MustNotHaveHappened();

            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved),
                            userId,
                            currentTime,
                            A<PossibleEmailUpdate>.That.Matches(
                                update => PossibleEmailUpdateTestHelper.PossibleEmailUpdatesMatch(
                                    update,
                                    new PossibleEmailUpdate
                                    {
                                        OldEmail = "",
                                        NewEmail = model.CentreSpecificEmail,
                                        NewEmailIsVerified = false,
                                    }
                                )
                            ),
                            null
                        )
                )
                .MustHaveHappened();
            A.CallTo(
                () => registrationDataService.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                    A<DelegateRegistrationModel>._,
                    A<int>._,
                    A<int>._,
                    A<DateTime>._,
                    A<PossibleEmailUpdate>._
                )
            ).MustNotHaveHappened();
            approved.Should().BeTrue();
        }

        [Test]
        public void CreateDelegateAccountForExistingUser_with_unapproved_IP_registers_delegate_as_unapproved()
        {
            // Given
            const int userId = 2;
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();
            var currentTime = DateTime.Now;
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);

            // When
            var (_, approved, _) = registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                "987.654.321.100",
                false
            );

            // Then
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    model.CentreSpecificEmail!,
                    model.Centre,
                    userId
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(() => userDataService.CentreSpecificEmailIsInUseAtCentre(A<string>._, A<int>._))
                .MustNotHaveHappened();

            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                            A<DelegateRegistrationModel>.That.Matches(d => !d.Approved),
                            userId,
                            currentTime,
                            A<PossibleEmailUpdate>.That.Matches(
                                update => PossibleEmailUpdateTestHelper.PossibleEmailUpdatesMatch(
                                    update,
                                    new PossibleEmailUpdate
                                    {
                                        OldEmail = "",
                                        NewEmail = model.CentreSpecificEmail,
                                        NewEmailIsVerified = false,
                                    }
                                )
                            ),
                            null
                        )
                )
                .MustHaveHappened();
            A.CallTo(
                () => registrationDataService.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                    A<DelegateRegistrationModel>._,
                    A<int>._,
                    A<int>._,
                    A<DateTime>._,
                    A<PossibleEmailUpdate>._
                )
            ).MustNotHaveHappened();
            approved.Should().BeFalse();
        }

        [Test]
        public void CreateDelegateAccountForExistingUser_adds_new_delegate_to_groups()
        {
            // Given
            const int userId = 2;
            const int delegateId = 2;
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel(answer6: null);
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(),
                new AdminAccount[] { },
                new DelegateAccount[] { }
            );

            A.CallTo(
                () => registrationDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                    A<DelegateRegistrationModel>.That.Matches(
                        m =>
                            m.Centre == model.Centre &&
                            m.CentreSpecificEmail == model.CentreSpecificEmail &&
                            m.Answer1 == model.Answer1 &&
                            m.Answer2 == model.Answer2 &&
                            m.Answer3 == model.Answer3 &&
                            m.Answer4 == model.Answer4 &&
                            m.Answer5 == model.Answer5 &&
                            m.Answer6 == model.Answer6
                    ),
                    userId,
                    A<DateTime>._,
                    A<PossibleEmailUpdate>._,
                    A<IDbTransaction?>._
                )
            ).Returns((delegateId, "fake"));
            A.CallTo(() => userService.GetUserById(userEntity.UserAccount.Id)).Returns(userEntity);

            // When
            registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                "987.654.321.100",
                false
            );

            // Then
            A.CallTo(
                () => groupsService.AddNewDelegateToAppropriateGroups(
                    NewDelegateIdAndCandidateNumber.Item1,
                    A<DelegateRegistrationModel>.That.Matches(
                        m =>
                            m.Answer1 == model.Answer1 &&
                            m.Answer2 == model.Answer2 &&
                            m.Answer3 == model.Answer3 &&
                            m.Answer4 == model.Answer4 &&
                            m.Answer5 == model.Answer5 &&
                            m.Answer6 == model.Answer6 &&
                            m.JobGroup == userEntity.UserAccount.JobGroupId &&
                            m.Centre == model.Centre
                    )
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void
            CreateDelegateAccountForExistingUser_throws_exception_if_user_already_has_active_delegate_at_centre()
        {
            // Given
            const int userId = 2;
            const int existingDelegateId = 5;
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();
            var currentTime = DateTime.Now;
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);
            GivenUserEntityExistsWithDelegate(userId, existingDelegateId, model.Centre, true);

            // When
            Action act = () => registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                "::1",
                false
            );

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                            A<DelegateRegistrationModel>._,
                            A<int>._,
                            A<DateTime>._,
                            A<PossibleEmailUpdate>._,
                            A<IDbTransaction>._
                        )
                )
                .MustNotHaveHappened();
            A.CallTo(
                () => registrationDataService.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                    A<DelegateRegistrationModel>._,
                    A<int>._,
                    A<int>._,
                    A<DateTime>._,
                    A<PossibleEmailUpdate>._
                )
            ).MustNotHaveHappened();
            var expectedError =
                "Could not create account for delegate on registration. " +
                $"Failure: active delegate account with ID {existingDelegateId} already exists " +
                $"at centre with ID {model.Centre} for user with ID {userId}";
            act.Should().Throw<DelegateCreationFailedException>().WithMessage(expectedError);
        }

        [Test]
        public void
            CreateDelegateAccountForExistingUser_reregisters_delegate_if_user_already_has_inactive_delegate_at_centre()
        {
            // Given
            const int userId = 2;
            const int existingDelegateId = 5;
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();
            var currentTime = DateTime.Now;

            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);
            GivenUserEntityExistsWithDelegate(userId, existingDelegateId, model.Centre, false);

            // When
            registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                "::1",
                false
            );

            // Then
            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentre(A<string>._, A<int>._)
            ).MustNotHaveHappened();

            A.CallTo(
                () => userDataService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    model.CentreSpecificEmail!,
                    model.Centre,
                    userId
                )
            ).MustHaveHappenedOnceExactly();

            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                            A<DelegateRegistrationModel>._,
                            A<int>._,
                            A<DateTime>._,
                            A<PossibleEmailUpdate>._,
                            A<IDbTransaction>._
                        )
                )
                .MustNotHaveHappened();

            A.CallTo(
                () => registrationDataService.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                    A<DelegateRegistrationModel>.That.Matches(
                        d => d.Centre == model.Centre
                             && d.Approved
                             && d.CentreAccountIsActive
                             && d.Answer1 == model.Answer1
                             && d.Answer2 == model.Answer2
                             && d.Answer3 == model.Answer3
                             && d.Answer4 == model.Answer4
                             && d.Answer5 == model.Answer5
                             && d.Answer6 == model.Answer6
                    ),
                    userId,
                    existingDelegateId,
                    currentTime,
                    A<PossibleEmailUpdate>.That.Matches(
                        update => PossibleEmailUpdateTestHelper.PossibleEmailUpdatesMatch(
                            update,
                            new PossibleEmailUpdate
                            {
                                OldEmail = "",
                                NewEmail = model.CentreSpecificEmail,
                                NewEmailIsVerified = false,
                            }
                        )
                    )
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
            CreateDelegateAccountForExistingUser_calls_data_service_with_correct_value_for_centreEmailRequiresVerification_when_registering_new_delegate_account(
                bool newEmailIsVerifiedForUser
            )
        {
            // Given
            const int userId = 2;
            const string clientIp = ApprovedIpPrefix + ".100";
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();
            var currentTime = DateTime.Now;

            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);
            A.CallTo(
                () => emailVerificationDataService.AccountEmailIsVerifiedForUser(userId, model.CentreSpecificEmail)
            ).Returns(newEmailIsVerifiedForUser);

            // When
            registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                clientIp,
                false
            );

            // Then
            A.CallTo(
                () => emailVerificationDataService.AccountEmailIsVerifiedForUser(userId, model.CentreSpecificEmail)
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => registrationDataService.RegisterDelegateAccountAndCentreDetailForExistingUser(
                    A<DelegateRegistrationModel>.That.Matches(d => d.Approved),
                    userId,
                    currentTime,
                    A<PossibleEmailUpdate>.That.Matches(
                        update => PossibleEmailUpdateTestHelper.PossibleEmailUpdatesMatch(
                            update,
                            new PossibleEmailUpdate
                            {
                                OldEmail = string.Empty,
                                NewEmail = model.CentreSpecificEmail,
                                NewEmailIsVerified = newEmailIsVerifiedForUser,
                            }
                        )
                    ),
                    null
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void
            CreateDelegateAccountForExistingUser_calls_data_service_with_correct_value_for_NewEmailIsVerified_when_reregistering_delegate(
                bool newEmailIsVerifiedForUser
            )
        {
            // Given
            const int userId = 2;
            const int existingDelegateId = 5;
            const string? oldEmail = null;
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();
            var currentTime = DateTime.Now;
            A.CallTo(() => clockUtility.UtcNow).Returns(currentTime);
            GivenUserEntityExistsWithDelegate(userId, existingDelegateId, model.Centre, false);

            A.CallTo(() => userDataService.GetCentreEmail(userId, model.Centre)).Returns(oldEmail);
            A.CallTo(
                () => emailVerificationDataService.AccountEmailIsVerifiedForUser(userId, model.CentreSpecificEmail)
            ).Returns(newEmailIsVerifiedForUser);

            // When
            registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                "::1",
                false
            );

            // Then
            A.CallTo(
                () => emailVerificationDataService.AccountEmailIsVerifiedForUser(userId, model.CentreSpecificEmail)
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => registrationDataService.ReregisterDelegateAccountAndCentreDetailForExistingUser(
                    A<DelegateRegistrationModel>._,
                    userId,
                    existingDelegateId,
                    currentTime,
                    A<PossibleEmailUpdate>.That.Matches(
                        update => PossibleEmailUpdateTestHelper.PossibleEmailUpdatesMatch(
                            update,
                            new PossibleEmailUpdate
                            {
                                OldEmail = oldEmail,
                                NewEmail = model.CentreSpecificEmail,
                                NewEmailIsVerified = newEmailIsVerifiedForUser,
                            }
                        )
                    )
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void CreateDelegateAccountForExistingUser_sends_approval_email()
        {
            // Given
            const bool refactoredTrackingSystemEnabled = false;
            const int userId = 2;
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();
            GivenAdminsToNotifyHaveEmails(new[] { ApproverEmail });

            // When
            registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                string.Empty,
                refactoredTrackingSystemEnabled
            );

            // Then
            RegistrationRequiresApprovalEmailMustHaveBeenSentTo(ApproverEmail);
        }

        [Test]
        public void CreateDelegateAccountForExistingUser_sends_approval_email_with_old_site_approval_link()
        {
            // Given
            const bool refactoredTrackingSystemEnabled = false;
            const int userId = 2;
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();
            GivenAdminsToNotifyHaveEmails(new[] { ApproverEmail });

            // When
            registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                string.Empty,
                refactoredTrackingSystemEnabled
            );

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(
                        A<Email>.That.Matches(
                            e => e.Body.TextBody.Contains(OldSystemBaseUrl + "/tracking/approvedelegates")
                        )
                    )
            ).MustHaveHappened();
        }

        [Test]
        public void CreateDelegateAccountForExistingUser_sends_approval_email_with_refactored_tracking_system_link()
        {
            // Given
            const bool refactoredTrackingSystemEnabled = true;
            const int userId = 2;
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();
            GivenAdminsToNotifyHaveEmails(new[] { ApproverEmail });

            // When
            registrationService.CreateDelegateAccountForExistingUser(
                model,
                userId,
                string.Empty,
                refactoredTrackingSystemEnabled
            );

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(
                        A<Email>.That.Matches(
                            e => e.Body.TextBody.Contains(RefactoredSystemBaseUrl + "/TrackingSystem/Delegates/Approve")
                        )
                    )
            ).MustHaveHappened();
        }

        [Test]
        public void CreateDelegateAccountForExistingUser_approved_does_not_send_email()
        {
            // Given
            const int userId = 2;
            var model = RegistrationModelTestHelper.GetDefaultInternalDelegateRegistrationModel();

            // When
            registrationService.CreateDelegateAccountForExistingUser(model, userId, "123.456.789.100", false);

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
        }

        private void GivenNoPendingSupervisorDelegateRecordsForEmail()
        {
            A.CallTo(
                    () => supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                        A<int>._,
                        A<List<string?>>._
                    )
                )
                .Returns(new List<SupervisorDelegate>());
        }

        private void GivenPendingSupervisorDelegateIdsForEmailAre(IEnumerable<int> supervisorDelegateIds)
        {
            var supervisorDelegates = supervisorDelegateIds.Select(id => new SupervisorDelegate { ID = id });
            A.CallTo(
                    () => supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                        A<int>._,
                        A<List<string>>._
                    )
                )
                .Returns(supervisorDelegates);
        }

        private void UpdateToExistingAdminAccountMustNotHaveHappened()
        {
            A.CallTo(() => userDataService.ReactivateAdmin(A<int>._)).MustNotHaveHappened();
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

        private void GivenUserEntityExistsWithDelegate(
            int userId,
            int delegateId,
            int delegateCentreId,
            bool delegateActiveStatus
        )
        {
            A.CallTo(() => userService.GetUserById(userId)).Returns(
                new UserEntity(
                    new UserAccount(),
                    new List<AdminAccount>(),
                    new List<DelegateAccount>
                    {
                        UserTestHelper.GetDefaultDelegateAccount(
                            delegateId,
                            centreId: delegateCentreId,
                            active: delegateActiveStatus,
                            userId: userId
                        ),
                    }
                )
            );
        }

        private void GivenAdminsToNotifyHaveEmails(IEnumerable<string> adminEmails)
        {
            A.CallTo(() => notificationDataService.GetAdminRecipientsForCentreNotification(A<int>._, A<int>._))
                .Returns(
                    adminEmails.Select(
                        email => Builder<NotificationRecipient>.CreateNew().With(r => r.Email = email).Build()
                    )
                );
        }

        private void RegistrationRequiresApprovalEmailMustHaveBeenSentTo(string recipientEmail, int numberOfTimes = 1)
        {
            A.CallTo(
                () => emailService.SendEmail(
                    A<Email>.That.Matches(
                        email =>
                            email.To[0] == recipientEmail && email.To.Length == 1 &&
                            email.Cc.IsNullOrEmpty() &&
                            email.Bcc.IsNullOrEmpty() &&
                            email.Subject == "Digital Learning Solutions Registration Requires Approval"
                    )
                )
            ).MustHaveHappened(numberOfTimes, Times.Exactly);
        }
    }
}
