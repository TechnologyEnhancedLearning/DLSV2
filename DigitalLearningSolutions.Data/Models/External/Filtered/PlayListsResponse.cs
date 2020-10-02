namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class PlayListsResponse : FilteredResponse
    {
        [JsonProperty("result")]
        public IEnumerable<PlayList> Result { get; set; }
    }
}
