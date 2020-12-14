namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Text.RegularExpressions;
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
        public string CourseTitle { get; }
        public string ContentSource { get; }

        private static readonly Regex ScormRegex = new Regex(@".*imsmanifest\.xml$");

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
            CourseTitle = tutorialContent.CourseTitle;

            ContentSource = IsScormPath(tutorialContent.TutorialPath!) ? GetScormSource(config, tutorialContent)
                                                                       : GetHtmlSource(config, tutorialContent);
        }

        private static bool IsScormPath(string path) => ScormRegex.IsMatch(path);

        private string GetHtmlSource(IConfiguration config, TutorialContent tutorialContent)
        {
            return $"{tutorialContent.TutorialPath}" +
                   $"?CentreID={CentreId}" +
                   $"&CustomisationID={CustomisationId}" +
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
                   $"&CandidateID={CandidateId}" +
                   $"&Version={tutorialContent.Version}" +
                   $"&tutpath={tutorialContent.TutorialPath}";
        }
    }
}
