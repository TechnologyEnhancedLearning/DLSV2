namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Text;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models.Email;
    using Microsoft.Extensions.Logging;
    using MimeKit;

    public interface IEmailService
    {
        void SendEmail(Email email);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfigService configService;
        private readonly ILogger<EmailService> logger;
        private readonly ISmtpClientFactory smtpClientFactory;

        public EmailService(
            IConfigService configService,
            ISmtpClientFactory smtpClientFactory,
            ILogger<EmailService> logger)
        {
            this.configService = configService;
            this.smtpClientFactory = smtpClientFactory;
            this.logger = logger;
        }

        public void SendEmail(Email email)
        {
            var mailServerUsername = configService.GetConfigValue(ConfigService.MailUsername)
                                     ?? throw new ConfigValueMissingException(
                                         configService.GetConfigValueMissingExceptionMessage("MailServerUsername"));
            var mailServerPassword = configService.GetConfigValue(ConfigService.MailPassword)
                                     ?? throw new ConfigValueMissingException(
                                         configService.GetConfigValueMissingExceptionMessage("MailServerPassword"));
            var mailServerAddress = configService.GetConfigValue(ConfigService.MailServer)
                                    ?? throw new ConfigValueMissingException(
                                        configService.GetConfigValueMissingExceptionMessage("MailServerAddress"));
            var mailServerPortString = configService.GetConfigValue(ConfigService.MailPort)
                                       ?? throw new ConfigValueMissingException(
                                           configService.GetConfigValueMissingExceptionMessage("MailServerPortString"));
            var mailSenderAddress = configService.GetConfigValue(ConfigService.MailFromAddress)
                                    ?? throw new ConfigValueMissingException(
                                        configService.GetConfigValueMissingExceptionMessage("MailFromAddress"));

            var mailServerPort = int.Parse(mailServerPortString);

            MimeMessage message = CreateMessage(email, mailSenderAddress);

            try
            {
                using var client = smtpClientFactory.GetSmtpClient();
                client.Timeout = 10000;
                client.Connect(mailServerAddress, mailServerPort);

                client.Authenticate(mailServerUsername, mailServerPassword);

                client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception error)
            {
                logger.LogError(error, "Sending an email has failed");
            }
        }

        private MimeMessage CreateMessage(Email email, string mailSenderAddress)
        {
            var message = new MimeMessage();
            message.Prepare(EncodingConstraint.SevenBit);
            message.From.Add(MailboxAddress.Parse(mailSenderAddress));
            foreach (string toAddress in email.To)
            {
                message.To.Add(MailboxAddress.Parse(toAddress));
            }

            foreach (string ccAddress in email.Cc)
            {
                message.Cc.Add(MailboxAddress.Parse(ccAddress));
            }

            foreach (string bccAddress in email.Bcc)
            {
                message.Bcc.Add(MailboxAddress.Parse(bccAddress));
            }

            message.Subject = email.Subject;
            var htmlPart = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                ContentTransferEncoding = ContentEncoding.QuotedPrintable
            };
            htmlPart.SetText(Encoding.UTF8, email.Body.HtmlBody);
            var textPart = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                ContentTransferEncoding = ContentEncoding.QuotedPrintable
            };
            textPart.SetText(Encoding.UTF8, email.Body.TextBody);
            var multipartAlternative = new MultipartAlternative()
            {
                textPart,
                htmlPart
            };
            message.Body = multipartAlternative;
            return message;
        }
    }
}
