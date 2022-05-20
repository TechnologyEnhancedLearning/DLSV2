namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllBrandCoursesViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<BrandCourseViewModel> Courses;

        public AllBrandCoursesViewModel(IReadOnlyCollection<ApplicationWithSections> applications)
        {
            Courses = applications.Select(app => new BrandCourseViewModel(app));
            var categories = applications.Select(x => x.CategoryName).Distinct().OrderBy(x => x).ToList();
            var topics = applications.Select(x => x.CourseTopic).Distinct().OrderBy(x => x).ToList();
            Filters = LearningContentViewModelFilterOptions
                .GetFilterOptions(categories, topics).SelectAppliedFilterViewModels();
        }
    }
}
