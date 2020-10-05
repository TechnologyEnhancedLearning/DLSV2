namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class ParamAssetIdsRequest : FilteredApiRequest
    {
        [JsonProperty("params")]
        public LearningAssetIDs LearningAssetIDs { get; set; }
    }
}
