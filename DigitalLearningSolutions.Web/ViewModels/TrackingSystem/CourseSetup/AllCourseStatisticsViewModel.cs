namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllCourseStatisticsViewModel : BaseJavaScriptFilterableViewModel
    {
        public AllCourseStatisticsViewModel(
            IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> courses,
            IEnumerable<string> categories,
            IEnumerable<string> topics
        )
        {
            Courses = courses.Select(c => new SearchableCourseStatisticsViewModel(c));

            Filters = CourseStatisticsViewModelFilterOptions.GetFilterOptions(categories, topics)
                .SelectAppliedFilterViewModels();
        }

        public IEnumerable<SearchableCourseStatisticsViewModel> Courses { get; set; }
    }
}
