namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AvailablePageViewModel : BaseSearchablePageViewModel<AvailableCourse>
    {
        public readonly IEnumerable<AvailableCourseViewModel> AvailableCourses;
        public readonly string? BannerText;

        public AvailablePageViewModel(
            SearchSortFilterPaginationResult<AvailableCourse> result,
            string? bannerText
        ) : base(result, false, searchLabel: "Search courses")
        {
            BannerText = bannerText;
            AvailableCourses = result.ItemsToDisplay.Select(c => new AvailableCourseViewModel(c));
        }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.Name,
            CourseSortByOptions.Brand,
            CourseSortByOptions.Category,
            CourseSortByOptions.Topic,
        };

        public override bool NoDataFound => !AvailableCourses.Any() && NoSearchOrFilter;
    }
}
