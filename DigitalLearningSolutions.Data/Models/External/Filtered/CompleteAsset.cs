namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class CompleteAsset
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("completedStatus")]
        public string? CompletedStatus { get; set; }
    }
}
