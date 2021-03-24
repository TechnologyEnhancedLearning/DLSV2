namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models;
    using MimeKit;

    public interface INotificationService
    {
        void SendUnlockRequest(int progressId);
        void SendFrameworkCollaboratorInvite(int adminId, int frameworkId, int invitedByAdminId);
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
        public void SendUnlockRequest(int progressId)
        {
            var unlockData = notificationDataService.GetUnlockData(progressId);
            if (unlockData == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch Unlock Data. Progress ID: {progressId}");
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
        public void SendFrameworkCollaboratorInvite(int adminId, int frameworkId, int invitedByAdminId)
        {
            var collaboratorNotification = notificationDataService.GetCollaboratorNotification(adminId, frameworkId, invitedByAdminId);
            if (collaboratorNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch collaboratorNotification Data. adminId: {adminId}, frameworkId: {frameworkId}, invitedByAdminId: {invitedByAdminId})");
            }
            collaboratorNotification.Forename = collaboratorNotification.Forename == "" ? "Colleague" : collaboratorNotification.Forename;
            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.TrackingSystemBaseUrl).Replace("tracking/", "");
            if (trackingSystemBaseUrl.Contains("dls.nhs.uk")) { trackingSystemBaseUrl += "v2/"; }
            var frameworkUrl = new UriBuilder(trackingSystemBaseUrl);
            frameworkUrl.Path += $"Framework/Structure/{collaboratorNotification.FrameworkID}";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {collaboratorNotification?.Forename},
You have been identified as a {collaboratorNotification?.FrameworkRole} for the digital capability framework, {collaboratorNotification?.FrameworkName} by {collaboratorNotification?.InvitedByName} ({collaboratorNotification?.InvitedByEmail}).
To access the framework, visit this url: {frameworkUrl}. You will need to login to DLS to view the framework.",
                HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'>
                                    <p>Dear {collaboratorNotification?.Forename},</p>
<p>You have been identified as a {collaboratorNotification?.FrameworkRole} for the digital capability framework, {collaboratorNotification?.FrameworkName} by <a href='mailto:{collaboratorNotification?.InvitedByEmail}'>{collaboratorNotification?.InvitedByName}</a>.</p>
<p>To access the framework, <a href='{frameworkUrl}'>click here</a>. You will need to login to DLS to view the framework.</p>"
            };
            SendSMTPMessage(collaboratorNotification?.Email, collaboratorNotification?.InvitedByEmail, "DLS Digital Framework Contributor Invitation", builder);
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
