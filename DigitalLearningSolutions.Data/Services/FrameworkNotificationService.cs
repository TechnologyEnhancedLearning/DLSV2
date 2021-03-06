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
        void SendFrameworkCollaboratorInvite(int id, int invitedByAdminId);
        void SendCommentNotifications(int adminId, int frameworkId, int commentId, string comment, int? replyToCommentId, string? parentComment);
        void SendReviewRequest(int id, int invitedByAdminId, bool required, bool reminder);
        void SendReviewOutcomeNotification(int reviewId);
        void SendSupervisorDelegateInvite(int supervisorDelegateId);
        void SendSupervisorDelegateAcceptance(int supervisorDelegateId);
        void SendSupervisorDelegateRejected(int supervisorDelegateId);
        void SendSupervisorDelegateConfirmed(int superviseDelegateId);
    }
    public class FrameworkNotificationService : IFrameworkNotificationService
    {
        private readonly IConfigService configService;
        private readonly IEmailService emailService;
        private readonly IFrameworkService frameworkService;
        private readonly IRoleProfileService roleProfileService;
        private readonly ISupervisorService supervisorService;
        public FrameworkNotificationService(
           IFrameworkService frameworkService,
           IConfigService configService,
           IEmailService emailService,
           IRoleProfileService roleProfileService,
           ISupervisorService supervisorService
       )
        {
            this.frameworkService = frameworkService;
            this.configService = configService;
            this.emailService = emailService;
            this.roleProfileService = roleProfileService;
            this.supervisorService = supervisorService;
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
                commentUrl += $"{(replyToCommentId == null ? commentId : replyToCommentId)}";
                foreach (var recipient in recipients)
                {

                    if (recipient.Owner)
                    {
                        if (replyToCommentId == null)
                        {
                            emailSubject = $"New comment submitted against your framework: {baseFramework.FrameworkName}  - Digital Learning Solutions";
                        }
                        else
                        {
                            emailSubject = $"Comment thread response submitted against your framework: {baseFramework.FrameworkName}  - Digital Learning Solutions";
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
                        HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {recipient.FirstName},</p><p>A comment has been submitted  against the framework, <strong>{baseFramework.FrameworkName}</strong>, by <a href='mailto:{sender.Email}'>{sender.FirstName} {sender.LastName}</a>{(parentComment != null ? " in response to the thread <strong>" + parentComment + "</strong>" : ".")}</p><p>The comment reads: <strong>{comment}</strong></p><p><a href='{commentUrl}'>Click here</a> to view or reply to the comment in the framework system. You will need to login to DLS to view the framework.</p></body>"
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
            string emailSubjectLine = $"Framework {collaboratorNotification?.FrameworkRole} Invitation - Digital Learning Solutions";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear colleague,
                              You have been identified as a {collaboratorNotification?.FrameworkRole} for the framework, {collaboratorNotification?.FrameworkName}, by {collaboratorNotification?.InvitedByName} ({collaboratorNotification?.InvitedByEmail}).
                              To access the framework, visit this url: {frameworkUrl}. You will need to be registered on the Digital Learning Solutions platform to view the framework.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear colleague,</p><p>You have been identified as a {collaboratorNotification?.FrameworkRole} for the  framework, {collaboratorNotification?.FrameworkName}, by <a href='mailto:{collaboratorNotification?.InvitedByEmail}'>{collaboratorNotification?.InvitedByName}</a>.</p><p><a href='{frameworkUrl}'>Click here</a> to access the framework. You will need to be registered on the Digital Learning Solutions platform to view the framework.</p></body>"
            };
            emailService.SendEmail(new Email(emailSubjectLine, builder, collaboratorNotification.UserEmail, collaboratorNotification.InvitedByEmail));
        }

        public void SendReviewRequest(int id, int invitedByAdminId, bool required, bool reminder)
        {
            var collaboratorNotification = frameworkService.GetCollaboratorNotification(id, invitedByAdminId);
            if (collaboratorNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch collaboratorNotification Data. id: {id}, invitedByAdminId: {invitedByAdminId}");
            }
            var frameworkUrl = GetFrameworkUrl(collaboratorNotification.FrameworkID, "Structure");
            string emailSubjectLine = (reminder ? " REMINDER: " : "") + "Framework Review Request - Digital Learning Solutions";
            string signOffRequired = required ? "You are required to sign-off this framework before it can be published." : "You are not required to sign-off this framework before it is published.";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear colleague,
                              You have been requested to review the framework, {collaboratorNotification?.FrameworkName}, by {collaboratorNotification?.InvitedByName} ({collaboratorNotification?.InvitedByEmail}).
                              To review the framework, visit this url: {frameworkUrl}. Click the Review Framework button to submit your review and, if appropriate, sign-off the framework. {signOffRequired}. You will need to be registered on the Digital Learning Solutions platform to review the framework.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear colleague,</p><p>You have been requested to review the framework, {collaboratorNotification?.FrameworkName}, by <a href='mailto:{collaboratorNotification?.InvitedByEmail}'>{collaboratorNotification?.InvitedByName}</a>.</p><p><a href='{frameworkUrl}'>Click here</a> to review the framework. Click the Review Framework button to submit your review and, if appropriate, sign-off the framework.</p><p>{signOffRequired}</p><p>You will need to be registered on the Digital Learning Solutions platform to view the framework.</p></body>"
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

        public void SendReviewOutcomeNotification(int reviewId)
        {
            var outcomeNotification = frameworkService.GetFrameworkReviewNotification(reviewId);
            if (outcomeNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch review outcome Data. reviewId: {reviewId}");
            }
            var frameworkUrl = GetFrameworkUrl(outcomeNotification.FrameworkID, "Publish");
            string emailSubjectLine = $"Framework Review Outcome - {(outcomeNotification.SignedOff ? "Approved" : "Rejected")} - Digital Learning Solutions" ;
            string approvalStatus = outcomeNotification?.ReviewerFirstName + (outcomeNotification.SignedOff ? " approved the framework for publishing." : " did not approve the framework for publishing.");
            string commentsText = outcomeNotification?.ReviewerFirstName + (outcomeNotification.Comment != null ? " left the following review comment: " + outcomeNotification.Comment : " did not leave a review comment.");
            string commentsHtml = "<p>" + outcomeNotification?.ReviewerFirstName + (outcomeNotification.Comment != null ? " left the following review comment:</p><hr/><p>" + outcomeNotification.Comment + "</p><hr/>" : " did not leave a review comment.</p>");
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {outcomeNotification.OwnerFirstName},
                              Your framework, {outcomeNotification?.FrameworkName}, has been reviewed by {outcomeNotification?.ReviewerFirstName + " " + outcomeNotification?.ReviewerLastName} ({outcomeNotification?.UserEmail}).
                              {approvalStatus}
                              {commentsText}
                              The full framework review status, can be viewed by visiting: {frameworkUrl}. Once all of the required reviewers have approved the framework, you may publish it. You will need to login to the Digital Learning Solutions platform to access the framework.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {outcomeNotification.OwnerFirstName},</p><p>Your framework, {outcomeNotification?.FrameworkName}, has been reviewed by <a href='mailto:{outcomeNotification?.UserEmail}'>{outcomeNotification?.ReviewerFirstName + " " + outcomeNotification.ReviewerLastName}</a>.</p><p>{approvalStatus}</p>{commentsHtml}<p><a href='{frameworkUrl}'>Click here</a> to view the full review status for the framework. Once all of the required reviewers have approved the framework, you may publish it.</p><p>You will need to login to the Digital Learning Solutions platform to access the framework.</p></body>"
            };
            emailService.SendEmail(new Email(emailSubjectLine, builder, outcomeNotification.OwnerEmail, outcomeNotification.UserEmail));
        }
        public void SendSupervisorDelegateInvite(int supervisorDelegateId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);

            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.AppBaseUrl) ??
                                        throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("AppBaseUrl"));
            string emailSubjectLine = "Invite from Supervisor - Digital Learning Solutions";
            var builder = new BodyBuilder();
            var dlsUrlBuilder = new UriBuilder(trackingSystemBaseUrl);
            if (supervisorDelegate.CandidateID == null)
            {
                dlsUrlBuilder.Path += $"Register?centreid={supervisorDelegate.CentreId}&inviteid={supervisorDelegateId}";
                builder.TextBody = $@"Dear colleague,
                              You have been invited to register to access the NHS Health Education England, Digital Learning Solutions platform as a supervised delegate by {supervisorDelegate.SupervisorName} ({supervisorDelegate.SupervisorEmail}).
                              To register, visit {dlsUrlBuilder.Uri.ToString()}.
                              Registering using this link will confirm your acceptance of the invite. Your supervisor will then be able to assign role profile assessments and view and validate your self assessment results.";
                builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear colleague,</p><p>You have been invited to register to access the NHS Health Education England, Digital Learning Solutions platform as a supervised delegate by <a href='mailto:{supervisorDelegate.SupervisorEmail}'>{supervisorDelegate.SupervisorName}</a>.</p><p><a href='{dlsUrlBuilder.Uri.ToString()}'>Click here</a> to register and confirm your acceptance of the invite.</p><p>Your supervisor will then be able to assign role profile assessments and view and validate your self assessment results.</p>";
            }
            else
            {
                dlsUrlBuilder.Path += $"LearningPortal/ConfirmSupervisor/{supervisorDelegateId}";
                builder.TextBody = $@"Dear {supervisorDelegate.FirstName},
                              You have been identified as a supervised delegate by {supervisorDelegate.SupervisorName} ({supervisorDelegate.SupervisorEmail}) in the NHS Health Education England, Digital Learning Solutions (DLS) platform.
                              You are already registered as a delegate at the supervisor's DLS centre. To respond to their invite, please visit {dlsUrlBuilder.Uri.ToString()} (you may need to sign in using your existing DLS credentials).
                              Once you have accepted the invite, your supervisor will be able to assign role profile assessments and view and validate your self assessment results.";
                builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear  {supervisorDelegate.FirstName},</p><p>You have been identified as a supervised delegate by <a href='mailto:{supervisorDelegate.SupervisorEmail}'>{supervisorDelegate.SupervisorName}</a> in the NHS Health Education England, Digital Learning Solutions (DLS) platform.</p><p>You are already registered as a delegate at the supervisor's DLS centre. <a href='{dlsUrlBuilder.Uri.ToString()}'>Click here</a> to respond to their invite (you may need to sign in using your existing DLS credentials).</p><p>Once you have accepted the invite, your supervisor will be able to assign role profile assessments and view and validate your self assessment results.</p>";
            }
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.DelegateEmail));
        }

        public void SendSupervisorDelegateAcceptance(int supervisorDelegateId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);

            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.AppBaseUrl) ??
                                        throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("AppBaseUrl"));
            string emailSubjectLine = "Accepted Supervisor Invitation - Digital Learning Solutions";
            var builder = new BodyBuilder();
            var dlsUrlBuilder = new UriBuilder(trackingSystemBaseUrl);
            dlsUrlBuilder.Path += $"/Supervisor/MyStaff";
            builder.TextBody = $@"Dear {supervisorDelegate.SupervisorName},
                              {supervisorDelegate.FirstName} {supervisorDelegate.LastName} has accepted your invitation to become a member of your team in the NHS Health Education England, Digital Learning Solutions platform.
You are now confirmed as a supervisor for this delegate.
To manage their role profile assessments, please visit {dlsUrlBuilder.Uri.ToString()}.";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.SupervisorName}</p><p>{supervisorDelegate.FirstName} {supervisorDelegate.LastName} has accepted your invitation to become a member of your team in the NHS Health Education England, Digital Learning Solutions platform.</p><p>You are now confirmed as a supervisor for this delegate.</p><p><a href='{dlsUrlBuilder.Uri.ToString()}'>Click here</a> to manage their role profile assessments.</p>";
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.SupervisorEmail));
        }

        public void SendSupervisorDelegateRejected(int supervisorDelegateId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);

            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.AppBaseUrl) ??
                                        throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("AppBaseUrl"));
            string emailSubjectLine = "REJECTED Supervisor Invitation - Digital Learning Solutions";
            var builder = new BodyBuilder();
            var dlsUrlBuilder = new UriBuilder(trackingSystemBaseUrl);
            dlsUrlBuilder.Path += $"/Supervisor/MyStaff";
            builder.TextBody = $@"Dear {supervisorDelegate.SupervisorName},
                              {supervisorDelegate.FirstName} {supervisorDelegate.LastName} has rejected your invitation to become a member of your team in the NHS Health Education England, Digital Learning Solutions platform.
This delegate has been removed from your supervisor team.
To manage your team, please visit {dlsUrlBuilder.Uri.ToString()}.";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.SupervisorName}</p><p>{supervisorDelegate.FirstName} {supervisorDelegate.LastName} has rejected your invitation to become a member of your team in the NHS Health Education England, Digital Learning Solutions platform.</p><p>This delegate has been removed from your supervisor team.</p><p><a href='{dlsUrlBuilder.Uri.ToString()}'>Click here</a> to manage your team.</p>";
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.SupervisorEmail));
        }

        public void SendSupervisorDelegateConfirmed(int supervisorDelegateId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);

            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.AppBaseUrl) ??
                                        throw new ConfigValueMissingException(configService.GetConfigValueMissingExceptionMessage("AppBaseUrl"));
            string emailSubjectLine = "Supervisor Confirmed - Digital Learning Solutions";
            var builder = new BodyBuilder();
            var dlsUrlBuilder = new UriBuilder(trackingSystemBaseUrl);
            dlsUrlBuilder.Path += $"/LearningPortal/Current";
            builder.TextBody = $@"Dear {supervisorDelegate.FirstName},
                               {supervisorDelegate.SupervisorName} has accepted your request to be your supervisor for profile asessment activities in the NHS Health Education England, Digital Learning Solutions platform.
To access your role profile assessments, please visit {dlsUrlBuilder.Uri.ToString()}.";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.FirstName}</p><p>{supervisorDelegate.SupervisorName} has accepted your request to be your supervisor for profile asessment activities in the NHS Health Education England, Digital Learning Solutions platform.</p><p><a href='{dlsUrlBuilder.Uri.ToString()}'>Click here</a> to access your role profile assessments.</p>";
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.DelegateEmail));
        }
    }
}
