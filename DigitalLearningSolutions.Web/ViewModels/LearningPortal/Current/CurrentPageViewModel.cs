namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CurrentPageViewModel : BaseSearchablePageViewModel
    {
        public IEnumerable<CurrentLearningItemViewModel> CurrentCourses { get; }

        public override List<SelectListItem> SortByOptions { get; } = new List<SelectListItem> { item1, item2, item3, item4, item5, item6};

        private static SelectListItem item1 = new SelectListItem(SortByOptionTexts.Name, nameof(CurrentCourse.Name));
        private static SelectListItem item2 = new SelectListItem(SortByOptionTexts.StartedDate, nameof(CurrentCourse.StartedDate));
        private static SelectListItem item3 = new SelectListItem(SortByOptionTexts.LastAccessed, nameof(CurrentCourse.LastAccessed));
        private static SelectListItem item4 = new SelectListItem(SortByOptionTexts.CompleteByDate, nameof(CurrentCourse.CompleteByDate));
        private static SelectListItem item5 = new SelectListItem(SortByOptionTexts.DiagnosticScore, $"{nameof(CurrentCourse.HasDiagnostic)},{nameof(CurrentCourse.DiagnosticScore)}");
        private static SelectListItem item6 = new SelectListItem(SortByOptionTexts.PassedSections, $"{nameof(CurrentCourse.IsAssessed)},{nameof(CurrentCourse.Passes)}");

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

            CurrentCourses = paginatedItems.Select<BaseLearningItem, CurrentLearningItemViewModel>(course =>
            {
                if (course is CurrentCourse currentCourse)
                {
                    return new CurrentCourseViewModel(currentCourse);
                }

                return new SelfAssessmentCardViewModel((SelfAssessment)course);
            });
        }
    }
}
