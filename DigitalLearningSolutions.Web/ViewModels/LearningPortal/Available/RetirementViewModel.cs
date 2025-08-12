using DigitalLearningSolutions.Web.Attributes;
using System;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
  
    public class RetirementViewModel
    {
        public RetirementViewModel()
        {

        }
        public RetirementViewModel(int selfAssessmentId, DateTime? retirementDate, string name)
        {
            SelfAssessmentId = selfAssessmentId;
            RetirementDate = retirementDate;
            Name = name;
        }
        public string Name { get; set; } = string.Empty;
        public int SelfAssessmentId { get; set; }
        public DateTime? RetirementDate { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "Please tick the checkbox to confirm you wish to perform this action")]
        public bool ActionConfirmed { get; set; }
    }
}
