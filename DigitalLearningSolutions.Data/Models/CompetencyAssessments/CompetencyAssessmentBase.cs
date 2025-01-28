namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class CompetencyAssessmentBase
    {
        public int ID { get; set; }
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int BrandID { get; set; }
        public int? ParentCompetencyAssessmentID { get; set; }
        public bool National { get; set; }
        public bool Public { get; set; }
        public int OwnerAdminID { get; set; }
        public int? NRPProfessionalGroupID { get; set; }
        public int? NRPSubGroupID { get; set; }
        public int? NRPRoleID { get; set; }
        public int PublishStatusID { get; set; }
        public int UserRole { get; set; }

    }
}

