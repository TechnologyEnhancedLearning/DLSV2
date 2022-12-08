using System;
using System.Collections.Generic;
using System.Text;
using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Factories;
using DigitalLearningSolutions.Data.Models.Email;
using DigitalLearningSolutions.Data.Utilities;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IEmailSchedulerService
    {
        void SendEmail(Email email);
        void SendEmails(IEnumerable<Email> emails);
        void ScheduleEmail(Email email, string addedByProcess, DateTime? deliveryDate = null);
        void ScheduleEmails(IEnumerable<Email> emails, string addedByProcess, DateTime? deliveryDate = null);
    }

    public class EmailSchedulerService : IEmailSchedulerService
    {
        private readonly IConfigDataService configDataService;
        private readonly IEmailDataService emailDataService;
        private readonly ILogger<EmailSchedulerService> logger;
        private readonly ISmtpClientFactory smtpClientFactory;
        private readonly IClockUtility clockUtility;

        public EmailSchedulerService(
            IEmailDataService emailDataService,
            IConfigDataService configDataService,
            ISmtpClientFactory smtpClientFactory,
            ILogger<EmailSchedulerService> logger,
            IClockUtility clockUtility
        )
        {
            this.emailDataService = emailDataService;
            this.configDataService = configDataService;
            this.smtpClientFactory = smtpClientFactory;
            this.logger = logger;
            this.clockUtility = clockUtility;
        }

        public void SendEmail(Email email)
        {
            SendEmails(new[] { email });
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

        public void ScheduleEmail(Email email, string addedByProcess, DateTime? deliveryDate = null)
        {
            ScheduleEmails(new[] { email }, addedByProcess, deliveryDate);
        }

        public void ScheduleEmails(IEnumerable<Email> emails, string addedByProcess, DateTime? deliveryDate = null)
        {
            var senderAddress = GetMailConfig().MailSenderAddress;


            var urgent = deliveryDate?.Date.Equals(clockUtility.UtcToday) ?? false;


            emailDataService.ScheduleEmails(emails, senderAddress, addedByProcess, urgent, deliveryDate);
        }

        private void SendSingleEmailFromClient(
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
            var mailServerUsername = configDataService.GetConfigValue(ConfigDataService.MailUsername)
                                     ?? throw new ConfigValueMissingException
                                     (
                                         configDataService.GetConfigValueMissingExceptionMessage("MailServerUsername")
                                     );
            var mailServerPassword = configDataService.GetConfigValue(ConfigDataService.MailPassword)
                                     ?? throw new ConfigValueMissingException
                                     (
                                         configDataService.GetConfigValueMissingExceptionMessage("MailServerPassword")
                                     );
            var mailServerAddress = configDataService.GetConfigValue(ConfigDataService.MailServer)
                                    ?? throw new ConfigValueMissingException
                                    (
                                        configDataService.GetConfigValueMissingExceptionMessage("MailServerAddress")
                                    );
            var mailServerPortString = configDataService.GetConfigValue(ConfigDataService.MailPort)
                                       ?? throw new ConfigValueMissingException
                                       (
                                           configDataService.GetConfigValueMissingExceptionMessage("MailServerPortString")
                                       );
            var mailSenderAddress = configDataService.GetConfigValue(ConfigDataService.MailFromAddress)
                                    ?? throw new ConfigValueMissingException
                                    (
                                        configDataService.GetConfigValueMissingExceptionMessage("MailFromAddress")
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
            //Sets body content encoding to quoted-printable to avoid rejection by NHS email servers
            var htmlPart = new TextPart(TextFormat.Html)
            {
                ContentTransferEncoding = ContentEncoding.QuotedPrintable,
            };
            htmlPart.SetText(Encoding.UTF8, body.HtmlBody);
            var textPart = new TextPart(TextFormat.Plain)
            {
                ContentTransferEncoding = ContentEncoding.QuotedPrintable,
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
