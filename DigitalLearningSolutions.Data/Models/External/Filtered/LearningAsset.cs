namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class LearningAsset
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("hashID")]
        public string? HashID { get; set; }
        [JsonProperty("title")]
        public string? Title { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("mainImage")]
        public string? MainImage { get; set; }
        [JsonProperty("screenshotImage")]
        public string? ScreenshotImage { get; set; }
        [JsonProperty("screenshotImageMobile")]
        public string? ScreenshotImageMobile { get; set; }
        [JsonProperty("typeLabel")]
        public string? TypeLabel { get; set; }
        [JsonProperty("lengthSeconds")]
        public string? LengthSeconds { get; set; }
        [JsonProperty("lengthLabel")]
        public string? LengthLabel { get; set; }
        [JsonProperty("summaryUrl")]
        public string? SummaryUrl { get; set; }
        [JsonProperty("directUrl")]
        public string? DirectUrl { get; set; }
        [JsonProperty("isFavourite")]
        public bool IsFavourite { get; set; }
        [JsonProperty("launched")]
        public bool Launched { get; set; }
        [JsonProperty("dismissed")]
        public bool Dismissed { get; set; }
        [JsonProperty("completed")]
        public bool Completed { get; set; }
        [JsonProperty("completedStatus")]
        public string? CompletedStatus { get; set; }
        [JsonProperty("notes")]
        public string? Notes { get; set; }
        [JsonProperty("restrictedCode")]
        public string? RestrictedCode { get; set; }
        [JsonProperty("restrictedLabel")]
        public string? RestrictedLabel { get; set; }
        [JsonProperty("provider")]
        public Provider? Provider { get; set; }
        [JsonProperty("competencyList")]
        public List<FilteredCompetency> CompetencyList { get; set; } = new List<FilteredCompetency>();
    }
}
