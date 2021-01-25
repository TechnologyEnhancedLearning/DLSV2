namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;

    public class CurrentPageViewModel : BaseCoursePageViewModel
    {
        public IEnumerable<CurrentLearningItemViewModel> CurrentCourses { get; }

        public override SelectList SortByOptions { get; } = new SelectList(new[]
        {
            SortByOptionTexts.Name,
            SortByOptionTexts.StartedDate,
            SortByOptionTexts.LastAccessed,
            SortByOptionTexts.CompleteByDate,
            SortByOptionTexts.DiagnosticScore,
            SortByOptionTexts.PassedSections
        });

        public CurrentPageViewModel(
            IEnumerable<CurrentCourse> currentCourses,
            string? searchString,
            string sortBy,
            string sortDirection,
            IEnumerable<SelfAssessment> selfAssessments,
            string? bannerText,
            int page
        ) : base(searchString, sortBy, sortDirection, bannerText, page)
        {
            var allItems = currentCourses.Cast<CurrentLearningItem>().ToList();
            foreach (SelfAssessment selfAssessment in selfAssessments)
            {
                allItems.Add(selfAssessment);
            }
            var sortedItems = SortingHelper.SortAllItems(
                allItems,
                sortBy,
                sortDirection
            );
            var filteredItems = SearchHelper.FilterLearningItems(sortedItems, SearchString).ToList();
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
