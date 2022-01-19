namespace DigitalLearningSolutions.Data.Models.Tracker
{
    using Newtonsoft.Json;

    public class DiagnosticOutcome
    {
        [JsonProperty(Required = Required.Always)]
        public int TutorialId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int MyScore { get; set; }
    }
}
