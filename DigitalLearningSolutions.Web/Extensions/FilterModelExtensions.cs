namespace DigitalLearningSolutions.Web.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class FilterModelExtensions
    {
        public static IEnumerable<AppliedFilterViewModel> SelectAppliedFilterViewModels(
            this IEnumerable<FilterModel> filterViewModels
        )
        {
            return filterViewModels.Select(
                f => f.FilterOptions.Select(
                    fo => new AppliedFilterViewModel(fo.DisplayText, f.FilterName, fo.FilterValue)
                )
            ).SelectMany(af => af).Distinct();
        }
    }
}
