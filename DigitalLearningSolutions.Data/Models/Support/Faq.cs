namespace DigitalLearningSolutions.Data.Models.Support
{
    using System;

    public class Faq : BaseSearchableItem
    {
        public int FaqId { get; set; }
        public string Ahtml { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Published { get; set; }
        public string Qanchor { get; set; }
        public string Qtext { get; set; }
        public int TargetGroup { get; set; }
        public int Weighting { get; set; }
        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? $"{Qtext} {Ahtml}";
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
