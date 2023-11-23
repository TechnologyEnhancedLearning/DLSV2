namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class PlayListResponse : FilteredResponse
    {
        [JsonProperty("result")]
        public PlayList Result { get; set; }
    }
}
