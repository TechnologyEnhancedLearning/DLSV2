namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Web.Attributes;
    using System;

    public class RetiringSelfAssessmentViewModel
    {
        public int SelfAssessmentID { get; set; }
        public int SupervisorDelegateID { get; set; }
        public DateTime? RetirementDate { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "Please tick the checkbox to confirm you wish to perform this action")]
        public bool ActionConfirmed { get; set; }
    }
}
