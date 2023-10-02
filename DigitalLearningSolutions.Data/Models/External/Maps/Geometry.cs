namespace DigitalLearningSolutions.Data.Models.External.Maps
{
    using Newtonsoft.Json;

    public class Geometry
    {
        [JsonProperty("location")]
        public Coordinates Location { get; set; } = new Coordinates();

        [JsonProperty("location_type")]
        public string LocationType { get; set; } = string.Empty;

        [JsonProperty("viewport")]
        public Viewport Viewport { get; set; } = new Viewport();
    }
}
