﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllDelegateGroupsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableDelegateGroupViewModel> DelegateGroups;

        public AllDelegateGroupsViewModel(IEnumerable<Group> groups)
        {
            DelegateGroups = groups.Select(g => new SearchableDelegateGroupViewModel(g));
        }
    }
}
