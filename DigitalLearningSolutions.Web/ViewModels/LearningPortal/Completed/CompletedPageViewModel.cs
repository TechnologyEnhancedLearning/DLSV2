namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
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
            string? bannerText
        ) : base (searchString, sortBy, sortDirection, bannerText)
        {

            var sortedItems = SortingHelper.SortAllItems(
                completedCourses,
                null,
                sortBy,
                sortDirection
            );
            CompletedCourses = sortedItems.Cast<CompletedCourse>().Select(completedCourse =>
                new CompletedCourseViewModel(completedCourse, config)
            );
        }
    }
}
