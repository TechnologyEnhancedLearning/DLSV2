namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ReviewCompetencySelfAsessmentViewModel
    {
        public DelegateSelfAssessment DelegateSelfAssessment { get; set; }
        public SupervisorDelegate SupervisorDelegate { get; set; }
        public Competency Competency { get; set; }
        public int? ResultSupervisorVerificationId { get; set; }
        public string? SupervisorComments { get; set; }
        [Required(ErrorMessage = "Comments are Required")]
        public bool SignedOff { get; set; }
        [Required(ErrorMessage = "Required")]
        public string SupervisorSignedOff { get; set; }
        public string Status { get; set; }
        public DateTime? Verified { get; set; }
        public string SupervisorName { get; set; }
    }
}
