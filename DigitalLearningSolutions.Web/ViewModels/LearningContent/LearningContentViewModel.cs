namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class LearningContentViewModel : BaseSearchablePageViewModel<ApplicationWithSections>
    {
        public readonly IEnumerable<BrandCourseViewModel> Applications;
        public readonly int BrandId;
        public readonly string? Description;
        public readonly string Name;
        public readonly IEnumerable<TutorialSummary> Tutorials;

        public LearningContentViewModel(
            SearchSortFilterPaginationResult<ApplicationWithSections> result,
            IEnumerable<FilterModel> availableFilters,
            BrandDetail brand,
            IEnumerable<TutorialSummary> tutorials
        ) : base(result, true, availableFilters)
        {
            BrandId = brand.BrandID;
            Name = brand.BrandName;
            Description = brand.BrandDescription;
            Tutorials = tutorials;
            Applications = result.ItemsToDisplay.Select(app => new BrandCourseViewModel(app));
        }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            BrandCoursesSortByOption.Title,
            BrandCoursesSortByOption.Popularity,
            BrandCoursesSortByOption.Length,
            BrandCoursesSortByOption.CreatedDate,
        };

        public override bool NoDataFound => !Applications.Any() && NoSearchOrFilter;
    }
}
