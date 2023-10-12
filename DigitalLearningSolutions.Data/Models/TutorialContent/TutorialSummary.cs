namespace DigitalLearningSolutions.Data.Models.TutorialContent
{
    public class TutorialSummary
    {
        public int TutorialId { get; set; }

        public string TutorialName { get; set; } = string.Empty;

        public string Objectives { get; set; } = string.Empty;

        public string TutorialPath { get; set; } = string.Empty;

        public string SupportingMatsPath { get; set; } = string.Empty;
    }
}
