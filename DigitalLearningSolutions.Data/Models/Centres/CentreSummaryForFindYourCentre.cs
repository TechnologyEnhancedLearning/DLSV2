namespace DigitalLearningSolutions.Data.Models.Centres
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class CentreSummaryForFindYourCentre : BaseSearchableItem
    {
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? WebUrl { get; set; }
        public string? Hours { get; set; }
        public string? TrainingLocations { get; set; }
        public string? TrustsCovered { get; set; }
        public string? GeneralInfo { get; set; }
        public bool SelfRegister { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? CentreName;
            set => SearchableNameOverrideForFuzzySharp = value;
        }

        public string RegionFilter => FilteringHelper.BuildFilterValueString(
            nameof(RegionName),
            nameof(RegionName),
            RegionName
        );

        public string WebsiteHref => GenerateWebsiteHref();

        public string EmailHref => GenerateEmailHref();

        private string GenerateWebsiteHref()
        {
            return $"https://{WebUrl}";
        }

        private string GenerateEmailHref()
        {
            return $"mailto:{Email}";
        }
    }
}
