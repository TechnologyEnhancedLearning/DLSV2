namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class PlayListsResponse
    {
        [JsonProperty("jsonrpc")]
        public string? Jsonrpc { get; set; }
        [JsonProperty("result")]
        public IEnumerable<PlayList> Result { get; set; }
        [JsonProperty("id")]
        public string? Id { get; set; }
    }
}
