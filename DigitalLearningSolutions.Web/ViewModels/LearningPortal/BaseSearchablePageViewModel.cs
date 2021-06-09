//namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using DigitalLearningSolutions.Data.Models;
//    using Microsoft.AspNetCore.Mvc;
//    using Microsoft.AspNetCore.Mvc.Rendering;

//    public abstract class BaseSearchablePageViewModel
//    {
//        [BindProperty] public string SortDirection { get; set; }
//        [BindProperty] public string SortBy { get; set; }
//        public int Page { get; protected set; }
//        public int TotalPages { get; protected set; }
//        public int MatchingSearchResults;

//        public readonly string? BannerText;
//        public abstract SelectList SortByOptions { get; }

//        public const string DescendingText = "Descending";
//        public const string AscendingText = "Ascending";

//        private const int ItemsPerPage = 10;

//        public readonly string? SearchString;

//        protected BaseSearchablePageViewModel(
//            string? searchString,
//            string sortBy,
//            string sortDirection,
//            string? bannerText,
//            int page
//        )
//        {
//            BannerText = bannerText;
//            SortBy = sortBy;
//            SortDirection = sortDirection;
//            SearchString = searchString;
//            Page = page;
//        }

//        protected IEnumerable<BaseLearningItem> PaginateItems(IList<BaseLearningItem> items) {
//            if (items.Count > ItemsPerPage)
//            {
//                items = items.Skip(OffsetFromPageNumber(Page)).Take(ItemsPerPage).ToList();
//            }

//            return items;
//        }

//        protected void SetTotalPages()
//        {
//            TotalPages = (int)Math.Ceiling(MatchingSearchResults / (double)ItemsPerPage);
//            if (Page < 1 || Page > TotalPages)
//            {
//                Page = 1;
//            }
//        }

//        private int OffsetFromPageNumber(int pageNumber) =>
//            (pageNumber - 1) * ItemsPerPage;
//    }
//    public static class SortByOptionTexts
//    {
//        public const string
//            Name = "Activity Name",
//            StartedDate = "Enrolled Date",
//            LastAccessed = "Last Accessed Date",
//            CompleteByDate = "Complete By Date",
//            CompletedDate = "Completed Date",
//            DiagnosticScore = "Diagnostic Score",
//            PassedSections = "Passed Sections",
//            Brand = "Brand",
//            Category = "Category",
//            Topic = "Topic";
//    }
//}
