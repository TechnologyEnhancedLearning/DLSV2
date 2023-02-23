namespace DigitalLearningSolutions.Data.Models.SelfAssessments.Export
{
    using System;
    public class DCSAOutcomeSummary
    {
        public int? EnrolledMonth { get; set; }
        public int? EnrolledYear { get; set; }
        public string? JobGroup { get; set; }
        public string? CentreField1 { get; set; }
        public string? CentreField2 { get; set; }
        public string? CentreField3 { get; set; }
        public string? Status { get; set; }
        public int? LearningLaunched { get; set; }
        public int? LearningCompleted { get; set; }
        public int? DataInformationAndContentConfidence { get; set; }
        public int? DataInformationAndContentRelevance { get; set; }
        public int? TeachinglearningAndSelfDevelopmentConfidence { get; set; }
        public int? TeachinglearningAndSelfDevelopmentRelevance { get; set; }
        public int? CommunicationCollaborationAndParticipationConfidence { get; set; }
        public int? CommunicationCollaborationAndParticipationRelevance { get; set; }
        public int? TechnicalProficiencyConfidence { get; set; }
        public int? TechnicalProficiencyRelevance { get; set; }
        public int? CreationInnovationAndResearchConfidence { get; set; }
        public int? CreationInnovationAndResearchRelevance { get; set; }
        public int? DigitalIdentityWellbeingSafetyAndSecurityConfidence { get; set; }
        public int? DigitalIdentityWellbeingSafetyAndSecurityRelevance { get; set; }
    }
}
