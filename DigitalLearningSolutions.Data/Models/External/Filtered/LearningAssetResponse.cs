namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class LearningAssetResponse : FilteredResponse
    {
        [JsonProperty("result")]
        public List<LearningAsset>? Result { get; set; }
    }
}
