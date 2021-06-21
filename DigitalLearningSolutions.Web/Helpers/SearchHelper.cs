namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using FuzzySharp;
    using FuzzySharp.SimilarityRatio;
    using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
    using System;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;

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
        public static IEnumerable<BrandedFramework> FilterFrameworks(IEnumerable<BrandedFramework> frameworks, string? searchString, int minMatchScore, bool stripStopWords)
        {
            if (searchString == null)
            {
                return frameworks;
            }
            if (stripStopWords)
            {
                searchString = CleanSearchedWords(searchString);
            }
            var query = new BrandedFramework()
            {
                FrameworkName = searchString.ToLower()
            };
            if (stripStopWords)
            {
                var results = Process.ExtractSorted(
                query,
                frameworks,
                framework => framework.FrameworkName.ToLower(),
                ScorerCache.Get<DefaultRatioScorer>(),
                minMatchScore
            );
                return results.Select(result => result.Value);
            }
            else
            {
                var results = Process.ExtractSorted(
                query,
                frameworks,
                framework => framework.FrameworkName.ToLower(),
                ScorerCache.Get<PartialRatioScorer>(),
                minMatchScore
            );
                return results.Select(result => result.Value);
            }
        }
        public static IEnumerable<RoleProfile> FilterRoleProfiles(IEnumerable<RoleProfile> roleProfiles, string? searchString, int minMatchScore, bool stripStopWords)
        {
            if (searchString == null)
            {
                return roleProfiles;
            }
            if (stripStopWords)
            {
                searchString = CleanSearchedWords(searchString);
            }
            var query = new RoleProfile()
            {
                RoleProfileName = searchString.ToLower()
            };
            if (stripStopWords)
            {
                var results = Process.ExtractSorted(
                query,
                roleProfiles,
                roleProfile => roleProfile.RoleProfileName.ToLower(),
                ScorerCache.Get<DefaultRatioScorer>(),
                minMatchScore
            );
                return results.Select(result => result.Value);
            }
            else
            {
                var results = Process.ExtractSorted(
                query,
                roleProfiles,
                roleProfile => roleProfile.RoleProfileName.ToLower(),
                ScorerCache.Get<PartialRatioScorer>(),
                minMatchScore
            );
                return results.Select(result => result.Value);
            }
        }
        private static string[] stopWordsArrary = new string[] { "a", "about", "actually", "after", "also", "am", "an", "and", "any", "are", "as", "at", "be", "because", "but", "by",
                                                "could", "do", "each", "either", "en", "for", "from", "has", "have", "how",
                                                "i", "if", "in", "is", "it", "its", "just", "of", "or", "so", "some", "such", "that",
                                                "the", "their", "these", "thing", "this", "to", "too", "very", "was", "we", "well", "what", "when", "where",
                                                "who", "will", "with", "you", "your", "framework", "competency", "capability", "competence", "skill", "profile", "job", "role"
                                            };

        /// 
		/// Removes stop words from the specified search string.
		/// 
		public static string CleanSearchedWords(string searchedWords)
        {

            searchedWords = searchedWords
                                            .Replace("\\", string.Empty)
                                            .Replace("|", string.Empty)
                                            .Replace("(", string.Empty)
                                            .Replace(")", string.Empty)
                                            .Replace("[", string.Empty)
                                            .Replace("]", string.Empty)
                                            .Replace("*", string.Empty)
                                            .Replace("?", string.Empty)
                                            .Replace("}", string.Empty)
                                            .Replace("{", string.Empty)
                                            .Replace("^", string.Empty)
                                            .Replace("+", string.Empty);

            // transform search string into array of words
            char[] wordSeparators = new char[] { ' ', '\n', '\r', ',', ';', '.', '!', '?', '-', ' ', '"', '\'' };
            string[] words = searchedWords.Split(wordSeparators, StringSplitOptions.RemoveEmptyEntries);

            // Create and initializes a new StringCollection.
            StringCollection myStopWordsCol = new StringCollection();
            // Add a range of elements from an array to the end of the StringCollection.
            myStopWordsCol.AddRange(stopWordsArrary);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i].ToLowerInvariant().Trim();
                if (word.Length > 1 && !myStopWordsCol.Contains(word))
                    sb.Append(word + " ");
            }

            return sb.ToString();
        }
    }
}
