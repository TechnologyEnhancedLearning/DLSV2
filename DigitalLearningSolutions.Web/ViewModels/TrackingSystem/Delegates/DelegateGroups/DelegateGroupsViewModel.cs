namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public class DelegateGroupsViewModel
    {
        public DelegateGroupsViewModel(IEnumerable<Group> groups)
        {
            DelegateGroups = groups.Select(g => new SearchableDelegateGroupViewModel(g));
        }

        public IEnumerable<SearchableDelegateGroupViewModel> DelegateGroups { get; set; }
    }
}
