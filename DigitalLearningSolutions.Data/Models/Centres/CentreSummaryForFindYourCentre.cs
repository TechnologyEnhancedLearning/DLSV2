namespace DigitalLearningSolutions.Data.Models.Centres
{
    using System.Text.RegularExpressions;
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

        public string WebsiteHref => GenerateUrl(WebUrl);

        public string EmailHref => $"mailto:{Email}";

        public bool IsValidUrl(string url)
        {
            var regex = new Regex(
                @"[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)"
            );

            return regex.IsMatch(url);
        }

        private string GenerateUrl(string? url)
        {
            return url!.StartsWith("http")
                ? url
                : $"https://{WebUrl}";
        }
    }
}
