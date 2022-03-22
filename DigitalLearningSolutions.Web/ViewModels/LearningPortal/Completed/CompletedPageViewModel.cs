namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.Extensions.Configuration;

    public class CompletedPageViewModel : BaseSearchablePageViewModel<CompletedLearningItem>
    {
        public readonly string? BannerText;

        public CompletedPageViewModel(
            SearchSortFilterPaginationResult<CompletedLearningItem> result,
            bool apiIsAccessible,
            IConfiguration config,
            string? bannerText
        ) : base(result, false, searchLabel: "Search your completed courses")
        {
            ApiIsAccessible = apiIsAccessible;
            BannerText = bannerText;

            CompletedActivities = result.ItemsToDisplay.Select<BaseLearningItem, CompletedLearningItemViewModel>(
                activity =>
                {
                    return activity switch
                    {
                        CompletedCourse completedCourse => new CompletedCourseViewModel(completedCourse, config),
                        _ => new CompletedLearningResourceViewModel((CompletedActionPlanResource)activity),
                    };
                }
            );
        }

        public IEnumerable<CompletedLearningItemViewModel> CompletedActivities { get; }

        public bool ApiIsAccessible { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.Name,
            CourseSortByOptions.StartedDate,
            CourseSortByOptions.LastAccessed,
            CourseSortByOptions.CompletedDate,
        };

        public override bool NoDataFound => !CompletedActivities.Any() && NoSearchOrFilter;
    }
}
