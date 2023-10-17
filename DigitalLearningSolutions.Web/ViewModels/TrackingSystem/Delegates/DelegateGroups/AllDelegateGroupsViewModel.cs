namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;


    public class AllDelegateGroupsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableDelegateGroupViewModel> DelegateGroups;

        public AllDelegateGroupsViewModel(List<Group> groups, IEnumerable<CentreRegistrationPrompt> registrationPrompts)
        {
            DelegateGroups = groups.Select(g =>
                {
                    var cardId = $"{g.GroupId}-card";
                    return new SearchableDelegateGroupViewModel(g, new ReturnPageQuery(1, cardId));
                }
            );

            var addedByAdmins = groups
                .Select(g => new GroupDelegateAdmin
                {
                    GroupId = g.GroupId,
                    AdminId = g.AddedByAdminId,
                    Forename = g.AddedByFirstName,
                    Surname = g.AddedByLastName,
                    Active = g.AddedByAdminActive
                })
                .GroupBy(g => g.GroupId)
                .Select(g => g.First())
                .AsEnumerable();

            Filters = DelegateGroupsViewModelFilterOptions.GetDelegateGroupFilterModels(addedByAdmins, registrationPrompts)
                .SelectAppliedFilterViewModels();
        }
    }
}
