namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Helpers;
    using System;

    public class EnrolDelegateSetCompletByDateViewModel
    {
        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public CompetencyAssessment CompetencyAssessment { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public OldDateValidator.ValidationResult? CompleteByValidationResult { get; set; }
    }
}
