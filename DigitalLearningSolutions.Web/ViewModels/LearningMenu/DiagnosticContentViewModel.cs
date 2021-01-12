namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class DiagnosticContentViewModel
    {
        public int CustomisationId { get; }
        public int SectionId { get; }
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string ContentSource { get; }
        public List<int> Tutorials { get; }
        private int CentreId { get; }
        private int CandidateId { get; }
        private int ProgressId { get; }

        public DiagnosticContentViewModel(
            IConfiguration config,
            DiagnosticContent diagnosticContent,
            List<int> selectedTutorials,
            int customisationId,
            int centreId,
            int sectionId,
            int progressId,
            int candidateId
        )
        {
            CustomisationId = customisationId;
            CentreId = centreId;
            SectionId = sectionId;
            SectionName = diagnosticContent.SectionName;
            CandidateId = candidateId;
            ProgressId = progressId;
            CourseTitle = diagnosticContent.CourseTitle;
            Tutorials = selectedTutorials;

            ContentSource = ContentViewerHelper.IsScormPath(diagnosticContent.DiagnosticAssessmentPath)
                ? GetScormSource(config, diagnosticContent)
                : GetHtmlSource(config, diagnosticContent);
        }

        private string GetHtmlSource(
            IConfiguration config,
            DiagnosticContent diagnosticContent)
        {
            return $"{diagnosticContent.DiagnosticAssessmentPath}" +
                   $"?CentreID={CentreId}" +
                   $"&CustomisationID={CustomisationId}" +
                   $"&CandidateID={CandidateId}" +
                   $"&SectionID={SectionId}" +
                   $"&Version={diagnosticContent.Version}" +
                   $"&ProgressID={ProgressId}" +
                   "&type=diag" +
                   $"&TrackURL={config.GetTrackingUrl()}" +
                   $"&objlist=[{string.Join(",", Tutorials)}]" +
                   $"&plathresh={diagnosticContent.PassThreshold}";
        }

        private string GetScormSource(
            IConfiguration config,
            DiagnosticContent diagnosticContent)
        {
            return $"{config.GetScormPlayerUrl()}" +
                   $"?CentreID={CentreId}" +
                   $"&CustomisationID={CustomisationId}" +
                   $"&CandidateID={CandidateId}" +
                   $"&SectionID={SectionId}" +
                   $"&Version={diagnosticContent.Version}" +
                   $"&tutpath={diagnosticContent.DiagnosticAssessmentPath}" +
                   "&type=diag";
        }
    }
}
