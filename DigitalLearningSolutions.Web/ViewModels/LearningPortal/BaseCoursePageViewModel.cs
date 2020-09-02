namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public abstract class BaseCoursePageViewModel
    {
        [BindProperty] public string SortDirection { get; set; }
        [BindProperty] public string SortBy { get; set; }
        public int Page { get; }
        public int TotalPages { get; protected set; }

        public readonly string? BannerText;
        public abstract SelectList SortByOptions { get; }

        public const string DescendingText = "Descending";
        public const string AscendingText = "Ascending";

        protected const int ItemsPerPage = 10;
        private readonly int offset;

        public readonly string? SearchString;

        protected BaseCoursePageViewModel(
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText,
            int page
        )
        {
            BannerText = bannerText;
            SortBy = sortBy;
            SortDirection = sortDirection;
            SearchString = searchString;
            Page = page;
            offset = OffsetFromPageNumber(page);
        }

        protected IEnumerable<BaseLearningItem> PaginateItems(IList<BaseLearningItem> items) {
            if (items.Count > ItemsPerPage)
            {
                items = items.Skip(offset).Take(ItemsPerPage).ToList();
            }

            return items;
        }

        private static int OffsetFromPageNumber(int pageNumber) =>
            (pageNumber - 1) * ItemsPerPage;
    }
    public static class SortByOptionTexts
    {
        public const string
            Name = "Course Name",
            StartedDate = "Enrolled Date",
            LastAccessed = "Last Accessed Date",
            CompleteByDate = "Complete By Date",
            CompletedDate = "Completed Date",
            DiagnosticScore = "Diagnostic Score",
            PassedSections = "Passed Sections",
            Brand = "Brand",
            Category = "Category",
            Topic = "Topic";
    }
}
