using System;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SupervisorComments
{
    public class SupervisorCommentsViewModel
    {
        public int? AssessmentQuestionID { get; set; }
        public string? Name { get; set; }
        public string? Comments { get; set; }
        public int? CandidateID { get; set; }
        public int? CompetencyID { get; set; }
        public string? CompetencyName { get; set; }
        public int? SelfAssessmentID { get; set; }
        public int? CandidateAssessmentSupervisorID { get; set; }
        public int? SelfAssessmentResultId { get; set; }
        public DateTime? Verified { get; set; }
        public int? CompetencyGroupID { get; set; }

    }
}
