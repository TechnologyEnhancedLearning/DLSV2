namespace DigitalLearningSolutions.Data.Tests.Services
{
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class RegistrationServiceTests
    {
        private static readonly string approverEmail = "approver@email.com";
        private static readonly string approvedIpPrefix = "123.456.789";
        private static readonly string newCandidateNumber = "TU67";
        private static readonly string passwordHash = "hash";

        private readonly DelegateRegistrationModel failingRegistrationModel = new DelegateRegistrationModel
        (
            "Bad",
            "User",
            "fail@test.com",
            1,
            1,
            passwordHash,
            "answer1",
            "answer2",
            "answer3",
            "answer4",
            "answer5",
            "answer6"
        );

        private readonly DelegateRegistrationModel testRegistrationModel = new DelegateRegistrationModel
        (
            "Test",
            "User",
            "testuser@email.com",
            1,
            1,
            passwordHash,
            "answer1",
            "answer2",
            "answer3",
            "answer4",
            "answer5",
            "answer6"
        );

        private ICentresDataService centresDataService;
        private IEmailService emailService;
        private IPasswordDataService passwordDataService;
        private IRegistrationDataService registrationDataService;
        private RegistrationService registrationService;

        [SetUp]
        public void Setup()
        {
            registrationDataService = A.Fake<IRegistrationDataService>();
            passwordDataService = A.Fake<IPasswordDataService>();
            emailService = A.Fake<IEmailService>();
            centresDataService = A.Fake<ICentresDataService>();

            A.CallTo(() => centresDataService.GetCentreIpPrefixes(testRegistrationModel.Centre))
                .Returns(new[] { approvedIpPrefix });
            A.CallTo(() => centresDataService.GetCentreManagerDetails(A<int>._)).Returns((
                "Test", "Approver", approverEmail
            ));
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._)).Returns(
                newCandidateNumber
            );
            A.CallTo(() => registrationDataService.RegisterDelegate(failingRegistrationModel)).Returns("-1");

            registrationService = new RegistrationService
            (
                registrationDataService,
                passwordDataService,
                emailService,
                centresDataService
            );
        }

        [Test]
        public void Registering_delegate_with_approved_IP_registers_delegate_as_approved()
        {
            // Given
            var clientIp = approvedIpPrefix + ".100";

            // When
            var (_, approved) = registrationService.RegisterDelegate(testRegistrationModel, "localhost", clientIp);

            // Then
            A.CallTo(() =>
                    registrationDataService.RegisterDelegate(
                        A<DelegateRegistrationModel>.That.Matches(d => d.Approved)))
                .MustHaveHappened();
            Assert.That(approved);
        }

        [Test]
        public void Registering_delegate_on_localhost_registers_delegate_as_approved()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, "localhost", "::1");

            // Then
            A.CallTo(() =>
                    registrationDataService.RegisterDelegate(
                        A<DelegateRegistrationModel>.That.Matches(d => d.Approved)))
                .MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_with_unapproved_IP_registers_delegate_as_unapproved()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, "localhost", "987.654.321.100");

            // Then
            A.CallTo(() =>
                    registrationDataService.RegisterDelegate(
                        A<DelegateRegistrationModel>.That.Matches(d => !d.Approved)))
                .MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_sends_approval_email()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, "localhost", string.Empty);

            // Then
            A.CallTo(() =>
                emailService.SendEmail(A<Email>.That.Matches(e =>
                    e.To[0] == approverEmail &&
                    e.Cc.IsNullOrEmpty() &&
                    e.Bcc.IsNullOrEmpty() &&
                    e.Subject == "Digital Learning Solutions Registration Requires Approval"))
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_automatically_approved_does_not_send_email()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, "localhost", "123.456.789.100");

            // Then
            A.CallTo(() =>
                emailService.SendEmail(A<Email>._)).MustNotHaveHappened();
        }

        [Test]
        public void Registering_delegate_should_set_password()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, "localhost", string.Empty);

            // Then
            A.CallTo(() =>
                passwordDataService.SetPasswordByCandidateNumber(newCandidateNumber, passwordHash)
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_returns_candidate_number()
        {
            // When
            var candidateNumber =
                registrationService.RegisterDelegate(testRegistrationModel, "localhost", string.Empty).candidateNumber;

            // Then
            candidateNumber.Should().Be(newCandidateNumber);
        }

        [Test]
        public void Error_when_registering_delegate_returns_error_code()
        {
            // When
            var candidateNumber =
                registrationService.RegisterDelegate(failingRegistrationModel, "localhost", string.Empty)
                    .candidateNumber;

            // Then
            candidateNumber.Should().Be("-1");
        }

        [Test]
        public void Error_when_registering_delegate_fails_fast()
        {
            // When
            registrationService.RegisterDelegate(failingRegistrationModel, "localhost", string.Empty);

            // Then
            A.CallTo(() =>
                emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
            A.CallTo(() =>
                passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void Registering_admin_delegate_registers_delegate_as_approved()
        {
            // When
            registrationService.RegisterAdminDelegate(testRegistrationModel);

            // Then
            A.CallTo(() =>
                    registrationDataService.RegisterDelegate(
                        A<DelegateRegistrationModel>.That.Matches(d => d.Approved)))
                .MustHaveHappened();
        }

        [Test]
        public void Registering_admin_delegate_does_not_send_email()
        {
            // When
            registrationService.RegisterAdminDelegate(testRegistrationModel);

            // Then
            A.CallTo(() =>
                emailService.SendEmail(A<Email>._)).MustNotHaveHappened();
        }

        [Test]
        public void Registering_admin_delegate_should_set_password()
        {
            // When
            registrationService.RegisterAdminDelegate(testRegistrationModel);

            // Then
            A.CallTo(() =>
                passwordDataService.SetPasswordByCandidateNumber(newCandidateNumber, passwordHash)
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_admin_delegate_returns_candidate_number()
        {
            // When
            var candidateNumber = registrationService.RegisterAdminDelegate(testRegistrationModel);

            // Then
            candidateNumber.Should().Be(newCandidateNumber);
        }

        [Test]
        public void Error_when_registering_admin_delegate_returns_error_code()
        {
            // Given
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._)).Returns("-1");

            // When
            var candidateNumber = registrationService.RegisterAdminDelegate(failingRegistrationModel);

            // Then
            candidateNumber.Should().Be("-1");
        }

        [Test]
        public void Error_when_registering_admin_delegate_fails_fast()
        {
            // Given
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._)).Returns("-1");

            // When
            registrationService.RegisterAdminDelegate(failingRegistrationModel);

            // Then
            A.CallTo(() =>
                passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustNotHaveHappened();
        }
    }
}
