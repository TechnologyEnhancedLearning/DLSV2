﻿namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Constants;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using MimeKit;

    public interface IFrameworkNotificationService
    {
        void SendFrameworkCollaboratorInvite(int id, int invitedByAdminId);
        void SendCommentNotifications(int adminId, int frameworkId, int commentId, string comment, int? replyToCommentId, string? parentComment);
        void SendReviewRequest(int id, int invitedByAdminId, bool required, bool reminder, int centreId);
        void SendReviewOutcomeNotification(int reviewId, int centreId);
        void SendSupervisorDelegateInvite(int supervisorDelegateId, int adminId, int centreId);
        void SendSupervisorDelegateConfirmed(int superviseDelegateId, int adminId, int delegateUserId, int centreId);
        void SendSupervisorResultReviewed(int adminId, int supervisorDelegateId, int candidateAssessmentId, int resultId, int centreId);
        void SendSupervisorEnroledDelegate(int adminId, int supervisorDelegateId, int candidateAssessmentId, DateTime? completeByDate, int centreId);
        void SendReminderDelegateSelfAssessment(int adminId, int supervisorDelegateId, int candidateAssessmentId, int centreId);
        void SendSupervisorMultipleResultsReviewed(int adminId, int supervisorDelegateId, int candidateAssessmentId, int countResults, int centreId);
        void SendDelegateSupervisorNominated(int supervisorDelegateId, int selfAssessmentID, int delegateUserId, int centreId, int? selfAssessmentResultId = null);
        void SendResultVerificationRequest(int candidateAssessmentSupervisorId, int selfAssessmentID, int resultCount, int delegateUserId, int centreId, int? selfAssessmentResultId = null);
        void SendSignOffRequest(int candidateAssessmentSupervisorId, int selfAssessmentID, int delegateUserId, int centreId);
        void SendProfileAssessmentSignedOff(int supervisorDelegateId, int candidateAssessmentId, string? supervisorComments, bool signedOff, int adminId, int centreId);
        void SendSupervisorDelegateReminder(int supervisorDelegateId, int adminId, int centreId);
    }

    public class FrameworkNotificationService : IFrameworkNotificationService
    {

        private readonly IConfigDataService configDataService;
        private readonly IEmailService emailService;
        private readonly IFrameworkService frameworkService;
        private readonly IRoleProfileService roleProfileService;
        private readonly ISupervisorService supervisorService;
        private readonly ISelfAssessmentDataService selfAssessmentDataService;
        private readonly ICentresDataService centresDataService;
        public FrameworkNotificationService(
           IFrameworkService frameworkService,
           IConfigDataService configDataService,
           IEmailService emailService,
           IRoleProfileService roleProfileService,
           ISupervisorService supervisorService,
           ISelfAssessmentDataService selfAssessmentDataService,
           ICentresDataService centresDataService
       )
        {
            this.frameworkService = frameworkService;
            this.configDataService = configDataService;
            this.emailService = emailService;
            this.roleProfileService = roleProfileService;
            this.supervisorService = supervisorService;
            this.selfAssessmentDataService = selfAssessmentDataService;
            this.centresDataService = centresDataService;
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
            string emailSubjectLine = $"Framework {collaboratorNotification.FrameworkRole} Invitation - Digital Learning Solutions";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear colleague,
                              You have been identified as a {collaboratorNotification.FrameworkRole} for the framework, {collaboratorNotification.FrameworkName}, by {collaboratorNotification.InvitedByName} ({collaboratorNotification.InvitedByEmail}).
                              To access the framework, visit this url: {frameworkUrl}. You will need to be registered on the Digital Learning Solutions platform to view the framework.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear colleague,</p>
                                <p>You have been identified as a {collaboratorNotification.FrameworkRole} for the  framework, {collaboratorNotification.FrameworkName}, by <a href='mailto:{collaboratorNotification.InvitedByEmail}'>{collaboratorNotification.InvitedByName}</a>.</p>
                                <p><a href='{frameworkUrl}'>Click here</a> to access the framework. You will need to be registered on the Digital Learning Solutions platform to view the framework.</p>
                            </body>",
            };
            emailService.SendEmail(new Email(emailSubjectLine, builder, collaboratorNotification.UserEmail, collaboratorNotification.InvitedByEmail));
        }

        public void SendReviewRequest(int id, int invitedByAdminId, bool required, bool reminder, int centreId)
        {
            string centreName = GetCentreName(centreId);
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
                              You have been requested to review the framework, {collaboratorNotification?.FrameworkName}, by {collaboratorNotification?.InvitedByName} ({collaboratorNotification?.InvitedByEmail}) ({centreName}).
                              To review the framework, visit this url: {frameworkUrl}. Click the Review Framework button to submit your review and, if appropriate, sign-off the framework. {signOffRequired}. You will need to be registered on the Digital Learning Solutions platform to review the framework.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear colleague,</p><p>You have been requested to review the framework, {collaboratorNotification?.FrameworkName}, by <a href='mailto:{collaboratorNotification?.InvitedByEmail}'>{collaboratorNotification?.InvitedByName} ({centreName})</a>.</p><p><a href='{frameworkUrl}'>Click here</a> to review the framework. Click the Review Framework button to submit your review and, if appropriate, sign-off the framework.</p><p>{signOffRequired}</p><p>You will need to be registered on the Digital Learning Solutions platform to view the framework.</p></body>"
            };
            emailService.SendEmail(new Email(emailSubjectLine, builder, collaboratorNotification.UserEmail, collaboratorNotification.InvitedByEmail));
        }
        public string GetFrameworkUrl(int frameworkId, string tab)
        {
            var frameworkUrl = GetDLSUriBuilder();
            frameworkUrl.Path += $"Framework/{frameworkId}/{tab}/";
            return frameworkUrl.Uri.ToString();
        }
        public string GetCurrentActivitiesUrl()
        {
            var dlsUrlBuilder = GetDLSUriBuilder();
            dlsUrlBuilder.Path += "LearningPortal/Current";
            return dlsUrlBuilder.Uri.ToString();
        }
        public string GetSelfAssessmentUrl(int selfAssessmentId, bool overview = true)
        {
            var dlsUrlBuilder = GetDLSUriBuilder();
            dlsUrlBuilder.Path += $"LearningPortal/SelfAssessment/{selfAssessmentId}" + (overview ? "/overview" : "");
            return dlsUrlBuilder.Uri.ToString();
        }
        public string GetSupervisorReviewUrl()
        {
            return "";
        }
        public UriBuilder GetDLSUriBuilder()
        {
            var trackingSystemBaseUrl = configDataService.GetConfigValue(ConfigConstants.AppBaseUrl) ??
                                       throw new ConfigValueMissingException(configDataService.GetConfigValueMissingExceptionMessage("AppBaseUrl"));
            ;
            return new UriBuilder(trackingSystemBaseUrl);
        }

        public void SendReviewOutcomeNotification(int reviewId, int centreId)
        {
            string centreName = GetCentreName(centreId);
            var outcomeNotification = frameworkService.GetFrameworkReviewNotification(reviewId);
            if (outcomeNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch review outcome Data. reviewId: {reviewId}");
            }
            var frameworkUrl = GetFrameworkUrl(outcomeNotification.FrameworkID, "Publish");
            string emailSubjectLine = $"Framework Review Outcome - {(outcomeNotification.SignedOff ? "Approved" : "Rejected")} - Digital Learning Solutions";
            string approvalStatus = outcomeNotification.ReviewerFirstName + (outcomeNotification.SignedOff ? " approved the framework for publishing." : " did not approve the framework for publishing.");
            string commentsText = outcomeNotification.ReviewerFirstName + (outcomeNotification.Comment != null ? " left the following review comment: " + outcomeNotification.Comment : " did not leave a review comment.");
            string commentsHtml = "<p>" + outcomeNotification.ReviewerFirstName + (outcomeNotification.Comment != null ? " left the following review comment:</p><hr/><p>" + outcomeNotification.Comment + "</p><hr/>" : " did not leave a review comment.</p>");

            string reviewerFullName = $"{outcomeNotification.ReviewerFirstName} {outcomeNotification.ReviewerLastName} {(outcomeNotification.ReviewerActive == true ? "" : " (inactive)")}";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {outcomeNotification.OwnerFirstName},
                              Your framework, {outcomeNotification.FrameworkName}, has been reviewed by {reviewerFullName} ({outcomeNotification.UserEmail}) ({centreName}).
                              {approvalStatus}
                              {commentsText}
                              The full framework review status, can be viewed by visiting: {frameworkUrl}. Once all of the required reviewers have approved the framework, you may publish it. You will need to login to the Digital Learning Solutions platform to access the framework.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear {outcomeNotification.OwnerFirstName},</p>
                                <p>Your framework, {outcomeNotification.FrameworkName}, has been reviewed by <a href='mailto:{outcomeNotification.UserEmail}'>{reviewerFullName} ({centreName})</a>.</p>
                                <p>{approvalStatus}</p>
                                {commentsHtml}
                                <p><a href='{frameworkUrl}'>Click here</a> to view the full review status for the framework. Once all of the required reviewers have approved the framework, you may publish it.</p>
                                <p>You will need to login to the Digital Learning Solutions platform to access the framework.</p>
                            </body>",
            };
            emailService.SendEmail(new Email(emailSubjectLine, builder, outcomeNotification.OwnerEmail, outcomeNotification.UserEmail));
        }


        public void SendSupervisorDelegateInvite(int supervisorDelegateId, int adminId, int centreId)
        {

            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, 0);
            string emailSubjectLine = "Invite from Supervisor - Digital Learning Solutions";
            var builder = new BodyBuilder();
            var dlsUrlBuilder = GetDLSUriBuilder();
            if (supervisorDelegate.DelegateUserID == null)
            {
                string centreName = GetCentreName(centreId);
                dlsUrlBuilder.Path += "Register";
                dlsUrlBuilder.Query = $"centreid={supervisorDelegate.CentreId}&inviteid={supervisorDelegate.InviteHash}";
                builder.TextBody = $@"Dear colleague,
                              You have been invited to register to access the NHS England, Digital Learning Solutions platform as a supervised delegate by {supervisorDelegate.SupervisorName} ({supervisorDelegate.SupervisorEmail}) ({centreName}).
                              To register, visit {dlsUrlBuilder.Uri.ToString()}.
                              Registering using this link will confirm your acceptance of the invite. Your supervisor will then be able to assign role profile assessments and view and validate your self assessment results.";
                builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear colleague,</p><p>You have been invited to register to access the NHS England, Digital Learning Solutions platform as a supervised delegate by <a href='mailto:{supervisorDelegate.SupervisorEmail}'>{supervisorDelegate.SupervisorName} ({centreName})</a>.</p><p><a href='{dlsUrlBuilder.Uri.ToString()}'>Click here</a> to register and confirm your acceptance of the invite.</p><p>Your supervisor will then be able to assign role profile assessments and view and validate your self assessment results.</p></body>";
                emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.DelegateEmail));
            }
        }

        public void SendSupervisorDelegateConfirmed(int supervisorDelegateId, int adminId, int delegateUserId, int centreId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, delegateUserId);

            string centreName = GetCentreName(centreId);
            string emailSubjectLine = "Supervisor Confirmed - Digital Learning Solutions";
            var builder = new BodyBuilder();
            builder.TextBody = $@"Dear {supervisorDelegate.FirstName}
You have been identified as a supervised delegate by {supervisorDelegate.SupervisorName} ({supervisorDelegate.SupervisorEmail}) ({centreName}) in the NHS England, Digital Learning Solutions (DLS) platform.

You are already registered as a delegate at the supervisor's DLS centre so they can now assign competency self assessments and view and validate your self assessment results.

If this looks like a mistake, please contact {supervisorDelegate.SupervisorName} ({supervisorDelegate.SupervisorEmail}) ({centreName}) directly to correct.";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.FirstName}</p><p>{supervisorDelegate.SupervisorName} ({centreName}) has accepted your request to be your supervisor for profile asessment activities in the NHS England, Digital Learning Solutions platform.</p><p><a href='{GetCurrentActivitiesUrl()}'>Click here</a> to access your role profile assessments.</p></body>";
            string toEmail = (@adminId == 0 ? supervisorDelegate.DelegateEmail : supervisorDelegate.SupervisorEmail);
            emailService.SendEmail(new Email(emailSubjectLine, builder, toEmail));
        }

        public void SendSupervisorResultReviewed(int adminId, int supervisorDelegateId, int candidateAssessmentId, int resultId, int centreId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, 0);
            string centreName = GetCentreName(centreId);
            var competency = selfAssessmentDataService.GetCompetencyByCandidateAssessmentResultId(resultId, candidateAssessmentId, adminId);
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(candidateAssessmentId, supervisorDelegateId);
            var selfAssessmentUrl = GetSelfAssessmentUrl(delegateSelfAssessment.SelfAssessmentID, false);
            var commentString = supervisorDelegate.SupervisorName + ((bool)competency.AssessmentQuestions.First().SignedOff ? " confirmed your self assessment " : " did not confirm your self assessment ") + (competency.AssessmentQuestions.First().SupervisorComments != null ? "and left the following review comment: " + competency.AssessmentQuestions.First().SupervisorComments : "but did not leave a review comment.");
            string emailSubjectLine = $"{delegateSelfAssessment.SupervisorRoleTitle} Reviewed {competency.Vocabulary} - Digital Learning Solutions";
            var builder = new BodyBuilder();
            builder.TextBody = $@"Dear {supervisorDelegate.FirstName},
                               {supervisorDelegate.SupervisorName} ({centreName}) has reviewed your self assessment against the {competency.Vocabulary} '{competency.Name}' ({competency.CompetencyGroup}) in the NHS England, Digital Learning Solutions platform.
                               {commentString}
                               To access your {delegateSelfAssessment.RoleName} profile assessment, please visit {selfAssessmentUrl}.";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.FirstName}</p><p>{supervisorDelegate.SupervisorName} ({centreName})  has reviewed your self assessment against the {competency.Vocabulary} '{competency.Name}' ({competency.CompetencyGroup}) in the NHS England, Digital Learning Solutions platform.</p><p>{commentString}</p><p><a href='{selfAssessmentUrl}'>Click here</a> to access your  {delegateSelfAssessment.RoleName} profile assessment.</p></body>";
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.DelegateEmail));
        }

        public void SendSupervisorEnroledDelegate(int adminId, int supervisorDelegateId, int candidateAssessmentId, DateTime? completeByDate, int centreId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, 0);
            string centreName = GetCentreName(centreId);
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(candidateAssessmentId, supervisorDelegateId);
            var selfAssessmentUrl = GetSelfAssessmentUrl(delegateSelfAssessment.SelfAssessmentID, false);
            var completeByString = completeByDate == null ? $"Your {delegateSelfAssessment.SupervisorRoleTitle} did not specify a date by which the self assessment should be completed." : $"Your {delegateSelfAssessment.SupervisorRoleTitle} indicated that this self assessment should be completed by {completeByDate.Value.ToShortDateString()}.";
            var supervisorReviewString = delegateSelfAssessment.SupervisorResultsReview | delegateSelfAssessment.SupervisorSelfAssessmentReview ? $"You will be able to request review for your self assessments against this profile from your {delegateSelfAssessment.SupervisorRoleTitle}." : "";
            string emailSubjectLine = $"You have been enrolled on the profile assessment {delegateSelfAssessment.RoleName} by {supervisorDelegate.SupervisorName} - Digital Learning Solutions";
            var builder = new BodyBuilder();
            builder.TextBody = $@"Dear {supervisorDelegate.FirstName},
                               {supervisorDelegate.SupervisorName} ({centreName}) has enrolled you on the profile assessment '{delegateSelfAssessment.RoleName}' in the NHS England, Digital Learning Solutions platform.
                               {supervisorDelegate.SupervisorName} has identified themselves as your {delegateSelfAssessment.SupervisorRoleTitle} for this activity.
                               {completeByString}
                               {supervisorReviewString}
                               To access your {delegateSelfAssessment.RoleName} profile assessment, please visit {selfAssessmentUrl}.";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.FirstName}</p><p>{supervisorDelegate.SupervisorName} ({centreName}) has enrolled you on the profile assessment '{delegateSelfAssessment.RoleName}' in the NHS England, Digital Learning Solutions platform.</p>{(completeByString.Length > 0 ? $"<p>{completeByString}</p>" : "")}{(supervisorReviewString.Length > 0 ? $"<p>{supervisorReviewString}</p>" : "")}<p><a href='{selfAssessmentUrl}'>Click here</a> to access your  {delegateSelfAssessment.RoleName} profile assessment.</p></body>";
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.DelegateEmail));
        }

        public void SendReminderDelegateSelfAssessment(int adminId, int supervisorDelegateId, int candidateAssessmentId, int centreId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, 0);
            string centreName = GetCentreName(centreId);
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(candidateAssessmentId, supervisorDelegateId);
            var selfAssessmentUrl = GetSelfAssessmentUrl(delegateSelfAssessment.SelfAssessmentID);
            string emailSubjectLine = $"Reminder to complete the profile assessment {delegateSelfAssessment.RoleName} - Digital Learning Solutions";
            var builder = new BodyBuilder();

            builder.TextBody = $@"Dear {supervisorDelegate.FirstName},
                               This is a reminder sent by your {delegateSelfAssessment.RoleName}, {supervisorDelegate.SupervisorName} ({centreName}), to complete the profile assessment '{delegateSelfAssessment.RoleName}' in the NHS England, Digital Learning Solutions platform.
                               To access your {delegateSelfAssessment.RoleName} profile assessment, please visit {selfAssessmentUrl}.";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.FirstName}</p><p>This is a reminder sent by your {delegateSelfAssessment.RoleName}, {supervisorDelegate.SupervisorName} ({centreName}), to complete the profile assessment '{delegateSelfAssessment.RoleName}' in the NHS England, Digital Learning Solutions platform.</p><p><a href='{selfAssessmentUrl}'>Click here</a> to access your  {delegateSelfAssessment.RoleName} profile assessment.</p></body>";
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.DelegateEmail));
        }

        public void SendSupervisorMultipleResultsReviewed(int adminId, int supervisorDelegateId, int candidateAssessmentId, int countResults, int centreId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, 0);
            string centreName = GetCentreName(centreId);
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(candidateAssessmentId, supervisorDelegateId);
            var selfAssessmentUrl = GetSelfAssessmentUrl(delegateSelfAssessment.SelfAssessmentID);
            string emailSubjectLine = $"{delegateSelfAssessment.SupervisorRoleTitle} Confirmed {countResults} Results - Digital Learning Solutions";
            var builder = new BodyBuilder();
            builder.TextBody = $@"Dear {supervisorDelegate.FirstName},
                               {supervisorDelegate.SupervisorName} ({centreName}) has confirmed {countResults} of your self assessment results against the {delegateSelfAssessment.RoleName} profile assessment in the NHS England, Digital Learning Solutions platform.
                               To access your {delegateSelfAssessment.RoleName} profile assessment, please visit {selfAssessmentUrl}.";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.FirstName}</p><p> {supervisorDelegate.SupervisorName} ({centreName}) has confirmed {countResults} of your self assessment results against the {delegateSelfAssessment.RoleName} profile assessment in the NHS England, Digital Learning Solutions platform.</p><p><a href='{selfAssessmentUrl}'>Click here</a> to access your  {delegateSelfAssessment.RoleName} profile assessment.</p></body>";
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.DelegateEmail));
        }

        public void SendDelegateSupervisorNominated(int supervisorDelegateId, int selfAssessmentID, int delegateUserId, int centreId, int? selfAssessmentResultId = null)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, 0, delegateUserId);
            string centreName = GetCentreName(centreId);
            if (supervisorDelegate == null || supervisorDelegate.DelegateUserID == null || supervisorDelegate.SupervisorAdminID == null)
            {
                return;
            }
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentBySupervisorDelegateSelfAssessmentId(selfAssessmentID, supervisorDelegate.ID);
            string emailSubjectLine = $"{delegateSelfAssessment.SupervisorRoleTitle} Role Request - Digital Learning Solutions";
            var builder = new BodyBuilder();
            var profileReviewUrl = GetSupervisorProfileReviewUrl(supervisorDelegateId, delegateSelfAssessment.ID, selfAssessmentResultId);
            builder.TextBody = $@"Dear {supervisorDelegate.SupervisorName},
                              You have been identified by {supervisorDelegate.FirstName} {supervisorDelegate.LastName} ({supervisorDelegate.DelegateEmail}) ({centreName}) as their {delegateSelfAssessment.SupervisorRoleTitle} for the activity '{delegateSelfAssessment.RoleName}' in the NHS England, Digital Learning Solutions (DLS) platform.
                              To supervise this activity, please visit {profileReviewUrl} (sign in using your existing DLS credentials).";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.SupervisorName},</p><p>You have been identified by <a href='mailto:{supervisorDelegate.DelegateEmail}'>{supervisorDelegate.FirstName} {supervisorDelegate.LastName}  ({centreName}) </a> as their {delegateSelfAssessment.SupervisorRoleTitle} for the activity '{delegateSelfAssessment.RoleName}' in the NHS England, Digital Learning Solutions (DLS) platform.</p><p>You are already registered as a delegate at the supervisor's DLS centre. <a href='{profileReviewUrl}'>Click here</a> to supervise this activity (sign in using your existing DLS credentials).</p></body>";
            supervisorService.UpdateNotificationSent(supervisorDelegateId);
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.SupervisorEmail));
        }
        protected string GetSupervisorProfileReviewUrl(int supervisorDelegateId, int delegateSelfAssessmentId, int? selfAssessmentResultId = null)
        {
            var dlsUrlBuilder = GetDLSUriBuilder();
            dlsUrlBuilder.Path += $"Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{delegateSelfAssessmentId}/Review";
            if (selfAssessmentResultId.HasValue)
            {
                dlsUrlBuilder.Path += $"/{selfAssessmentResultId}";
            }
            return dlsUrlBuilder.Uri.ToString();
        }

        public void SendResultVerificationRequest(int candidateAssessmentSupervisorId, int selfAssessmentID, int resultCount, int delegateUserId, int centreId, int? selfAssessmentResultId)
        {
            var candidateAssessmentSupervisor = supervisorService.GetCandidateAssessmentSupervisorById(candidateAssessmentSupervisorId);
            int supervisorDelegateId = candidateAssessmentSupervisor.SupervisorDelegateId;
            int candidateAssessmentId = candidateAssessmentSupervisor.CandidateAssessmentID;
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, 0, delegateUserId);
            string centreName = GetCentreName(centreId);
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentBaseByCandidateAssessmentId(candidateAssessmentSupervisor.CandidateAssessmentID, 0);
            string emailSubjectLine = $"{delegateSelfAssessment.SupervisorRoleTitle} Self Assessment Results Review Request - Digital Learning Solutions";
            string? profileReviewUrl = GetSupervisorProfileReviewUrl(supervisorDelegateId, candidateAssessmentId, selfAssessmentResultId);
            BodyBuilder? builder = new BodyBuilder();
            builder.TextBody = $@"Dear {supervisorDelegate.SupervisorName},
                              {supervisorDelegate.FirstName} {supervisorDelegate.LastName} ({supervisorDelegate.DelegateEmail}) ({centreName}) has requested that you review {resultCount.ToString()} of their self assessment results for the activity '{delegateSelfAssessment.RoleName}' in the NHS England, Digital Learning Solutions (DLS) platform.
                              To review these results, please visit {profileReviewUrl} (sign in using your existing DLS credentials).";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.SupervisorName},</p><p><a href='mailto:{supervisorDelegate.DelegateEmail}'>{supervisorDelegate.FirstName} {supervisorDelegate.LastName} ({centreName})</a> has requested that you review {resultCount} of their self assessment results for the activity '{delegateSelfAssessment.RoleName}' in the NHS England, Digital Learning Solutions (DLS) platform.</p><p><a href='{profileReviewUrl}'>Click here</a> to review these results (sign in using your existing DLS credentials).</p></body>";
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.SupervisorEmail));
        }

        public void SendSignOffRequest(int candidateAssessmentSupervisorId, int selfAssessmentID, int delegateUserId, int centreId)
        {
            var candidateAssessmentSupervisor = supervisorService.GetCandidateAssessmentSupervisorById(candidateAssessmentSupervisorId);
            int supervisorDelegateId = candidateAssessmentSupervisor.SupervisorDelegateId;
            int candidateAssessmentId = candidateAssessmentSupervisor.CandidateAssessmentID;
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, 0, delegateUserId);
            string centreName = GetCentreName(centreId);
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentBaseByCandidateAssessmentId(candidateAssessmentSupervisor.CandidateAssessmentID, 0);
            string emailSubjectLine = $"{delegateSelfAssessment.SupervisorRoleTitle} Self Assessment Sign-off Request - Digital Learning Solutions";
            string? profileReviewUrl = GetSupervisorProfileReviewUrl(supervisorDelegateId, candidateAssessmentId);
            BodyBuilder? builder = new BodyBuilder();
            builder.TextBody = $@"Dear {supervisorDelegate.SupervisorName},
                              {supervisorDelegate.FirstName} {supervisorDelegate.LastName} ({supervisorDelegate.DelegateEmail}) ({centreName}) has requested that you sign-off of their self assessment the activity '{delegateSelfAssessment.RoleName}' in the NHS England, Digital Learning Solutions (DLS) platform.
                              To review and sign-off the self-assessment, please visit {profileReviewUrl} (sign in using your existing DLS credentials).";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.SupervisorName},</p><p><a href='mailto:{supervisorDelegate.DelegateEmail}'>{supervisorDelegate.FirstName} {supervisorDelegate.LastName} ({centreName})</a> has requested that you sign-off of their self assessment the activity '{delegateSelfAssessment.RoleName}' in the NHS England, Digital Learning Solutions (DLS) platform.</p><p><a href='{profileReviewUrl}'>Click here</a> to review and sign-off the self-assessment (sign in using your existing DLS credentials).</p></body>";
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.SupervisorEmail));
        }

        public void SendProfileAssessmentSignedOff(int supervisorDelegateId, int candidateAssessmentId, string? supervisorComments, bool signedOff, int adminId, int centreId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, 0);
            string centreName = GetCentreName(centreId);
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(candidateAssessmentId, supervisorDelegateId);
            var selfAssessmentUrl = GetSelfAssessmentUrl(delegateSelfAssessment.SelfAssessmentID);
            var commentString = supervisorDelegate.SupervisorName + (signedOff ? " signed off your profile assessment " : " rejected your profile assessment ") + (supervisorComments != null ? "and left the following review comment: " + supervisorComments : "but did not leave a review comment.");
            string emailSubjectLine = $"Profile assessment {(signedOff ? " signed off " : "rejected")} by {delegateSelfAssessment.SupervisorRoleTitle} - Digital Learning Solutions";
            var builder = new BodyBuilder();
            builder.TextBody = $@"Dear {supervisorDelegate.FirstName},
                               {supervisorDelegate.SupervisorName} ({centreName}) has reviewed your profile assessment {delegateSelfAssessment.RoleName} in the NHS England, Digital Learning Solutions platform.
                               {commentString}
                               To access your {delegateSelfAssessment.RoleName} profile assessment, please visit {selfAssessmentUrl}.";
            builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear {supervisorDelegate.FirstName}</p><p>{supervisorDelegate.SupervisorName} ({centreName}) has reviewed your profile assessment {delegateSelfAssessment.RoleName} in the NHS England, Digital Learning Solutions platform.</p><p>{commentString}</p><p><a href='{selfAssessmentUrl}'>Click here</a> to access your  {delegateSelfAssessment.RoleName} profile assessment.</p></body>";
            emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.DelegateEmail));
        }
        public void SendSupervisorDelegateReminder(int supervisorDelegateId, int adminId, int centreId)
        {
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, 0);
            string centreName = GetCentreName(centreId);
            string emailSubjectLine = "Registration reminder from supervisor - Digital Learning Solutions";
            var builder = new BodyBuilder();
            var dlsUrlBuilder = GetDLSUriBuilder();
            if (supervisorDelegate.DelegateUserID == null)
            {
                dlsUrlBuilder.Path += "Register";
                dlsUrlBuilder.Query = $"centreid={supervisorDelegate.CentreId}&inviteid={supervisorDelegate.InviteHash}";
                builder.TextBody = $@"Dear colleague,
                              This is a reminder to to register to access the NHS England, Digital Learning Solutions platform as a supervised delegate by {supervisorDelegate.SupervisorName} ({supervisorDelegate.SupervisorEmail}) ({centreName}).
                              To register, visit {dlsUrlBuilder.Uri.ToString()}.
                              Your supervisor will then be able to assign role profile assessments and view and validate your self assessment results.";
                builder.HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear colleague,</p><p>This is a reminder to register to access the NHS England, Digital Learning Solutions platform as a supervised delegate by <a href='mailto:{supervisorDelegate.SupervisorEmail}'>{supervisorDelegate.SupervisorName} ({centreName})</a>.</p><p><a href='{dlsUrlBuilder.Uri.ToString()}'>Click here</a> to register. </p><p>Your supervisor will then be able to assign role profile assessments and view and validate your self assessment results.</p></body>";
                emailService.SendEmail(new Email(emailSubjectLine, builder, supervisorDelegate.DelegateEmail));
            }
        }
        public string GetCentreName(int centreId)
        {
            return centresDataService.GetCentreName(centreId);
        }
    }
}
