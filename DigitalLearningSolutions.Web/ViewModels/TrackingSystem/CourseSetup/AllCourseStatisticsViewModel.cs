namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllCourseStatisticsViewModel : BaseJavaScriptFilterableViewModel
    {
        public AllCourseStatisticsViewModel(IEnumerable<CourseStatistics> courses, IEnumerable<string> categories, IEnumerable<string> topics)
        {
            courses = courses.ToList();
            Courses = courses.Select(c => new SearchableCourseStatisticsViewModel(c));

            Filters = CourseStatisticsViewModelFilterOptions.GetFilterOptions(categories, topics)
                .Select(
                    f => f.FilterOptions.Select(
                        fo => new AppliedFilterViewModel(fo.DisplayText, f.FilterName, fo.FilterValue)
                    )
                ).SelectMany(af => af).Distinct();
        }

        public IEnumerable<SearchableCourseStatisticsViewModel> Courses { get; set; }
    }
}
