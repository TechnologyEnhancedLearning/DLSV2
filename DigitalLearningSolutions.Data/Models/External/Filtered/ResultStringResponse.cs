namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class ResultStringResponse : FilteredResponse
    {
        [JsonProperty("result")]
        public string Result { get; set; }
    }
}
