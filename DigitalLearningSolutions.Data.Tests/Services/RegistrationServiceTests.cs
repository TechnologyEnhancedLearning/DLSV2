namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
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

        private ICentresDataService centresDataService = null!;
        private IConfiguration config = null!;
        private IEmailService emailService = null!;
        private IFrameworkNotificationService frameworkNotificationService = null!;
        private IPasswordDataService passwordDataService = null!;
        private IPasswordResetService passwordResetService = null!;
        private IRegistrationDataService registrationDataService = null!;
        private IRegistrationService registrationService = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;
        private IUserDataService userDataService = null!;

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
            frameworkNotificationService = A.Fake<IFrameworkNotificationService>();
            userDataService = A.Fake<IUserDataService>();

            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(OldSystemBaseUrl);
            A.CallTo(() => config["AppRootPath"]).Returns(RefactoredSystemBaseUrl);

            A.CallTo(() => centresDataService.GetCentreIpPrefixes(RegistrationModelTestHelper.Centre))
                .Returns(new[] { ApprovedIpPrefix });
            A.CallTo(() => centresDataService.GetCentreManagerDetails(A<int>._))
                .Returns(("Test", "Approver", ApproverEmail));

            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(A<DelegateRegistrationModel>._))
                .Returns(NewCandidateNumber);

            registrationService = new RegistrationService(
                registrationDataService,
                passwordDataService,
                passwordResetService,
                emailService,
                centresDataService,
                config,
                supervisorDelegateService,
                frameworkNotificationService,
                userDataService,
                new NullLogger<RegistrationService>()
            );
        }

        [Test]
        public void Registering_delegate_with_approved_IP_registers_delegate_as_approved()
        {
            // Given
            const string clientIp = ApprovedIpPrefix + ".100";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            var (_, approved) = registrationService.CreateDelegateAccountForNewUser(
                model,
                clientIp,
                false,
                null
            );

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterNewUserAndDelegateAccount(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved)
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
            registrationService.CreateDelegateAccountForNewUser(model, "::1", false, null);

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterNewUserAndDelegateAccount(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved)
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
            registrationService.CreateDelegateAccountForNewUser(model, "987.654.321.100", false, null);

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterNewUserAndDelegateAccount(
                            A<DelegateRegistrationModel>.That.Matches(d => !d.Approved)
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_sends_approval_email_with_old_site_approval_link()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.CreateDelegateAccountForNewUser(model, string.Empty, false, null);

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(
                        A<Email>.That.Matches(
                            e =>
                                e.To[0] == ApproverEmail &&
                                e.Cc.IsNullOrEmpty() &&
                                e.Bcc.IsNullOrEmpty() &&
                                e.Subject == "Digital Learning Solutions Registration Requires Approval" &&
                                e.Body.TextBody.Contains(OldSystemBaseUrl + "/tracking/approvedelegates")
                        )
                    )
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_sends_approval_email_with_refactored_tracking_system_link()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.CreateDelegateAccountForNewUser(model, string.Empty, true, null);

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(
                        A<Email>.That.Matches(
                            e =>
                                e.To[0] == ApproverEmail &&
                                e.Cc.IsNullOrEmpty() &&
                                e.Bcc.IsNullOrEmpty() &&
                                e.Subject == "Digital Learning Solutions Registration Requires Approval" &&
                                e.Body.TextBody.Contains(RefactoredSystemBaseUrl + "/TrackingSystem/Delegates/Approve")
                        )
                    )
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_automatically_approved_does_not_send_email()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.CreateDelegateAccountForNewUser(model, "123.456.789.100", false, null);

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_set_password()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            registrationService.CreateDelegateAccountForNewUser(model, string.Empty, false, null);

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
                registrationService.CreateDelegateAccountForNewUser(model, string.Empty, false, null)
                    .candidateNumber;

            // Then
            candidateNumber.Should().Be(NewCandidateNumber);
        }

        [Test]
        public void Registering_delegate_should_add_CandidateId_to_all_SupervisorDelegate_records_found_by_email()
        {
            // Given
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);
            A.CallTo(
                    () => userDataService.GetDelegateUserByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.Centre
                    )
                )
                .Returns(new DelegateUser { Id = 777 });

            // When
            registrationService.CreateDelegateAccountForNewUser(model, string.Empty, false, null, 999);

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>.That.IsSameSequenceAs(supervisorDelegateIds),
                    777
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
            registrationService.CreateDelegateAccountForNewUser(model, string.Empty, false, null, 999);

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_send_SupervisorDelegate_email_for_matching_id_record_only()
        {
            // Given
            const int supervisorDelegateId = 2;
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);

            // When
            registrationService.CreateDelegateAccountForNewUser(
                model,
                string.Empty,
                false,
                null,
                supervisorDelegateId
            );

            // Then
            A.CallTo(() => frameworkNotificationService.SendSupervisorDelegateAcceptance(supervisorDelegateId, 0))
                .MustHaveHappened();
            A.CallTo(
                () => frameworkNotificationService.SendSupervisorDelegateAcceptance(
                    A<int>.That.Matches(id => id != supervisorDelegateId),
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void Error_when_registering_delegate_with_duplicate_email()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(() => userDataService.GetAllExistingEmails()).Returns(new []{"testuser@email.com"});

            // When
            Action act = () => registrationService.CreateDelegateAccountForNewUser(model, string.Empty, false, null);

            // Then
            act.Should().Throw<DelegateCreationFailedException>();
            A.CallTo(
                () =>
                    registrationDataService.RegisterNewUserAndDelegateAccount(A<DelegateRegistrationModel>._)
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
            const string prn = "PRN";
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            var (_, approved) = registrationService.CreateDelegateAccountForNewUser(
                model,
                clientIp,
                false,
                prn
            );

            // Then
            A.CallTo(
                    () =>
                        userDataService.UpdateDelegateProfessionalRegistrationNumber(
                            A<int>._,
                            prn,
                            true
                        )
                )
                .MustHaveHappenedOnceExactly();
            approved.Should().BeTrue();
        }

        [Test]
        public void Registering_admin_delegate_registers_delegate_as_approved()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(model, 1);

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterNewUserAndDelegateAccount(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved)
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
            registrationService.RegisterCentreManager(model, 1);

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
            registrationService.RegisterCentreManager(model, 1);

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
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();

            // When
            registrationService.RegisterCentreManager(model, 1);

            // Then
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(A<DelegateRegistrationModel>._))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => registrationDataService.RegisterAdmin(model))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.SetCentreAutoRegistered(RegistrationModelTestHelper.Centre))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void Error_in_RegisterCentreManager_throws_exception()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(A<DelegateRegistrationModel>._)).Returns("-1");

            // When
            Action act = () => registrationService.RegisterCentreManager(model, 1);

            // Then
            act.Should().Throw<Exception>();
        }

        [Test]
        public void Error_in_RegisterCentreManager_fails_fast()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultCentreManagerRegistrationModel();
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(A<DelegateRegistrationModel>._)).Returns("-1");

            // When
            Action act = () => registrationService.RegisterCentreManager(model, 1);

            // Then
            act.Should().Throw<Exception>();
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(A<DelegateRegistrationModel>._))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustNotHaveHappened();
            A.CallTo(() => registrationDataService.RegisterAdmin(model))
                .MustNotHaveHappened();
            A.CallTo(() => centresDataService.SetCentreAutoRegistered(RegistrationModelTestHelper.Centre))
                .MustNotHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_sets_password_if_passwordHash_not_null()
        {
            // Given
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, "");

            // Then
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(model)).MustHaveHappened(1, Times.Exactly);
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
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, "");

            // Then
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(model)).MustHaveHappened(1, Times.Exactly);
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
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, baseUrl);

            // Then
            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    model.PrimaryEmail,
                    NewCandidateNumber,
                    baseUrl,
                    model.NotifyDate.Value,
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
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, baseUrl);

            // Then
            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    A<string>._,
                    NewCandidateNumber,
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
            A.CallTo(() => registrationDataService.RegisterNewUserAndDelegateAccount(model))
                .Returns(NewCandidateNumber);
            A.CallTo(
                    () => userDataService.GetDelegateUserByCandidateNumber(
                        NewCandidateNumber,
                        RegistrationModelTestHelper.Centre
                    )
                )
                .Returns(new DelegateUser { Id = 777 });

            // When
            registrationService.RegisterDelegateByCentre(model, baseUrl);

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>.That.IsSameSequenceAs(supervisorDelegateIds),
                    777
                )
            ).MustHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_should_not_send_SupervisorDelegate_email()
        {
            // Given
            const string baseUrl = "base.com";
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            var model = RegistrationModelTestHelper.GetDefaultDelegateRegistrationModel();
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);

            // When
            registrationService.RegisterDelegateByCentre(model, baseUrl);

            // Then
            A.CallTo(() => frameworkNotificationService.SendSupervisorDelegateAcceptance(A<int>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void PromoteDelegateToAdmin_throws_AdminCreationFailedException_if_delegate_has_no_first_name()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(firstName: null);
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(delegateUser);

            // When
            var result = Assert.Throws<AdminCreationFailedException>(
                () => registrationService.PromoteDelegateToAdmin(adminRoles, 1, 1)
            );

            // Then
            result.Error.Should().Be(AdminCreationError.UnexpectedError);
        }

        [Test]
        public void PromoteDelegateToAdmin_throws_AdminCreationFailedException_if_delegate_has_no_email()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: null);
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(delegateUser);

            // When
            var result = Assert.Throws<AdminCreationFailedException>(
                () => registrationService.PromoteDelegateToAdmin(adminRoles, 1, 1)
            );

            // Then
            result.Error.Should().Be(AdminCreationError.UnexpectedError);
        }

        [Test]
        public void PromoteDelegateToAdmin_throws_email_in_use_AdminCreationFailedException_if_admin_already_exists()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(adminUser);

            // When
            var result = Assert.Throws<AdminCreationFailedException>(
                () => registrationService.PromoteDelegateToAdmin(adminRoles, 1, 1)
            );

            // Then
            result.Error.Should().Be(AdminCreationError.EmailAlreadyInUse);
        }

        [Test]
        public void PromoteDelegateToAdmin_calls_data_service_with_expected_value()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var adminRoles = new AdminRoles(true, true, true, true, true, true, true);
            A.CallTo(() => userDataService.GetDelegateUserById(A<int>._)).Returns(delegateUser);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(null);

            // When
            registrationService.PromoteDelegateToAdmin(adminRoles, 1, 1);

            // Then
            A.CallTo(
                () => registrationDataService.RegisterAdmin(
                    A<AdminRegistrationModel>.That.Matches(
                        a =>
                            a.FirstName == delegateUser.FirstName &&
                            a.LastName == delegateUser.LastName &&
                            a.PrimaryEmail == delegateUser.EmailAddress &&
                            a.Centre == delegateUser.CentreId &&
                            a.PasswordHash == delegateUser.Password &&
                            a.Active &&
                            a.Approved &&
                            a.IsCentreAdmin == adminRoles.IsCentreAdmin &&
                            !a.IsCentreManager &&
                            a.IsContentManager == adminRoles.IsContentManager &&
                            a.ImportOnly == adminRoles.IsCmsAdministrator &&
                            a.IsContentCreator == adminRoles.IsContentCreator &&
                            a.IsTrainer == adminRoles.IsTrainer &&
                            a.IsSupervisor == adminRoles.IsSupervisor
                    )
                )
            ).MustHaveHappened();
        }

        private void GivenNoPendingSupervisorDelegateRecordsForEmail()
        {
            A.CallTo(
                    () => supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailAndCentre(
                        A<int>._,
                        A<string>._
                    )
                )
                .Returns(new List<SupervisorDelegate>());
        }

        private void GivenPendingSupervisorDelegateIdsForEmailAre(IEnumerable<int> supervisorDelegateIds)
        {
            var supervisorDelegates = supervisorDelegateIds.Select(id => new SupervisorDelegate { ID = id });
            A.CallTo(
                    () => supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailAndCentre(
                        A<int>._,
                        A<string>._
                    )
                )
                .Returns(supervisorDelegates);
        }
    }
}
