namespace DigitalLearningSolutions.Data.Models.External.Maps
{
    using Newtonsoft.Json;

    public class AddressComponent
    {
        [JsonProperty("long_name")]
        public string LongName { get; set; } = string.Empty;

        [JsonProperty("short_name")]
        public string ShortName { get; set; } = string.Empty; 

        [JsonProperty("types")]
        public string[]? Types { get; set; } 
    }
}
