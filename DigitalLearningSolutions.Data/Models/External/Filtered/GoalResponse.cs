namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class GoalResponse
    {
        [JsonProperty("jsonrpc")]
        public string? Jsonrpc { get; set; }
        [JsonProperty("result")]
        public Goal? Result { get; set; }
        [JsonProperty("id")]
        public string? Id { get; set; }
    }
}
