namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    using DigitalLearningSolutions.Data.Helpers;
    using FuzzySharp.SimilarityRatio.Scorer;

    public class SearchOptions
    {
        public SearchOptions(
            string? searchString,
            int searchMatchCutoff = GenericSearchHelper.MatchCutoffScore,
            bool useTokeniseScore = false,
            bool stripStopWords = false,
            IRatioScorer? scorer = null
        )
        {
            SearchString = searchString;
            SearchMatchCutoff = searchMatchCutoff;
            UseTokeniseScorer = useTokeniseScore;
            StripStopWords = stripStopWords;
            Scorer = scorer;
        }

        public string? SearchString { get; set; }

        public int SearchMatchCutoff { get; set; }

        public bool UseTokeniseScorer { get; set; }

        public bool StripStopWords { get; set; }

        public IRatioScorer? Scorer { get; set; }
    }
}
