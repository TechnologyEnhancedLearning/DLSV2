namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
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
        private IEmailService emailService = null!;
        private IPasswordDataService passwordDataService = null!;
        private IPasswordResetService passwordResetService = null!;
        private IRegistrationDataService registrationDataService = null!;
        private IRegistrationService registrationService = null!;
        private IConfiguration config = null!;

        [SetUp]
        public void Setup()
        {
            registrationDataService = A.Fake<IRegistrationDataService>();
            passwordDataService = A.Fake<IPasswordDataService>();
            passwordResetService = A.Fake<IPasswordResetService>();
            emailService = A.Fake<IEmailService>();
            centresDataService = A.Fake<ICentresDataService>();
            config = A.Fake<IConfiguration>();

            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(OldSystemBaseUrl);
            A.CallTo(() => config["AppRootPath"]).Returns(RefactoredSystemBaseUrl);

            A.CallTo(() => centresDataService.GetCentreIpPrefixes(testRegistrationModel.Centre))
                .Returns(new[] { ApprovedIpPrefix });
            A.CallTo(() => centresDataService.GetCentreManagerDetails(A<int>._))
                .Returns(("Test", "Approver", ApproverEmail));

            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._))
                .Returns(NewCandidateNumber);
            A.CallTo(() => registrationDataService.RegisterDelegate(failingRegistrationModel))
                .Returns("-1");

            registrationService = new RegistrationService(
                registrationDataService,
                passwordDataService,
                emailService,
                centresDataService,
                config
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
            registrationService.RegisterDelegate(failingRegistrationModel,  string.Empty, false);

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
            registrationService.RegisterDelegateByCentre(model);

            // Then
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(model)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordDataService.SetPasswordByCandidateNumber(NewCandidateNumber, PasswordHash)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void RegisterDelegateByCentre_does_not_set_password_if_passwordHash_is_null()
        {
            // Given
            var model = new DelegateRegistrationModel("firstName", "lastName", "email", 0, 0, null);
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(model)).Returns(NewCandidateNumber);

            // When
            registrationService.RegisterDelegateByCentre(model);

            // Then
            A.CallTo(() => registrationDataService.RegisterDelegateByCentre(model)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)).MustNotHaveHappened();
        }
    }
}
