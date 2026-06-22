using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Attributes;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class ConfirmClearRetirementDateViewModel
    {
        public int CompetencyAssessmentId { get; set; }
        public string CompetencyAssessmentName { get; set; }
        public ConfirmClearRetirementDateViewModel(CompetencyAssessmentBase? assessment)
        {   
            CompetencyAssessmentId = assessment.ID;
            CompetencyAssessmentName = assessment.CompetencyAssessmentName;
        }
        [BooleanMustBeTrue(ErrorMessage = "You must confirm before removing retirement date")]
        public bool Confirm { get; set; }
        public ConfirmClearRetirementDateViewModel()
        {
        }
    }
}
