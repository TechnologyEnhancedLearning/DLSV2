namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Data.Models.SessionData.Supervisor;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System.Collections.Generic;
    public class EnrolDelegateOnProfileAssessmentViewModel
    {
        public SessionEnrolOnCompetencyAssessment SessionEnrolOnCompetencyAssessment { get; set; }
        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public IEnumerable<CompetencyAssessment> CompetencyAssessments { get; set; }
    }
}
