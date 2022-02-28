namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllCourseStatisticsViewModel : BaseJavaScriptFilterableViewModel
    {
        public AllCourseStatisticsViewModel(CentreCourseDetails details)
        {
            Courses = details.Courses.Select(c => new SearchableCourseStatisticsViewModel(c));

            Filters = CourseStatisticsViewModelFilterOptions.GetFilterOptions(details.Categories, details.Topics)
                .SelectAppliedFilterViewModels();
        }

        public IEnumerable<SearchableCourseStatisticsViewModel> Courses { get; set; }
    }
}
