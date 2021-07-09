namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using System;

    public class ReviewedCompetency : Competency
    {
        public int? SelfAssessmentResultSupervisorVerificationId { get; set; }
        public DateTime? Requested { get; set; }
        public DateTime? Verified { get; set; }
        public string? SupervisorComments { get; set; }
        public bool? SignedOff { get; set; }
    }
}
