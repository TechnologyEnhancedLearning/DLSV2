namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using FuzzySharp;
    using FuzzySharp.SimilarityRatio;
    using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

    /// <summary>
    ///     This is the older version of the SearchHelper. For future search/sort implementations we should
    ///     be using the generic version <see cref="GenericSearchHelper" /> and implementing BaseSearchableItem
    ///     on the entity to be searched
    /// </summary>
    public static class SearchHelper
    {
        private static readonly string[] StopWordsArray =
        {
            "a", "about", "actually", "after", "also", "am", "an", "and", "any", "are", "as", "at", "be", "because",
            "but", "by",
            "could", "do", "each", "either", "en", "for", "from", "has", "have", "how",
            "i", "if", "in", "is", "it", "its", "just", "of", "or", "so", "some", "such", "that",
            "the", "their", "these", "thing", "this", "to", "too", "very", "was", "we", "well", "what", "when", "where",
            "who", "will", "with", "you", "your", "framework", "competency", "capability", "competence", "skill",
            "profile", "job", "role",
        };

        public static IEnumerable<RoleProfile> FilterRoleProfiles(
            IEnumerable<RoleProfile> roleProfiles,
            string? searchString,
            int minMatchScore,
            bool stripStopWords
        )
        {
            if (searchString == null)
            {
                return roleProfiles;
            }

            if (stripStopWords)
            {
                searchString = CleanSearchedWords(searchString);
            }

            var query = new RoleProfile
            {
                RoleProfileName = searchString.ToLower(),
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

        /// Removes stop words from the specified search string.
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
            char[] wordSeparators = { ' ', '\n', '\r', ',', ';', '.', '!', '?', '-', ' ', '"', '\'' };
            var words = searchedWords.Split(wordSeparators, StringSplitOptions.RemoveEmptyEntries);

            // Create and initializes a new StringCollection.
            var myStopWordsCol = new StringCollection();
            // Add a range of elements from an array to the end of the StringCollection.
            myStopWordsCol.AddRange(StopWordsArray);

            var sb = new StringBuilder();
            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i].ToLowerInvariant().Trim();
                if (word.Length > 1 && !myStopWordsCol.Contains(word))
                {
                    sb.Append(word + " ");
                }
            }

            return sb.ToString();
        }
    }
}
