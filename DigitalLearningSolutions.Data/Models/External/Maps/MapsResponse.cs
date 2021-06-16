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

        public bool HasNoResults()
        {
            return Status == "ZERO_RESULTS";
        }

        public bool ApiErrorOccurred()
        {
            return Status == "OVER_DAILY_LIMIT" ||
                   Status == "OVER_QUERY_LIMIT" ||
                   Status == "REQUEST_DENIED" ||
                   Status == "INVALID_REQUEST" ||
                   Status == "UNKNOWN_ERROR";
        }
    }
}
