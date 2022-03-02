namespace DigitalLearningSolutions.Web.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class FilterModelExtensions
    {
        public static IEnumerable<AppliedFilterViewModel> SelectAppliedFilterViewModels(
            this IEnumerable<FilterViewModel> filterViewModels
        )
        {
            return filterViewModels.Select(
                f => f.FilterOptions.Select(
                    fo => new AppliedFilterViewModel(fo.DisplayText, f.FilterName, fo.NewFilterToAdd)
                )
            ).SelectMany(af => af).Distinct();
        }
    }
}
