namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Threading;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;
    using NUnit.Framework;

    public class UnlockServiceTests
    {
        private UnlockService unlockService;
        private IUnlockDataService unlockDataService;
        private IConfigService configService;
        private ISmtpClientFactory smtpClientFactory;
        private ISmtpClient smtpClient;

        [SetUp]
        public void Setup()
        {
            unlockDataService = A.Fake<IUnlockDataService>();
            configService = A.Fake<IConfigService>();
            smtpClientFactory = A.Fake<ISmtpClientFactory>();
            smtpClient = A.Fake<ISmtpClient>();
            A.CallTo(() => smtpClientFactory.GetSmtpClient()).Returns(smtpClient);

            A.CallTo(() => unlockDataService.GetUnlockData(A<int>._)).Returns(new UnlockData
            {
                ContactEmail = "recipient@example.com",
                ContactForename = "Forename",
                CourseName = "Course Name",
                CustomisationId = 22,
                DelegateEmail = "cc@example.com",
                DelegateName = "Delegate Name"
            });

            A.CallTo(() => configService.GetConfigValue(ConfigService.TrackingSystemBaseUrl)).Returns("https://example.com");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailPort)).Returns("25");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailUsername)).Returns("username");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailPassword)).Returns("password");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailServer)).Returns("smtp.example.com");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailFromAddress)).Returns("test@example.com");

            unlockService = new UnlockService(unlockDataService, configService, smtpClientFactory);
        }

        [Test]
        public void Trying_to_send_mail_with_null_config_values_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailPassword)).Returns(null);

            // Then
            Assert.Throws<ConfigValueMissingException>(() => unlockService.SendUnlockRequest(1));
        }

        [Test]
        public void The_sender_email_address_is_correct()
        {
            // When
            unlockService.SendUnlockRequest(1);

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.From.ToString() == "test@example.com"
                        ),
                        default(CancellationToken),
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_recipient_email_address_is_correct()
        {
            // When
            unlockService.SendUnlockRequest(1);

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.To.ToString() == "recipient@example.com"
                        ),
                        default(CancellationToken),
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_cc_email_address_is_correct()
        {
            // When
            unlockService.SendUnlockRequest(1);

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.Cc.ToString() == "cc@example.com"
                        ),
                        default(CancellationToken),
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_server_credentials_are_correct()
        {
            // When
            unlockService.SendUnlockRequest(1);

            // Then
            A.CallTo(() =>
                    smtpClient.Authenticate("username", "password", default(CancellationToken))
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_server_details_are_correct()
        {
            // When
            unlockService.SendUnlockRequest(1);

            // Then
            A.CallTo(() =>
                    smtpClient.Connect(
                        "smtp.example.com",
                        25,
                        SecureSocketOptions.Auto,
                        default(CancellationToken)
                    )
                )
                .MustHaveHappened();
        }
    }
}
