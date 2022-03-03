namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllDelegateCourseStatisticsViewModel : BaseJavaScriptFilterableViewModel
    {
        public AllDelegateCourseStatisticsViewModel(CentreCourseDetails details)
        {
            Courses = details.Courses.Select(c => new SearchableDelegateCourseStatisticsViewModel(c));

            Filters = DelegateCourseStatisticsViewModelFilterOptions.GetFilterOptions(details.Categories, details.Topics)
                .SelectAppliedFilterViewModels();
        }

        public IEnumerable<SearchableDelegateCourseStatisticsViewModel> Courses { get; set; }
    }
}
