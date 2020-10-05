namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class ParamIdRequest : FilteredApiRequest
    {
        [JsonProperty("params")]
        public ObjectId ObjectId { get; set; }
    }
}
