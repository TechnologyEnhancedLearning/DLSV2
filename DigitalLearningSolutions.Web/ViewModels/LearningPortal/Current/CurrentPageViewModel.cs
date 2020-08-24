namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using FuzzySharp;
    using FuzzySharp.SimilarityRatio;
    using FuzzySharp.SimilarityRatio.Scorer;
    using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;

    public class CurrentPageViewModel : BaseCoursePageViewModel
    {
        public IEnumerable<NamedItemViewModel> CurrentCourses { get; }

        public override SelectList SortByOptions { get; } = new SelectList(new[]
        {
            SortByOptionTexts.CourseName,
            SortByOptionTexts.StartedDate,
            SortByOptionTexts.LastAccessed,
            SortByOptionTexts.CompleteByDate,
            SortByOptionTexts.DiagnosticScore,
            SortByOptionTexts.PassedSections
        });

        // This is the lower threshold for the search match score. This value was determined by trial and error.
        // If there are any issues with strange search results, changing this value or the scorer strategy would
        // be a good place to start.
        private const int MatchCutoffScore = 70;

        public CurrentPageViewModel(
            IEnumerable<CurrentCourse> currentCourses,
            IConfiguration config,
            string? searchString,
            string sortBy,
            string sortDirection,
            SelfAssessment? selfAssessment,
            string? bannerText
        ) : base(searchString, sortBy, sortDirection, bannerText)
        {

            var sortedItems = SortingHelper.SortAllItems(
                currentCourses,
                selfAssessment,
                sortBy,
                sortDirection
            );
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
}
