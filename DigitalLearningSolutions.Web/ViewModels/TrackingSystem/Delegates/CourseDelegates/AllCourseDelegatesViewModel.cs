namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllCourseDelegatesViewModel : BaseJavaScriptFilterableViewModel
    {
        public AllCourseDelegatesViewModel(IEnumerable<CourseDelegate> delegates)
        {
            Delegates = delegates.Select(d => new SearchableCourseDelegateViewModel(d));
            Filters = CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels()
                .SelectAppliedFilterViewModels();
        }

        public IEnumerable<SearchableCourseDelegateViewModel> Delegates { get; set; }
    }
}
