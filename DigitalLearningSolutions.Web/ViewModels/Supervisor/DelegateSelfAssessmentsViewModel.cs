namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System.Collections.Generic;

    public class DelegateSelfAssessmentsViewModel
    {
        public bool IsNominatedSupervisor { get; set; }
        public SupervisorDelegateDetail SupervisorDelegateDetail { get; set; }
        public IEnumerable<DelegateSelfAssessment> DelegateSelfAssessments { get; set; }
    }
}
