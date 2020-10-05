namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class AccessTokenResponse : FilteredResponse
    {
        [JsonProperty("result")]
        public AccessToken? Result { get; set; }
    }
}
