namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Text.RegularExpressions;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class ContentViewerViewModel
    {
        public TutorialContent TutorialContent { get; }
        public int CustomisationId { get; }
        public int CentreId { get; }
        public int SectionId { get; }
        public int TutorialId { get; }
        public int CandidateId { get; }
        public int ProgressId { get; }
        public string ContentSource { get; }

        private static readonly Regex scormRegex = new Regex(@".*imsmanifest\.xml$");

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
            TutorialContent = tutorialContent;
            CustomisationId = customisationId;
            CentreId = centreId;
            SectionId = sectionId;
            TutorialId = tutorialId;
            CandidateId = candidateId;
            ProgressId = progressId;

            ContentSource = IsScormPath(TutorialContent.TutorialPath!) ? GetScormSource(config) : GetHtmlSource(config);
        }

        private static bool IsScormPath(string path) => scormRegex.IsMatch(path);

        private string GetHtmlSource(IConfiguration config)
        {
            return $"{TutorialContent.TutorialPath}" +
                   $"?CentreID={CentreId}" +
                   $"&CustomisationID={CustomisationId}" +
                   $"&CandidateID={CandidateId}" +
                   $"&Version={TutorialContent.Version}" +
                   $"&ProgressID={ProgressId}" +
                   "&type=learn" +
                   $"&TrackURL={config.GetTrackingUrl()}";
        }

        private string GetScormSource(IConfiguration config)
        {
            return $"{config.GetScormPlayerUrl()}" +
                   $"?CentreID={CentreId}" +
                   $"&CustomisationID={CustomisationId}" +
                   $"&CandidateID={CandidateId}" +
                   $"&Version={TutorialContent.Version}" +
                   $"&tutpath={TutorialContent.TutorialPath}";
        }
    }
}
