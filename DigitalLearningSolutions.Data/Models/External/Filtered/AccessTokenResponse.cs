namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class AccessTokenResponse
    {
        [JsonProperty("jsonrpc")]
        public string? Jsonrpc { get; set; }
        [JsonProperty("result")]
        public AccessToken? Result { get; set; }
        [JsonProperty("id")]
        public string? Id { get; set; }
    }
}
