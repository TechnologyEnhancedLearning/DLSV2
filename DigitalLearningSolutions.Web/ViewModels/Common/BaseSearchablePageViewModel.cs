﻿namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public abstract class BaseSearchablePageViewModel
    {
        public const string DescendingText = "Descending";
        public const string AscendingText = "Ascending";

        private const int ItemsPerPage = 10;

        public readonly string? SearchString;
        public readonly string? BannerText;

        public int MatchingSearchResults;

        protected BaseSearchablePageViewModel(
            string? searchString,
            string sortBy,
            string sortDirection,
            int page,
            string? bannerText = null
        )
        {
            SortBy = sortBy;
            SortDirection = sortDirection;
            SearchString = searchString;
            Page = page;
            BannerText = bannerText;
        }

        [BindProperty] public string SortDirection { get; set; }

        [BindProperty] public string SortBy { get; set; }
        
        public int Page { get; protected set; }

        public int TotalPages { get; protected set; }

        public abstract List<SelectListItem> SortByOptions { get; }

        protected IEnumerable<T> PaginateItems<T>(IList<T> items)
        {
            if (items.Count > ItemsPerPage)
            {
                items = items.Skip(OffsetFromPageNumber(Page)).Take(ItemsPerPage).ToList();
            }

            return items;
        }

        protected void SetTotalPages()
        {
            TotalPages = (int)Math.Ceiling(MatchingSearchResults / (double)ItemsPerPage);
            if (Page < 1 || Page > TotalPages)
            {
                Page = 1;
            }
        }

        private int OffsetFromPageNumber(int pageNumber)
        {
            return (pageNumber - 1) * ItemsPerPage;
        }
    }

    public static class SortByOptionTexts
    {
        public const string
            Name = "Activity Name",
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
