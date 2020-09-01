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
        public IEnumerable<NamedItemViewModel> CurrentCourses { get; }

        public override SelectList SortByOptions { get; } = new SelectList(new[]
        {
            SortByOptionTexts.CourseName,
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
            int page
        ) : base(searchString, sortBy, sortDirection, bannerText, page)
        {
            var sortedItems = SortingHelper.SortAllItems(
                currentCourses,
                selfAssessment,
                sortBy,
                sortDirection
            );
            var filteredItems = SearchHelper.FilterNamedItems(sortedItems, SearchString).ToList();

            var paginatedItems = PaginateItems(filteredItems);
            TotalPages = (int)Math.Ceiling(filteredItems.Count / (double)ItemsPerPage);

            CurrentCourses = paginatedItems.Select<NamedItem, NamedItemViewModel>(course =>
            {
                if (course is CurrentCourse currentCourse)
                {
                    return new CurrentCourseViewModel(currentCourse, config);
                }

                return new SelfAssessmentCardViewModel()
                {
                    Name = course.Name
                };
            });
        }
    }
}
