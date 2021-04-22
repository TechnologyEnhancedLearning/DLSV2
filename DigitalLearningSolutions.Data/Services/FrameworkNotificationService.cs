﻿namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using MimeKit;
using System.Collections.Generic;
    using System.Linq;

    public interface IFrameworkNotificationService
    {
        void SendFrameworkCollaboratorInvite(int adminId, int frameworkId, int invitedByAdminId);
        void SendCommentNotifications(int adminId, int frameworkId, int commentId, string comment, int? replyToCommentId, string? parentComment);
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
            if(recipients.Count > 1)
            {
                var baseFramework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
                var sender = recipients.First();
                recipients.RemoveAt(0);
                string emailSubject = "";
                var commentUrl = getBaseURL();
                commentUrl.Path += $"Framework/{frameworkId}/Comments/{(replyToCommentId == null ? commentId : replyToCommentId)}";
                
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
        public UriBuilder getBaseURL()
        {
            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.TrackingSystemBaseUrl)?.Replace("tracking/", "") ??
                                        throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("TrackingSystemBaseUrl"));
            ;
            if (trackingSystemBaseUrl.Contains("dls.nhs.uk"))
            {
                trackingSystemBaseUrl += "v2/";
            }

            var baseUrl = new UriBuilder(trackingSystemBaseUrl);
            return baseUrl;
        }
        public void SendFrameworkCollaboratorInvite(int adminId, int frameworkId, int invitedByAdminId)
        {
            var collaboratorNotification = frameworkService.GetCollaboratorNotification(adminId, frameworkId, invitedByAdminId);
            if (collaboratorNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch collaboratorNotification Data. adminId: {adminId}, frameworkId: {frameworkId}, invitedByAdminId: {invitedByAdminId})");
            }
            var frameworkUrl = getBaseURL();
            frameworkUrl.Path += $"Framework/Structure/{collaboratorNotification.FrameworkID}";
            string emailSubjectLine = "DLS Digital Framework Contributor Invitation";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {collaboratorNotification?.Forename},
You have been identified as a {collaboratorNotification?.FrameworkRole} for the framework, {collaboratorNotification?.FrameworkName} by {collaboratorNotification?.InvitedByName} ({collaboratorNotification?.InvitedByEmail}).
To access the framework, visit this url: {frameworkUrl}. You will need to login to DLS to view the framework.",
                HtmlBody = $@"<body style= 'font - family: Calibri; font - size: small;'><p>Dear {collaboratorNotification?.Forename},</p><p>You have been identified as a {collaboratorNotification?.FrameworkRole} for the  framework, {collaboratorNotification?.FrameworkName} by <a href='mailto:{collaboratorNotification?.InvitedByEmail}'>{collaboratorNotification?.InvitedByName}</a>.</p><p><a href='{frameworkUrl}'>Click here</a> to access the framework. You will need to login to DLS to view the framework.</p>"
            };

            emailService.SendEmail(new Email(emailSubjectLine, builder, collaboratorNotification.Email, collaboratorNotification.InvitedByEmail));
        }
    }
}
