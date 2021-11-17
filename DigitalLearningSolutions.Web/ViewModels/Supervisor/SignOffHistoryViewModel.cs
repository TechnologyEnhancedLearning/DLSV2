namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using System.Collections.Generic;

    public class SignOffHistoryViewModel
    {
        public SupervisorDelegateDetail? SupervisorDelegateDetail { get; set; }
        public DelegateSelfAssessment? DelegateSelfAssessment { get; set; }
        public IEnumerable<SupervisorSignOff>? SupervisorSignOffs { get; set; }
    }
}
