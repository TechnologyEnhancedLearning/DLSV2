namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    public class CompetencyAssessmentCollaboratorNotification : CompetencyAssessmentCollaboratorDetail
    {
        public string InvitedByEmail { get; set; } = string.Empty;
        public string InvitedByName { get; set; } = string.Empty;
        public string CompetencyAssessmentName { get; set; } = string.Empty;
    }
}
