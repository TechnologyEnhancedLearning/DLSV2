namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class Profile
    {
        [JsonProperty("function")]
        public string? Function { get; set; }
        [JsonProperty("sector")]
        public string? Sector { get; set; }
        [JsonProperty("seniority")]
        public string? Seniority { get; set; }
    }
}
