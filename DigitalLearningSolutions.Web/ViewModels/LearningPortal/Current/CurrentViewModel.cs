namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using FuzzySharp;
    using FuzzySharp.SimilarityRatio;
    using FuzzySharp.SimilarityRatio.Scorer;
    using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;

    public class CurrentViewModel
    {
        private readonly IConfiguration config;

        [BindProperty] public string SortDirection { get; set; }
        [BindProperty] public string SortBy { get; set; }

        public IEnumerable<NamedItemViewModel> CurrentCourses { get; }

        public readonly SelectList SortByOptions = new SelectList(new[]
        {
            SortByOptionTexts.CourseName,
            SortByOptionTexts.StartedDate,
            SortByOptionTexts.LastAccessed,
            SortByOptionTexts.CompleteByDate,
            SortByOptionTexts.DiagnosticScore,
            SortByOptionTexts.PassedSections
        });

        public const string AscendingText = "Ascending";
        public const string DescendingText = "Descending";
        public readonly string? BannerText;
        public readonly string? SearchString;

        // This is the lower threshold for the search match score. This value was determined by trial and error.
        // If there are any issues with strange search results, changing this value or the scorer strategy would
        // be a good place to start.
        private const int MatchCutoffScore = 70;

        public CurrentViewModel(
            IEnumerable<CurrentCourse> currentCourses,
            IConfiguration config,
            string? searchString,
            string sortBy,
            string sortDirection,
            SelfAssessment? selfAssessment,
            string? bannerText
        )
        {
            this.config = config;
            BannerText = bannerText;
            SearchString = searchString;
            SortBy = sortBy;
            SortDirection = sortDirection;

            var sortedItems = SortAllItems(currentCourses, selfAssessment);
            var filteredItems = FilterNamedItems(sortedItems);
            CurrentCourses = filteredItems.Select<NamedItem, NamedItemViewModel>(course =>
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

        private IEnumerable<NamedItem> SortAllItems(IEnumerable<CurrentCourse> currentCourses, SelfAssessment? selfAssessment)
        {
            if (SortBy == SortByOptionTexts.CourseName)
            {
                return SortByName(currentCourses, selfAssessment);
            }

            IEnumerable<NamedItem> sortedCourses = SortCourses(currentCourses);
            if (selfAssessment == null)
            {
                return sortedCourses;
            }

            return SortDirection == DescendingText
                ? sortedCourses.Append(selfAssessment)
                : sortedCourses.Prepend(selfAssessment);
        }

        private IEnumerable<NamedItem> SortByName(IEnumerable<CurrentCourse> currentCourses, SelfAssessment? selfAssessment)
        {
            var allItems = new List<NamedItem>(currentCourses);
            if (selfAssessment != null)
            {
                allItems.Add(selfAssessment);
            }

            return SortDirection == DescendingText
                ? allItems.OrderByDescending(course => course.Name)
                : allItems.OrderBy(course => course.Name);
        }

        private IEnumerable<CurrentCourse> SortCourses(IEnumerable<CurrentCourse> currentCourses)
        {
            return SortBy switch
            {
                SortByOptionTexts.StartedDate => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.StartedDate)
                    : currentCourses.OrderBy(course => course.StartedDate),
                SortByOptionTexts.LastAccessed => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.LastAccessed)
                    : currentCourses.OrderBy(course => course.LastAccessed),
                SortByOptionTexts.CompleteByDate => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.CompleteByDate)
                    : currentCourses.OrderBy(course => course.CompleteByDate),
                SortByOptionTexts.DiagnosticScore => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.HasDiagnostic)
                        .ThenByDescending(course => course.DiagnosticScore)
                    : currentCourses.OrderBy(course => course.HasDiagnostic)
                        .ThenBy(course => course.DiagnosticScore),
                SortByOptionTexts.PassedSections => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.IsAssessed)
                        .ThenByDescending(course => course.Passes)
                    : currentCourses.OrderBy(course => course.IsAssessed)
                        .ThenBy(course => course.Passes),
                _ => currentCourses
            };
        }

        private IEnumerable<NamedItem> FilterNamedItems(IEnumerable<NamedItem> namedItems)
        {
            if (SearchString == null)
            {
                return namedItems;
            }

            var query = new CurrentCourse()
            {
                CourseName = SearchString
            };

            var results = Process.ExtractAll(
                query,
                namedItems,
                currentCourse => currentCourse.Name.ToLower(),
                GetScorer(SearchString),
                MatchCutoffScore
            );
            return results.Select(result => result.Value);
        }

        private static IRatioScorer GetScorer(string searchString)
        {
            return searchString.Any(char.IsDigit)
                ? ScorerCache.Get<TokenSetScorer>()
                : ScorerCache.Get<PartialTokenAbbreviationScorer>();
        }
    }

    public static class SortByOptionTexts
    {
        public const string
            CourseName = "Course Name",
            StartedDate = "Enrolled Date",
            LastAccessed = "Last Accessed Date",
            CompleteByDate = "Complete By Date",
            DiagnosticScore = "Diagnostic Score",
            PassedSections = "Passed Sections";
    }
}
