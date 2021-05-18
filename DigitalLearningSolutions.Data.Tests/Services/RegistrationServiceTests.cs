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
        private IRegistrationDataService registrationDataService;
        private IPasswordDataService passwordDataService;
        private IEmailService emailService;
        private ICentresDataService centresDataService;
        private RegistrationService registrationService;

        private static string approverEmail = "approver@email.com";
        private static string newCandidateNumber = "TU67";
        private static string passwordHash = "hash";
        private DelegateRegistrationModel testRegistrationModel = new DelegateRegistrationModel(
            "Test", "User", "testuser@email.com", 1, 1, passwordHash, false);
        private DelegateRegistrationModel failingRegistrationModel = new DelegateRegistrationModel(
            "Bad", "User", "fail@test.com", 1, 1, passwordHash, false);

        [SetUp]
        public void Setup()
        {
            registrationDataService = A.Fake<IRegistrationDataService>();
            passwordDataService = A.Fake<IPasswordDataService>();
            emailService = A.Fake<IEmailService>();
            centresDataService = A.Fake<ICentresDataService>();

            A.CallTo(() => centresDataService.GetContactInfo(A<int>._)).Returns((
                "Test", "Approver", approverEmail
            ));
            A.CallTo(() => registrationDataService.RegisterDelegate(A<DelegateRegistrationModel>._)).Returns(
                newCandidateNumber
            );
            A.CallTo(() => registrationDataService.RegisterDelegate(failingRegistrationModel)).Returns("-1");

            registrationService = new RegistrationService(registrationDataService, passwordDataService, emailService,
                centresDataService);
        }

        [Test]
        public void Registering_delegate_sends_approval_email()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, "localhost");

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
        public void Registering_delegate_should_set_password()
        {
            // When
            registrationService.RegisterDelegate(testRegistrationModel, "localhost");

            // Then
            A.CallTo(() =>
                passwordDataService.SetPasswordByCandidateNumber(newCandidateNumber, passwordHash)
            ).MustHaveHappened();
        }

        [Test]
        public void Registering_delegate_returns_candidate_number()
        {
            // When
            var candidateNumber = registrationService.RegisterDelegate(testRegistrationModel, "localhost");

            // Then
            candidateNumber.Should().Be(newCandidateNumber);
        }

        [Test]
        public void Error_when_registering_returns_error_code()
        {
            // When
            var candidateNumber = registrationService.RegisterDelegate(failingRegistrationModel, "localhost");

            // Then
            candidateNumber.Should().Be("-1");
        }

        [Test]
        public void Error_when_registering_fails_fast()
        {
            // When
            registrationService.RegisterDelegate(failingRegistrationModel, "localhost");

            // Then
            A.CallTo(() =>
                emailService.SendEmail(A<Email>._)
            ).MustNotHaveHappened();
            A.CallTo(() =>
                passwordDataService.SetPasswordByCandidateNumber(A<string>._, A<string>._)
            ).MustNotHaveHappened();
        }
    }
}
