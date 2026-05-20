namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using DigitalLearningSolutions.Data.Constants;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using MimeKit;

    public interface ISelfAssessmentNotificationService
    {
        void SendCompetencyAssessmentCollaboratorInvite(int id, int invitedByAdminId);
        void SendReviewRequestForSelfAssessment(int id, int invitedByAdminId, bool required, bool reminder, int centreId);
        void SendSelfAssessmentsReviewOutcomeNotification(int reviewId, int centreId);
    }

    public class SelfAssessmentNotificationService : ISelfAssessmentNotificationService
    {

        private readonly IConfigDataService configDataService;
        private readonly IEmailService emailService;
        private readonly ICompetencyAssessmentService competencyAssessmentService;
        private readonly ICentresDataService centresDataService;

        public SelfAssessmentNotificationService(
           ICompetencyAssessmentService competencyAssessmentService,
           IConfigDataService configDataService,
           IEmailService emailService,
           ICentresDataService centresDataService
       )
        {
            this.competencyAssessmentService = competencyAssessmentService;
            this.configDataService = configDataService;
            this.emailService = emailService;
            this.centresDataService = centresDataService;
        }
        public void SendCompetencyAssessmentCollaboratorInvite(int id, int invitedByAdminId)
        {
            var collaboratorNotification = competencyAssessmentService.GetCollaboratorNotification(id, invitedByAdminId);
            if (collaboratorNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch collaboratorNotification Data. id: {id}, invitedByAdminId: {invitedByAdminId})");
            }

            var competencyAssessmentUrl = GetCompetencyAssessmentkUrl(collaboratorNotification.SelfAssessmentID, "Manage");
            string emailSubjectLine = $"Self-assessment {collaboratorNotification.CompetencyAssessmentRole} Invitation - Digital Learning Solutions";

            var builder = new BodyBuilder
            {
                TextBody = $@"Dear colleague,
                               You have been identified as a {collaboratorNotification.CompetencyAssessmentRole} for {collaboratorNotification.CompetencyAssessmentName} by {collaboratorNotification.InvitedByName} ({collaboratorNotification.InvitedByEmail}).
                              To access the assessment, visit this url: {competencyAssessmentUrl}. You must be registered on the Digital Learning Solutions platform to view the self-assessment.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear colleague,</p>
                                  <p>You have been identified as a {collaboratorNotification.CompetencyAssessmentRole} for {collaboratorNotification.CompetencyAssessmentName}, by <a href='mailto:{collaboratorNotification.InvitedByEmail}'>{collaboratorNotification.InvitedByName}</a>.</p>
                                <p><a href='{competencyAssessmentUrl}'>Use this link</a> to access the self-assessment. You must be registered on the Digital Learning Solutions platform to view the self-assessment.</p>
                            </body>",
            };
            emailService.SendEmail(new Email(emailSubjectLine, builder, collaboratorNotification.UserEmail, collaboratorNotification.InvitedByEmail));
        }
        public void SendReviewRequestForSelfAssessment(int id, int invitedByAdminId, bool required, bool reminder, int centreId)
        {
            string centreName = GetCentreName(centreId);
            var collaboratorNotification = this.competencyAssessmentService.GetCollaboratorNotification(id, invitedByAdminId);
            if (collaboratorNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch collaboratorNotification Data. id: {id}, invitedByAdminId: {invitedByAdminId}");
            }
            var competencyAssessmentUrl = GetSelfAssessmentUrl(collaboratorNotification.SelfAssessmentID, "Review", collaboratorNotification.SelfAssessmentReviewID);
            string emailSubjectLine = (reminder ? " REMINDER: " : "") + "Self-assessment Review Request - Digital Learning Solutions";
            string signOffRequired = required ? "You are required to sign-off this self-assessment before it can be published." : "You are not required to sign-off this self-assessment before it is published.";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear colleague,
                              You have been requested to review the self-assessment, {collaboratorNotification?.CompetencyAssessmentName}, by {collaboratorNotification?.InvitedByName} ({collaboratorNotification?.InvitedByEmail}) ({centreName}).
                              To review the self-assessment, visit this url: {competencyAssessmentUrl}. Click the Review self-assessment button to submit your review and, if appropriate, sign-off the self-assessment. {signOffRequired}. You will need to be registered on the Digital Learning Solutions platform to review the self-assessment.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'><p>Dear colleague,</p><p>You have been requested to review the self-assessment, {collaboratorNotification?.CompetencyAssessmentName}, by <a href='mailto:{collaboratorNotification?.InvitedByEmail}'>{collaboratorNotification?.InvitedByName} ({centreName})</a>.</p><p><a href='{competencyAssessmentUrl}'>Click here</a> to review the self-assessment. Click the Review self-assessment button to submit your review and, if appropriate, sign-off the self-assessment.</p><p>{signOffRequired}</p><p>You will need to be registered on the Digital Learning Solutions platform to view the self-assessment.</p></body>"
            };
            emailService.SendEmail(new Email(emailSubjectLine, builder, collaboratorNotification.UserEmail, collaboratorNotification.InvitedByEmail));
        }
        public void SendSelfAssessmentsReviewOutcomeNotification(int reviewId, int centreId)
        {
            string centreName = GetCentreName(centreId);
            var outcomeNotification = this.competencyAssessmentService.GetSelfAssessmentReviewNotification(reviewId);
            if (outcomeNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch review outcome Data. reviewId: {reviewId}");
            }
            var competencyAssessmentUrl = GetSelfAssessmentUrl(outcomeNotification.SelfAssessmentID, "PublishReview");
            string emailSubjectLine = $"Self-assessment Review Outcome - {(outcomeNotification.SignedOff ? "Approved" : "Rejected")} - Digital Learning Solutions";
            string approvalStatus = outcomeNotification.ReviewerFirstName + (outcomeNotification.SignedOff ? " approved the self-assessment for publishing." : " did not approve the self-assessment for publishing.");
            string commentsText = outcomeNotification.ReviewerFirstName + (outcomeNotification.Comment != null ? " left the following review comment: " + outcomeNotification.Comment : " did not leave a review comment.");
            string commentsHtml = "<p>" + outcomeNotification.ReviewerFirstName + (outcomeNotification.Comment != null ? " left the following review comment:</p><hr/><p>" + outcomeNotification.Comment + "</p><hr/>" : " did not leave a review comment.</p>");
            string reviewerFullName = $"{outcomeNotification.ReviewerFirstName} {outcomeNotification.ReviewerLastName} {(outcomeNotification.ReviewerActive == true ? "" : " (inactive)")}";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {outcomeNotification.OwnerFirstName},
                              Your self-assessment, {outcomeNotification.SelfAssessmentName}, has been reviewed by {reviewerFullName} ({outcomeNotification.UserEmail}) ({centreName}).
                              {approvalStatus}
                              {commentsText}
                              The full self-assessment review status, can be viewed by visiting: {competencyAssessmentUrl}. Once all of the required reviewers have approved the self-assessment, you may publish it. You will need to login to the Digital Learning Solutions platform to access the self-assessment.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                <p>Dear {outcomeNotification.OwnerFirstName},</p>
                                <p>Your self-assessment, {outcomeNotification.SelfAssessmentName}, has been reviewed by <a href='mailto:{outcomeNotification.UserEmail}'>{reviewerFullName} ({centreName})</a>.</p>
                                <p>{approvalStatus}</p>
                                {commentsHtml}
                                <p><a href='{competencyAssessmentUrl}'>Click here</a> to view the full review status for the self-assessment. Once all of the required reviewers have approved the self-assessment, you may publish it.</p>
                                <p>You will need to login to the Digital Learning Solutions platform to access the self-assessment.</p>
                            </body>",
            };
            emailService.SendEmail(new Email(emailSubjectLine, builder, outcomeNotification.OwnerEmail, outcomeNotification.UserEmail));
        }

        public string GetSelfAssessmentUrl(int selfAssessmentID, string actionName, int? id = null)
        {
            var selfAssessmentUrl = GetDLSUriBuilder();

            selfAssessmentUrl.Path += id != null
                        ? $"Self-Assessment/{selfAssessmentID}/{id}/{actionName}"
                        : $"Self-Assessment/{selfAssessmentID}/{actionName}";
            return selfAssessmentUrl.Uri.ToString();
        }
        public string GetCompetencyAssessmentkUrl(int competencyAssessmentID, string tab)
        {
            var competencyAssessmentUrl = GetDLSUriBuilder();
            competencyAssessmentUrl.Path += $"CompetencyAssessments/{competencyAssessmentID}/{tab}/";
            return competencyAssessmentUrl.Uri.ToString();
        }
        public UriBuilder GetDLSUriBuilder()
        {
            var trackingSystemBaseUrl = configDataService.GetConfigValue(ConfigConstants.AppBaseUrl) ??
                                       throw new ConfigValueMissingException(configDataService.GetConfigValueMissingExceptionMessage("AppBaseUrl"));
            return new UriBuilder(trackingSystemBaseUrl);
        }
        public string GetCentreName(int centreId)
        {
            return centresDataService.GetCentreName(centreId);
        }
    }
}
