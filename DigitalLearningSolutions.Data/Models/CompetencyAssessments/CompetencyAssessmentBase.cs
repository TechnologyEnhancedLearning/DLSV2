namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    using System.ComponentModel.DataAnnotations;
    public class CompetencyAssessmentBase
    {
        public int ID { get; set; }
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int BrandID { get; set; }
        public int CategoryID { get; set; }
        public int? ParentCompetencyAssessmentID { get; set; }
        public bool National { get; set; }
        public bool Public { get; set; }
        public int OwnerAdminID { get; set; }
        public int? NRPProfessionalGroupID { get; set; }
        public int? NRPSubGroupID { get; set; }
        public int? NRPRoleID { get; set; }
        public int PublishStatusID { get; set; }
        public int UserRole { get; set; }
        public string? Vocabulary { get; set; }
        public bool EnforceRoleRequirementsForSignOff { get; set; }
        public bool IncludeRequirementsFilters { get; set; }
        public int? MinimumOptionalCompetencies { get; set; }
        public string? ManageOptionalCompetenciesPrompt { get; set; }
        public bool IncludeLearnerDeclarationPrompt { get; set; }
        public bool IncludesSignposting { get; set; }
        public bool LinearNavigation { get; set; }
        public bool UseDescriptionExpanders { get; set; }
        public string? QuestionLabel { get; set; }
        public string? ReviewerCommentsLabel { get; set; }
        public bool SupervisorSelfAssessmentReview { get; set; }
        public bool SupervisorResultsReview { get; set; }
        public string? SignOffRequestorStatement { get; set; }
        public string? SignOffSupervisorStatement { get; set; }
    }
}

