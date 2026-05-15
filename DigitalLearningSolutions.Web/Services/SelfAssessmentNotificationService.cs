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
    }

    public class SelfAssessmentNotificationService : ISelfAssessmentNotificationService
    {

        private readonly IConfigDataService configDataService;
        private readonly IEmailService emailService;
        private readonly ICompetencyAssessmentService competencyAssessmentService;
        public SelfAssessmentNotificationService(
           ICompetencyAssessmentService competencyAssessmentService,
           IConfigDataService configDataService,
           IEmailService emailService
       )
        {
            this.competencyAssessmentService = competencyAssessmentService;
            this.configDataService = configDataService;
            this.emailService = emailService;
        }
        public void SendCompetencyAssessmentCollaboratorInvite(int id, int invitedByAdminId)
        {
            var collaboratorNotification = competencyAssessmentService.GetCollaboratorNotification(id, invitedByAdminId);
            if (collaboratorNotification == null)
            {
                throw new NotificationDataException($"No record found when trying to fetch collaboratorNotification Data. id: {id}, invitedByAdminId: {invitedByAdminId})");
            }

            var competencyAssessmentUrl = GetCompetencyAssessmentkUrl(collaboratorNotification.SelfAssessmentID, "Manage");
            string emailSubjectLine = $"Competency assessment {collaboratorNotification.CompetencyAssessmentRole} Invitation - Digital Learning Solutions";

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

    }
}
