namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    public class GoalResponse : FilteredResponse
    {
        [JsonProperty("result")]
        public Goal? Result { get; set; }
    }
}
