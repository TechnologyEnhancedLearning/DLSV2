namespace DigitalLearningSolutions.Data.Models.External.Maps
{
    using Newtonsoft.Json;

    public class Coordinates
    {
        [JsonProperty("lat")]
        public string Latitude { get; set; } = string.Empty;

        [JsonProperty("lng")]
        public string Longitude { get; set; } = string.Empty;
    }
}
