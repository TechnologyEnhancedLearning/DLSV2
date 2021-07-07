namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
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
            string? bannerText,
            int page
        ) : base(searchString, sortBy, sortDirection, page)
        {
            BannerText = bannerText;
            var allItems = currentCourses.Cast<CurrentLearningItem>().ToList();
            foreach (SelfAssessment selfAssessment in selfAssessments)
            {
                allItems.Add(selfAssessment);
            }

            var sortedItems = GenericSortingHelper.SortAllItems(
                allItems.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);

            CurrentCourses = paginatedItems.Select<BaseLearningItem, CurrentLearningItemViewModel>(
                course =>
                {
                    if (course is CurrentCourse currentCourse)
                    {
                        return new CurrentCourseViewModel(currentCourse);
                    }

                    return new SelfAssessmentCardViewModel((SelfAssessment)course);
                }
            );
        }

        public IEnumerable<CurrentLearningItemViewModel> CurrentCourses { get; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.Name,
            CourseSortByOptions.StartedDate,
            CourseSortByOptions.LastAccessed,
            CourseSortByOptions.CompleteByDate,
            CourseSortByOptions.DiagnosticScore,
            CourseSortByOptions.PassedSections
        };
    }
}
