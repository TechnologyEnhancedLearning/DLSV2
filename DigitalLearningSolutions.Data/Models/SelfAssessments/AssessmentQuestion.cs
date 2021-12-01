namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using System;
    using System.Collections.Generic;

    public class AssessmentQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string? MaxValueDescription { get; set; }
        public string? MinValueDescription { get; set; }
        public int? ResultId { get; set; }
        public int? Result { get; set; }
        public string? ScoringInstructions { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int AssessmentQuestionInputTypeID { get; set; }
        public bool IncludeComments { get; set; }
        public IEnumerable<LevelDescriptor> LevelDescriptors {get; set;}
        public string? SupportingComments { get; set; }
        public int? SelfAssessmentResultSupervisorVerificationId { get; set; }
        public DateTime? Requested { get; set; }
        public DateTime? Verified { get; set; }
        public string? SupervisorComments { get; set; }
        public bool? SignedOff { get; set; }
        public bool? UserIsVerifier { get; set; }
        public int ResultRAG { get; set; }
        public string? CommentsPrompt { get; set; }
        public string? CommentsHint { get; set; }
        public bool Required { get; set; }
    }
}
