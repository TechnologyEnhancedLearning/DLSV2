namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllDelegateCourseStatisticsViewModel : BaseJavaScriptFilterableViewModel
    {
        public AllDelegateCourseStatisticsViewModel(
            IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> courses,
            IEnumerable<string> categories,
            IEnumerable<string> topics
        )
        {
            Courses = courses.Select(c => new SearchableDelegateCourseStatisticsViewModel(c));

            Filters = DelegateCourseStatisticsViewModelFilterOptions.GetFilterOptions(categories, topics)
                .SelectAppliedFilterViewModels();
        }

        public IEnumerable<SearchableDelegateCourseStatisticsViewModel> Courses { get; set; }
    }
}
