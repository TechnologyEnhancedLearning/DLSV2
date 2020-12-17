namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    public class TutorialNextLinkViewModel
    {
        public int CustomisationId { get; }
        public int SectionId { get; }
        public string? PostLearningAssessmentPath { get; }
        public int? NextTutorialId { get; }
        public int? NextSectionId { get; }

        public TutorialNextLinkViewModel(
            int customisationId,
            int sectionId,
            string? postLearningAssessmentPath,
            int? nextTutorialId,
            int? nextSectionId
        )
        {
            CustomisationId = customisationId;
            SectionId = sectionId;
            PostLearningAssessmentPath = postLearningAssessmentPath;
            NextTutorialId = nextTutorialId;
            NextSectionId = nextSectionId;
        }
    }
}
