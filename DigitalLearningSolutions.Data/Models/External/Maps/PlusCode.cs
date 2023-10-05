namespace DigitalLearningSolutions.Data.Models.External.Maps
{
    using Newtonsoft.Json;

    public class PlusCode
    {
        [JsonProperty("compound_code")]
        public string CompoundCode { get; set; } = string.Empty;

        [JsonProperty("global_code")]
        public string GlobalCode { get; set; } = string.Empty;
    }
}
