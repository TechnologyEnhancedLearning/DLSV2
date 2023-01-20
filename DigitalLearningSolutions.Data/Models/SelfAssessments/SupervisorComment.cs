using System;

namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class SupervisorComment
    {
        public int? AssessmentQuestionID { get; set; }
        public string? Name { get; set; }
        public string? SupervisorName { get; set; }
        public string? RoleName { get; set; }
        public string? Comments { get; set; }
        public int? DelegateUserID { get; set; }
        public int? CompetencyID { get; set; }
        public string? CompetencyName { get; set; }
        public int? SelfAssessmentID { get; set; }
        public int? CandidateAssessmentSupervisorID { get; set; }
        public int? SelfAssessmentResultId { get; set; }
        public DateTime? Verified { get; set; }
        public int? CompetencyGroupID { get; set; }
        public string? Vocabulary { get; set; }
        public bool SignedOff { get; set; }
        public string? ReviewerCommentsLabel { get; set; }
    }
}
