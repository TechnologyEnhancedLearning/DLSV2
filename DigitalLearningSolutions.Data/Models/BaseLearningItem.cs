﻿namespace DigitalLearningSolutions.Data.Models
{
    public abstract class BaseLearningItem : BaseSearchableItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool HasDiagnostic { get; set; }
        public bool HasLearning { get; set; }
        public bool IsAssessed { get; set; }
        public bool IsSelfAssessment { get; set; }
        public bool IncludesSignposting { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? Name;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
