namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;

    public class AvailablePageViewModel : BaseCoursePageViewModel
    {
        public readonly IEnumerable<AvailableCourseViewModel> AvailableCourses;

        public override SelectList SortByOptions { get; } = new SelectList(new[]
        {
            SortByOptionTexts.CourseName,
            SortByOptionTexts.Brand,
            SortByOptionTexts.Category,
            SortByOptionTexts.Topic
        });

        public AvailablePageViewModel(
            IEnumerable<AvailableCourse> availableCourses,
            IConfiguration config,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText
        ) : base(searchString, sortBy, sortDirection, bannerText)
        {
            var sortedItems = SortingHelper.SortAllItems(
                availableCourses,
                null,
                sortBy,
                sortDirection
            );
            var filteredItems = SearchHelper.FilterNamedItems(sortedItems, SearchString);
            AvailableCourses = filteredItems.Cast<AvailableCourse>().Select(c => new AvailableCourseViewModel(c, config));
        }
    }
}
