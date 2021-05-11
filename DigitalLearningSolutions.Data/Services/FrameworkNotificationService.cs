namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using MimeKit;
    using System.Collections.Generic;
    using System.Linq;

    public interface IFrameworkNotificationService
    {
        void SendFrameworkCollaboratorInvite(int id, int invitedByAdminId);
        void SendCommentNotifications(int adminId, int frameworkId, int commentId, string comment, int? replyToCommentId, string? parentComment);
        void SendReviewRequest(int id, int invitedByAdminId);
    }
    public class FrameworkNotificationService : IFrameworkNotificationService
    {
        private readonly IConfigService configService;
        private readonly IEmailService emailService;
        private readonly IFrameworkService frameworkService;
        public FrameworkNotificationService(
           IFrameworkService frameworkService,
           IConfigService configService,
           IEmailService emailService
       )
        {
            this.frameworkService = frameworkService;
            this.configService = configService;
            this.emailService = emailService;
        }

        public void SendCommentNotifications(int adminId, int frameworkId, int commentId, string comment, int? replyToCommentId, string? parentComment)
        {
            var recipients = frameworkService.GetCommentRecipients(frameworkId, adminId, replyToCommentId);
            if (recipients.Count > 1)
            {
                var baseFramework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
                var sender = recipients.First();
                recipients.RemoveAt(0);
                string emailSubject = "";
                var commentUrl = GetFrameworkUrl(frameworkId, "Comments");
                commentUrl += $"/{(replyToCommentId == null ? commentId : replyToCommentId)}";
                foreach (var recipient in recipients)
                {

                    if (recipient.Owner)
                    {
                        if (replyToCommentId == null)
                        {
                            emailSubject = "New comment submitted against your framework: " + baseFramework.FrameworkName;
                        }
                        else
                        {
                            emailSubject = "Comment thread response submitted against your framework: " + baseFramework.FrameworkName;
                        }
                    }
                    else
                    {
                        emailSubject = "Response submitted to a framework comment thread you are involved in";
                    }
                    var builder = new BodyBuilder
                    {
                        TextBody = $@"Dear {recipient.FirstName},
                                    A comment has been submitted  against the framework, {baseFramework.FrameworkName} by {sender.FirstName} {sender.LastName} ({sender.Email}){(parentComment != null ? " in response to the thread " + parentComment : "")}.
                                    The comment reads: {comment}
                                    To view or reply to the comment in the framework system, visit this url: {commentUrl}. You will need to login to DLS to view the framework.",
                        HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'><p>Dear {recipient.FirstName},</p><p>A comment has been submitted  against the framework, <strong>{baseFramework.FrameworkName}</strong>, by <a href='mailto:{sender.Email}'>{sender.FirstName} {sender.LastName}</a>{(parentComment != null ? " in response to the thread <strong>" + parentComment + "</strong>" : ".")}</p><p>The comment reads: <strong>{comment}</strong></p><p><a href='{commentUrl}'>Click here</a> to view or reply to the comment in the framework system. You will need to login to DLS to view the framework.</p>"
                    };
                    emailService.SendEmail(new Email(emailSubject, builder, recipient.Email));
                }
            }
        }
        public void SendFrameworkCollaboratorInvite(int id, int invitedByAdminId)
        {
            var collaboratorNotification = frameworkService.GetCollaboratorNotification(id, invitedByAdminId);
            if (collaboratorNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch collaboratorNotification Data. id: {id}, invitedByAdminId: {invitedByAdminId})");
            }
            var frameworkUrl = GetFrameworkUrl(collaboratorNotification.FrameworkID, "Structure");
            string emailSubjectLine = $"DLS Digital Framework {collaboratorNotification?.FrameworkRole} Invitation";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear colleague,
                              You have been identified as a {collaboratorNotification?.FrameworkRole} for the framework, {collaboratorNotification?.FrameworkName}, by {collaboratorNotification?.InvitedByName} ({collaboratorNotification?.InvitedByEmail}).
                              To access the framework, visit this url: {frameworkUrl}. You will need to be registered on the Digital Learning Solutions platform to view the framework.",
                HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'><p>Dear colleague,</p><p>You have been identified as a {collaboratorNotification?.FrameworkRole} for the  framework, {collaboratorNotification?.FrameworkName}, by <a href='mailto:{collaboratorNotification?.InvitedByEmail}'>{collaboratorNotification?.InvitedByName}</a>.</p><p><a href='{frameworkUrl}'>Click here</a> to access the framework. You will need to be registered on the Digital Learning Solutions platform to view the framework.</p>"
            };
            emailService.SendEmail(new Email(emailSubjectLine, builder, collaboratorNotification.UserEmail, collaboratorNotification.InvitedByEmail));
        }

        public void SendReviewRequest(int id, int invitedByAdminId)
        {
            var collaboratorNotification = frameworkService.GetCollaboratorNotification(id, invitedByAdminId);
            if (collaboratorNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch collaboratorNotification Data. id: {id}, invitedByAdminId: {invitedByAdminId})");
            }
            var frameworkUrl = GetFrameworkUrl(collaboratorNotification.FrameworkID, "Structure");
            string emailSubjectLine = "DLS Digital Framework Review Request";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear colleague,
                              You have been requested to review the framework, {collaboratorNotification?.FrameworkName}, by {collaboratorNotification?.InvitedByName} ({collaboratorNotification?.InvitedByEmail}).
                              To review the framework, visit this url: {frameworkUrl}. Click the Review Framework button to submit your review and, if appropriate, sign-off the framework. You will need to be registered on the Digital Learning Solutions platform to review the framework.",
                HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'><p>Dear colleague,</p><p>You have been requested to review the framework, {collaboratorNotification?.FrameworkName}, by <a href='mailto:{collaboratorNotification?.InvitedByEmail}'>{collaboratorNotification?.InvitedByName}</a>.</p><p><a href='{frameworkUrl}'>Click here</a> to review the framework. Click the Review Framework button to submit your review and, if appropriate, sign-off the framework. You will need to be registered on the Digital Learning Solutions platform to view the framework.</p>"
            };
            emailService.SendEmail(new Email(emailSubjectLine, builder, collaboratorNotification.UserEmail, collaboratorNotification.InvitedByEmail));
        }
        public string GetFrameworkUrl(int frameworkId, string tab)
        {
            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.AppBaseUrl) ??
                                       throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("AppBaseUrl"));
            ;
            var frameworkUrl = new UriBuilder(trackingSystemBaseUrl);
            frameworkUrl.Path += $"Framework/{frameworkId}/{tab}/";
            return frameworkUrl.Uri.ToString();
        }
    }
}
