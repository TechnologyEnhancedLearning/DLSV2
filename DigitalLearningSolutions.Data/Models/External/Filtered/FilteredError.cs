namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class FilteredError
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("data")]
        public string? Data { get; set; }
        [JsonProperty("message")]
        public string? Message { get; set; }
    }
}
