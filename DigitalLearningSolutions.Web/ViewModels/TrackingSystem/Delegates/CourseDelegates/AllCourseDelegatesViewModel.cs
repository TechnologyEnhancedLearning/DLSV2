namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public class AllCourseDelegatesViewModel : BaseJavaScriptFilterableViewModel
    {
        public AllCourseDelegatesViewModel(IEnumerable<CourseDelegate> delegates, IList<CourseAdminField> adminFields)
        {
            Delegates = delegates.Select(
                d => new DelegateCourseInfoViewModel(
                    d,
                    DelegateAccessRoute.CourseDelegates,
                    new ReturnPageQuery(1, $"{d.DelegateId}-card")
                )
            );
            Filters = CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels(adminFields)
                .SelectAppliedFilterViewModels();
        }

        public IEnumerable<DelegateCourseInfoViewModel> Delegates { get; set; }
    }
}
