namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using NUnit.Framework;

    public class EmailServiceTests
    {
        private IConfigDataService configDataService = null!;
        private IEmailDataService emailDataService = null!;
        private EmailService emailService = null!;
        private ISmtpClient smtpClient = null!;

        [SetUp]
        public void Setup()
        {
            emailDataService = A.Fake<IEmailDataService>();
            configDataService = A.Fake<IConfigDataService>();
            var smtpClientFactory = A.Fake<ISmtpClientFactory>();
            smtpClient = A.Fake<ISmtpClient>();
            A.CallTo(() => smtpClientFactory.GetSmtpClient()).Returns(smtpClient);

            A.CallTo(() => configDataService.GetConfigValue(ConfigDataService.MailPort)).Returns("25");
            A.CallTo(() => configDataService.GetConfigValue(ConfigDataService.MailUsername)).Returns("username");
            A.CallTo(() => configDataService.GetConfigValue(ConfigDataService.MailPassword)).Returns("password");
            A.CallTo(() => configDataService.GetConfigValue(ConfigDataService.MailServer)).Returns("smtp.example.com");
            A.CallTo(() => configDataService.GetConfigValue(ConfigDataService.MailFromAddress))
                .Returns("test@example.com");

            var logger = A.Fake<ILogger<EmailService>>();
            emailService = new EmailService(emailDataService, configDataService, smtpClientFactory, logger);
        }

        [TestCase(ConfigDataService.MailPort)]
        [TestCase(ConfigDataService.MailUsername)]
        [TestCase(ConfigDataService.MailPassword)]
        [TestCase(ConfigDataService.MailServer)]
        [TestCase(ConfigDataService.MailFromAddress)]
        public void Trying_to_send_mail_with_null_config_values_should_throw_an_exception(string configKey)
        {
            // Given
            A.CallTo(() => configDataService.GetConfigValue(configKey)).Returns(null);

            // Then
            Assert.Throws<ConfigValueMissingException>(() => emailService.SendEmail(EmailTestHelper.GetDefaultEmail()));
        }

        [Test]
        public void The_server_credentials_are_correct()
        {
            // When
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail());

            // Then
            A.CallTo(
                    () =>
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
            A.CallTo(
                    () =>
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
            A.CallTo(
                    () =>
                        smtpClient.Send(
                            A<MimeMessage>.That.Matches(
                                m =>
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
            A.CallTo(
                    () =>
                        smtpClient.Send(
                            A<MimeMessage>.That.Matches(
                                m =>
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
            A.CallTo(
                    () =>
                        smtpClient.Send(
                            A<MimeMessage>.That.Matches(
                                m =>
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
            const string htmlBody = "<body style= 'font-family: Calibri; font-size: small;'>" +
                                    "<p>Test Body</p>" +
                                    "</body>";
            emailService.SendEmail(
                EmailTestHelper.GetDefaultEmail(
                    body: new BodyBuilder
                    {
                        TextBody = "Test body",
                        HtmlBody = htmlBody,
                    }
                )
            );

            // Then
            A.CallTo(
                    () =>
                        smtpClient.Send(
                            A<MimeMessage>.That.Matches(
                                m =>
                                    m.HtmlBody.ToString() == htmlBody
                            ),
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
            A.CallTo(
                    () =>
                        smtpClient.Send(
                            A<MimeMessage>.That.Matches(
                                m =>
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
            emailService.SendEmail(
                EmailTestHelper.GetDefaultEmail(new[] { "recipient1@example.com", "recipient2@example.com" })
            );

            // Then
            A.CallTo(
                    () =>
                        smtpClient.Send(
                            A<MimeMessage>.That.Matches(
                                m =>
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
            A.CallTo(
                    () =>
                        smtpClient.Send(
                            A<MimeMessage>.That.Matches(
                                m =>
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
            emailService.SendEmail(EmailTestHelper.GetDefaultEmail(cc: new[] { "cc1@example.com", "cc2@example.com" }));

            // Then
            A.CallTo(
                    () =>
                        smtpClient.Send(
                            A<MimeMessage>.That.Matches(
                                m =>
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
            A.CallTo(
                    () =>
                        smtpClient.Send(
                            A<MimeMessage>.That.Matches(
                                m =>
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
            emailService.SendEmail(
                EmailTestHelper.GetDefaultEmail(bcc: new[] { "bcc1@example.com", "bcc2@example.com" })
            );

            // Then
            A.CallTo(
                    () =>
                        smtpClient.Send(
                            A<MimeMessage>.That.Matches(
                                m =>
                                    m.Bcc.ToString() == "bcc1@example.com, bcc2@example.com"
                            ),
                            default,
                            null
                        )
                )
                .MustHaveHappened();
        }

        [Test]
        public void ScheduleEmails_schedules_emails_correctly()
        {
            // Given
            var emails = new List<Email>
            {
                EmailTestHelper.GetDefaultEmailToSingleRecipient("to1@example.com"),
                EmailTestHelper.GetDefaultEmailToSingleRecipient("to2@example.com"),
                EmailTestHelper.GetDefaultEmailToSingleRecipient("to3@example.com")
            };
            var deliveryDate = new DateTime(2200, 1, 1);
            const string addedByProcess = "some process";

            // When
            emailService.ScheduleEmails(emails, addedByProcess, deliveryDate);

            // Then
            A.CallTo(() => emailDataService.ScheduleEmails(emails, A<string>._, addedByProcess, false, deliveryDate))
                .MustHaveHappened();
        }

        [Test]
        public void ScheduleEmails_sets_urgent_true_if_same_day()
        {
            // Given
            var emails = new List<Email> { EmailTestHelper.GetDefaultEmailToSingleRecipient("to@example.com") };
            var deliveryDate = DateTime.Today;
            const string addedByProcess = "some process";

            // When
            emailService.ScheduleEmails(emails, addedByProcess, deliveryDate);

            // Then
            A.CallTo(() => emailDataService.ScheduleEmails(emails, A<string>._, addedByProcess, true, deliveryDate))
                .MustHaveHappened();
        }
    }
}
