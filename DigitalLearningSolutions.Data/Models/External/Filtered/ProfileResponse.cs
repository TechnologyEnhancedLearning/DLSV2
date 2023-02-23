namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class ProfileResponse : FilteredResponse
    {
        [JsonProperty("result")]
        public Profile? Result { get; set; }
    }
}
