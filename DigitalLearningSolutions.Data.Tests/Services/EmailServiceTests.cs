namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using NUnit.Framework;

    public class EmailServiceTests
    {
        private IEmailDataService emailDataService;
        private IConfigService configService;
        private EmailService emailService;
        private ISmtpClient smtpClient;

        [SetUp]
        public void Setup()
        {
            emailDataService = A.Fake<IEmailDataService>();
            configService = A.Fake<IConfigService>();
            var smtpClientFactory = A.Fake<ISmtpClientFactory>();
            smtpClient = A.Fake<ISmtpClient>();
            A.CallTo(() => smtpClientFactory.GetSmtpClient()).Returns(smtpClient);

            A.CallTo(() => configService.GetConfigValue(ConfigService.MailPort)).Returns("25");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailUsername)).Returns("username");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailPassword)).Returns("password");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailServer)).Returns("smtp.example.com");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailFromAddress)).Returns("test@example.com");

            var logger = A.Fake<ILogger<EmailService>>();
            emailService = new EmailService(emailDataService, configService, smtpClientFactory, logger);
        }

        [TestCase(ConfigService.MailPort)]
        [TestCase(ConfigService.MailUsername)]
        [TestCase(ConfigService.MailPassword)]
        [TestCase(ConfigService.MailServer)]
        [TestCase(ConfigService.MailFromAddress)]
        public void Trying_to_send_mail_with_null_config_values_should_throw_an_exception(string configKey)
        {
            // Given
            A.CallTo(() => configService.GetConfigValue(configKey)).Returns(null);

            // Then
            Assert.Throws<ConfigValueMissingException>(() => emailService.SendEmail(EmailTestHelper.GetDefaultEmail()));
        }

        [Test]
        public void The_server_credentials_are_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail());

            // Then
            A.CallTo(() =>
                    smtpClient.Authenticate("username", "password", default)
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_server_details_are_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail());

            // Then
            A.CallTo(() =>
                    smtpClient.Connect(
                        "smtp.example.com",
                        25,
                        SecureSocketOptions.Auto,
                        default
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_sender_email_address_is_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail());

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.From.ToString() == "test@example.com"
                        ),
                        default,
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_email_subject_line_is_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail());

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.Subject.ToString() == "Test Subject Line"
                        ),
                        default,
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_email_text_body_is_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail());

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.TextBody.ToString() == "Test body"
                        ),
                        default,
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_email_HTML_body_is_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail());

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.HtmlBody.ToString() == EmailTestHelper.DefaultHtmlBody),
                        default,
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_recipient_email_address_is_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail());

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.To.ToString() == "recipient@example.com"
                        ),
                        default,
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_recipient_email_addresses_are_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail(new string[2] { "recipient1@example.com", "recipient2@example.com" }));

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.To.ToString() == "recipient1@example.com, recipient2@example.com"
                        ),
                        default,
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_cc_email_address_is_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail());

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.Cc.ToString() == "cc@example.com"
                        ),
                        default,
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_cc_email_addresses_are_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail(cc: new string[2] { "cc1@example.com", "cc2@example.com" }));

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.Cc.ToString() == "cc1@example.com, cc2@example.com"
                        ),
                        default,
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_bcc_email_address_is_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail());

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.Bcc.ToString() == "bcc@example.com"
                        ),
                        default,
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_bcc_email_addresses_are_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail(bcc: new string[2] { "bcc1@example.com", "bcc2@example.com" }));

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.Bcc.ToString() == "bcc1@example.com, bcc2@example.com"
                        ),
                        default,
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void ScheduleEmail_schedules_email_correctly()
        {
            // When
            var email = EmailTestHelper.GetDefaultEmailToSingleRecipient("to@example.com");
            var deliveryDate = new DateTime(2200, 1, 1);
            emailService.ScheduleEmail(email, "some process", deliveryDate);

            // Then
            A.CallTo(
                    () =>
                        emailDataService.ScheduleEmail(email, A<string>._, "some process", false, deliveryDate)
                )
                .MustHaveHappened();
        }

        [Test]
        public void ScheduleEmail_sets_urgent_true_if_same_day()
        {
            // When
            var email = EmailTestHelper.GetDefaultEmailToSingleRecipient("to@example.com");
            emailService.ScheduleEmail(email, "some process", DateTime.Today);

            // Then
            A.CallTo(
                    () =>
                        emailDataService.ScheduleEmail(email, A<string>._, "some process", true, DateTime.Today)
                )
                .MustHaveHappened();
        }
    }
}
