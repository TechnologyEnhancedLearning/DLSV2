namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;

    public class BrandTutorialSummaryViewModel
    {
        public BrandTutorialSummaryViewModel(TutorialSummary tutorialSummary)
        {
            TutorialId = tutorialSummary.TutorialId;
            TutorialName = tutorialSummary.TutorialName;
            Objectives = tutorialSummary.Objectives;
            TutorialPath = tutorialSummary.TutorialPath;
            SupportingMatsPath = tutorialSummary.SupportingMatsPath;
        }

        public int TutorialId { get; set; }
        public string TutorialName { get; set; }
        public string Objectives { get; set; }
        public string TutorialPath { get; set; }
        public string SupportingMatsPath { get; set; }
    }
}
