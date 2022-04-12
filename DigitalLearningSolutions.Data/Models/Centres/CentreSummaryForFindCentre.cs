namespace DigitalLearningSolutions.Data.Models.Centres
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class CentreSummaryForFindCentre : BaseSearchableItem
    {
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string? pwTelephone { get; set; }
        public string? pwEmail { get; set; }
        public string? pwWebURL { get; set; }
        public string? pwHours { get; set; }
        public string? pwTrainingLocations { get; set; }
        public string? pwTrustsCovered { get; set; }
        public string? pwGeneralInfo { get; set; }
        public bool kbSelfRegister { get; set; }
        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? CentreName;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
