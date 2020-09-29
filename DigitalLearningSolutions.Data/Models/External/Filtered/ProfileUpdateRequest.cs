namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class ProfileUpdateRequest
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        [JsonProperty("method")]
        public string? Method { get; set; }
        [JsonProperty("params")]
        public Profile Profile { get; set; }
        [JsonProperty("jsonrpc")]
        public string? JSonRPC { get; set; }
    }
}
