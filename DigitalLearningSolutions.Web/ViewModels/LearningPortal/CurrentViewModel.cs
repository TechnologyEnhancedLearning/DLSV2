namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
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
        private readonly IEnumerable<CurrentCourse> currentCourses;
        private readonly IConfiguration config;

        [BindProperty] public string SortDirection { get; set; }
        [BindProperty] public string SortBy { get; set; }

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
        public readonly SelfAssessment? SelfAssessment;
        public readonly string? BannerText;
        public readonly string? SearchString;

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
            SelfAssessment = selfAssessment;
            SortBy = sortBy;
            SortDirection = sortDirection;

            var filteredCurrentCourses = FilterCurrentCourses(currentCourses);
            this.currentCourses = SortCurrentCourses(filteredCurrentCourses);
        }

        private IEnumerable<CurrentCourse> FilterCurrentCourses(IEnumerable<CurrentCourse> allCurrentCourses)
        {
            if (SearchString == null)
            {
                return allCurrentCourses;
            }

            var query = new CurrentCourse()
            {
                CourseName = SearchString
            };

            // This is the lower threshold for the search match score. This value was determined by trial and error.
            // If there are any issues with strange search results, changing this value or the scorer strategy would
            // be a good place to start.
            const int matchCutoffScore = 70;

            var results = Process.ExtractAll(
                query,
                allCurrentCourses,
                currentCourse => currentCourse.CourseName.ToLower(),
                GetScorer(SearchString),
                matchCutoffScore
            );

            return results.Select(result => result.Value);
        }

        private static IRatioScorer GetScorer(string searchString)
        {
            return searchString.Any(char.IsDigit)
                ? ScorerCache.Get<TokenSetScorer>()
                : ScorerCache.Get<PartialTokenAbbreviationScorer>();
        }

        private IEnumerable<CurrentCourse> SortCurrentCourses(IEnumerable<CurrentCourse> currentCourses)
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
                SortByOptionTexts.CourseName => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.CourseName)
                    : currentCourses.OrderBy(course => course.CourseName),
                _ => currentCourses
            };
        }

        public IEnumerable<CurrentCourseViewModel> CurrentCourses
        {
            get
            {
                return currentCourses.Select(c => new CurrentCourseViewModel(c, config));
            }
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
