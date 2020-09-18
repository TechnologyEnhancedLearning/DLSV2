namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using System.Collections.Generic;
    public class LearningAsset
    {
#pragma warning disable IDE1006 // Naming Styles
        public int Id { get; set; }
        public string? hashID { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public string? mainImage { get; set; }
        public string? screenshotImage { get; set; }
        public string? screenshotImageMobile { get; set; }
        public string? typeLabel { get; set; }
        public string? lengthSeconds { get; set; }
        public string? lengthLabel { get; set; }
        public string? summaryUrl { get; set; }
        public string? directUrl { get; set; }
        public bool isFavourite { get; set; }
        public bool launched { get; set; }
        public bool dismissed { get; set; }
        public bool completed { get; set; }
        public string? notes { get; set; }
        public string? restrictedCode { get; set; }
        public string? restrictedLabel { get; set; }
        public Provider provider { get; set; }
        public List<Competency> competencyList { get; set; } = new List<Competency>();
#pragma warning restore IDE1006 // Naming Styles
    }
}
