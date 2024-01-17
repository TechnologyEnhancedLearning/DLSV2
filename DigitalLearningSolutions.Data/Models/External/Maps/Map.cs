namespace DigitalLearningSolutions.Data.Models.External.Maps
{
    using Newtonsoft.Json;

    public class Map
    {
        [JsonProperty("address_components")]
        public AddressComponent[]? AddressComponents { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; } = string.Empty;

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; } = new Geometry();

        [JsonProperty("place_id")]
        public string PlaceId { get; set; } = string.Empty;

        [JsonProperty("plus_code")]
        public PlusCode PlusCode { get; set; } = new PlusCode();

        [JsonProperty("types")]
        public string[]? Types { get; set; }
    }
}
