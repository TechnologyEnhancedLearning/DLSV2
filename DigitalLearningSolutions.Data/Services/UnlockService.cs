namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models;
    using MailKit.Net.Smtp;
    using MimeKit;

    public interface IUnlockService
    {
        public void SendUnlockRequest(int progressId);
    }

    public class UnlockService : IUnlockService
    {
        private readonly IUnlockDataService unlockDataService;
        private readonly IConfigService configService;
        private readonly ISmtpClientFactory smtpClientFactory;
        public UnlockService(
            IUnlockDataService unlockDataService,
            IConfigService configService,
            ISmtpClientFactory smtpClientFactory
            )
        {
            this.unlockDataService = unlockDataService;
            this.configService = configService;
            this.smtpClientFactory = smtpClientFactory;
        }

        public void SendUnlockRequest(int progressId)
        {
            var unlockData = unlockDataService.GetUnlockData(progressId);
            if (unlockData == null)
            {
                throw new UnlockDataMissingException($"No record found when trying to fetch Unlock Data. Progress ID: {progressId}");
            }

            unlockData.ContactForename = unlockData.ContactForename == "" ? "Colleague" : unlockData.ContactForename;

            var mailServerUsername = configService.GetConfigValue(ConfigService.MailUsername);
            var mailServerPassword = configService.GetConfigValue(ConfigService.MailPassword);
            var mailServerAddress = configService.GetConfigValue(ConfigService.MailServer);
            var mailServerPortString = configService.GetConfigValue(ConfigService.MailPort);
            var mailSenderAddress = configService.GetConfigValue(ConfigService.MailFromAddress);
            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.TrackingSystemBaseUrl);
            if (
                mailServerUsername == null
                || mailServerPassword == null
                || mailServerAddress == null
                || mailServerPortString == null
                || mailSenderAddress == null
                || trackingSystemBaseUrl == null
            )
            {
                throw GenerateConfigValueMissingException(mailServerUsername, mailServerPassword, mailServerAddress, mailServerPortString, mailSenderAddress, trackingSystemBaseUrl);
            }

            var mailServerPort = int.Parse(mailServerPortString);
            var unlockUrl = new UriBuilder(trackingSystemBaseUrl);
            unlockUrl.Path += "coursedelegates";
            unlockUrl.Query = $"CustomisationID={unlockData.CustomisationId}";

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(mailSenderAddress));
            message.To.Add(MailboxAddress.Parse(unlockData.ContactEmail));
            message.Cc.Add(MailboxAddress.Parse(unlockData.DelegateEmail));
            message.Subject = "Digital Learning Solutions Progress Unlock Request";

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


            message.Body = builder.ToMessageBody();

            using var client = smtpClientFactory.GetSmtpClient();
            client.Timeout = 10000;
            client.Connect(mailServerAddress, mailServerPort);

            client.Authenticate(mailServerUsername, mailServerPassword);

            client.Send(message);
            client.Disconnect(true);
        }

        private static ConfigValueMissingException GenerateConfigValueMissingException(
            string? mailServerUsername,
            string? mailServerPassword,
            string? mailServerAddress,
            string? mailServerPortString,
            string? mailSenderAddress,
            string? trackingSystemBaseUrl
            )
        {
            if (mailServerUsername == null)
            {
                return new ConfigValueMissingException("Encountered an error while trying to send an unlock request email: The value of mailserverUsername is null");
            }

            if (mailServerPassword == null)
            {
                return new ConfigValueMissingException("Encountered an error while trying to send an unlock request email: The value of mailserverPassword is null");
            }

            if (mailServerAddress == null)
            {
                return new ConfigValueMissingException("Encountered an error while trying to send an unlock request email: The value of mailServerAddress is null");
            }

            if (mailServerPortString == null)
            {
                return new ConfigValueMissingException("Encountered an error while trying to send an unlock request email: The value of mailServerPortString is null");
            }

            if (mailSenderAddress == null)
            {
                return new ConfigValueMissingException("Encountered an error while trying to send an unlock request email: The value of mailSenderAddress is null");
            }

            return new ConfigValueMissingException("Encountered an error while trying to send an unlock request email: The value of trackingSystemBaseUrl is null");
        }
    }
}
