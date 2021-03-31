namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models.Email;
    using MimeKit;

    public interface IEmailService
    {
        void SendEmail(Email email);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfigService configService;
        private readonly ISmtpClientFactory smtpClientFactory;

        public EmailService(
            IConfigService configService,
            ISmtpClientFactory smtpClientFactory
        )
        {
            this.configService = configService;
            this.smtpClientFactory = smtpClientFactory;
        }

        public void SendEmail(Email email)
        {
            var mailServerUsername = configService.GetConfigValue(ConfigService.MailUsername)
                                     ?? throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("MailServerUsername"));
            var mailServerPassword = configService.GetConfigValue(ConfigService.MailPassword)
                                     ?? throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("MailServerPassword"));
            var mailServerAddress = configService.GetConfigValue(ConfigService.MailServer)
                                    ?? throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("MailServerAddress"));
            var mailServerPortString = configService.GetConfigValue(ConfigService.MailPort)
                                       ?? throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("MailServerPortString"));
            var mailSenderAddress = configService.GetConfigValue(ConfigService.MailFromAddress)
                                    ?? throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("MailFromAddress"));

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
            catch { }
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
    }
}
