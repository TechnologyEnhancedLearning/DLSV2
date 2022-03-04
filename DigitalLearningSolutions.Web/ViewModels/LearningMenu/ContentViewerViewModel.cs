namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class ContentViewerViewModel
    {
        public int CustomisationId { get; }
        public int CentreId { get; }
        public int SectionId { get; }
        public int TutorialId { get; }
        public int CandidateId { get; }
        public int ProgressId { get; }
        public string TutorialName { get; }
        public string SectionName { get; }
        public string CourseTitle { get; }
        public string ContentSource { get; }

        public ContentViewerViewModel(
            IConfiguration config,
            TutorialContent tutorialContent,
            int customisationId,
            int centreId,
            int sectionId,
            int tutorialId,
            int candidateId,
            int progressId
        )
        {
            CustomisationId = customisationId;
            CentreId = centreId;
            SectionId = sectionId;
            TutorialId = tutorialId;
            CandidateId = candidateId;
            ProgressId = progressId;

            TutorialName = tutorialContent.TutorialName;
            SectionName = tutorialContent.SectionName;
            CourseTitle = tutorialContent.CourseTitle;

            ContentSource = ContentViewerHelper.IsScormPath(tutorialContent.TutorialPath!)
                ? GetScormSource(config, tutorialContent)
                : GetHtmlSource(config, tutorialContent);
        }

        private string GetHtmlSource(IConfiguration config, TutorialContent tutorialContent)
        {
            return $"{tutorialContent.TutorialPath}" +
                   $"?CentreID={CentreId}" +
                   $"&CustomisationID={CustomisationId}" +
                   $"&TutorialID={TutorialId}" +
                   $"&CandidateID={CandidateId}" +
                   $"&Version={tutorialContent.Version}" +
                   $"&ProgressID={ProgressId}" +
                   "&type=learn" +
                   $"&TrackURL={config.GetTrackingUrl()}";
        }

        private string GetScormSource(IConfiguration config, TutorialContent tutorialContent)
        {
            return $"{config.GetScormPlayerUrl()}" +
                   $"?CentreID={CentreId}" +
                   $"&CustomisationID={CustomisationId}" +
                   $"&TutorialID={TutorialId}" +
                   $"&CandidateID={CandidateId}" +
                   $"&Version={tutorialContent.Version}" +
                   $"&tutpath={tutorialContent.TutorialPath}";
        }
    }
}
