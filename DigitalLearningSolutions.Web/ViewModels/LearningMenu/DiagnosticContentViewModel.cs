namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Extensions;
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
            SectionId = sectionId;
            SectionName = diagnosticContent.SectionName;
            CourseTitle = diagnosticContent.CourseTitle;

            var tutorials = diagnosticContent.CanSelectTutorials
                ? selectedTutorials
                : diagnosticContent.Tutorials;

            ContentSource = ContentViewerHelper.IsScormPath(diagnosticContent.DiagnosticAssessmentPath)
                ? ContentViewerHelper.GetScormAssessmentSource(
                    config.GetScormPlayerUrl(),
                    centreId,
                    customisationId,
                    candidateId,
                    sectionId,
                    diagnosticContent.Version,
                    diagnosticContent.DiagnosticAssessmentPath,
                    type)
                : ContentViewerHelper.GetHtmlAssessmentSource(
                    diagnosticContent.DiagnosticAssessmentPath,
                    centreId,
                    customisationId,
                    candidateId,
                    sectionId,
                    diagnosticContent.Version,
                    progressId,
                    type,
                    config.GetTrackingUrl(),
                    tutorials,
                    diagnosticContent.PassThreshold);
        }
    }
}
