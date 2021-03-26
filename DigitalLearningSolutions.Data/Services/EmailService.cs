namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models.Email;
    using MimeKit;

    public interface IEmailService
    {
        void SendEmail(Email email);
        // make class of email details with To, CC, (BCC?) as string lists, and body as BodyBuilder

    }


    public class EmailService : IEmailService
    {
        private readonly IConfigService configService;
        private readonly ISmtpClientFactory smtpClientFactory;

        public EmailService (
            IConfigService configService,
            ISmtpClientFactory smtpClientFactory
        )
        {
            this.configService = configService;
            this.smtpClientFactory = smtpClientFactory;
        }

        public void SendEmail(Email email)
        {
            var mailServerUsername = configService.GetConfigValue(ConfigService.MailUsername);
            var mailServerPassword = configService.GetConfigValue(ConfigService.MailPassword);
            var mailServerAddress = configService.GetConfigValue(ConfigService.MailServer);
            var mailServerPortString = configService.GetConfigValue(ConfigService.MailPort);
            var mailSenderAddress = configService.GetConfigValue(ConfigService.MailFromAddress);
            if (
                mailServerUsername == null
                || mailServerPassword == null
                || mailServerAddress == null
                || mailServerPortString == null
                || mailSenderAddress == null
            )
            {
                var errorMessage = GenerateConfigValueMissingMessage(
                    mailServerUsername,
                    mailServerPassword,
                    mailServerAddress,
                    mailServerPortString
                );
                throw new ConfigValueMissingException(errorMessage);
            }

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
            catch 
            {
            }
        }

        private MimeMessage CreateMessage(Email email, string mailSenderAddress)
        {
            var message = new MimeMessage();
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
            message.Body = email.Body.ToMessageBody();

            return message;
        }

        private static string GenerateConfigValueMissingMessage(
            string? mailServerUsername,
            string? mailServerPassword,
            string? mailServerAddress,
            string? mailServerPortString
            )
        {
            if (mailServerUsername == null)
            {
                return "Encountered an error while trying to send an email: The value of mailserverUsername is null";
            }

            if (mailServerPassword == null)
            {
                return "Encountered an error while trying to send an email: The value of mailserverPassword is null";
            }

            if (mailServerAddress == null)
            {
                return "Encountered an error while trying to send an email: The value of mailServerAddress is null";
            }

            if (mailServerPortString == null)
            {
                return "Encountered an error while trying to send an email: The value of mailServerPortString is null";
            }

            return "Encountered an error while trying to send an email: The value of mailSenderAddress is null";
            
        }
    }
}
