namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    public class CompetencyAssessmentCollaboratorDetail : CompetencyAssessmentCollaborator
    {
        public string? UserEmail { get; set; }
        public bool? UserActive { get; set; }
        public string? CompetencyAssessmentRole { get; set; }
    }
}
