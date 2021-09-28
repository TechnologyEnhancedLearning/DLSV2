namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
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
        private const string PasswordHash = "hash";
        private const string RefactoredSystemBaseUrl = "refactoredUrl";
        private const string OldSystemBaseUrl = "oldUrl";

        private readonly DelegateRegistrationModel failingRegistrationModel = new DelegateRegistrationModel(
            "Bad",
            "User",
            "fail@test.com",
            1,
            1,
            PasswordHash,
            "answer1",
            "answer2",
            "answer3",
            "answer4",
            "answer5",
            "answer6"
        );

        private readonly DelegateRegistrationModel testRegistrationModel = new DelegateRegistrationModel(
            "Test",
            "User",
            "testuser@email.com",
            1,
            1,
            PasswordHash,
            "answer1",
            "answer2",
            "answer3",
            "answer4",
            "answer5",
            "answer6"
        );

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

            A.CallTo(() => centresDataService.GetCentreIpPrefixes(testRegistrationModel.Centre))
                .Returns(new[] { ApprovedIpPrefix });
            A.CallTo(() => centresDataService.GetCentreManagerDetails(A<int>._))
                .Returns(("Test", "Approver", ApproverEmail));

            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._))
                .Returns(NewCandidateNumber);
            A.CallTo(() => registrationDataService.RegisterDelegate(failingRegistrationModel)).Returns("-1");

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

            // When
            var (_, approved) = registrationService.RegisterDelegate(testRegistrationModel, clientIp, false);

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegate(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved)
                        )
                )
                .MustHaveHappened();
            Assert.That(approved);
        }

        [Test]
        public void Registering_delegate_on_localhost_registers_delegate_as_approved()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, "::1", false);

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegate(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved)
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_with_unapproved_IP_registers_delegate_as_unapproved()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, "987.654.321.100", false);

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegate(
                            A<DelegateRegistrationModel>.That.Matches(d => !d.Approved)
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_sends_approval_email_with_old_site_approval_link()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, string.Empty, false);

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
            // When
            registrationService.RegisterDelegate(testRegistrationModel, string.Empty, true);

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
            // When
            registrationService.RegisterDelegate(testRegistrationModel, "123.456.789.100", false);

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_set_password()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, string.Empty, false);

            // Then
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(NewCandidateNumber, PasswordHash)
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_returns_candidate_number()
        {
            // When
            var candidateNumber =
                registrationService.RegisterDelegate(testRegistrationModel, string.Empty, false).candidateNumber;

            // Then
            candidateNumber.Should().Be(NewCandidateNumber);
        }

        [Test]
        public void Registering_delegate_should_add_CandidateId_to_all_SupervisorDelegate_records_found_by_email()
        {
            // Given
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);
            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(NewCandidateNumber, testRegistrationModel.Centre))
                .Returns(new DelegateUser { Id = 777 });

            // When
            registrationService.RegisterDelegate(testRegistrationModel, string.Empty, false, 999);

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>.That.IsSameSequenceAs(supervisorDelegateIds),
                    777
                )
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_add_Confirmed_only_to_SupervisorDelegate_record_with_matching_id()
        {
            // Given
            const int matchingSupervisorDelegateId = 2;
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);

            // When
            registrationService.RegisterDelegate(
                testRegistrationModel,
                string.Empty,
                false,
                matchingSupervisorDelegateId
            );

            // Then
            A.CallTo(
                () => supervisorDelegateService.ConfirmSupervisorDelegateRecord(matchingSupervisorDelegateId)
            ).MustHaveHappened();
            A.CallTo(
                () => supervisorDelegateService.ConfirmSupervisorDelegateRecord(
                    A<int>.That.Matches(id => id != matchingSupervisorDelegateId)
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_not_update_any_SupervisorDelegate_records_if_none_found()
        {
            // Given
            GivenNoPendingSupervisorDelegateRecordsForEmail();

            // When
            registrationService.RegisterDelegate(testRegistrationModel, string.Empty, false, 999);

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            A.CallTo(() => supervisorDelegateService.ConfirmSupervisorDelegateRecord(A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_send_SupervisorDelegate_email_for_matching_id_record_only()
        {
            // Given
            const int supervisorDelegateId = 2;
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);

            // When
            registrationService.RegisterDelegate(testRegistrationModel, string.Empty, false, supervisorDelegateId);

            // Then
            A.CallTo(() => frameworkNotificationService.SendSupervisorDelegateAcceptance(supervisorDelegateId))
                .MustHaveHappened();
            A.CallTo(
                () => frameworkNotificationService.SendSupervisorDelegateAcceptance(
                    A<int>.That.Matches(id => id != supervisorDelegateId)
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void Error_when_registering_delegate_returns_error_code()
        {
            // When
            var candidateNumber =
                registrationService.RegisterDelegate(failingRegistrationModel, string.Empty, false)
                    .candidateNumber;

            // Then
            candidateNumber.Should().Be("-1");
        }

        [Test]
        public void Error_when_registering_delegate_fails_fast()
        {
            // When
            registrationService.RegisterDelegate(failingRegistrationModel, string.Empty, false);

            // Then
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
        public void Registering_admin_delegate_registers_delegate_as_approved()
        {
            // When
            registrationService.RegisterCentreManager(testRegistrationModel);

            // Then
            A.CallTo(
                    () =>
                        registrationDataService.RegisterDelegate(
                            A<DelegateRegistrationModel>.That.Matches(d => d.Approved)
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void Registering_admin_delegate_does_not_send_email()
        {
            // When
            registrationService.RegisterCentreManager(testRegistrationModel);

            // Then
            A.CallTo(
                () =>
                    emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_admin_delegate_should_set_password()
        {
            // When
            registrationService.RegisterCentreManager(testRegistrationModel);

            // Then
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(NewCandidateNumber, PasswordHash)
            ).MustHaveHappened();
        }

        [Test]
        public void RegisterCentreManager_calls_all_relevant_registration_methods()
        {
            // When
            registrationService.RegisterCentreManager(testRegistrationModel);

            // Then
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => registrationDataService.RegisterCentreManagerAdmin(testRegistrationModel))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => centresDataService.SetCentreAutoRegistered(testRegistrationModel.Centre))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void Error_in_RegisterCentreManager_throws_exception()
        {
            // Given
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._)).Returns("-1");

            // When
            Action act = () => registrationService.RegisterCentreManager(failingRegistrationModel);

            // Then
            act.Should().Throw<Exception>();
        }

        [Test]
        public void Error_in_RegisterCentreManager_fails_fast()
        {
            // Given
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._)).Returns("-1");

            // When
            Action act = () => registrationService.RegisterCentreManager(failingRegistrationModel);

            // Then
            act.Should().Throw<Exception>();
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () =>
                    passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustNotHaveHappened();
            A.CallTo(() => registrationDataService.RegisterCentreManagerAdmin(testRegistrationModel))
                .MustNotHaveHappened();
            A.CallTo(() => centresDataService.SetCentreAutoRegistered(testRegistrationModel.Centre))
                .MustNotHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_sets_password_if_passwordHash_not_null()
        {
            // Given
            var model = new DelegateRegistrationModel("firstName", "lastName", "email", 0, 0, PasswordHash);
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, "");

            // Then
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(model)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordDataService.SetPasswordByCandidateNumber(NewCandidateNumber, PasswordHash))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void RegisterDelegateByCentre_does_not_set_password_if_passwordHash_is_null()
        {
            // Given
            var model = new DelegateRegistrationModel("firstName", "lastName", "email", 0, 0, null);
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model, "");

            // Then
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(model)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_schedules_welcome_email_if_notify_date_set()
        {
            // Given
            const string baseUrl = "base.com";
            var model = new DelegateRegistrationModel("firstName", "lastName", "email@test.com", 0, 0, null)
                { NotifyDate = new DateTime(2200, 1, 1) };

            // When
            registrationService.RegisterDelegateByCentre(model, baseUrl);

            // Then
            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    model.Email,
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
            var model = new DelegateRegistrationModel("firstName", "lastName", "email@test.com", 0, 0, null);

            // When
            registrationService.RegisterDelegateByCentre(model, baseUrl);

            // Then
            A.CallTo(
                () => passwordResetService.GenerateAndScheduleDelegateWelcomeEmail(
                    A<string>._,
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
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(testRegistrationModel))
                .Returns(NewCandidateNumber);
            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(NewCandidateNumber, testRegistrationModel.Centre))
                .Returns(new DelegateUser { Id = 777 });

            // When
            registrationService.RegisterDelegateByCentre(testRegistrationModel, baseUrl);

            // Then
            A.CallTo(
                () => supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(
                    A<IEnumerable<int>>.That.IsSameSequenceAs(supervisorDelegateIds),
                    777
                )
            ).MustHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_should_not_add_Confirmed_to_any_SupervisorDelegate_record()
        {
            // Given
            const string baseUrl = "base.com";
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);

            // When
            registrationService.RegisterDelegateByCentre(testRegistrationModel, baseUrl);

            // Then
            A.CallTo(() => supervisorDelegateService.ConfirmSupervisorDelegateRecord(A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void RegisterDelegateByCentre_should_not_send_SupervisorDelegate_email()
        {
            // Given
            const string baseUrl = "base.com";
            var supervisorDelegateIds = new List<int> { 1, 2, 3, 4, 5 };
            GivenPendingSupervisorDelegateIdsForEmailAre(supervisorDelegateIds);

            // When
            registrationService.RegisterDelegateByCentre(testRegistrationModel, baseUrl);

            // Then
            A.CallTo(() => frameworkNotificationService.SendSupervisorDelegateAcceptance(A<int>._))
                .MustNotHaveHappened();
        }

        private void GivenNoPendingSupervisorDelegateRecordsForEmail()
        {
            A.CallTo(() => supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailAndCentre(A<int>._, A<string>._))
                .Returns(new List<SupervisorDelegate>());
        }

        private void GivenPendingSupervisorDelegateIdsForEmailAre(IEnumerable<int> supervisorDelegateIds)
        {
            var supervisorDelegates = supervisorDelegateIds.Select(id => new SupervisorDelegate { ID = id });
            A.CallTo(() => supervisorDelegateService.GetPendingSupervisorDelegateRecordsByEmailAndCentre(A<int>._, A<string>._))
                .Returns(supervisorDelegates);
        }
    }
}
