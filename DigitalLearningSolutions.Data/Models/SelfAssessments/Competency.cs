﻿namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System;
    using System.Collections.Generic;

    public class Competency
    {
        public int Id { get; set; }
        public int RowNo { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? QuestionLabel { get; set; }
        public string CompetencyGroup { get; set; } = string.Empty;
        public int CompetencyGroupID { get; set; }
        public string CompetencyGroupDescription { get; set; } = string.Empty;
        public string? Vocabulary { get; set; }
        public bool Optional { get; set; }
        public bool AlwaysShowDescription { get; set; }
        public bool IncludedInSelfAssessment { get; set; }
        public DateTime? Verified { get; set; }
        public DateTime? Requested { get; set; }
        public DateTime? EmailSent { get; set; }
        public bool? SignedOff { get; set; }
        public DateTime? SupervisorVerificationRequested { get; set; }
        public int? SupervisorVerificationId { get; set; }
        public int? CandidateAssessmentSupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public string? CentreName { get; set; }
        public int? SelfAssessmentStructureId { get; set; }
        public List<AssessmentQuestion> AssessmentQuestions { get; set; } = new List<AssessmentQuestion>();
        public IEnumerable<CompetencyFlag> CompetencyFlags { get; set; } = new List<CompetencyFlag>();
    }
}
