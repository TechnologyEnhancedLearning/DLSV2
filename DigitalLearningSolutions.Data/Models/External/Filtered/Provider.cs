namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class Provider
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("image")]
        public string? Image { get; set; }
        [JsonProperty("favicon")]
        public string? Favicon { get; set; }
    }
}
