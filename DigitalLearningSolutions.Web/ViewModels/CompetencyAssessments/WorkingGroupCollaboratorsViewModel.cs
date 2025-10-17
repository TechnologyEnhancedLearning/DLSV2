namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using System.Collections.Generic;

    public class WorkingGroupCollaboratorsViewModel
    {
        public int CompetencyAssessmentID { get; set; }
        public IEnumerable<CompetencyAssessmentCollaboratorDetail>? Collaborators { get; set; }
        public int AdminID { get; set; }
        public bool? CompetencyAssessmentTaskStatus { get; set; }
        public string? UserEmail { get; set; }
        public bool Error { get; set; }
    }
}
