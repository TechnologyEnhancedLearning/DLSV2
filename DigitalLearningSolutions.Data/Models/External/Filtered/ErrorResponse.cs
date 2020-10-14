namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class ErrorResponse : FilteredResponse
    {
        [JsonProperty("error")]
        public FilteredError? Error { get; set; }
    }
}
