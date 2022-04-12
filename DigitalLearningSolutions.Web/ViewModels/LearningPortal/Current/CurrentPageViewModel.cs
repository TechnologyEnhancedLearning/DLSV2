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
            string? bannerText
        ) : base(result, false, searchLabel: "Search your current courses")
        {
            ApiIsAccessible = apiIsAccessible;
            BannerText = bannerText;

            CurrentActivities = result.ItemsToDisplay.Select<BaseLearningItem, CurrentLearningItemViewModel>(
                activity =>
                {
                    return activity switch
                    {
                        CurrentCourse currentCourse => new CurrentCourseViewModel(
                            currentCourse,
                            result.GetReturnPageQuery($"{currentCourse.Id}-course-card")
                        ),
                        SelfAssessment selfAssessment => new SelfAssessmentCardViewModel(
                            selfAssessment,
                            result.GetReturnPageQuery($"{selfAssessment.Id}-sa-card")
                        ),
                        _ => new CurrentLearningResourceViewModel(
                            (ActionPlanResource)activity,
                            result.GetReturnPageQuery($"{activity.Id}-lhr-card")
                        ),
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
