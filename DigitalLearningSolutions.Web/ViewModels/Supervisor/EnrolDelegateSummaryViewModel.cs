namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System;

    public class EnrolDelegateSummaryViewModel
    {
        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public CompetencyAssessment CompetencyAssessment { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public string SupervisorRoleName { get; set; }
        public int SupervisorRoleCount { get; set; }
        public bool AllowSupervisorRoleSelection { get; set; }
    }
}
