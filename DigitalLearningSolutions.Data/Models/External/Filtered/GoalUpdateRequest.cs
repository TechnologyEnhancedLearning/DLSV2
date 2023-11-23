namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class GoalUpdateRequest : FilteredApiRequest
    {
        [JsonProperty("params")]
        public Goal Goal { get; set; }
    }
}
