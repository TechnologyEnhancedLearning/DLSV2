﻿namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.Supervisor;

    public class AllStaffListViewModel
    {
        public readonly CentreRegistrationPrompts CentreRegistrationPrompts;
        public readonly IEnumerable<SupervisorDelegateDetailViewModel> SupervisorDelegateDetailViewModels;

        public AllStaffListViewModel(
            IEnumerable<SupervisorDelegateDetail> supervisorDelegates,
            CentreRegistrationPrompts centreRegistrationPrompts,
            bool isSupervisor
        )
        {
            SupervisorDelegateDetailViewModels =
                supervisorDelegates.Select(
                    supervisor => new SupervisorDelegateDetailViewModel(
                        supervisor,
                        new ReturnPageQuery(
                            1,
                            $"{supervisor.ID}-card",
                            PaginationOptions.DefaultItemsPerPage
                        ) ,
                        isSupervisor
                    )
                );
            CentreRegistrationPrompts = centreRegistrationPrompts;
        }
    }
}
