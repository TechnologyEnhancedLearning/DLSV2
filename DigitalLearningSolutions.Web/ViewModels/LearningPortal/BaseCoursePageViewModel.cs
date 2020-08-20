namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public abstract class BaseCoursePageViewModel
    {
        [BindProperty] public string SortDirection { get; set; }
        [BindProperty] public string SortBy { get; set; }

        public readonly string? BannerText;
        public abstract SelectList SortByOptions { get; }

        public const string DescendingText = "Descending";

        public readonly string? SearchString;

        public BaseCoursePageViewModel(
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText
        )
        {
            BannerText = bannerText;
            SortBy = sortBy;
            SortDirection = sortDirection;
            SearchString = searchString;
        }
    }
    public static class SortByOptionTexts
    {
        public const string
            CourseName = "Course Name",
            StartedDate = "Enrolled Date",
            LastAccessed = "Last Accessed Date",
            CompleteByDate = "Complete By Date",
            CompletedDate = "Completed Date",
            DiagnosticScore = "Diagnostic Score",
            PassedSections = "Passed Sections";
    }
}
