namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using DigitalLearningSolutions.Data.Models;
    using FuzzySharp;
    using FuzzySharp.SimilarityRatio;
    using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

    public class GenericSearchHelper
    {
        // This is the lower threshold for the search match score. This value was determined by trial and error,
        // as was the scorer strategy. If there are any issues with strange search results, changing this value or
        // the scorer strategy would be a good place to start. See https://github.com/JakeBayer/FuzzySharp for
        // documentation on the different scorer strategies.
        private const int MatchCutoffScore = 80;

        private static readonly string[] StopWordsArray =
        {
            "a", "about", "actually", "after", "also", "am", "an", "and", "any", "are", "as", "at", "be", "because",
            "but", "by", "could", "do", "each", "either", "en", "for", "from", "has", "have", "how", "i", "if", "in",
            "is", "it", "its", "just", "of", "or", "so", "some", "such", "that", "the", "their", "these", "thing",
            "this", "to", "too", "very", "was", "we", "well", "what", "when", "where", "who", "will", "with", "you",
            "your", "framework", "competency", "capability", "competence", "skill"
        };

        private static Func<T, string> FullProcessor<T>() where T : BaseSearchableItem =>
            item => Regex.Replace(item.SearchableName, "[^ a-zA-Z0-9]", " ").ToLower().Trim();
        
        private static Func<T, string> SimpleProcessor<T>() where T : BaseSearchableItem =>
            item => item.SearchableName.ToLower();

        public static IEnumerable<T> SearchItems<T>(
            IEnumerable<T> items,
            string? searchString,
            int matchCutOffScore = MatchCutoffScore,
            bool stripStopWords = false,
            bool useFullProcessorMethod = false,
            bool useTokeniseScorer = false
        ) where T : BaseSearchableItem
        {
            if (searchString == null)
            {
                return items;
            }

            if (stripStopWords)
            {
                searchString = CleanSearchedWords(searchString);
            }

            var query = Activator.CreateInstance(typeof(T)) as BaseSearchableItem;
            query!.SearchableName = searchString;

            var ratioScorer = useTokeniseScorer ? ScorerCache.Get<PartialTokenSetScorer>() :
                stripStopWords ? ScorerCache.Get<DefaultRatioScorer>() :
                    ScorerCache.Get<PartialRatioScorer>();

            var results = Process.ExtractAll(
                (T)query,
                items,
                useFullProcessorMethod ? FullProcessor<T>() : SimpleProcessor<T>(),
                ratioScorer,
                matchCutOffScore
            );

            return results.Select(result => result.Value);
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
            string[] words = searchedWords.Split(wordSeparators, StringSplitOptions.RemoveEmptyEntries);

            // Create and initializes a new StringCollection.
            StringCollection myStopWordsCol = new StringCollection();
            // Add a range of elements from an array to the end of the StringCollection.
            myStopWordsCol.AddRange(StopWordsArray);

            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < words.Length; i++)
            {
                string word = words[i].ToLowerInvariant().Trim();
                if (word.Length > 1 && !myStopWordsCol.Contains(word))
                {
                    sb.Append(word + " ");
                }
            }

            return sb.ToString();
        }
    }
}
