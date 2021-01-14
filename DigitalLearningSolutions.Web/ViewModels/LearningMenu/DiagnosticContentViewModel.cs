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
        private const string type = "diag";
        private List<int> Tutorials { get; }
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
            return ContentViewerHelper.GetHtmlAssessmentSource(
                diagnosticContent.DiagnosticAssessmentPath,
                CentreId,
                CustomisationId,
                CandidateId,
                SectionId,
                diagnosticContent.Version,
                ProgressId,
                type,
                config.GetTrackingUrl(),
                Tutorials,
                diagnosticContent.PassThreshold
            );
        }

        private string GetScormSource(
            IConfiguration config,
            DiagnosticContent diagnosticContent)
        {
            return ContentViewerHelper.GetScormAssessmentSource(
                config.GetScormPlayerUrl(),
                CentreId,
                CustomisationId,
                CandidateId,
                SectionId,
                diagnosticContent.Version,
                diagnosticContent.DiagnosticAssessmentPath,
                type
            );
        }
    }
}
