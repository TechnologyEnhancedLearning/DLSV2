namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System.Collections.Generic;
    using System.Linq;

    public class AllStaffListViewModel
    {
        public readonly CentreRegistrationPrompts CentreRegistrationPrompts;
        public readonly IEnumerable<SupervisorDelegateDetailViewModel> SupervisorDelegateDetailViewModels;
        public readonly SupervisorDelegateDetailViewModel SelfSupervisorDelegateDetailViewModels;

        public AllStaffListViewModel(
            IEnumerable<SupervisorDelegateDetail> supervisorDelegates,
            CentreRegistrationPrompts centreRegistrationPrompts,
            bool isSupervisor, int? loggedInUserId = 0
        )
        {
            SupervisorDelegateDetailViewModels =
                supervisorDelegates.Where(x => x.DelegateUserID != loggedInUserId).Select(
                    supervisor => new SupervisorDelegateDetailViewModel(
                        supervisor,
                        new ReturnPageQuery(
                            1,
                            $"{supervisor.ID}-card",
                            PaginationOptions.DefaultItemsPerPage
                        ),
                        isSupervisor,
                        loggedInUserId
                    )
                );
            SelfSupervisorDelegateDetailViewModels =
                supervisorDelegates.Where(x => x.DelegateUserID == loggedInUserId).Select(
                    supervisor => new SupervisorDelegateDetailViewModel(
                        supervisor,
                        new ReturnPageQuery(
                            1,
                            $"{supervisor.ID}-card",
                            PaginationOptions.DefaultItemsPerPage
                        ),
                        isSupervisor,
                        loggedInUserId
                    )
                ).FirstOrDefault();
            CentreRegistrationPrompts = centreRegistrationPrompts;
        }
    }
}
