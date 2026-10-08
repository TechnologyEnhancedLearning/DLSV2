namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;

    public class CurrentPageViewModel : BaseSearchablePageViewModel<CurrentLearningItem>
    {
        public readonly string? BannerText;

        public CurrentPageViewModel(
            SearchSortFilterPaginationResult<CurrentLearningItem> result,
            bool apiIsAccessible,
            string? bannerText,
            IEnumerable<CurrentLearningItem>? allCurrentActivities = null
        ) : base(result, false, searchLabel: "Search")
        {
            ApiIsAccessible = apiIsAccessible;
            BannerText = bannerText;

            CurrentActivities = MapActivities(result.ItemsToDisplay, result);
            AllCurrentActivities = MapActivities(allCurrentActivities ?? result.ItemsToDisplay, result);
        }

        public IEnumerable<CurrentLearningItemViewModel> CurrentActivities { get; }

        public IEnumerable<CurrentLearningItemViewModel> AllCurrentActivities { get; }

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

        private static IEnumerable<CurrentLearningItemViewModel> MapActivities(
            IEnumerable<CurrentLearningItem> activities,
            SearchSortFilterPaginationResult<CurrentLearningItem> result
        )
        {
            return activities.Select<BaseLearningItem, CurrentLearningItemViewModel>(
                activity =>
                {
                    var itemId = activity switch
                    {
                        CurrentCourse => $"{activity.Id}-course-card",
                        SelfAssessment => $"{activity.Id}-sa-card",
                        _ => $"{activity.Id}-lhr-card",
                    };
                    var returnPageQuery = result.GetReturnPageQuery(itemId);

                    return activity switch
                    {
                        CurrentCourse currentCourse => new CurrentCourseViewModel(currentCourse, returnPageQuery),
                        SelfAssessment selfAssessment => new SelfAssessmentCardViewModel(selfAssessment, returnPageQuery),
                        _ => new CurrentLearningResourceViewModel((ActionPlanResource)activity, returnPageQuery),
                    };
                }
            );
        }
    }
}
