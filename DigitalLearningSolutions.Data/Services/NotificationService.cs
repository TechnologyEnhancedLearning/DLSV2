namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models;
    using MimeKit;

    public interface INotificationService
    {
        void SendUnlockRequest(int progressId);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationDataService notificationDataService;
        private readonly IConfigService configService;
        private readonly ISmtpClientFactory smtpClientFactory;
        public NotificationService(
            INotificationDataService notificationDataService,
            IConfigService configService,
            ISmtpClientFactory smtpClientFactory
            )
        {
            this.notificationDataService = notificationDataService;
            this.configService = configService;
            this.smtpClientFactory = smtpClientFactory;
        }
        public void SendSMTPMessage(string to, string? cc, string subject, BodyBuilder body)
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
                    mailServerPortString,
                    mailSenderAddress
                );
                throw new ConfigValueMissingException(errorMessage);
            }

            var mailServerPort = int.Parse(mailServerPortString);
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(mailSenderAddress));
            message.To.Add(MailboxAddress.Parse(to));
            if (cc != null)
            {
                message.Cc.Add(MailboxAddress.Parse(cc));
            }
            message.Subject = subject;
            message.Body = body.ToMessageBody();
            using var client = smtpClientFactory.GetSmtpClient();
            client.Timeout = 10000;
            client.Connect(mailServerAddress, mailServerPort);

            client.Authenticate(mailServerUsername, mailServerPassword);

            client.Send(message);
            client.Disconnect(true);
        }
        public void SendUnlockRequest(int progressId)
        {
            var unlockData = notificationDataService.GetUnlockData(progressId);
            if (unlockData == null)
            {
                throw new UnlockDataMissingException($"No record found when trying to fetch Unlock Data. Progress ID: {progressId}");
            }

            unlockData.ContactForename = unlockData.ContactForename == "" ? "Colleague" : unlockData.ContactForename;


            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.TrackingSystemBaseUrl);
            var unlockUrl = new UriBuilder(trackingSystemBaseUrl);
            unlockUrl.Path += "coursedelegates";
            unlockUrl.Query = $"CustomisationID={unlockData.CustomisationId}";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {unlockData?.ContactForename}
Digital Learning Solutions Delegate, {unlockData?.DelegateName}, has requested that you unlock their progress for the course {unlockData?.CourseName}.
They have reached the maximum number of assessment attempt allowed without passing.
To review and unlock their progress, visit the this url: ${unlockUrl}.",
                HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'>
                                    <p>Dear {unlockData?.ContactForename}</p>
                                    <p>Digital Learning Solutions Delegate, {unlockData?.DelegateName}, has requested that you unlock their progress for the course {unlockData?.CourseName}</p>
                                    <p>They have reached the maximum number of assessment attempt allowed without passing.</p><p>To review and unlock their progress, <a href='{unlockUrl}'>click here</a>.</p>
                                </body>"
            };
            SendSMTPMessage(unlockData.ContactEmail, unlockData.DelegateEmail, "Digital Learning Solutions Progress Unlock Request", builder);
        }

        private static string GenerateConfigValueMissingMessage(
            string? mailServerUsername,
            string? mailServerPassword,
            string? mailServerAddress,
            string? mailServerPortString,
            string? mailSenderAddress
            )
        {
            if (mailServerUsername == null)
            {
                return "Encountered an error while trying to send an unlock request email: The value of mailserverUsername is null";
            }

            if (mailServerPassword == null)
            {
                return "Encountered an error while trying to send an unlock request email: The value of mailserverPassword is null";
            }

            if (mailServerAddress == null)
            {
                return "Encountered an error while trying to send an unlock request email: The value of mailServerAddress is null";
            }

            if (mailServerPortString == null)
            {
                return "Encountered an error while trying to send an unlock request email: The value of mailServerPortString is null";
            }

            if (mailSenderAddress == null)
            {
                return "Encountered an error while trying to send an unlock request email: The value of mailSenderAddress is null";
            }

            return "Encountered an error while trying to send an unlock request email: The value of trackingSystemBaseUrl is null";
        }
    }
}
