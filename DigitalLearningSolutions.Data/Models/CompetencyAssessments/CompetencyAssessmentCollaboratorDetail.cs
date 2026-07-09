using System;

namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    public class CompetencyAssessmentCollaboratorDetail : CompetencyAssessmentCollaborator
    {
        public string? UserEmail { get; set; }
        public bool? UserActive { get; set; }
        public string? CompetencyAssessmentRole { get; set; }
        public int? SelfAssessmentReviewID { get; set; }
        public bool SignOffRequired { get; set; }
        public DateTime? ReviewRequested { get; set; }
        public DateTime? ReviewComplete { get; set; }
    }
}
