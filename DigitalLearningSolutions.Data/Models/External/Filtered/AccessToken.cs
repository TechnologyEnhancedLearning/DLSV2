namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class AccessToken
    {
        [JsonProperty("jwt_access_token")]
        public string? Jwt_access_token { get; set; }
        [JsonProperty("refresh_token")]
        public string? Refresh_token { get; set; }
    }
}
