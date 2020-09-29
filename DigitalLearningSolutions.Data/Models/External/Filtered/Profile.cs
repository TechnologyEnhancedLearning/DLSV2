namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class Profile
    {
        [JsonProperty("function")]
        public int Function { get; set; }
        [JsonProperty("sector")]
        public int Sector { get; set; }
        [JsonProperty("seniority")]
        public int Seniority { get; set; }
    }
}
