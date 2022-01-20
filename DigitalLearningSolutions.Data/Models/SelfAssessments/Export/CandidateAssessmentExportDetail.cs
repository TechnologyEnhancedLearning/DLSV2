namespace DigitalLearningSolutions.Data.Models.SelfAssessments.Export
{
    using System;
    using System.ComponentModel;
    public class CandidateAssessmentExportDetail
    {
        public string? Group { get; set; }
        public string? Competency { get; set; }
        public string? Description { get; set; }
        [DisplayName("Competency Optional")]
        public bool CompetencyOptional { get; set; }
        [DisplayName("Assessment Question")]
        public string? AssessmentQuestion { get; set; }
        [DisplayName("Self Assessment Required")]
        public bool SelfAssessmentRequired { get; set; }
        [DisplayName("Self Assessment Result")]
        public string? SelfAssessmentResult { get; set; }
        [DisplayName("Self Assessment Comments")]
        public string? SelfAssessmentComments { get; set; }
        public string? Reviewer { get; set; }
        [DisplayName("Reviewer PRN")]
        public string? ReviewerPrn { get; set; }
        public DateTime? Reviewed { get; set; }
        [DisplayName("Reviewer Comments")]
        public string? ReviewerComments { get; set; }
        [DisplayName("Reviewer Verified")]
        public bool ReviewerVerified { get; set; }
        [DisplayName("Role Requirements")]
        public string? RoleRequirements { get; set; }
    }
}
