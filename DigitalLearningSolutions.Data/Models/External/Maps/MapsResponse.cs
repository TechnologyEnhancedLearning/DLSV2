namespace DigitalLearningSolutions.Data.Models.External.Maps
{
    using Newtonsoft.Json;

    public class MapsResponse
    {
        [JsonProperty("results")]
        public Map[] Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error_message")]
        public string? ErrorMessage { get; set; }
    }
}
