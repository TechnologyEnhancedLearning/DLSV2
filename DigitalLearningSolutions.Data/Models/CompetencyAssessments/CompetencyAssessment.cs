namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    using System;
    public class CompetencyAssessment : CompetencyAssessmentBase
    {
        public DateTime CreatedDate { get; set; }
        public string? Brand
        {
            get => brand;
            set => brand = GetValidOrNull(value);
        }
        public string? Category
        {
            get => category;
            set => category = GetValidOrNull(value);
        }
        public string? ParentCompetencyAssessment { get; set; }
        public string? Owner { get; set; }
        public DateTime? Archived { get; set; }
        public DateTime LastEdit { get; set; }
        public string? LinkedFrameworks { get; set; }
        public string? NRPProfessionalGroup { get; set; }
        public string? NRPSubGroup { get; set; }
        public string? NRPRole { get; set; }
        public int? CompetencyAssessmentReviewID { get; set; }
        private string? brand;
        private string? category;
        private static string? GetValidOrNull(string? toValidate)
        {
            return toValidate != null && toValidate.ToLower() == "undefined" ? null : toValidate;
        }
    }
}
