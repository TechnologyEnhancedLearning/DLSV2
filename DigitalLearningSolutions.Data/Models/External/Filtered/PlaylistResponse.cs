namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class PlaylistResponse
    {
        [JsonProperty("jsonrpc")]
        public string? Jsonrpc { get; set; }
        [JsonProperty("result")]
        public PlayList? Result { get; set; }
        [JsonProperty("id")]
        public string? Id { get; set; }
    }
}
