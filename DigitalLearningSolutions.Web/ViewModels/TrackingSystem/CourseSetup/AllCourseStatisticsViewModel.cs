namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.Extensions.Configuration;

    public class AllCourseStatisticsViewModel : BaseJavaScriptFilterableViewModel
    {
        public AllCourseStatisticsViewModel(CentreCourseDetails details, IConfiguration config)
        {
            Courses = details.Courses.Select(c => new SearchableCourseStatisticsViewModel(c, config));

            Filters = CourseStatisticsViewModelFilterOptions.GetFilterOptions(details.Categories, details.Topics)
                .SelectAppliedFilterViewModels();
        }

        public IEnumerable<SearchableCourseStatisticsViewModel> Courses { get; set; }
    }
}
