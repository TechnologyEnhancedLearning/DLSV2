namespace DigitalLearningSolutions.Web.ControllerHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using MailKit.Net.Smtp;
    using MimeKit;

    public static class UnlockRequestMailHelper
    {
        public static void SendUnlockRequest(int id, IUnlockDataService unlockDataService, IConfigService configService)
        {
            var unlockData = unlockDataService.GetUnlockData(id);
            if (unlockData == null)
            {
                throw new UnlockDataMissingException();
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
                throw new ConfigValueMissingException();
            }

            var mailServerPort = int.Parse(mailServerPortString);
            var unlockUrl = new UriBuilder(trackingSystemBaseUrl);
            unlockUrl.Path += "centrecandidate.aspx";
            unlockUrl.Query = $"tab=course&CustomisationID={unlockData.CustomisationId}";

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

            using (var client = new SmtpClient())
            {
                client.Timeout = 10000;
                client.Connect(mailServerAddress, mailServerPort);

                client.Authenticate(mailServerUsername, mailServerPassword);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
