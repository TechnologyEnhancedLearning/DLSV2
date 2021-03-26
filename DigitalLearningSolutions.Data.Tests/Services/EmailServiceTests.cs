namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Threading;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;
    using NUnit.Framework;

    public class EmailServiceTests
    {
        private EmailService emailService;
        private IConfigService configService;
        private ISmtpClient smtpClient;
        private Email testEmail;
        private Email multipleAddresseesTestEmail;

        [SetUp]
        public void Setup()
        {
            BodyBuilder emailTestBody = new BodyBuilder
            {
                TextBody = "Test body",
                HtmlBody = @"<body style= 'font - family: Calibri; font - size: small;'>
                                    <p>Test Body</p>
                                </body>"
            };

            testEmail = new Email(
                "recipient@example.com",
                "Test Subject Line",
                emailTestBody,
                "cc@example.com",
                "bcc@example.com");

            multipleAddresseesTestEmail = new Email(
                new string[2]{"recipient1@example.com", "recipient2@example.com"},
                "Test Subject Line",
                emailTestBody,
                new string[2]{"cc1@example.com", "cc2@example.com"},
                new string[2]{"bcc1@example.com","bcc2@example.com"});

            configService = A.Fake<IConfigService>();
            var smtpClientFactory = A.Fake<ISmtpClientFactory>();
            smtpClient = A.Fake<ISmtpClient>();
            A.CallTo(() => smtpClientFactory.GetSmtpClient()).Returns(smtpClient);

            A.CallTo(() => configService.GetConfigValue(ConfigService.MailPort)).Returns("25");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailUsername)).Returns("username");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailPassword)).Returns("password");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailServer)).Returns("smtp.example.com");
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailFromAddress)).Returns("test@example.com");

            emailService = new EmailService(configService, smtpClientFactory);
            // Going to want to copy the others over, but also perform it with string lists in the to/cc/bcc etc.
        }

        [Test]
        public void Trying_to_send_mail_with_null_config_values_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => configService.GetConfigValue(ConfigService.MailPassword)).Returns(null);

            // Then
            Assert.Throws<ConfigValueMissingException>(() => emailService.SendEmail(testEmail));
        }

        [Test]
        public void The_server_credentials_are_correct()
        {
            // When
            emailService.SendEmail(testEmail);

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
            emailService.SendEmail(testEmail);

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

        [Test]
        public void The_sender_email_address_is_correct()
        {
            // When
            emailService.SendEmail(testEmail);

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
        public void The_email_subject_line_is_correct()
        {
            // When
            emailService.SendEmail(testEmail);

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.Subject.ToString() == "Test Subject Line"
                        ),
                        default(CancellationToken),
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_email_text_body_is_correct()
        {
            // When
            emailService.SendEmail(testEmail);

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.TextBody.ToString() == "Test body"
                        ),
                        default(CancellationToken),
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_email_HTML_body_is_correct()
        {
            // When
            emailService.SendEmail(testEmail);

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.HtmlBody.ToString() == "<body style= 'font - family: Calibri; font - size: small;'>\r\n" +
                            "                                    <p>Test Body</p>\r\n" +
                            "                                </body>"
                        
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
            emailService.SendEmail(testEmail);

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
        public void The_recipient_email_addresses_are_correct()
        {
            // When
            emailService.SendEmail(multipleAddresseesTestEmail);

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.To.ToString() == "recipient1@example.com, recipient2@example.com"
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
            emailService.SendEmail(testEmail);

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
        public void The_cc_email_addresses_are_correct()
        {
            // When
            emailService.SendEmail(multipleAddresseesTestEmail);

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.Cc.ToString() == "cc1@example.com, cc2@example.com"
                        ),
                        default(CancellationToken),
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_bcc_email_address_is_correct()
        {
            // When
            emailService.SendEmail(testEmail);

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.Bcc.ToString() == "bcc@example.com"
                        ),
                        default(CancellationToken),
                        null
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void The_bcc_email_addresses_are_correct()
        {
            // When
            emailService.SendEmail(multipleAddresseesTestEmail);

            // Then
            A.CallTo(() =>
                    smtpClient.Send(
                        A<MimeMessage>.That.Matches(m =>
                            m.Bcc.ToString() == "bcc1@example.com, bcc2@example.com"
                        ),
                        default(CancellationToken),
                        null
                    )
                )
                .MustHaveHappened();
        }

    }
}
