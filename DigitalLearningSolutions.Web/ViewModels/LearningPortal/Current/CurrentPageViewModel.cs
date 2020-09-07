namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
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
            IConfiguration config,
            string? searchString,
            string sortBy,
            string sortDirection,
            SelfAssessment? selfAssessment,
            string? bannerText,
            int page,
            int itemsPerPage = 10
        ) : base(searchString, sortBy, sortDirection, bannerText, page, itemsPerPage)
        {
            var allItems = currentCourses.Cast<CurrentLearningItem>().ToList();
            if (selfAssessment != null)
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

            TotalPages = (int)Math.Ceiling(filteredItems.Count / (double)ItemsPerPage);
            if (Page < 1 || Page > TotalPages)
            {
                Page = 1;
            }

            var paginatedItems = PaginateItems(filteredItems);

            CurrentCourses = paginatedItems.Select<BaseLearningItem, CurrentLearningItemViewModel>(course =>
            {
                if (course is CurrentCourse currentCourse)
                {
                    return new CurrentCourseViewModel(currentCourse, config);
                }

                return new SelfAssessmentCardViewModel((SelfAssessment)course);
            });
        }
    }
}
