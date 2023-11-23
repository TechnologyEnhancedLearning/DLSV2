namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class CompleteAssetRequest : FilteredApiRequest
    {
        [JsonProperty("params")]
        public CompleteAsset CompleteAsset { get; set; }
    }
}
