namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllDelegateGroupsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableDelegateGroupViewModel> DelegateGroups;

        public AllDelegateGroupsViewModel(IEnumerable<Group> groups, IEnumerable<(int, string)> registrationPrompts)
        {
            groups = groups.ToList();
            DelegateGroups = groups.Select(g => new SearchableDelegateGroupViewModel(g));

            var admins = groups.Select(g => (AddedByAdminID: g.AddedByAdminId, g.AddedByName)).Distinct();

            Filters = new[]
            {
                new FilterViewModel(
                    "AddedByAdminId",
                    "Added by",
                    DelegateGroupsViewModelFilterOptions.GetAddedByOptions(admins)
                ),
                new FilterViewModel(
                    nameof(Group.LinkedToField),
                    "Linked field",
                    DelegateGroupsViewModelFilterOptions.GetLinkedFieldOptions(registrationPrompts)
                )
            }.Select(
                f => f.FilterOptions.Select(
                    fo => new AppliedFilterViewModel(fo.DisplayText, f.FilterName, fo.FilterValue)
                )
            ).SelectMany(af => af).Distinct();
        }
    }
}
