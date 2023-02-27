namespace DigitalLearningSolutions.Data.Models.External.Maps
{
    using Newtonsoft.Json;

    public class Geometry
    {
        [JsonProperty("location")]
        public Coordinates Location { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        [JsonProperty("viewport")]
        public Viewport Viewport { get; set; }
    }
}
