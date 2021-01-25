﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
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
            SortByOptionTexts.Name,
            SortByOptionTexts.Brand,
            SortByOptionTexts.Category,
            SortByOptionTexts.Topic
        });

        public AvailablePageViewModel(
            IEnumerable<AvailableCourse> availableCourses,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? bannerText,
            int page
        ) : base(searchString, sortBy, sortDirection, bannerText, page)
        {
            var sortedItems = SortingHelper.SortAllItems(
                availableCourses,
                sortBy,
                sortDirection
            );
            var filteredItems = SearchHelper.FilterLearningItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = PaginateItems(filteredItems);
            AvailableCourses = paginatedItems.Cast<AvailableCourse>().Select(c => new AvailableCourseViewModel(c));
        }
    }
}
