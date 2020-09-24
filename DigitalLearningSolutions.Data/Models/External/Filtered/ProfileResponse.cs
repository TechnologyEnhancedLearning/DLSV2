namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class ProfileResponse
    {
        [JsonProperty("jsonrpc")]
        public string? Jsonrpc { get; set; }
        [JsonProperty("result")]
        public Profile? Result { get; set; }
        [JsonProperty("id")]
        public string? Id { get; set; }
        }
    }
