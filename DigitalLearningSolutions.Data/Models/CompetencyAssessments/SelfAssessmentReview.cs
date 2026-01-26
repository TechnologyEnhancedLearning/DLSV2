using System;


namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    public  class SelfAssessmentReview
    {
        public int ID { get; set; }
        public int SelfAssessmentID { get; set; }
        public int SelfAssessmentCollaboratorID { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public bool IsRegistered { get; set; }
        public DateTime ReviewRequested { get; set; }
        public DateTime? ReviewComplete { get; set; }
        public bool SignedOff { get; set; }
        public int? SelfAssessmentCommentID { get; set; }
        public string? Comment { get; set; }
        public bool SignOffRequired { get; set; }
    }
}
