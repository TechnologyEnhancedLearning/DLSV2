namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
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
        private readonly IEmailService emailService;

        public NotificationService(
            INotificationDataService notificationDataService,
            IConfigService configService,
            IEmailService emailService
            )
        {
            this.notificationDataService = notificationDataService;
            this.configService = configService;
            this.emailService = emailService;
        }

        public void SendUnlockRequest(int progressId)
        {
            var unlockData = notificationDataService.GetUnlockData(progressId);
            if (unlockData == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch Unlock Data. Progress ID: {progressId}");
            }

            unlockData.ContactForename = unlockData.ContactForename == "" ? "Colleague" : unlockData.ContactForename;
            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.TrackingSystemBaseUrl) ??
                                        throw new ConfigValueMissingException(GenerateConfigValueMissingMessage());
            var unlockUrl = new UriBuilder(trackingSystemBaseUrl);
            unlockUrl.Path += "coursedelegates";
            unlockUrl.Query = $"CustomisationID={unlockData.CustomisationId}";
            string emailSubjectLine = "Digital Learning Solutions Progress Unlock Request";
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

            emailService.SendEmail(new Email(unlockData.ContactEmail, emailSubjectLine, builder, unlockData.DelegateEmail));
        }

        public void SendFrameworkCollaboratorInvite(int adminId, int frameworkId, int invitedByAdminId)
        {
            var collaboratorNotification = notificationDataService.GetCollaboratorNotification(adminId, frameworkId, invitedByAdminId);
            if (collaboratorNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch collaboratorNotification Data. adminId: {adminId}, frameworkId: {frameworkId}, invitedByAdminId: {invitedByAdminId})");
            }
            collaboratorNotification.Forename = collaboratorNotification.Forename == "" ? "Colleague" : collaboratorNotification.Forename;
            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.TrackingSystemBaseUrl)?.Replace("tracking/", "") ??
                                        throw new ConfigValueMissingException(GenerateConfigValueMissingMessage());
            if (trackingSystemBaseUrl.Contains("dls.nhs.uk")) { trackingSystemBaseUrl += "v2/"; }
            var frameworkUrl = new UriBuilder(trackingSystemBaseUrl);
            frameworkUrl.Path += $"Framework/Structure/{collaboratorNotification.FrameworkID}";
            string emailSubjectLine = "DLS Digital Framework Contributor Invitation";
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

            emailService.SendEmail(new Email(collaboratorNotification.Email, emailSubjectLine, builder, collaboratorNotification.InvitedByEmail));
        }

        private static string GenerateConfigValueMissingMessage()
        {
            return "Encountered an error while trying to send an unlock request email: The value of trackingSystemBaseUrl is null";
        }
    }
}
