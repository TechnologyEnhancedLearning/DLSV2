namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class PlayList
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("type")]
        public string? Type { get; set; }
        [JsonProperty("typeExtra")]
        public List<FilteredCompetency> TypeExtra { get; set; }
        [JsonProperty("laList")]
        public LaList LaList { get; set; }
        public List<LearningAsset>? LearningAssets { get; set; }
    }
}
