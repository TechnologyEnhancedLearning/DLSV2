namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class Goal
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("importance")]
        public float Importance { get; set; }
        [JsonProperty("confidence")]
        public float Confidence { get; set; }
    }
}
