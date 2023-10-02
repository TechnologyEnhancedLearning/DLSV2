namespace DigitalLearningSolutions.Data.Models.External.Maps
{
    using Newtonsoft.Json;

    public class Viewport
    {
        [JsonProperty("northeast")]
        public Coordinates? Northeast { get; set; } 

        [JsonProperty("southwest")]
        public Coordinates? Southwest { get; set; } 
    }
}
