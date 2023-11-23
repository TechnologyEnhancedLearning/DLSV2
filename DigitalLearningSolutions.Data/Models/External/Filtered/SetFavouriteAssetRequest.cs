namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class SetFavouriteAssetRequest : FilteredApiRequest
    {
        [JsonProperty("params")]
        public FavouriteAsset FavouriteAsset { get; set; }
    }
}
