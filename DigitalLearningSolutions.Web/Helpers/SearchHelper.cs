namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using FuzzySharp;
    using FuzzySharp.SimilarityRatio;
    using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

    public static class SearchHelper
    {
        // This is the lower threshold for the search match score. This value was determined by trial and error,
        // as was the scorer strategy. If there are any issues with strange search results, changing this value or
        // the scorer strategy would be a good place to start. See https://github.com/JakeBayer/FuzzySharp for
        // documentation on the different scorer strategies.
        private const int MatchCutoffScore = 80;

        public static IEnumerable<BaseLearningItem> FilterLearningItems(IEnumerable<BaseLearningItem> learningItems, string? searchString)
        {
            if (searchString == null)
            {
                return learningItems;
            }

            var query = new CurrentCourse()
            {
                Name = searchString
            };

            var results = Process.ExtractAll(
                query,
                learningItems,
                currentCourse => currentCourse.Name.ToLower(),
                ScorerCache.Get<PartialRatioScorer>(),
                MatchCutoffScore
            );
            return results.Select(result => result.Value);
        }
        public static IEnumerable<BrandedFramework> FilterFrameworks(IEnumerable<BrandedFramework> frameworks, string? searchString, int minMatchScore)
        {
            if (searchString == null)
            {
                return frameworks;
            }

            var query = new BrandedFramework()
            {
                FrameworkName = searchString
            };

            var results = Process.ExtractAll(
                query,
                frameworks,
                framework => framework.FrameworkName.ToLower(),
                ScorerCache.Get<PartialRatioScorer>(),
                minMatchScore
            );
            return results.Select(result => result.Value);
        }
    }
}
