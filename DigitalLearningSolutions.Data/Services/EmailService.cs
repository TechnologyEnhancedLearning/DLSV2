namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models.Email;
    using MailKit.Net.Smtp;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using MimeKit.Text;

    public interface IEmailService
    {
        void SendEmail(Email email);
        void SendEmails(IEnumerable<Email> emails);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfigService configService;
        private readonly ILogger<EmailService> logger;
        private readonly ISmtpClientFactory smtpClientFactory;

        public EmailService
        (
            IConfigService configService,
            ISmtpClientFactory smtpClientFactory,
            ILogger<EmailService> logger
        )
        {
            this.configService = configService;
            this.smtpClientFactory = smtpClientFactory;
            this.logger = logger;
        }

        public void SendEmail(Email email)
        {
            SendEmails(new [] {email});
        }

        public void SendEmails(IEnumerable<Email> emails)
        {
            var mailConfig = GetMailConfig();

            try
            {
                using var client = smtpClientFactory.GetSmtpClient();
                client.Timeout = 10000;
                client.Connect(mailConfig.MailServerAddress, mailConfig.MailServerPort);
                client.Authenticate(mailConfig.MailServerUsername, mailConfig.MailServerPassword);

                foreach (var email in emails)
                {
                    SendSingleEmailFromClient(email, mailConfig.MailSenderAddress, client);
                }

                client.Disconnect(true);
            }
            catch (Exception error)
            {
                logger.LogError(error, "Sending emails has failed");
            }
        }

        private void SendSingleEmailFromClient
        (
            Email email,
            string mailSenderAddress,
            ISmtpClient client
        )
        {
            try
            {
                MimeMessage message = CreateMessage(email, mailSenderAddress);
                client.Send(message);
            }
            catch (Exception error)
            {
                logger.LogError(error, "Sending an email has failed");
            }
        }

        private (string MailServerUsername, string MailServerPassword, string MailServerAddress, int MailServerPort,
            string MailSenderAddress) GetMailConfig()
        {
            var mailServerUsername = configService.GetConfigValue(ConfigService.MailUsername)
                                     ?? throw new ConfigValueMissingException
                                     (
                                         configService.GetConfigValueMissingExceptionMessage("MailServerUsername")
                                     );
            var mailServerPassword = configService.GetConfigValue(ConfigService.MailPassword)
                                     ?? throw new ConfigValueMissingException
                                     (
                                         configService.GetConfigValueMissingExceptionMessage("MailServerPassword")
                                     );
            var mailServerAddress = configService.GetConfigValue(ConfigService.MailServer)
                                    ?? throw new ConfigValueMissingException
                                    (
                                        configService.GetConfigValueMissingExceptionMessage("MailServerAddress")
                                    );
            var mailServerPortString = configService.GetConfigValue(ConfigService.MailPort)
                                       ?? throw new ConfigValueMissingException
                                       (
                                           configService.GetConfigValueMissingExceptionMessage("MailServerPortString")
                                       );
            var mailSenderAddress = configService.GetConfigValue(ConfigService.MailFromAddress)
                                    ?? throw new ConfigValueMissingException
                                    (
                                        configService.GetConfigValueMissingExceptionMessage("MailFromAddress")
                                    );

            var mailServerPort = int.Parse(mailServerPortString);

            return (mailServerUsername, mailServerPassword, mailServerAddress, mailServerPort, mailSenderAddress);
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
            message.Body = GetMultipartAlternativeFromBody(email.Body);
            return message;
        }

        private MultipartAlternative GetMultipartAlternativeFromBody(BodyBuilder body)
        {
            //Sets body content encooding to quoated-printable to avoid rejection by NHS email servers
            var htmlPart = new TextPart(TextFormat.Html)
            {
                ContentTransferEncoding = ContentEncoding.QuotedPrintable
            };
            htmlPart.SetText(Encoding.UTF8, body.HtmlBody);
            var textPart = new TextPart(TextFormat.Plain)
            {
                ContentTransferEncoding = ContentEncoding.QuotedPrintable
            };
            textPart.SetText(Encoding.UTF8, body.TextBody);
            var multipartAlternative = new MultipartAlternative
            {
                textPart,
                htmlPart
            };
            return multipartAlternative;
        }
    }
}
