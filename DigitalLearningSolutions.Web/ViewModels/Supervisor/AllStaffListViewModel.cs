namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System.Collections.Generic;
    using System.Linq;

    public class AllStaffListViewModel
    {
        public readonly IEnumerable<SupervisorDelegateDetailViewModel> SupervisorDelegateDetailViewModels;
        public readonly CentreRegistrationPrompts CentreRegistrationPrompts;

        public AllStaffListViewModel(
            IEnumerable<SupervisorDelegateDetail> supervisorDelegates,
            CentreRegistrationPrompts centreRegistrationPrompts
        )
        {
            SupervisorDelegateDetailViewModels =
                supervisorDelegates.Select(supervisor => new SupervisorDelegateDetailViewModel(supervisor, 1));
            CentreRegistrationPrompts = centreRegistrationPrompts;
        }
    }
}
