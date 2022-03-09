namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    using DigitalLearningSolutions.Data.Helpers;

    public class SearchOptions
    {
        public SearchOptions(string? searchString, int? searchMatchCutoff = null)
        {
            SearchString = searchString;
            SearchMatchCutoff = searchMatchCutoff ?? GenericSearchHelper.MatchCutoffScore;
        }

        public string? SearchString { get; set; }

        public int SearchMatchCutoff { get; set; }
    }
}
