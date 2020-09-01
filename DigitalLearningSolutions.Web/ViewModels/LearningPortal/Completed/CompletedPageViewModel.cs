namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;

    public class CompletedPageViewModel : BaseCoursePageViewModel
    {
        public IEnumerable<CompletedCourseViewModel> CompletedCourses { get; }

        public override SelectList SortByOptions { get; } = new SelectList(new[]
        {
            SortByOptionTexts.CourseName,
            SortByOptionTexts.StartedDate,
            SortByOptionTexts.LastAccessed,
            SortByOptionTexts.CompletedDate
        });

        public CompletedPageViewModel(
            IEnumerable<CompletedCourse> completedCourses,
            IConfiguration config,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText,
            int page
        ) : base (searchString, sortBy, sortDirection, bannerText, page)
        {

            var sortedItems = SortingHelper.SortAllItems(
                completedCourses,
                null,
                sortBy,
                sortDirection
            );
            var filteredItems = SearchHelper.FilterNamedItems(sortedItems, SearchString).ToList();
            var paginatedItems = PaginateItems(filteredItems);
            CompletedCourses = paginatedItems.Cast<CompletedCourse>().Select(completedCourse =>
                new CompletedCourseViewModel(completedCourse, config)
            );
        }
    }
}
