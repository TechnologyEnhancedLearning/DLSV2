namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SelectCourseAllCoursesViewModel : BaseJavaScriptFilterableViewModel
    {
        public SelectCourseAllCoursesViewModel(
            IEnumerable<ApplicationDetails> applicationOptions,
            IEnumerable<string> categories,
            IEnumerable<string> topics
        )
        {
            var applicationsList = applicationOptions.ToList();
            ApplicationOptions = applicationsList.Select(a => new FilterableApplicationSelectListItemViewModel(a));

            Filters = SelectCourseViewModelFilterOptions.GetAllCategoriesFilters(categories, topics)
                .SelectAppliedFilterViewModels();
        }

        public IEnumerable<FilterableApplicationSelectListItemViewModel> ApplicationOptions { get; set; }
    }
}
