namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System.Collections.Generic;

    public class AllStaffListViewModel
    {
        public readonly IEnumerable<SupervisorDelegateDetail> SupervisorDelegateDetails;
        public readonly CentreCustomPrompts CentreCustomPrompts;

        public AllStaffListViewModel(IEnumerable<SupervisorDelegateDetail> supervisorDelegates, CentreCustomPrompts centreCustomPrompts)
        {
            SupervisorDelegateDetails = supervisorDelegates;
            CentreCustomPrompts = centreCustomPrompts;
        }
    }
}
