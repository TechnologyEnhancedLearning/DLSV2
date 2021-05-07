namespace DigitalLearningSolutions.Data.Tests.Services
{
    using Castle.Core.Internal;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
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
            "Test", "User", "testuser@email.com", 1, 1, passwordHash);

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
    }
}
