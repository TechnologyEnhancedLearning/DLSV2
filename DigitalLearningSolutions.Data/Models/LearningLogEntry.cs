namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class LearningLogEntry : BaseSearchableItem
    {
        public DateTime When { get; set; }
        public int? LearningTime { get; set; }
        public string? AssessmentTaken { get; set; }
        public int? AssessmentScore { get; set; }
        public bool? AssessmentStatus { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? string.Empty;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
