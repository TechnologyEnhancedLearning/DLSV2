namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    public class ReviewCompetencySelfAsessmentViewModel
    {
        public DelegateSelfAssessment DelegateSelfAssessment { get; set; }
        public SupervisorDelegate SupervisorDelegate { get; set; }
        public Competency Competency { get; set; }
    }
}
