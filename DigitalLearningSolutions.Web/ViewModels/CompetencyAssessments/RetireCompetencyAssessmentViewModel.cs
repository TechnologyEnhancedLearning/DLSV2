using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.ComponentModel.DataAnnotations;
using DigitalLearningSolutions.Data.Models.CompetencyAssessments;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class RetireCompetencyAssessmentViewModel
    {
        public int CompetencyAssessmentId { get; set; }

        [Required(ErrorMessage = "Please enter a reason for retirement")]
        [StringLength(2000, ErrorMessage = "Reason must be 2000 characters or fewer")]
        public string? RetirementReason { get; set; }

        public DateTime? RetirementDate { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public RetireCompetencyAssessmentViewModel(int competencyAssessmentId, CompetencyAssessmentBase? assessment)
        {
            if (assessment?.RetirementDate != null)
            {
                Day = assessment.RetirementDate.Value.Day;
                Month = assessment.RetirementDate.Value.Month;
                Year = assessment.RetirementDate.Value.Year;
            }
            RetirementReason = assessment.RetirementReason;
            CompetencyAssessmentId = competencyAssessmentId;
        }
        public RetireCompetencyAssessmentViewModel()
        {
        }
    }
}
