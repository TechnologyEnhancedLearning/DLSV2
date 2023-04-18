namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using static DigitalLearningSolutions.Web.Helpers.RequiredWhenValidationHelper;

    public class SignOffProfileAssessmentViewModel
    {
        public SelfAssessmentResultSummary? SelfAssessmentResultSummary { get; set; }
        public SupervisorDelegate? SupervisorDelegate { get; set; }

        public IEnumerable<CandidateAssessmentSupervisorVerificationSummary>? CandidateAssessmentSupervisorVerificationSummaries { get; set; }
        public int? CandidateAssessmentSupervisorVerificationId { get; set; }
        [MaxLength(1500)]
        [RequiredWhen("SignedOff", false, AllowEmptyStrings = false, ErrorMessage = "Comments are required when rejecting a self assessment (when Sign-off is unchecked).")]
        public string? SupervisorComments { get; set; }
        public bool SignedOff { get; set; }
    }
}
