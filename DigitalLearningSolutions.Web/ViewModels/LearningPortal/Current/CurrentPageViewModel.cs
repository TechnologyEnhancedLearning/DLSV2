namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;

    public class CurrentPageViewModel : BaseSearchablePageViewModel
    {
        public readonly string? BannerText;

        public CurrentPageViewModel(
            IEnumerable<CurrentCourse> currentCourses,
            string? searchString,
            string sortBy,
            string sortDirection,
            IEnumerable<SelfAssessment> selfAssessments,
            IEnumerable<ActionPlanResource> actionPlanResources,
            bool apiIsAccessible,
            string? bannerText,
            int page
        ) : base(searchString, page, false, sortBy, sortDirection, searchLabel: "Search your current courses")
        {
            ApiIsAccessible = apiIsAccessible;
            BannerText = bannerText;
            var allItems = currentCourses.Cast<CurrentLearningItem>().ToList();
            allItems.AddRange(selfAssessments);
            allItems.AddRange(actionPlanResources);

            var sortedItems = GenericSortingHelper.SortAllItems(
                allItems.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);

            CurrentActivities = paginatedItems.Select<BaseLearningItem, CurrentLearningItemViewModel>(
                activity =>
                {
                    return activity switch
                    {
                        CurrentCourse currentCourse => new CurrentCourseViewModel(currentCourse),
                        SelfAssessment selfAssessment => new SelfAssessmentCardViewModel(selfAssessment),
                        _ => new CurrentLearningResourceViewModel((ActionPlanResource)activity),
                    };
                }
            );
        }

        public IEnumerable<CurrentLearningItemViewModel> CurrentActivities { get; }

        public bool ApiIsAccessible { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.Name,
            CourseSortByOptions.StartedDate,
            CourseSortByOptions.LastAccessed,
            CourseSortByOptions.CompleteByDate,
            CourseSortByOptions.DiagnosticScore,
            CourseSortByOptions.PassedSections,
        };

        public override bool NoDataFound => !CurrentActivities.Any() && NoSearchOrFilter;
    }
}
