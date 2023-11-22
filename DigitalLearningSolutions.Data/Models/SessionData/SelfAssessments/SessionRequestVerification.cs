﻿namespace DigitalLearningSolutions.Data.Models.SessionData.SelfAssessments
{
    using System;
    using System.Collections.Generic;

    public class SessionRequestVerification
    {
        public int SelfAssessmentID { get; set; }
        public string SelfAssessmentName { get; set; } = string.Empty;
        public string Vocabulary { get; set; } = string.Empty;
        public int CandidateAssessmentSupervisorId { get; set; }
        public List<int>? ResultIds { get; set; }
        public bool SupervisorSelfAssessmentReview { get; set; }
    }
}
