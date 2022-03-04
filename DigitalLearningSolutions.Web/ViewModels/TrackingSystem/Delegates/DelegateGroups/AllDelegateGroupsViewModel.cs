namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllDelegateGroupsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableDelegateGroupViewModel> DelegateGroups;

        public AllDelegateGroupsViewModel(List<Group> groups, IEnumerable<CentreRegistrationPrompt> registrationPrompts)
        {
            DelegateGroups = groups.Select(g => new SearchableDelegateGroupViewModel(g, null));

            var admins = groups.Select(
                g => (g.AddedByAdminId, DisplayStringHelper.GetPotentiallyInactiveAdminName(
                    g.AddedByFirstName,
                    g.AddedByLastName,
                    g.AddedByAdminActive
                ))
            ).Distinct();

            Filters = new[]
            {
                new FilterModel(
                    nameof(Group.AddedByAdminId),
                    "Added by",
                    DelegateGroupsViewModelFilterOptions.GetAddedByOptions(admins)
                ),
                new FilterModel(
                    nameof(Group.LinkedToField),
                    "Linked field",
                    DelegateGroupsViewModelFilterOptions.GetLinkedFieldOptions(registrationPrompts)
                ),
            }.SelectAppliedFilterViewModels();
        }
    }
}
