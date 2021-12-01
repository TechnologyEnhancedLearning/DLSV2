namespace DigitalLearningSolutions.Data.Models.SessionData.SelfAssessments
{
    using System;
    using System.Collections.Generic;

    public class SessionRequestVerification
    {
        public SessionRequestVerification()
        {
            Id = new Guid();
        }
        public Guid Id { get; set; }
        public int SelfAssessmentID { get; set; }
        public string SelfAssessmentName { get; set; }
        public string Vocabulary { get; set; }
        public int CandidateAssessmentSupervisorId { get; set; }
        public List<int>? ResultIds { get; set; }
        public bool SupervisorSelfAssessmentReview { get; set; }
    }
}
