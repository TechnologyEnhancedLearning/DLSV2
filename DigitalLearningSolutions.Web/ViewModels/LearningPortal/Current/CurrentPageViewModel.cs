namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;

    public class CurrentPageViewModel : BaseSearchablePageViewModel
    {
        public CurrentPageViewModel(
            IEnumerable<CurrentCourse> currentCourses,
            string? searchString,
            string sortBy,
            string sortDirection,
            IEnumerable<SelfAssessment> selfAssessments,
            string? bannerText,
            int page
        ) : base(searchString, sortBy, sortDirection, page, bannerText)
        {
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
            var paginatedItems = PaginateItems(filteredItems);

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
            CourseSortByOptionTexts.Name,
            CourseSortByOptionTexts.StartedDate,
            CourseSortByOptionTexts.LastAccessed,
            CourseSortByOptionTexts.CompleteByDate,
            CourseSortByOptionTexts.DiagnosticScore,
            CourseSortByOptionTexts.PassedSections
        };
    }
}
