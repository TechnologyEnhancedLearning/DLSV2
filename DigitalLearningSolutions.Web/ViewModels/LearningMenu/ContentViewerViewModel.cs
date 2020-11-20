namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class ContentViewerViewModel
    {
        public string HtmlSource { get; }
        public string ScormSource { get; }

        public ContentViewerViewModel(IConfiguration config)
        {
            HtmlSource = GetHtmlSource(config);
            ScormSource = GetScormSource(config);
        }

        private static string GetHtmlSource(IConfiguration config)
        {
            const string tutorialPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course508/Section1904/Tutorials/Intro to Social Media/itspplayer.html";
            const int centreId = 101;
            const int customisationId = 24861;
            const int candidateId = 254480;
            const int version = 2;
            const string progressId = "276837";
            return $"{tutorialPath}" +
                   $"?CentreID={centreId}" +
                   $"&CustomisationID={customisationId}" +
                   $"&CandidateID={candidateId}" +
                   $"&Version={version}" +
                   $"&ProgressID={progressId}" +
                   "&type=learn" +
                   $"&TrackURL={config.GetTrackingUrl()}";
        }

        private static string GetScormSource(IConfiguration config)
        {
            const string tutorialPath = "https://www.dls.nhs.uk/cms/CMSContent/Course589/Section2295/Tutorials/2 Patient Reg PDS/imsmanifest.xml";
            const int centreId = 101;
            const int customisationId = 37545;
            const int candidateId = 254480;
            const int version = 1;
            return $"{config.GetScormPlayerUrl()}" +
                   $"?CentreID={centreId}" +
                   $"&CustomisationID={customisationId}" +
                   $"&CandidateID={candidateId}" +
                   $"&Version={version}" +
                   $"&tutpath={tutorialPath}";
        }
    }
}
